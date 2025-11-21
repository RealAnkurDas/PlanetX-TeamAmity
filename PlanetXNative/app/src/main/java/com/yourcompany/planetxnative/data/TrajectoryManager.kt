package com.yourcompany.planetxnative.data

import android.content.Context
import java.io.BufferedReader
import java.io.InputStreamReader
import kotlin.math.pow
import kotlin.math.sqrt

object TrajectoryManager {
    private var waypoints: List<TrajectoryWaypoint> = emptyList()
    private var isInitialized = false
    
    // Image scale configuration (adjust based on your trajectory image)
    private const val IMAGE_WIDTH_PIXELS = 1920.0  // Adjust to your image
    private const val IMAGE_HEIGHT_PIXELS = 1080.0  // Adjust to your image
    private const val TRAJECTORY_SPAN_AU = 3.0  // Approximate span of trajectory in AU
    
    /**
     * Load trajectory waypoints from CSV in assets folder
     */
    fun loadTrajectory(context: Context) {
        if (isInitialized) return
        
        try {
            val inputStream = context.assets.open("spacecraft_trajectory.csv")
            val reader = BufferedReader(InputStreamReader(inputStream))
            
            val waypointsList = mutableListOf<TrajectoryWaypoint>()
            
            // Skip header
            reader.readLine()
            
            // Read waypoints
            reader.forEachLine { line ->
                val parts = line.split(",")
                if (parts.size >= 6) {
                    val id = parts[0].toIntOrNull() ?: return@forEachLine
                    val xPixels = parts[1].toDoubleOrNull() ?: return@forEachLine
                    val yPixels = parts[2].toDoubleOrNull() ?: return@forEachLine
                    val phaseName = parts[3].trim()
                    val phaseDate = parts[4].trim()
                    val color = parts[5].trim()
                    
                    // Find matching phase
                    val phase = MissionPhase.values().find { it.phaseName == phaseName } 
                        ?: MissionPhase.LAUNCH
                    
                    val waypoint = TrajectoryWaypoint(
                        id = id,
                        xPixels = xPixels,
                        yPixels = yPixels,
                        phase = phase,
                        phaseDate = phaseDate,
                        color = color
                    )
                    
                    waypointsList.add(waypoint)
                }
            }
            
            reader.close()
            
            // Convert pixel coordinates to AU
            convertPixelsToAU(waypointsList)
            
            // Calculate mission days for each waypoint
            assignMissionDays(waypointsList)
            
            // Calculate velocities
            calculateVelocities(waypointsList)
            
            waypoints = waypointsList.sortedBy { it.id }
            isInitialized = true
            
        } catch (e: Exception) {
            e.printStackTrace()
            // Use fallback dummy data if file not found
            waypoints = createFallbackTrajectory()
            isInitialized = true
        }
    }
    
    /**
     * Convert pixel coordinates to astronomical units (AU)
     */
    private fun convertPixelsToAU(waypoints: List<TrajectoryWaypoint>) {
        val centerX = IMAGE_WIDTH_PIXELS / 2
        val centerY = IMAGE_HEIGHT_PIXELS / 2
        val scale = TRAJECTORY_SPAN_AU / IMAGE_WIDTH_PIXELS
        
        for (wp in waypoints) {
            // Convert to centered coordinates and scale
            wp.xAU = (wp.xPixels - centerX) * scale
            wp.yAU = (centerY - wp.yPixels) * scale  // Flip Y axis
            wp.zAU = 0.0  // Assuming 2D trajectory
        }
    }
    
    /**
     * Assign mission days to waypoints based on phases
     */
    private fun assignMissionDays(waypoints: List<TrajectoryWaypoint>) {
        val wpByPhase = waypoints.groupBy { it.phase }
        
        for ((phase, phaseWaypoints) in wpByPhase) {
            val phaseStartDay = phase.getDaysFromMissionStart()
            val phaseDuration = phase.getDaysToNext() ?: 0
            
            phaseWaypoints.forEachIndexed { index, wp ->
                // Distribute waypoints evenly across phase duration
                val fraction = if (phaseWaypoints.size > 1) {
                    index.toDouble() / (phaseWaypoints.size - 1)
                } else 0.0
                
                wp.missionDay = (phaseStartDay + fraction * phaseDuration).toLong()
            }
        }
    }
    
