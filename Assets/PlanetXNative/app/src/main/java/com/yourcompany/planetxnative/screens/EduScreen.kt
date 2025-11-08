package com.yourcompany.planetxnative.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp

@Composable
fun EduScreen() {
    var selectedPart by remember { mutableStateOf<String?>(null) }
    
    LazyColumn(
        modifier = Modifier
            .fillMaxSize()
            .background(MaterialTheme.colorScheme.background)
            .padding(16.dp),
        verticalArrangement = Arrangement.spacedBy(16.dp)
    ) {
        // Header
        item {
            Text(
                "Mission to Justitia",
                style = MaterialTheme.typography.headlineMedium,
                fontWeight = FontWeight.Bold,
                color = MaterialTheme.colorScheme.primary
            )
        }
        
        // 3D Model Placeholder with clickable points
        item {
            SpacecraftModelCard(
                onPartClick = { selectedPart = it }
            )
        }
        
        // Selected Part Details
        selectedPart?.let { part ->
            item {
                PartDetailCard(part) { selectedPart = null }
            }
        }
        
        // About the Program
        item {
            InfoCard(
                title = "About the Mission",
                icon = Icons.Default.Info,
                content = """
                    The Justitia Exploration Mission represents humanity's first attempt to land on asteroid 269 Justitia, 
                    a large main-belt asteroid located approximately 2.8 AU from the Sun.
                    
                    This ambitious mission aims to study the asteroid's composition, internal structure, and potential 
                    resources while demonstrating advanced autonomous navigation and AI-driven decision-making systems 
                    in deep space operations.
                """.trimIndent()
            )
        }
        
        // Objectives
        item {
            InfoCard(
                title = "Mission Objectives",
                icon = Icons.Default.Flag,
                content = """
                    PRIMARY OBJECTIVES:
                    • Successfully navigate from Earth to Justitia using AI-optimized trajectories
                    • Establish stable orbit around the asteroid
                    • Deploy surface landers and conduct sample collection
                    • Analyze asteroid composition and internal structure
                    
                    SECONDARY OBJECTIVES:
                    • Test advanced autonomous navigation systems
                    • Validate AI-based threat detection and mitigation
                    • Demonstrate long-duration deep space operations
                    • Assess asteroid mining feasibility
                """.trimIndent()
            )
        }
        
        // Scientific Background
        item {
            InfoCard(
                title = "Scientific Background",
                icon = Icons.Default.Science,
                content = """
                    ASTEROID 269 JUSTITIA
                    • Discovery: September 21, 1887
                    • Diameter: ~53 km
                    • Orbital Period: 4.6 years
                    • Distance from Sun: 2.8 AU
                    • Classification: X-type asteroid
                    
                    SCIENTIFIC SIGNIFICANCE:
                    Justitia's reddish surface suggests the presence of organic compounds, making it a prime target 
                    for studying the building blocks of life. Its size and composition could provide insights into 
                    the early solar system's formation and the potential for future resource extraction.
                    
                    TRAJECTORY ANALYSIS:
                    The mission utilizes a Hohmann transfer orbit with multiple gravity-assist maneuvers. 
                    Advanced machine learning algorithms continuously optimize the trajectory based on real-time 
                    solar wind data, gravitational perturbations, and fuel efficiency metrics.
                """.trimIndent()
            )
        }
        
        // Technical Specifications
        item {
            InfoCard(
                title = "Spacecraft Specifications",
                icon = Icons.Default.Build,
                content = """
                    PROPULSION:
                    • Main Engine: Ion Drive (2000s specific impulse)
                    • RCS: 12 Hydrazine thrusters
                    • Fuel Capacity: 450 kg
                    
                    POWER:
                    • Solar Arrays: 15 kW at 1 AU
                    • Battery Backup: 5 kWh Li-ion
                    
                    COMMUNICATIONS:
                    • X-band: 8 Mbps downlink at Earth
                    • Ka-band: 25 Mbps high-gain antenna
                    
                    AUTONOMY:
                    • AI Navigation: Reinforcement Learning-based trajectory optimization
                    • Hazard Detection: Real-time computer vision
                    • Decision Making: Multi-agent coordination system
                """.trimIndent()
            )
        }
    }
}

