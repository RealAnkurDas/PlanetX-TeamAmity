package com.yourcompany.planetxnative.screens

import androidx.compose.foundation.Canvas
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.Path
import androidx.compose.ui.graphics.drawscope.Stroke
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.yourcompany.planetxnative.data.SampleData

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun DashboardScreen() {
    val telemetry = remember { SampleData.currentTelemetry }
    val parts = remember { SampleData.spacecraftParts }
    val insights = remember { SampleData.aiInsights }
    val status = remember { SampleData.missionStatus }
    
    LazyColumn(
        modifier = Modifier
            .fillMaxSize()
            .background(MaterialTheme.colorScheme.background)
            .padding(16.dp),
        verticalArrangement = Arrangement.spacedBy(16.dp)
    ) {
        // Mission Status Card - Top
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
}

@Composable
fun MissionStatusCard(status: com.yourcompany.planetxnative.data.MissionStatus) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.primaryContainer)
    ) {
        Column(modifier = Modifier.padding(16.dp)) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Column {
                    Text("Mission Status", style = MaterialTheme.typography.titleLarge, fontWeight = FontWeight.Bold)
                    Text("Currently en route to Asteroid Justitia (2.3 AU from Sun)", 
                        style = MaterialTheme.typography.bodyMedium,
                        color = MaterialTheme.colorScheme.onPrimaryContainer.copy(alpha = 0.7f))
                }
                Icon(Icons.Default.Rocket, contentDescription = null, modifier = Modifier.size(32.dp))
            }
            
            Spacer(modifier = Modifier.height(16.dp))
            
            // Progress bar with phases
            Column {
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween
                ) {
                    listOf("Launch", "Cruise", "Approach", "Orbit", "Landing").forEach { phase ->
                        Text(
                            phase, 
                            style = MaterialTheme.typography.labelSmall,
                            fontWeight = if (phase == status.currentPhase) FontWeight.Bold else FontWeight.Normal,
                            color = if (phase == status.currentPhase) 
                                MaterialTheme.colorScheme.primary 
                            else 
                                MaterialTheme.colorScheme.onPrimaryContainer.copy(alpha = 0.5f)
                        )
                    }
                }
                LinearProgressIndicator(
                    progress = { status.progress },
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(8.dp)
                        .clip(RoundedCornerShape(4.dp)),
                )
            }
            
            Spacer(modifier = Modifier.height(12.dp))
            
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                Column {
                    Text("Next Event", style = MaterialTheme.typography.labelMedium, 
                        color = MaterialTheme.colorScheme.onPrimaryContainer.copy(alpha = 0.7f))
                    Text(status.nextEvent, style = MaterialTheme.typography.bodyLarge, fontWeight = FontWeight.SemiBold)
                }
                Column(horizontalAlignment = Alignment.End) {
                    Text("Countdown", style = MaterialTheme.typography.labelMedium,
                        color = MaterialTheme.colorScheme.onPrimaryContainer.copy(alpha = 0.7f))
                    Text(status.eventCountdown, style = MaterialTheme.typography.bodyLarge, fontWeight = FontWeight.SemiBold)
                }
            }
        }
    }
}

@Composable
fun MinimapCard() {
    Card(
        modifier = Modifier.fillMaxWidth()
    ) {
        Column(
            modifier = Modifier.padding(16.dp)
        ) {
            Text("Journey Map", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold)
            Spacer(modifier = Modifier.height(16.dp))
            
            // Journey visualization using Compose Row
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                CheckpointMarker("🌍", "Earth", false)
                Divider(modifier = Modifier.weight(1f).height(2.dp))
                CheckpointMarker("🪐", "Venus", false)
                Divider(modifier = Modifier.weight(1f).height(2.dp))
                CheckpointMarker("🚀", "Current", true)
                Divider(modifier = Modifier.weight(1f).height(2.dp))
                CheckpointMarker("☄️", "Belt", false)
                Divider(modifier = Modifier.weight(1f).height(2.dp))
                CheckpointMarker("🌑", "Justitia", false)
            }
            
            Spacer(modifier = Modifier.height(12.dp))
            
            // Progress indicator
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    "0.0 AU",
                    style = MaterialTheme.typography.labelSmall,
                    color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f)
                )
                Text(
                    "Current: 2.3 AU from Sun",
                    style = MaterialTheme.typography.labelSmall,
                    fontWeight = FontWeight.SemiBold,
                    color = MaterialTheme.colorScheme.primary
                )
                Text(
                    "2.8 AU",
                    style = MaterialTheme.typography.labelSmall,
                    color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f)
                )
            }
        }
    }
}

