package com.yourcompany.planetxnative.screens

import androidx.compose.foundation.Canvas
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.ui.unit.Dp
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.draw.drawBehind
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.geometry.Size
import androidx.compose.ui.graphics.drawscope.DrawScope
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.Path
import androidx.compose.ui.graphics.drawscope.Stroke
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.yourcompany.planetxnative.data.SampleData
import com.yourcompany.planetxnative.data.MissionClock
import com.yourcompany.planetxnative.ui.theme.*
import com.yourcompany.planetxnative.R
import androidx.compose.runtime.*

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun DashboardScreen() {
    // Live data that updates
    var telemetry by remember { mutableStateOf(SampleData.currentTelemetry) }
    var parts by remember { mutableStateOf(SampleData.spacecraftParts) }
    var insights by remember { mutableStateOf(SampleData.aiInsights) }
    var status by remember { mutableStateOf(SampleData.missionStatus) }
    var missionDay by remember { mutableStateOf(MissionClock.currentMissionDay.value) }
    var missionDate by remember { mutableStateOf(MissionClock.getCurrentMissionDate().toString()) }
    var currentPhase by remember { mutableStateOf(MissionClock.getCurrentPhase().phaseName) }
    
    var realTime by remember { mutableStateOf(java.time.LocalDateTime.now()) }
    
    // Update data periodically
    LaunchedEffect(Unit) {
        while (true) {
            kotlinx.coroutines.delay(100)  // Update every 100ms for live feel
            
            // Update mission clock
            MissionClock.update()
            
            // Update real time
            realTime = java.time.LocalDateTime.now()
            
            // Fetch fresh data
            telemetry = SampleData.currentTelemetry
            parts = SampleData.spacecraftParts
            insights = SampleData.aiInsights
            status = SampleData.missionStatus
            missionDay = MissionClock.currentMissionDay.value
            missionDate = MissionClock.getCurrentMissionDate().toString()
            currentPhase = MissionClock.getCurrentPhase().phaseName
        }
    }
    
    Box(modifier = Modifier.fillMaxSize()) {
        // Animated star field background
        PixelStarField(
            modifier = Modifier.fillMaxSize(),
            starCount = 200,
            animate = true
        )
        
        // Scanline effect overlay
        PixelScanlineEffect(modifier = Modifier.fillMaxSize())
        
        LazyColumn(
            modifier = Modifier
                .fillMaxSize()
                .background(SpaceBlack.copy(alpha = 0.7f))
                .padding(top = 16.dp, start = 16.dp, end = 16.dp, bottom = 0.dp),
            verticalArrangement = Arrangement.spacedBy(16.dp),
            contentPadding = PaddingValues(bottom = 16.dp)
        ) {
        // Mission Clock Card - Very Top
        item {
            MissionClockCard(missionDay, missionDate, currentPhase, realTime)
        }
        
        // Mission Status Card
        item {
            MissionStatusCard(status)
        }
        
        // Minimap at center
        item {
            MinimapCard()
        }
        
        // Telemetry Data
        item {
            TelemetryCard(telemetry)
        }
        
        // AI Insights
        item {
            AIInsightsCard(insights)
        }
        
        // Spacecraft Parts Health
        item {
            SpacecraftHealthCard(parts)
        }
        }
        
        // Animated asteroids flying across - on top of everything
        PixelAsteroidField(
            modifier = Modifier.fillMaxSize(),
            maxAsteroids = 5,
            spawnIntervalMs = 3500L
        )
    }
}

