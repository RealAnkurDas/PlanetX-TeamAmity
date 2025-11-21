package com.yourcompany.planetxnative.data

import kotlin.math.abs
import kotlin.math.atan2
import kotlin.math.cos
import kotlin.math.sin
import kotlin.random.Random

object TelemetryCalculator {
    
    private const val INITIAL_FUEL = 100.0
    private const val FUEL_CONSUMPTION_RATE = 0.15  // % per day (faster to show lower fuel)
    private const val BASE_HEALTH = 100
    private const val HEALTH_DEGRADATION_RATE = 0.01  // % per day (much slower)
    
    // Health flicker state
    private var lastHealthUpdate = 0L
    private val healthCache = mutableMapOf<String, Int>()
    private val healthBaseline = mutableMapOf<String, Int>()
    
    /**
     * Calculate complete spacecraft telemetry for current mission time
     */
    fun calculateTelemetry(missionDay: Long): SpacecraftTelemetry {
        val state = TrajectoryManager.getStateAtDay(missionDay)
        
        return SpacecraftTelemetry(
            velocity = 8.67,  // Fixed at 8.67 km/s as requested
            distanceFromSun = state.distanceFromSun,
            altitude = calculateAltitude(state),
            pitch = calculatePitch(state),
            roll = calculateRoll(),
            yaw = calculateYaw(state),
            fuelReserve = calculateFuelReserve(missionDay)
        )
    }
    
    /**
     * Calculate mission status
     */
    fun calculateMissionStatus(missionDay: Long): MissionStatus {
        val phase = MissionClock.getCurrentPhase()
        val daysToNext = MissionClock.getDaysToNextPhase()
        
        val nextPhase = MissionPhase.values().getOrNull(phase.ordinal + 1)
        val nextEvent = nextPhase?.phaseName ?: "Mission Complete"
        
        val countdown = if (daysToNext != null) {
            formatCountdown(daysToNext)
        } else {
            "Complete"
        }
        
        return MissionStatus(
            currentPhase = phase.phaseName,
            nextEvent = nextEvent,
            eventCountdown = countdown,
            progress = MissionClock.getMissionProgress(),
            eta = formatETA(MissionPhase.getTotalMissionDays() - missionDay)
        )
    }
    
    /**
     * Calculate spacecraft systems health with realistic degradation and flicker
     */
    fun calculateSpacecraftHealth(missionDay: Long): List<SpacecraftPart> {
        val currentTime = System.currentTimeMillis()
        val baseDegradation = (missionDay * HEALTH_DEGRADATION_RATE).toInt()
        
        // Update baseline health every 5 seconds (slow degradation)
        if (currentTime - lastHealthUpdate > 5000) {
            lastHealthUpdate = currentTime
            
            // Initialize baselines if not set (start at 80-100%)
            if (healthBaseline.isEmpty()) {
                healthBaseline["Main Engine"] = Random.nextInt(85, 96)
                healthBaseline["RCS Thrusters"] = Random.nextInt(88, 98)
                healthBaseline["Solar Panels"] = Random.nextInt(82, 94)
                healthBaseline["Communications"] = Random.nextInt(90, 99)
            }
            
            // Slowly decrease baseline over time (only if mission progresses)
            healthBaseline.forEach { (key, value) ->
                if (missionDay > 100 && Random.nextInt(0, 10) == 0) {
                    healthBaseline[key] = (value - 1).coerceIn(80, 100)
                }
            }
        }
        
        // Apply small random flicker (±1-2%) on each update
        return listOf(
            SpacecraftPart("Main Engine", (healthBaseline["Main Engine"] ?: 90) + Random.nextInt(-2, 3)),
            SpacecraftPart("RCS Thrusters", (healthBaseline["RCS Thrusters"] ?: 92) + Random.nextInt(-2, 3)),
            SpacecraftPart("Solar Panels", (healthBaseline["Solar Panels"] ?: 88) + Random.nextInt(-2, 3)),
            SpacecraftPart("Communications", (healthBaseline["Communications"] ?: 95) + Random.nextInt(-1, 2))
        )
    }
    
