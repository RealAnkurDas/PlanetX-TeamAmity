package com.yourcompany.planetxnative.screens

import android.content.Context
import android.content.Intent
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Brush
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.yourcompany.planetxnative.UnityHolderActivity
import com.yourcompany.planetxnative.ui.theme.*
import com.yourcompany.planetxnative.R
import androidx.compose.foundation.border

@Composable
fun VisualizerScreen() {
    val context = LocalContext.current
    var selectedPath by remember { mutableStateOf(1) }
    
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
        // Header
        item {
            PixelGlowBorder(
                modifier = Modifier.fillMaxWidth(),
                baseColor = NeonPink
            ) {
                Box(
                    modifier = Modifier
                        .fillMaxWidth()
                        .background(PixelContainer)
                        .padding(16.dp)
                ) {
                    Column(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        Text(
                            "🚀",
                            fontSize = 60.sp
                        )
                        Text(
                            "TRAJECTORY EXPLORER",
                            style = MaterialTheme.typography.headlineSmall,
                            fontWeight = FontWeight.Bold,
                            color = NeonPink
                        )
                        Text(
                            "PLAN • SIMULATE • OPTIMIZE",
                            style = MaterialTheme.typography.bodySmall,
                            color = RocketSilver,
                            fontWeight = FontWeight.Bold,
                            textAlign = TextAlign.Center
                        )
                    }
                }
            }
        }
        
        // Mission Trajectory Button
        item {
            PixelCard(
                modifier = Modifier.fillMaxWidth(),
                backgroundColor = PixelSurface,
                borderColor = NeonGreen
            ) {
                Text("MISSION TRAJECTORY", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold, color = NeonGreen)
                Spacer(modifier = Modifier.height(8.dp))
                Text(
                    "UAE SPACE AGENCY OFFICIAL ROUTE",
                    style = MaterialTheme.typography.bodySmall,
                    color = RocketSilver,
                    fontWeight = FontWeight.Bold
                )
                Spacer(modifier = Modifier.height(12.dp))
                
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.spacedBy(12.dp)
                ) {
                    PixelButton(
                        onClick = { 
                            (context as? com.yourcompany.planetxnative.MainActivity)?.launchUnity(
                                com.yourcompany.planetxnative.MainActivity.SCENE_SOLAR_SYSTEM_3D
                            )
                        },
                        modifier = Modifier
                            .weight(1f)
                            .height(60.dp),
                        backgroundColor = PixelContainer,
                        textColor = NeonGreen,
                        borderColor = NeonGreen
                    ) {
                        Column(horizontalAlignment = Alignment.CenterHorizontally) {
                            Text("LAUNCH 3D", fontSize = 14.sp, fontWeight = FontWeight.Bold)
                        }
                    }
                    
                    PixelButton(
                        onClick = { 
                            (context as? com.yourcompany.planetxnative.MainActivity)?.launchUnity(
                                com.yourcompany.planetxnative.MainActivity.SCENE_SOLAR_SYSTEM_AR
                            )
                        },
                        modifier = Modifier
                            .weight(1f)
                            .height(60.dp),
                        backgroundColor = PixelContainer,
                        textColor = NeonGreen,
                        borderColor = NeonGreen
                    ) {
                        Column(horizontalAlignment = Alignment.CenterHorizontally) {
                            Text("LAUNCH AR", fontSize = 14.sp, fontWeight = FontWeight.Bold)
                        }
                    }
                }
            }
        }
        
        // Genetic Algorithm Trajectory
        item {
            PixelCard(
                modifier = Modifier.fillMaxWidth(),
                backgroundColor = PixelContainer,
                borderColor = NeonCyan
            ) {
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        androidx.compose.foundation.Image(
                            painter = androidx.compose.ui.res.painterResource(id = com.yourcompany.planetxnative.R.drawable.ai_bot),
                            contentDescription = "AI Bot",
                            modifier = androidx.compose.ui.Modifier.size(40.dp),
                            contentScale = androidx.compose.ui.layout.ContentScale.Fit
                        )
                        Spacer(modifier = Modifier.width(12.dp))
                        Column {
                            Text(
                                "GENETIC ALGORITHM",
                                style = MaterialTheme.typography.titleLarge,
                                fontWeight = FontWeight.Bold,
                                color = NeonCyan
                            )
                            Text(
                                "OPTIMIZED TRAJECTORY",
                                style = MaterialTheme.typography.titleMedium,
                                fontWeight = FontWeight.Bold,
                                color = NeonCyan
                            )
                            Text(
                                "[COMPUTING ROUTES...]",
                                style = MaterialTheme.typography.bodySmall,
                                color = SuccessGreen,
                                fontWeight = FontWeight.Bold
                            )
                        }
                    }
                    
                    Spacer(modifier = Modifier.height(16.dp))
                    
                    PixelButton(
                        onClick = { 
                            (context as? com.yourcompany.planetxnative.MainActivity)?.launchUnity(
                                com.yourcompany.planetxnative.MainActivity.SCENE_GA_TRAJECTORY
                            )
                        },
                        modifier = Modifier
                            .fillMaxWidth()
                            .height(60.dp),
                        backgroundColor = NeonCyan,
                        textColor = SpaceBlack,
                        borderColor = NeonCyan
                    ) {
                        Text("LAUNCH SIMULATION", fontSize = 16.sp, fontWeight = FontWeight.Bold)
                    }
            }
        }
        
        // Launch Parameters
        item {
            PixelCard(
                modifier = Modifier.fillMaxWidth(),
                backgroundColor = SpaceDeepBlue,
                borderColor = NeonOrange
            ) {
                    Text(
                        "BECOME A SPACE EXPLORER!",
                        style = MaterialTheme.typography.titleLarge,
                        fontWeight = FontWeight.Bold,
                        color = NeonOrange
                    )
                    Spacer(modifier = Modifier.height(8.dp))
                    Text(
                        "RANDOMIZED LAUNCH PARAMETERS",
                        style = MaterialTheme.typography.bodyMedium,
                        color = StarYellow,
                        fontWeight = FontWeight.Bold
                    )
                    Spacer(modifier = Modifier.height(12.dp))
                    Text(
                        "Each launch uses randomly generated parameters to explore different trajectory possibilities. This helps train and improve the path finding algorithm by testing diverse scenarios.",
                        style = MaterialTheme.typography.bodySmall,
                        color = StarWhite,
                        fontWeight = FontWeight.Bold,
                        lineHeight = 18.sp
                    )
                    Spacer(modifier = Modifier.height(16.dp))
                    Text(
                        "[ CONTRIBUTING TO AI TRAINING ]",
                        style = MaterialTheme.typography.bodySmall,
                        color = SuccessGreen,
                        fontWeight = FontWeight.Bold
                    )
                    
                    Spacer(modifier = Modifier.height(20.dp))
                    
                    PixelGlowBorder(
                        modifier = Modifier.fillMaxWidth(),
                        baseColor = SuccessGreen
                    ) {
                        PixelButton(
                            onClick = { 
                                (context as? com.yourcompany.planetxnative.MainActivity)?.launchUnity(
                                    com.yourcompany.planetxnative.MainActivity.SCENE_CUSTOM_LAUNCH
                                )
                            },
                            modifier = Modifier
                                .fillMaxWidth()
                                .height(70.dp),
                            backgroundColor = PixelContainer,
                            textColor = SuccessGreen,
                            borderColor = SuccessGreen
                        ) {
                            Row(
                                verticalAlignment = Alignment.CenterVertically,
                                horizontalArrangement = Arrangement.Center,
                                modifier = Modifier.fillMaxWidth()
                            ) {
                                Text("🚀", fontSize = 32.sp)
                                Spacer(modifier = Modifier.width(12.dp))
                                Column(horizontalAlignment = Alignment.Start) {
                                    Text("LAUNCH WITH RANDOM PARAMETERS", fontSize = 16.sp, fontWeight = FontWeight.Bold)
                                    Text("HELP TRAIN THE PATH FINDING AI", fontSize = 11.sp, fontWeight = FontWeight.Bold)
                                }
                            }
                        }
                    }
            }
        }
        
        // Simulation Info
        item {
            PixelCard(
                modifier = Modifier.fillMaxWidth(),
                backgroundColor = PixelSurface,
                borderColor = NeonPurple
            ) {
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        Text("ℹ️", fontSize = 24.sp)
                        Spacer(modifier = Modifier.width(8.dp))
                        Text("SIMULATION INFO", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold, color = NeonPurple)
                    }
                    Spacer(modifier = Modifier.height(8.dp))
                    PixelDivider(color = NeonPurple)
                    Spacer(modifier = Modifier.height(12.dp))
                    Text(
                        "3 SIMULATIONS AVAILABLE:",
                        style = MaterialTheme.typography.labelLarge,
                        color = NeonPurple,
                        fontWeight = FontWeight.Bold
                    )
                    Spacer(modifier = Modifier.height(8.dp))
                    Text(
                        """
                        [1] ACTUAL TRAJECTORY
                        Official UAE Space Agency route
                        
                        [2] GENETIC ALGORITHM (AI)
                        AI-optimized path using evolution
                        
                        [3] RANDOMIZED EXPLORATION
                        Random parameters to train path finding
                        """.trimIndent(),
                        style = MaterialTheme.typography.bodySmall,
                        lineHeight = 18.sp,
                        color = StarWhite,
                        fontWeight = FontWeight.Bold
                    )
            }
        }
        
        // Bottom spacing
        item {
            Spacer(modifier = Modifier.height(32.dp))
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
fun PathOption(
    pathNumber: Int,
    name: String,
    details: String,
    score: Int,
    isSelected: Boolean,
    onClick: () -> Unit
) {
    Box(
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 4.dp)
            .border(
                width = if (isSelected) 3.dp else 2.dp,
                color = if (isSelected) NeonCyan else RocketSilver.copy(alpha = 0.5f),
                shape = RoundedCornerShape(0.dp)
            )
            .background(
                if (isSelected) PixelSurface else SpaceDeepBlue
            )
            .clickable { onClick() }
            .padding(12.dp)
    ) {
        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Row(verticalAlignment = Alignment.CenterVertically, modifier = Modifier.weight(1f)) {
                Box(
                    modifier = Modifier
                        .size(40.dp)
                        .background(
                            if (isSelected) PixelContainer else SpaceMidnight,
                            RoundedCornerShape(0.dp)
                        )
                        .border(
                            2.dp,
                            if (isSelected) NeonCyan else RocketSilver,
                            RoundedCornerShape(0.dp)
                        ),
                    contentAlignment = Alignment.Center
                ) {
                    Text(
                        "[$pathNumber]",
                        color = if (isSelected) NeonCyan else RocketSilver,
                        fontWeight = FontWeight.Bold,
                        fontSize = 16.sp
                    )
                }
                Spacer(modifier = Modifier.width(12.dp))
                Column {
                    Text(name.uppercase(), style = MaterialTheme.typography.bodySmall, fontWeight = FontWeight.Bold, color = StarWhite)
                    Text(details.uppercase(), style = MaterialTheme.typography.bodySmall, 
                        color = RocketSilver, fontWeight = FontWeight.Bold)
                }
            }
            Column(horizontalAlignment = Alignment.End) {
                Text("SCORE:", style = MaterialTheme.typography.labelSmall, 
                    color = RocketSilver, fontWeight = FontWeight.Bold)
                Text("$score%", style = MaterialTheme.typography.titleMedium, 
                    fontWeight = FontWeight.Bold,
                    color = if (score >= 90) SuccessGreen else WarningYellow)
            }
        }
    }
}


