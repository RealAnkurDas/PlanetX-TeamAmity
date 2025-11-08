package com.yourcompany.planetxnative.screens

import android.content.Context
import android.content.Intent
import androidx.compose.foundation.background
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
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.yourcompany.planetxnative.UnityHolderActivity

@Composable
fun VisualizerScreen() {
    val context = LocalContext.current
    var selectedPath by remember { mutableStateOf(1) }
    var launchVelocity by remember { mutableStateOf(11.5f) }
    var launchAngle by remember { mutableStateOf(28.5f) }
    
    LazyColumn(
        modifier = Modifier
            .fillMaxSize()
            .background(
                Brush.verticalGradient(
                    colors = listOf(
                        MaterialTheme.colorScheme.background,
                        MaterialTheme.colorScheme.surface
                    )
                )
            )
            .padding(16.dp),
        verticalArrangement = Arrangement.spacedBy(16.dp)
    ) {
        // Header
        item {
            Column(
                modifier = Modifier.fillMaxWidth(),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                Text(
                    "🚀",
                    fontSize = 60.sp
                )
                Text(
                    "Mission Visualizer",
                    style = MaterialTheme.typography.headlineMedium,
                    fontWeight = FontWeight.Bold,
                    color = MaterialTheme.colorScheme.primary
                )
                Text(
                    "Plan and simulate your journey to Justitia",
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.7f),
                    textAlign = TextAlign.Center
                )
            }
        }
        
        // AI Path Optimization
        item {
            Card(
                modifier = Modifier.fillMaxWidth(),
                colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.primaryContainer)
            ) {
                Column(modifier = Modifier.padding(16.dp)) {
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        Icon(
                            Icons.Default.AutoGraph,
                            contentDescription = null,
                            tint = MaterialTheme.colorScheme.primary,
                            modifier = Modifier.size(32.dp)
                        )
                        Spacer(modifier = Modifier.width(12.dp))
                        Column {
                            Text(
                                "AI Trajectory Optimization",
                                style = MaterialTheme.typography.titleLarge,
                                fontWeight = FontWeight.Bold
                            )
                            Text(
                                "Computing optimal paths...",
                                style = MaterialTheme.typography.bodySmall,
                                color = MaterialTheme.colorScheme.onPrimaryContainer.copy(alpha = 0.7f)
                            )
                        }
                    }
                    
                    Spacer(modifier = Modifier.height(16.dp))
                    
                    Text("Top 5 Optimal Trajectories", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.SemiBold)
                    Spacer(modifier = Modifier.height(8.dp))
                    
                    // Path options
                    listOf(
                        Triple("Fuel-Efficient Path", "152 days, 340 kg fuel", 95),
                        Triple("Fastest Path", "128 days, 425 kg fuel", 88),
                        Triple("Balanced Path", "143 days, 365 kg fuel", 92),
                        Triple("Safe Path (Low Solar Activity)", "158 days, 350 kg fuel", 87),
                        Triple("Gravity-Assist Path", "147 days, 355 kg fuel", 90)
                    ).forEachIndexed { index, (name, details, score) ->
                        PathOption(
                            pathNumber = index + 1,
                            name = name,
                            details = details,
                            score = score,
                            isSelected = selectedPath == index + 1,
                            onClick = { selectedPath = index + 1 }
                        )
                    }
                }
            }
        }
        
        // Launch Parameters
        item {
            Card(modifier = Modifier.fillMaxWidth()) {
                Column(modifier = Modifier.padding(16.dp)) {
                    Text(
                        "Launch Parameters",
                        style = MaterialTheme.typography.titleLarge,
                        fontWeight = FontWeight.Bold
                    )
                    Text(
                        "Adjust initial conditions for your mission",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f)
                    )
                    
                    Spacer(modifier = Modifier.height(16.dp))
                    
                    // Launch Velocity Slider
                    Column {
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceBetween
                        ) {
                            Text("Launch Velocity", style = MaterialTheme.typography.bodyMedium)
                            Text("${String.format("%.1f", launchVelocity)} km/s", 
                                style = MaterialTheme.typography.bodyMedium,
                                fontWeight = FontWeight.Bold,
                                color = MaterialTheme.colorScheme.primary)
                        }
                        Slider(
                            value = launchVelocity,
                            onValueChange = { launchVelocity = it },
                            valueRange = 10f..15f,
                            steps = 49
                        )
                    }
                    
                    Spacer(modifier = Modifier.height(12.dp))
                    
                    // Launch Angle Slider
                    Column {
                        Row(
                            modifier = Modifier.fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceBetween
                        ) {
                            Text("Launch Angle", style = MaterialTheme.typography.bodyMedium)
                            Text("${String.format("%.1f", launchAngle)}°", 
                                style = MaterialTheme.typography.bodyMedium,
                                fontWeight = FontWeight.Bold,
                                color = MaterialTheme.colorScheme.primary)
                        }
                        Slider(
                            value = launchAngle,
                            onValueChange = { launchAngle = it },
                            valueRange = 0f..45f,
                            steps = 89
                        )
                    }
                }
            }
        }
        
        // Simulation Info
        item {
            Card(
                modifier = Modifier.fillMaxWidth(),
                colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.secondaryContainer)
            ) {
                Column(modifier = Modifier.padding(16.dp)) {
                    Row(verticalAlignment = Alignment.CenterVertically) {
                        Icon(Icons.Default.Info, contentDescription = null)
                        Spacer(modifier = Modifier.width(8.dp))
                        Text("How It Works", style = MaterialTheme.typography.titleMedium, fontWeight = FontWeight.Bold)
                    }
                    Spacer(modifier = Modifier.height(8.dp))
                    Text(
                        """
                        The AR/3D simulation uses Reinforcement Learning agents to navigate your spacecraft from Earth to Justitia. 
                        
                        • Choose your optimal path or adjust launch parameters
                        • The AI will continuously optimize trajectory in real-time
                        • Control boosters during flight to influence the path
                        • If you deviate, the AI will automatically recalculate and correct course
                        • Experience a time-accelerated journey through the solar system
                        """.trimIndent(),
                        style = MaterialTheme.typography.bodyMedium,
                        lineHeight = 20.sp
                    )
                }
            }
        }
        
        // Launch Button
        item {
            Button(
                onClick = { launchUnityAR(context) },
                modifier = Modifier
                    .fillMaxWidth()
                    .height(72.dp),
                colors = ButtonDefaults.buttonColors(
                    containerColor = MaterialTheme.colorScheme.primary
                ),
                shape = RoundedCornerShape(16.dp)
            ) {
                Icon(
                    Icons.Default.ViewInAr,
                    contentDescription = null,
                    modifier = Modifier.size(32.dp)
                )
                Spacer(modifier = Modifier.width(16.dp))
                Column(horizontalAlignment = Alignment.Start) {
                    Text(
                        "Launch AR Solar System",
                        fontSize = 18.sp,
                        fontWeight = FontWeight.Bold
                    )
                    Text(
                        "Experience the mission in 3D/AR",
                        fontSize = 12.sp
                    )
                }
            }
        }
        
        // Bottom spacing
        item {
            Spacer(modifier = Modifier.height(32.dp))
        }
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
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 4.dp),
        onClick = onClick,
        colors = CardDefaults.cardColors(
            containerColor = if (isSelected) 
                MaterialTheme.colorScheme.primary.copy(alpha = 0.2f)
            else 
                MaterialTheme.colorScheme.surface
        )
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(12.dp),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Row(verticalAlignment = Alignment.CenterVertically, modifier = Modifier.weight(1f)) {
                Box(
                    modifier = Modifier
                        .size(40.dp)
                        .background(
                            if (isSelected) MaterialTheme.colorScheme.primary else MaterialTheme.colorScheme.surfaceVariant,
                            RoundedCornerShape(8.dp)
                        ),
                    contentAlignment = Alignment.Center
                ) {
                    Text(
                        "$pathNumber",
                        color = if (isSelected) Color.White else MaterialTheme.colorScheme.onSurface,
                        fontWeight = FontWeight.Bold,
                        fontSize = 18.sp
                    )
                }
                Spacer(modifier = Modifier.width(12.dp))
                Column {
                    Text(name, style = MaterialTheme.typography.bodyMedium, fontWeight = FontWeight.SemiBold)
                    Text(details, style = MaterialTheme.typography.bodySmall, 
                        color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f))
                }
            }
            Column(horizontalAlignment = Alignment.End) {
                Text("Score", style = MaterialTheme.typography.labelSmall, 
                    color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f))
                Text("$score%", style = MaterialTheme.typography.titleMedium, 
                    fontWeight = FontWeight.Bold,
                    color = if (score >= 90) Color.Green else MaterialTheme.colorScheme.primary)
            }
        }
    }
}

fun launchUnityAR(context: Context) {
    val intent = Intent(context, UnityHolderActivity::class.java)
    context.startActivity(intent)
}