@Composable
fun MissionStatusCard(status: com.yourcompany.planetxnative.data.MissionStatus) {
    PixelCard(
        modifier = Modifier.fillMaxWidth(),
        backgroundColor = PixelContainer,
        borderColor = NeonCyan
    ) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Column {
                    Text("MISSION STATUS", style = MaterialTheme.typography.titleLarge, fontWeight = FontWeight.Bold, color = NeonCyan)
                    Text("CURRENTLY: CRUISE", 
                        style = MaterialTheme.typography.bodyMedium,
                        color = StarWhite.copy(alpha = 0.8f),
                        fontWeight = FontWeight.Bold)
                }
                Image(
                    painter = painterResource(id = R.drawable.mbr_explorer),
                    contentDescription = "MBR Explorer",
                    modifier = Modifier.size(40.dp),
                    contentScale = ContentScale.Fit
                )
            }
            
            Spacer(modifier = Modifier.height(16.dp))
            
            // Progress bar with phases
            Column {
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween
                ) {
                    listOf("LAUNCH", "CRUISE", "APPROACH", "ORBIT", "LANDING").forEach { phase ->
                        Text(
                            phase, 
                            style = MaterialTheme.typography.labelSmall,
                            fontWeight = FontWeight.Bold,
                            color = if (phase == "CRUISE") 
                                NeonCyan 
                            else 
                                RocketSilver.copy(alpha = 0.5f)
                        )
                    }
                }
                Spacer(modifier = Modifier.height(8.dp))
                PixelProgressBar(
                    progress = status.progress,
                    modifier = Modifier.fillMaxWidth(),
                    fillColor = NeonCyan,
                    borderColor = NeonCyan,
                    showPercentage = false
                )
            }
            
            Spacer(modifier = Modifier.height(12.dp))
            
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                Column {
                    Text("NEXT EVENT:", style = MaterialTheme.typography.labelMedium, 
                        color = NeonPink)
                    Text("EARTH GRAVITY ASSIST", style = MaterialTheme.typography.bodyLarge, fontWeight = FontWeight.Bold, color = StarWhite)
                }
                Column(horizontalAlignment = Alignment.End) {
                    Text("T-MINUS:", style = MaterialTheme.typography.labelMedium,
                        color = NeonOrange)
                    Text(status.eventCountdown, style = MaterialTheme.typography.bodyLarge, fontWeight = FontWeight.Bold, color = StarYellow)
                }
            }
    }
}