@Composable
fun SpacecraftModelCard(onPartClick: (String) -> Unit) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .height(300.dp)
    ) {
        Box(
            modifier = Modifier
                .fillMaxSize()
                .padding(16.dp),
            contentAlignment = Alignment.Center
        ) {
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                verticalArrangement = Arrangement.Center
            ) {
                Text(
                    "🛰️",
                    fontSize = 80.sp,
                    modifier = Modifier.padding(bottom = 16.dp)
                )
                Text(
                    "Interactive 3D Model",
                    style = MaterialTheme.typography.titleLarge,
                    fontWeight = FontWeight.Bold
                )
                Text(
                    "Click on components to learn more",
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f)
                )
                
                Spacer(modifier = Modifier.height(24.dp))
                
                // Clickable points of interest
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceEvenly
                ) {
                    ClickableComponent("Solar Panels") { onPartClick("Solar Panels") }
                    ClickableComponent("Main Engine") { onPartClick("Main Engine") }
                    ClickableComponent("Antenna") { onPartClick("Antenna") }
                    ClickableComponent("Sensors") { onPartClick("Sensors") }
                }
            }
        }
    }
}

@Composable
fun ClickableComponent(name: String, onClick: () -> Unit) {
    Column(
        horizontalAlignment = Alignment.CenterHorizontally,
        modifier = Modifier.clickable { onClick() }
    ) {
        Box(
            modifier = Modifier
                .size(40.dp)
                .clip(CircleShape)
                .background(MaterialTheme.colorScheme.primaryContainer)
                .border(2.dp, MaterialTheme.colorScheme.primary, CircleShape),
            contentAlignment = Alignment.Center
        ) {
            Text(
                "i",
                color = MaterialTheme.colorScheme.primary,
                fontWeight = FontWeight.Bold,
                fontSize = 20.sp
            )
        }
        Spacer(modifier = Modifier.height(4.dp))
        Text(
            name,
            style = MaterialTheme.typography.labelSmall,
            color = MaterialTheme.colorScheme.primary
        )
    }
}

@Composable
fun PartDetailCard(partName: String, onDismiss: () -> Unit) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.tertiaryContainer)
    ) {
        Column(modifier = Modifier.padding(16.dp)) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    partName,
                    style = MaterialTheme.typography.titleLarge,
                    fontWeight = FontWeight.Bold
                )
                IconButton(onClick = onDismiss) {
                    Icon(Icons.Default.Close, contentDescription = "Close")
                }
            }
            Spacer(modifier = Modifier.height(8.dp))
            Text(
                when (partName) {
                    "Solar Panels" -> "15 kW deployable solar arrays provide primary power. Designed to maintain efficiency even at 2.8 AU from the Sun with specialized triple-junction cells."
                    "Main Engine" -> "Ion drive propulsion system provides continuous low-thrust acceleration with exceptional fuel efficiency (2000s Isp). Essential for the multi-year journey."
                    "Antenna" -> "High-gain Ka-band antenna maintains 25 Mbps communication with Earth. Automatically tracks Earth's position for optimal signal strength."
                    "Sensors" -> "Suite of cameras, spectrometers, and LIDAR for navigation, science, and hazard detection. AI-enhanced computer vision processes data in real-time."
                    else -> "Component information not available."
                },
                style = MaterialTheme.typography.bodyMedium
            )
        }
    }
}

@Composable
fun InfoCard(title: String, icon: androidx.compose.ui.graphics.vector.ImageVector, content: String) {
    Card(modifier = Modifier.fillMaxWidth()) {
        Column(modifier = Modifier.padding(16.dp)) {
            Row(verticalAlignment = Alignment.CenterVertically) {
                Icon(
                    icon,
                    contentDescription = null,
                    tint = MaterialTheme.colorScheme.primary,
                    modifier = Modifier.size(28.dp)
                )
                Spacer(modifier = Modifier.width(12.dp))
                Text(
                    title,
                    style = MaterialTheme.typography.titleLarge,
                    fontWeight = FontWeight.Bold
                )
            }
            Spacer(modifier = Modifier.height(12.dp))
            Text(
                content,
                style = MaterialTheme.typography.bodyMedium,
                lineHeight = 20.sp
            )
        }
    }
}

