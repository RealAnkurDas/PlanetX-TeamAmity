package com.yourcompany.planetxnative.data

data class TrajectoryWaypoint(
    val id: Int,
    val xPixels: Double,
    val yPixels: Double,
    val phase: MissionPhase,
    val phaseDate: String,
    val color: String
) {
    // Convert pixel coordinates to AU (will be set by TrajectoryManager based on image scale)
    var xAU: Double = 0.0
    var yAU: Double = 0.0
    var zAU: Double = 0.0
    
    // Calculated velocity (km/s)
    var velocityKmS: Double = 0.0
    
    // Mission day at this waypoint
    var missionDay: Long = 0
}

data class Position3D(
    val x: Double,
    val y: Double,
    val z: Double
) {
    fun distanceFromOrigin(): Double {
        return kotlin.math.sqrt(x * x + y * y + z * z)
    }
    
    fun distanceTo(other: Position3D): Double {
        val dx = x - other.x
        val dy = y - other.y
        val dz = z - other.z
        return kotlin.math.sqrt(dx * dx + dy * dy + dz * dz)
    }
}

data class Velocity3D(
    val vx: Double,
    val vy: Double,
    val vz: Double
) {
    fun magnitude(): Double {
        return kotlin.math.sqrt(vx * vx + vy * vy + vz * vz)
    }
}

data class TrajectoryState(
    val missionDay: Long,
    val position: Position3D,  // In AU
    val velocity: Velocity3D,  // In km/s
    val phase: MissionPhase,
    val distanceFromSun: Double  // In AU
)