@Composable
fun MinimapCard() {
    PixelCard(
        modifier = Modifier.fillMaxWidth(),
        backgroundColor = SpaceDeepBlue,
        borderColor = NeonPurple
    ) {
            Text("TRAJECTORY MAP", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold, color = NeonPurple)
            Spacer(modifier = Modifier.height(16.dp))
            
            Column(verticalArrangement = Arrangement.spacedBy(12.dp)) {
                // Row 1: Phases 1-4 (left to right)
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    PhaseBox(R.drawable.mbr_explorer, "Launch", false, Modifier.weight(1f))
                    Text("→", fontSize = 20.sp, color = NeonCyan, fontWeight = FontWeight.Bold)
                    PhaseBox(R.drawable.planet_venus, "Venus GA", true, Modifier.weight(1f))
                    Text("→", fontSize = 20.sp, color = NeonCyan, fontWeight = FontWeight.Bold)
                    PhaseBox(R.drawable.planet_earth, "Earth GA", false, Modifier.weight(1f))
                    Text("→", fontSize = 20.sp, color = NeonCyan, fontWeight = FontWeight.Bold)
                    PhaseBox(R.drawable.asteroid_westerwald, "Westerwald", false, Modifier.weight(1f))
                }
                
                // Connector arrow down
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.End
                ) {
                    Text("↓", fontSize = 24.sp, color = NeonCyan, fontWeight = FontWeight.Bold)
                    Spacer(modifier = Modifier.width(40.dp))
                }
                
                // Row 2: Phases 8-5 (right to left)
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    PhaseBox(R.drawable.planet_mars, "Mars GA", false, Modifier.weight(1f))
                    Text("←", fontSize = 20.sp, color = NeonCyan, fontWeight = FontWeight.Bold)
                    PhaseBox(R.drawable.asteroid_rockox, "Rockox", false, Modifier.weight(1f))
                    Text("←", fontSize = 20.sp, color = NeonCyan, fontWeight = FontWeight.Bold)
                    PhaseBox(R.drawable.asteroid_ghaf, "Ghaf", false, Modifier.weight(1f))
                    Text("←", fontSize = 20.sp, color = NeonCyan, fontWeight = FontWeight.Bold)
                    PhaseBox(R.drawable.asteroid_chimaera, "Chimaera", false, Modifier.weight(1f))
                }
                
                // Connector arrow down
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.Start
                ) {
                    Spacer(modifier = Modifier.width(40.dp))
                    Text("↓", fontSize = 24.sp, color = NeonCyan.copy(alpha = 0.5f), fontWeight = FontWeight.Bold)
                }
                
                // Row 3: Phases 9-12 (left to right)
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    PhaseBox(R.drawable.asteroid_ousha, "Ousha", false, Modifier.weight(1f))
                    Text("→", fontSize = 20.sp, color = NeonCyan.copy(alpha = 0.5f), fontWeight = FontWeight.Bold)
                    PhaseBox(R.drawable.asteroid_moza, "Moza", false, Modifier.weight(1f))
                    Text("→", fontSize = 20.sp, color = NeonCyan.copy(alpha = 0.5f), fontWeight = FontWeight.Bold)
                    PhaseBox(R.drawable.asteroid_justitia, "Justitia", false, Modifier.weight(1f))
                    Text("→", fontSize = 20.sp, color = NeonCyan.copy(alpha = 0.5f), fontWeight = FontWeight.Bold)
                    PhaseBox(R.drawable.mbr_explorer_on_justitia, "Landing", false, Modifier.weight(1f))
                }
            }
            
            Spacer(modifier = Modifier.height(12.dp))
            
            // Progress indicator
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    "[JUL 2028]",
                    style = MaterialTheme.typography.labelSmall,
                    color = RocketSilver,
                    fontWeight = FontWeight.Bold
                )
                PixelBadge(
                    text = "PHASE 2/12",
                    backgroundColor = PixelContainer,
                    textColor = NeonCyan,
                    borderColor = NeonCyan
                )
                Text(
                    "[MAY 2035]",
                    style = MaterialTheme.typography.labelSmall,
                    color = RocketSilver,
                    fontWeight = FontWeight.Bold
                )
            }
    }
}

@Composable
fun PhaseBox(imageResId: Int, label: String, isCurrent: Boolean, modifier: Modifier = Modifier) {
    Column(
        horizontalAlignment = Alignment.CenterHorizontally,
        modifier = modifier
    ) {
        Box(
            modifier = Modifier
                .fillMaxWidth()
                .background(
                    if (isCurrent) PixelContainer else SpaceDeepBlue,
                    RoundedCornerShape(0.dp)
                )
                .border(
                    width = if (isCurrent) 4.dp else 2.dp,
                    color = if (isCurrent) NeonCyan else RocketSilver.copy(alpha = 0.5f),
                    shape = RoundedCornerShape(0.dp)
                )
                .padding(8.dp),
            contentAlignment = Alignment.Center
        ) {
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                verticalArrangement = Arrangement.Center
            ) {
                Image(
                    painter = painterResource(id = imageResId),
                    contentDescription = label,
                    modifier = Modifier.size(if (isCurrent) 36.dp else 32.dp),
                    contentScale = ContentScale.Fit
                )
                Spacer(modifier = Modifier.height(6.dp))
                Text(
                    label,
                    style = MaterialTheme.typography.labelSmall,
                    fontWeight = if (isCurrent) FontWeight.ExtraBold else FontWeight.Bold,
                    color = if (isCurrent) NeonCyan else StarWhite,
                    maxLines = 2,
                    fontSize = if (isCurrent) 10.sp else 9.sp,
                    lineHeight = if (isCurrent) 11.sp else 10.sp
                )
            }
        }
    }
}

