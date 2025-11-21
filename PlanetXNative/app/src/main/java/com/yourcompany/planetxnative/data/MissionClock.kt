package com.yourcompany.planetxnative.data

import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import java.time.LocalDate
import java.time.LocalDateTime
import java.time.temporal.ChronoUnit

object MissionClock {
    // Configuration
    private const val MISSION_START_DAY = 139  // Start at day 139 of mission (July 20, 2028)
    private const val TIME_ACCELERATION = 60  // 1 real second = 1 mission minute
    
    private val _currentMissionDay = MutableStateFlow(MISSION_START_DAY.toLong())
    val currentMissionDay: StateFlow<Long> = _currentMissionDay.asStateFlow()
    
    private var lastUpdateTime: Long = System.currentTimeMillis()
    private var isRunning: Boolean = true
    
    /**
     * Update mission time based on elapsed real time
     */
    fun update() {
        if (!isRunning) return
        
        val currentTime = System.currentTimeMillis()
        val elapsedRealSeconds = (currentTime - lastUpdateTime) / 1000.0
        val elapsedMissionSeconds = elapsedRealSeconds * TIME_ACCELERATION
        val elapsedMissionDays = elapsedMissionSeconds / 86400.0  // Convert to days
        
        _currentMissionDay.value = _currentMissionDay.value + elapsedMissionDays.toLong()
        lastUpdateTime = currentTime
        
        // Clamp to mission duration
        val maxDay = MissionPhase.getTotalMissionDays()
        if (_currentMissionDay.value > maxDay) {
            _currentMissionDay.value = maxDay
            pause()  // Stop at mission end
        }
    }
    
    /**
     * Get current mission date (actual calendar date)
     */
    fun getCurrentMissionDate(): LocalDate {
        return MissionPhase.LAUNCH.startDate.plusDays(_currentMissionDay.value)
    }
    
    /**
     * Get current mission phase
     */
    fun getCurrentPhase(): MissionPhase {
        return MissionPhase.getPhaseAtDay(_currentMissionDay.value)
    }
    
    /**
     * Get days until next phase
     */
    fun getDaysToNextPhase(): Long? {
        val currentPhase = getCurrentPhase()
        val nextPhase = MissionPhase.values().getOrNull(currentPhase.ordinal + 1) ?: return null
        return nextPhase.getDaysFromMissionStart() - _currentMissionDay.value
    }
    
    /**
     * Get progress through current phase (0.0 to 1.0)
     */
    fun getPhaseProgress(): Float {
        val currentPhase = getCurrentPhase()
        val phaseStartDay = currentPhase.getDaysFromMissionStart()
        val phaseDuration = currentPhase.getDaysToNext() ?: return 1.0f
        
        val daysSincePhaseStart = _currentMissionDay.value - phaseStartDay
        return (daysSincePhaseStart.toFloat() / phaseDuration.toFloat()).coerceIn(0f, 1f)
    }
    
    /**
     * Get overall mission progress (0.0 to 1.0)
     */
    fun getMissionProgress(): Float {
        val totalDays = MissionPhase.getTotalMissionDays().toFloat()
        return (_currentMissionDay.value.toFloat() / totalDays).coerceIn(0f, 1f)
    }
    
    /**
     * Set mission to specific day (for testing/debugging)
     */
    fun setMissionDay(day: Long) {
        _currentMissionDay.value = day.coerceIn(0, MissionPhase.getTotalMissionDays())
        lastUpdateTime = System.currentTimeMillis()
    }
    
    /**
     * Pause mission clock
     */
    fun pause() {
        isRunning = false
    }
    
    /**
     * Resume mission clock
     */
    fun resume() {
        isRunning = true
        lastUpdateTime = System.currentTimeMillis()
    }
    
    /**
     * Reset mission to start
     */
    fun reset() {
        _currentMissionDay.value = MISSION_START_DAY.toLong()
        lastUpdateTime = System.currentTimeMillis()
        isRunning = true
    }
    
    /**
     * Get formatted time string
     */
    fun getFormattedMissionTime(): String {
        val days = _currentMissionDay.value
        val years = days / 365
        val remainingDays = days % 365
        
        return if (years > 0) {
            "Year $years, Day $remainingDays"
        } else {
            "Day $days"
        }
    }
}

