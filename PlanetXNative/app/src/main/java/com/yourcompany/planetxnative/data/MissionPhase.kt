package com.yourcompany.planetxnative.data

import java.time.LocalDate
import java.time.temporal.ChronoUnit

enum class MissionPhase(
    val phaseName: String,
    val startDate: LocalDate,
    val color: Long
) {
    LAUNCH("Launch", LocalDate.of(2028, 3, 3), 0xFFFF0000),
    VENUS_GA("Venus Gravity Assist", LocalDate.of(2028, 7, 10), 0xFFFF8800),
    EARTH_GA("Earth Gravity Assist", LocalDate.of(2029, 5, 24), 0xFFFFFF00),
    WESTERWALD("Westerwald", LocalDate.of(2030, 2, 18), 0xFF88FF00),
    CHIMAERA("Chimaera", LocalDate.of(2030, 6, 14), 0xFF00FF00),
    ROCKOX("Rockox", LocalDate.of(2031, 1, 14), 0xFF00FFFF),
    MARS_GA("Mars Gravity Assist", LocalDate.of(2031, 9, 23), 0xFF0088FF),
    VA28("2000 VA28", LocalDate.of(2032, 7, 24), 0xFF4B0082),
    RC76("1998 RC76", LocalDate.of(2032, 12, 15), 0xFF8800FF),
    SG6("1999 SG6", LocalDate.of(2033, 9, 2), 0xFFFF00FF),
    JUSTITIA("Justitia Arrival", LocalDate.of(2034, 10, 30), 0xFFFF0088),
    DEPLOY("Deploy Lander", LocalDate.of(2035, 5, 1), 0xFF880000);

    fun getDaysFromMissionStart(): Long {
        return ChronoUnit.DAYS.between(LAUNCH.startDate, startDate)
    }

    fun getDaysToNext(): Long? {
        val nextPhase = values().getOrNull(ordinal + 1) ?: return null
        return ChronoUnit.DAYS.between(startDate, nextPhase.startDate)
    }

    companion object {
        fun getPhaseAtDay(missionDay: Long): MissionPhase {
            return values().lastOrNull { missionDay >= it.getDaysFromMissionStart() } ?: LAUNCH
        }

        fun getTotalMissionDays(): Long {
            return ChronoUnit.DAYS.between(LAUNCH.startDate, DEPLOY.startDate)
        }
    }
}