    /**
     * Generate AI insights based on mission state
     */
    fun generateAIInsights(missionDay: Long): List<AIInsight> {
        val phase = MissionClock.getCurrentPhase()
        val fuel = calculateFuelReserve(missionDay)
        val daysToNext = MissionClock.getDaysToNextPhase()
        
        val insights = mutableListOf<AIInsight>()
        
        // Trajectory status
        insights.add(AIInsight(
            "INFO",
            "Trajectory nominal. Current path within 0.1% of optimal.",
            System.currentTimeMillis()
        ))
        
        // Fuel efficiency
        if (fuel > 60) {
            insights.add(AIInsight(
                "INFO",
                "Fuel consumption ${(100 - fuel).toInt()}% below projected. Excellent efficiency.",
                System.currentTimeMillis() - 3600000
            ))
        } else if (fuel < 40) {
            insights.add(AIInsight(
                "WARNING",
                "Fuel reserves at ${fuel.toInt()}%. Monitoring consumption closely.",
                System.currentTimeMillis() - 1800000
            ))
        }
        
        // Phase-specific insights
        if (daysToNext != null && daysToNext < 7) {
            insights.add(AIInsight(
                "WARNING",
                "Approaching ${phase.phaseName}. Initiating pre-event checks in ${daysToNext}d.",
                System.currentTimeMillis() - 7200000
            ))
        }
        
        // Random environmental events
        if (missionDay % 100 == 42L) {
            insights.add(AIInsight(
                "WARNING",
                "Minor solar flare detected. Radiation levels elevated by 12%.",
                System.currentTimeMillis() - 14400000
            ))
        }
        
        return insights.take(3)  // Show top 3 insights
    }
    
    // Helper calculation methods
    
    private fun calculateAltitude(state: TrajectoryState): Double {
        // Simplified: altitude above ecliptic plane in km
        return abs(state.position.z) * 149597870.7
    }
    
    private fun calculatePitch(state: TrajectoryState): Double {
        // Calculate pitch from trajectory direction
        val phase = state.phase
        return when (phase) {
            MissionPhase.LAUNCH -> -15.0 + Random.nextDouble(-2.0, 2.0)
            MissionPhase.VENUS_GA, MissionPhase.EARTH_GA, MissionPhase.MARS_GA -> 
                Random.nextDouble(-5.0, 5.0)
            else -> Random.nextDouble(-3.0, 3.0)
        }
    }
    
    private fun calculateRoll(): Double {
        // Small random roll
        return Random.nextDouble(-2.0, 2.0)
    }
    
    private fun calculateYaw(state: TrajectoryState): Double {
        // Calculate yaw from position
        return atan2(state.position.y, state.position.x) * 180 / Math.PI
    }
    
    private fun calculateFuelReserve(missionDay: Long): Double {
        // Fuel decreases over time with accelerated consumption at phase transitions
        var fuel = INITIAL_FUEL - (missionDay * FUEL_CONSUMPTION_RATE)
        
        // Extra fuel consumption at major events
        val phase = MissionPhase.getPhaseAtDay(missionDay)
        val phasePenalty = when (phase) {
            MissionPhase.LAUNCH -> 5.0
            MissionPhase.VENUS_GA, MissionPhase.EARTH_GA, MissionPhase.MARS_GA -> 3.0
            MissionPhase.JUSTITIA -> 8.0
            else -> 1.0
        }
        
        fuel -= phasePenalty
        
        return fuel.coerceIn(0.0, 100.0)
    }
    
    private fun randomVariation(): Int {
        return Random.nextInt(-3, 4)
    }
    
    private fun formatCountdown(days: Long): String {
        return when {
            days == 0L -> "Today"
            days == 1L -> "1 day"
            days < 7 -> "${days}d"
            days < 30 -> "${days / 7}w ${days % 7}d"
            else -> "${days / 30}mo ${(days % 30) / 7}w"
        }
    }
    
    private fun formatETA(daysRemaining: Long): String {
        return when {
            daysRemaining < 0 -> "Arrived"
            daysRemaining < 1 -> "< 1 day"
            daysRemaining < 7 -> "$daysRemaining days"
            daysRemaining < 30 -> "${daysRemaining / 7} weeks"
            daysRemaining < 365 -> "${daysRemaining / 30} months"
            else -> "${daysRemaining / 365} years ${(daysRemaining % 365) / 30} months"
        }
    }
}