@Composable
fun TelemetryCard(telemetry: com.yourcompany.planetxnative.data.SpacecraftTelemetry) {
    PixelCard(
        modifier = Modifier.fillMaxWidth(),
        backgroundColor = SpaceDeepBlue,
        borderColor = NeonGreen
    ) {
            Text("TELEMETRY DATA", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold, color = NeonGreen)
            Spacer(modifier = Modifier.height(12.dp))
            
             Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                TelemetryItem("Velocity", "${String.format("%.2f", telemetry.velocity)} km/s", Icons.Default.Speed)
                TelemetryItem("Fuel", "${String.format("%.2f", telemetry.fuelReserve)}%", Icons.Default.LocalGasStation)
            }
            Spacer(modifier = Modifier.height(12.dp))
            Text("TILT:", style = MaterialTheme.typography.labelMedium, color = NeonGreen, fontWeight = FontWeight.Bold)
            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                TelemetryItem("Pitch", "${String.format("%.2f", telemetry.pitch)}°", Icons.Default.Height)
                TelemetryItem("Roll", "${String.format("%.2f", telemetry.roll)}°", Icons.Default.RotateRight)
            }
    }
}

@Composable
fun RowScope.TelemetryItem(label: String, value: String, icon: androidx.compose.ui.graphics.vector.ImageVector) {
    Column(
        modifier = Modifier
            .weight(1f)
            .background(PixelContainer)
            .border(2.dp, NeonGreen.copy(alpha = 0.3f), RoundedCornerShape(0.dp))
            .padding(8.dp),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Icon(icon, contentDescription = null, tint = NeonGreen, modifier = Modifier.size(24.dp))
        Spacer(modifier = Modifier.height(4.dp))
        Text(value, style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold, color = StarYellow)
        Text(label.uppercase(), style = MaterialTheme.typography.bodySmall, color = RocketSilver)
    }
}

@Composable
fun AIInsightsCard(insights: List<com.yourcompany.planetxnative.data.AIInsight>) {
    PixelCard(
        modifier = Modifier.fillMaxWidth(),
        backgroundColor = PixelContainer,
        borderColor = NeonPink
    ) {
            Row(verticalAlignment = Alignment.CenterVertically) {
                Image(
                    painter = painterResource(id = R.drawable.ai_bot),
                    contentDescription = "AI Bot",
                    modifier = Modifier.size(32.dp),
                    contentScale = ContentScale.Fit
                )
                Spacer(modifier = Modifier.width(8.dp))
                Text("AI THREAT SCAN", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold, color = NeonPink)
            }
            Spacer(modifier = Modifier.height(12.dp))
            
            insights.forEach { insight ->
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(vertical = 6.dp),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    PixelStatusLED(
                        isActive = true,
                        activeColor = when (insight.type) {
                            "CRITICAL" -> DangerRed
                            "WARNING" -> WarningYellow
                            else -> SuccessGreen
                        }
                    )
                    Spacer(modifier = Modifier.width(12.dp))
                    Text(insight.message.uppercase(), 
                        style = MaterialTheme.typography.bodySmall, 
                        color = StarWhite,
                        fontWeight = FontWeight.Bold)
                }
            }
    }
}

