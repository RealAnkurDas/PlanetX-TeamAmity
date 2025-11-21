package com.yourcompany.planetxnative.data

data class SpacecraftTelemetry(
    val velocity: Double, // km/s
    val distanceFromSun: Double, // AU
    val altitude: Double, // km
    val pitch: Double, // degrees
    val roll: Double, // degrees
    val yaw: Double, // degrees
    val fuelReserve: Double // percentage
)

data class SpacecraftPart(
    val name: String,
    val health: Int // 0-100
)

data class AIInsight(
    val type: String, // "WARNING", "INFO", "CRITICAL"
    val message: String,
    val timestamp: Long
)

data class MissionStatus(
    val currentPhase: String,
    val nextEvent: String,
    val eventCountdown: String,
    val progress: Float, // 0.0 to 1.0
    val eta: String
)

// Live telemetry provider
object LiveTelemetryProvider {
    /**
     * Get current spacecraft telemetry based on mission time
     */
    fun getCurrentTelemetry(): SpacecraftTelemetry {
        val missionDay = MissionClock.currentMissionDay.value
        return TelemetryCalculator.calculateTelemetry(missionDay)
    }
    
    /**
     * Get current spacecraft systems health
     */
    fun getSpacecraftHealth(): List<SpacecraftPart> {
        val missionDay = MissionClock.currentMissionDay.value
        return TelemetryCalculator.calculateSpacecraftHealth(missionDay)
    }
    
    /**
     * Get AI insights
     */
    fun getAIInsights(): List<AIInsight> {
        val missionDay = MissionClock.currentMissionDay.value
        return TelemetryCalculator.generateAIInsights(missionDay)
    }
    
    /**
     * Get mission status
     */
    fun getMissionStatus(): MissionStatus {
        val missionDay = MissionClock.currentMissionDay.value
        return TelemetryCalculator.calculateMissionStatus(missionDay)
    }
}

// Legacy sample data (kept for compatibility during transition)
object SampleData {
    val currentTelemetry get() = LiveTelemetryProvider.getCurrentTelemetry()
    val spacecraftParts get() = LiveTelemetryProvider.getSpacecraftHealth()
    val aiInsights get() = LiveTelemetryProvider.getAIInsights()
    val missionStatus get() = LiveTelemetryProvider.getMissionStatus()
    
    val journeyCheckpoints = listOf(
        "Earth" to 0.0,
        "Venus Orbit" to 0.72,
        "Current Position" to 2.3,
        "Asteroid Belt" to 2.7,
        "Justitia" to 2.8
    )
}

