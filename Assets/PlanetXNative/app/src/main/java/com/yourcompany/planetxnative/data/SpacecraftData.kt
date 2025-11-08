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

// Sample data
object SampleData {
    val currentTelemetry = SpacecraftTelemetry(
        velocity = 14.3,
        distanceFromSun = 2.3,
        altitude = 450000.0,
        pitch = -2.4,
        roll = 0.8,
        yaw = 1.2,
        fuelReserve = 78.5
    )
    
    val spacecraftParts = listOf(
        SpacecraftPart("Main Engine", 95),
        SpacecraftPart("RCS Thrusters", 88),
        SpacecraftPart("Solar Panels", 92),
        SpacecraftPart("Communications", 97),
        SpacecraftPart("Life Support", 100),
        SpacecraftPart("Navigation", 94),
        SpacecraftPart("Heat Shield", 85)
    )
    
    val aiInsights = listOf(
        AIInsight("INFO", "Trajectory nominal. Current path within 0.2% of optimal.", System.currentTimeMillis()),
        AIInsight("WARNING", "Minor solar flare detected. Radiation levels elevated by 15%.", System.currentTimeMillis() - 3600000),
        AIInsight("INFO", "Fuel consumption 3% below projected. Excellent efficiency.", System.currentTimeMillis() - 7200000)
    )
    
    val missionStatus = MissionStatus(
        currentPhase = "Cruise",
        nextEvent = "Landing burn initiation",
        eventCountdown = "3d 4h 23m",
        progress = 0.68f,
        eta = "5 days 18 hours"
    )
    
    val journeyCheckpoints = listOf(
        "Earth" to 0.0,
        "Venus Orbit" to 0.72,
        "Current Position" to 2.3,
        "Asteroid Belt" to 2.7,
        "Justitia" to 2.8
    )
}