@Composable
fun SpacecraftHealthCard(parts: List<com.yourcompany.planetxnative.data.SpacecraftPart>) {
    PixelCard(
        modifier = Modifier.fillMaxWidth(),
        backgroundColor = SpaceDeepBlue,
        borderColor = NeonOrange
    ) {
            Text("SYSTEMS STATUS", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold, color = NeonOrange)
            Spacer(modifier = Modifier.height(12.dp))
            
            // Image 1 with text overlays
            BoxWithConstraints(modifier = Modifier.fillMaxWidth()) {
                val imageWidth = 1080f  // Original image width
                val imageHeight = 1920f  // Original image height
                
                // Calculate scale factor based on rendered width
                val scaleFactor = maxWidth.value / imageWidth
                
                Image(
                    painter = painterResource(id = R.drawable.system_status_1),
                    contentDescription = "System Status 1",
                    modifier = Modifier.fillMaxWidth(),
                    contentScale = ContentScale.FillWidth
                )
                
                // Box 1: High gain antenna at (763, 302)
                Text(
                    text = "${parts.find { it.name.contains("Communication", ignoreCase = true) }?.health ?: 95}%",
                    modifier = Modifier
                        .offset(
                            x = ((763 + 43) * scaleFactor).dp,
                            y = ((302 + 10) * scaleFactor).dp
                        ),
                    color = NeonCyan,
                    fontSize = 20.sp,
                    fontWeight = FontWeight.ExtraBold,
                    style = MaterialTheme.typography.bodyMedium
                )
                
                // Box 2: Hydrazine thrusters at (700, 765)
                Text(
                    text = "${parts.find { it.name.contains("RCS", ignoreCase = true) }?.health ?: 92}%",
                    modifier = Modifier
                        .offset(
                            x = ((700 + 50) * scaleFactor).dp,
                            y = ((765 + 10) * scaleFactor).dp
                        ),
                    color = NeonCyan,
                    fontSize = 20.sp,
                    fontWeight = FontWeight.ExtraBold,
                    style = MaterialTheme.typography.bodyMedium
                )
                
                // Box 3: Propulsion thrusters at (206, 1625)
                Text(
                    text = "${parts.find { it.name.contains("Engine", ignoreCase = true) }?.health ?: 88}%",
                    modifier = Modifier
                        .offset(
                            x = ((206 + 45) * scaleFactor).dp,
                            y = ((1625 + 8) * scaleFactor).dp
                        ),
                    color = NeonCyan,
                    fontSize = 20.sp,
                    fontWeight = FontWeight.ExtraBold,
                    style = MaterialTheme.typography.bodyMedium
                )
            }
            
            // Image 2 with text overlays
            BoxWithConstraints(modifier = Modifier.fillMaxWidth()) {
                val imageWidth = 1080f
                val imageHeight = 1920f
                val scaleFactor = maxWidth.value / imageWidth
                
                Image(
                    painter = painterResource(id = R.drawable.system_status_2),
                    contentDescription = "System Status 2",
                    modifier = Modifier.fillMaxWidth(),
                    contentScale = ContentScale.FillWidth
                )
                
                // Box 1: Instruments at (790, 286)
                Text(
                    text = "${parts.getOrNull(0)?.health ?: 97}%",
                    modifier = Modifier
                        .offset(
                            x = ((790 + 40) * scaleFactor).dp,
                            y = ((286 + 10) * scaleFactor).dp
                        ),
                    color = NeonCyan,
                    fontSize = 20.sp,
                    fontWeight = FontWeight.ExtraBold,
                    style = MaterialTheme.typography.bodyMedium
                )
                
                // Box 2: Thermal Control at (750, 756)
                Text(
                    text = "${parts.getOrNull(1)?.health ?: 93}%",
                    modifier = Modifier
                        .offset(
                            x = ((750 + 50) * scaleFactor).dp,
                            y = ((756 + 17) * scaleFactor).dp
                        ),
                    color = NeonCyan,
                    fontSize = 20.sp,
                    fontWeight = FontWeight.ExtraBold,
                    style = MaterialTheme.typography.bodyMedium
                )
                
                // Box 3: Solar Cells at (120, 1290)
                Text(
                    text = "${parts.getOrNull(2)?.health ?: 89}%",
                    modifier = Modifier
                        .offset(
                            x = ((120 + 50) * scaleFactor).dp,
                            y = ((1290 + 15) * scaleFactor).dp
                        ),
                    color = NeonCyan,
                    fontSize = 20.sp,
                    fontWeight = FontWeight.ExtraBold,
                    style = MaterialTheme.typography.bodyMedium
                )
                
                // Box 4: Hinge at (94, 1671)
                Text(
                    text = "${parts.getOrNull(3)?.health ?: 91}%",
                    modifier = Modifier
                        .offset(
                            x = ((94 + 50) * scaleFactor).dp,
                            y = ((1671 + 25) * scaleFactor).dp
                        ),
                    color = NeonCyan,
                    fontSize = 20.sp,
                    fontWeight = FontWeight.ExtraBold,
                    style = MaterialTheme.typography.bodyMedium
                )
            }
            
            // Image 3 with text overlays
            BoxWithConstraints(modifier = Modifier.fillMaxWidth()) {
                val imageWidth = 1080f
                val imageHeight = 1920f
                val scaleFactor = maxWidth.value / imageWidth
                
                Image(
                    painter = painterResource(id = R.drawable.system_status_3),
                    contentDescription = "System Status 3",
                    modifier = Modifier.fillMaxWidth(),
                    contentScale = ContentScale.FillWidth
                )
                
                // Box 1: Solar cells at (756, 333)
                Text(
                    text = "${parts.getOrNull(2)?.health ?: 89}%",
                    modifier = Modifier
                        .offset(
                            x = ((756 + 45) * scaleFactor).dp,
                            y = ((333 + 15) * scaleFactor).dp
                        ),
                    color = NeonCyan,
                    fontSize = 20.sp,
                    fontWeight = FontWeight.ExtraBold,
                    style = MaterialTheme.typography.bodyMedium
                )
                
                // Box 2: Hinge at (723, 817)
                Text(
                    text = "${parts.getOrNull(3)?.health ?: 91}%",
                    modifier = Modifier
                        .offset(
                            x = ((723 + 55) * scaleFactor).dp,
                            y = ((817 + 17) * scaleFactor).dp
                        ),
                    color = NeonCyan,
                    fontSize = 20.sp,
                    fontWeight = FontWeight.ExtraBold,
                    style = MaterialTheme.typography.bodyMedium
                )
            }
    }
}