@Composable
fun CheckpointMarker(emoji: String, label: String, isCurrent: Boolean) {
    Column(
        horizontalAlignment = Alignment.CenterHorizontally,
        modifier = Modifier.width(50.dp)
    ) {
        Box(
            modifier = Modifier
                .size(if (isCurrent) 48.dp else 40.dp)
                .background(
                    if (isCurrent) MaterialTheme.colorScheme.primaryContainer else MaterialTheme.colorScheme.surfaceVariant,
                    CircleShape
                )
                .border(
                    width = if (isCurrent) 3.dp else 0.dp,
                    color = if (isCurrent) MaterialTheme.colorScheme.primary else Color.Transparent,
                    shape = CircleShape
                ),
            contentAlignment = Alignment.Center
        ) {
            Text(
                emoji,
                fontSize = if (isCurrent) 24.sp else 20.sp
            )
        }
        Spacer(modifier = Modifier.height(4.dp))
        Text(
            label,
            style = MaterialTheme.typography.labelSmall,
            fontWeight = if (isCurrent) FontWeight.Bold else FontWeight.Normal,
            color = if (isCurrent) MaterialTheme.colorScheme.primary else MaterialTheme.colorScheme.onSurface.copy(alpha = 0.7f),
            maxLines = 1
        )
    }
}

@Composable
fun TelemetryCard(telemetry: com.yourcompany.planetxnative.data.SpacecraftTelemetry) {
    Card(modifier = Modifier.fillMaxWidth()) {
        Column(modifier = Modifier.padding(16.dp)) {
            Text("Spacecraft Telemetry", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold)
            Spacer(modifier = Modifier.height(12.dp))
            
            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                TelemetryItem("Velocity", "${telemetry.velocity} km/s", Icons.Default.Speed)
                TelemetryItem("Distance", "${telemetry.distanceFromSun} AU", Icons.Default.Public)
            }
            Spacer(modifier = Modifier.height(12.dp))
            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                TelemetryItem("Altitude", "${String.format("%.0f", telemetry.altitude)} km", Icons.Default.Height)
                TelemetryItem("Fuel", "${telemetry.fuelReserve}%", Icons.Default.LocalGasStation)
            }
            Spacer(modifier = Modifier.height(12.dp))
            Text("Attitude", style = MaterialTheme.typography.labelMedium, color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.7f))
            Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                Text("Pitch: ${telemetry.pitch}°", style = MaterialTheme.typography.bodySmall)
                Text("Roll: ${telemetry.roll}°", style = MaterialTheme.typography.bodySmall)
                Text("Yaw: ${telemetry.yaw}°", style = MaterialTheme.typography.bodySmall)
            }
        }
    }
}

@Composable
fun RowScope.TelemetryItem(label: String, value: String, icon: androidx.compose.ui.graphics.vector.ImageVector) {
    Column(
        modifier = Modifier.weight(1f),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Icon(icon, contentDescription = null, tint = MaterialTheme.colorScheme.primary)
        Text(value, style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold)
        Text(label, style = MaterialTheme.typography.bodySmall, color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f))
    }
}

@Composable
fun AIInsightsCard(insights: List<com.yourcompany.planetxnative.data.AIInsight>) {
    Card(modifier = Modifier.fillMaxWidth()) {
        Column(modifier = Modifier.padding(16.dp)) {
            Row(verticalAlignment = Alignment.CenterVertically) {
                Icon(Icons.Default.Psychology, contentDescription = null, tint = MaterialTheme.colorScheme.primary)
                Spacer(modifier = Modifier.width(8.dp))
                Text("AI Insights & Threat Detection", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold)
            }
            Spacer(modifier = Modifier.height(12.dp))
            
            insights.forEach { insight ->
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(vertical = 6.dp),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Box(
                        modifier = Modifier
                            .size(8.dp)
                            .clip(CircleShape)
                            .background(
                                when (insight.type) {
                                    "CRITICAL" -> Color.Red
                                    "WARNING" -> Color(0xFFFFA500)
                                    else -> Color.Green
                                }
                            )
                    )
                    Spacer(modifier = Modifier.width(12.dp))
                    Text(insight.message, style = MaterialTheme.typography.bodyMedium)
                }
            }
        }
    }
}

@Composable
fun SpacecraftHealthCard(parts: List<com.yourcompany.planetxnative.data.SpacecraftPart>) {
    Card(modifier = Modifier.fillMaxWidth()) {
        Column(modifier = Modifier.padding(16.dp)) {
            Text("Spacecraft Systems Health", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold)
            Spacer(modifier = Modifier.height(12.dp))
            
            parts.forEach { part ->
                Column(modifier = Modifier.padding(vertical = 4.dp)) {
                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.SpaceBetween
                    ) {
                        Text(part.name, style = MaterialTheme.typography.bodyMedium)
                        Text(
                            "${part.health}%",
                            style = MaterialTheme.typography.bodyMedium,
                            fontWeight = FontWeight.Bold,
                            color = when {
                                part.health >= 90 -> Color.Green
                                part.health >= 70 -> Color(0xFFFFA500)
                                else -> Color.Red
                            }
                        )
                    }
                    LinearProgressIndicator(
                        progress = { part.health / 100f },
                        modifier = Modifier
                            .fillMaxWidth()
                            .height(6.dp)
                            .clip(RoundedCornerShape(3.dp)),
                        color = when {
                            part.health >= 90 -> Color.Green
                            part.health >= 70 -> Color(0xFFFFA500)
                            else -> Color.Red
                        }
                    )
                }
            }
        }
    }
}