    /**
     * Calculate velocities between waypoints
     */
    private fun calculateVelocities(waypoints: List<TrajectoryWaypoint>) {
        for (i in waypoints.indices) {
            val current = waypoints[i]
            val next = waypoints.getOrNull(i + 1)
            
            if (next != null) {
                val dx = (next.xAU - current.xAU) * 149597870.7  // AU to km
                val dy = (next.yAU - current.yAU) * 149597870.7
                val dz = (next.zAU - current.zAU) * 149597870.7
                val distance = sqrt(dx * dx + dy * dy + dz * dz)
                
                val dt = (next.missionDay - current.missionDay) * 86400.0  // days to seconds
                current.velocityKmS = if (dt > 0) distance / dt else 15.0  // Default to 15 km/s
            } else {
                current.velocityKmS = waypoints.getOrNull(i - 1)?.velocityKmS ?: 15.0
            }
        }
    }
    
    /**
     * Get trajectory state at a specific mission day using Catmull-Rom interpolation
     */
    fun getStateAtDay(missionDay: Long): TrajectoryState {
        if (waypoints.isEmpty()) {
            return createDefaultState(missionDay)
        }
        
        // Find surrounding waypoints
        val idx = waypoints.indexOfFirst { it.missionDay > missionDay }
        
        return when {
            idx == -1 -> {
                // Past last waypoint
                val last = waypoints.last()
                createStateFromWaypoint(last, missionDay)
            }
            idx == 0 -> {
                // Before first waypoint
                val first = waypoints.first()
                createStateFromWaypoint(first, missionDay)
            }
            else -> {
                // Interpolate between waypoints
                interpolateState(waypoints[idx - 1], waypoints[idx], missionDay)
            }
        }
    }
    
    /**
     * Linear interpolation between two waypoints
     */
    private fun interpolateState(
        wp1: TrajectoryWaypoint,
        wp2: TrajectoryWaypoint,
        missionDay: Long
    ): TrajectoryState {
        val t = ((missionDay - wp1.missionDay).toDouble() / 
                (wp2.missionDay - wp1.missionDay).toDouble()).coerceIn(0.0, 1.0)
        
        val x = wp1.xAU + t * (wp2.xAU - wp1.xAU)
        val y = wp1.yAU + t * (wp2.yAU - wp1.yAU)
        val z = wp1.zAU + t * (wp2.zAU - wp1.zAU)
        
        val velocity = wp1.velocityKmS + t * (wp2.velocityKmS - wp1.velocityKmS)
        
        val position = Position3D(x, y, z)
        val phase = MissionPhase.getPhaseAtDay(missionDay)
        
        return TrajectoryState(
            missionDay = missionDay,
            position = position,
            velocity = Velocity3D(velocity, 0.0, 0.0),  // Simplified
            phase = phase,
            distanceFromSun = position.distanceFromOrigin()
        )
    }
    
    private fun createStateFromWaypoint(wp: TrajectoryWaypoint, missionDay: Long): TrajectoryState {
        return TrajectoryState(
            missionDay = missionDay,
            position = Position3D(wp.xAU, wp.yAU, wp.zAU),
            velocity = Velocity3D(wp.velocityKmS, 0.0, 0.0),
            phase = MissionPhase.getPhaseAtDay(missionDay),
            distanceFromSun = sqrt(wp.xAU.pow(2) + wp.yAU.pow(2) + wp.zAU.pow(2))
        )
    }
    
    private fun createDefaultState(missionDay: Long): TrajectoryState {
        val phase = MissionPhase.getPhaseAtDay(missionDay)
        return TrajectoryState(
            missionDay = missionDay,
            position = Position3D(1.0, 0.0, 0.0),
            velocity = Velocity3D(15.0, 0.0, 0.0),
            phase = phase,
            distanceFromSun = 1.0
        )
    }
    
    /**
     * Create fallback trajectory data if CSV not found
     */
    private fun createFallbackTrajectory(): List<TrajectoryWaypoint> {
        return listOf(
            TrajectoryWaypoint(1, 0.0, 0.0, MissionPhase.LAUNCH, "2028-03-03", "#FF0000").apply {
                xAU = 1.0; yAU = 0.0; missionDay = 0; velocityKmS = 10.0
            },
            TrajectoryWaypoint(2, 0.0, 0.0, MissionPhase.JUSTITIA, "2034-10-30", "#FF0088").apply {
                xAU = 2.8; yAU = 0.0; missionDay = 2432; velocityKmS = 15.0
            }
        )
    }
    
    fun getWaypoints(): List<TrajectoryWaypoint> = waypoints
}