@Composable
fun MissionClockCard(missionDay: Long, missionDate: String, currentPhase: String, realTime: java.time.LocalDateTime) {
    PixelGlowBorder(
        modifier = Modifier.fillMaxWidth(),
        baseColor = NeonPurple
    ) {
        Box(
            modifier = Modifier
                .fillMaxWidth()
                .background(PixelSurface)
        ) {
            Column(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(16.dp)
            ) {
                // Real Time Clock
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween
                ) {
                    Text(
                        "REAL TIME:",
                        style = MaterialTheme.typography.labelMedium,
                        color = NeonPurple,
                        fontWeight = FontWeight.Bold
                    )
                    Text(
                        realTime.format(java.time.format.DateTimeFormatter.ofPattern("HH:mm:ss")),
                        style = MaterialTheme.typography.titleMedium,
                        fontWeight = FontWeight.Bold,
                        color = SuccessGreen
                    )
                }
                
                Spacer(modifier = Modifier.height(8.dp))
                PixelDivider(color = NeonPurple)
                Spacer(modifier = Modifier.height(12.dp))
                
                // Mission Time and Phase
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Column {
                        Text(
                            "MISSION CLOCK:",
                            style = MaterialTheme.typography.labelMedium,
                            color = NeonPurple,
                            fontWeight = FontWeight.Bold
                        )
                        Text(
                            "DAY $missionDay",
                            style = MaterialTheme.typography.headlineSmall,
                            fontWeight = FontWeight.Bold,
                            color = StarYellow
                        )
                        Text(
                            missionDate.uppercase(),
                            style = MaterialTheme.typography.bodySmall,
                            color = RocketSilver
                        )
                    }
                    
                    Column(horizontalAlignment = Alignment.End) {
                        Text(
                            "PHASE:",
                            style = MaterialTheme.typography.labelMedium,
                            color = NeonPurple,
                            fontWeight = FontWeight.Bold
                        )
                        Text(
                            currentPhase.uppercase(),
                            style = MaterialTheme.typography.titleMedium,
                            fontWeight = FontWeight.Bold,
                            color = NeonCyan
                        )
                    }
                }
            }
        }
    }
}

