package com.yourcompany.planetxnative.screens

import android.content.Context
import android.content.Intent
import androidx.compose.foundation.Image
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
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.yourcompany.planetxnative.ui.theme.*
import com.yourcompany.planetxnative.UnityHolderActivity
import com.yourcompany.planetxnative.R

@Composable
fun EduScreen() {
    var expandedSection by remember { mutableStateOf<String?>("mission") }
    val context = androidx.compose.ui.platform.LocalContext.current
    
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
                baseColor = NeonCyan
            ) {
                Box(
                    modifier = Modifier
                        .fillMaxWidth()
                        .background(PixelContainer)
                        .padding(16.dp),
                    contentAlignment = Alignment.Center
                ) {
                    Text(
                        "MISSION DATABASE",
                        style = MaterialTheme.typography.headlineSmall,
                        fontWeight = FontWeight.Bold,
                        color = NeonCyan
                    )
                }
            }
        }
        
        // 3D Model Simulation Button
        item {
            SimulationButtonCard(context)
        }
        
        // Expandable Sections
        item {
            ExpandableInfoSection(
                title = "ABOUT THE MISSION",
                icon = Icons.Default.Info,
                color = NeonCyan,
                isExpanded = expandedSection == "mission",
                onToggle = { expandedSection = if (expandedSection == "mission") null else "mission" }
            ) {
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween
                ) {
                    Column(modifier = Modifier.weight(1f)) {
                        InfoContent(
                            items = listOf(
                                "TARGET" to "Asteroid 269 Justitia",
                                "DISTANCE" to "2.8 AU from Sun",
                                "TYPE" to "Main-belt asteroid",
                                "MISSION" to "First landing attempt",
                                "GOAL" to "Study composition & resources",
                                "TECH" to "AI autonomous navigation"
                            )
                        )
                    }
                    Spacer(modifier = Modifier.width(12.dp))
                    Image(
                        painter = painterResource(id = R.drawable.asteroid_justitia),
                        contentDescription = "Justitia",
                        modifier = Modifier.size(80.dp),
                        contentScale = ContentScale.Fit
                    )
                }
            }
        }
        
        item {
            ExpandableInfoSection(
                title = "MISSION OBJECTIVES",
                icon = Icons.Default.Flag,
                color = NeonPink,
                isExpanded = expandedSection == "objectives",
                onToggle = { expandedSection = if (expandedSection == "objectives") null else "objectives" }
            ) {
                Row(
                    modifier = Modifier.fillMaxWidth()
                ) {
                    Column(modifier = Modifier.weight(1f), verticalArrangement = Arrangement.spacedBy(8.dp)) {
                        Text("PRIMARY:", style = MaterialTheme.typography.labelMedium, color = NeonPink, fontWeight = FontWeight.Bold)
                        BulletPoint("Navigate Earth → Justitia (AI-optimized)")
                        BulletPoint("Establish stable orbit")
                        BulletPoint("Deploy landers & collect samples")
                        BulletPoint("Analyze composition & structure")
                        
                        Spacer(modifier = Modifier.height(8.dp))
                        PixelDivider(color = NeonPink.copy(alpha = 0.3f))
                        Spacer(modifier = Modifier.height(8.dp))
                        
                        Text("SECONDARY:", style = MaterialTheme.typography.labelMedium, color = NeonPink, fontWeight = FontWeight.Bold)
                        BulletPoint("Test autonomous navigation")
                        BulletPoint("Validate AI threat detection")
                        BulletPoint("Deep space operations demo")
                        BulletPoint("Assess mining feasibility")
                    }
                    Spacer(modifier = Modifier.width(12.dp))
                    Column(
                        verticalArrangement = Arrangement.spacedBy(12.dp),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        Image(
                            painter = painterResource(id = R.drawable.mbr_explorer_on_justitia),
                            contentDescription = "Explorer on Justitia",
                            modifier = Modifier.size(50.dp),
                            contentScale = ContentScale.Fit
                        )
                        Image(
                            painter = painterResource(id = R.drawable.planet_earth),
                            contentDescription = "Earth",
                            modifier = Modifier.size(50.dp),
                            contentScale = ContentScale.Fit
                        )
                        Image(
                            painter = painterResource(id = R.drawable.space_sample),
                            contentDescription = "Sample",
                            modifier = Modifier.size(50.dp),
                            contentScale = ContentScale.Fit
                        )
                        Image(
                            painter = painterResource(id = R.drawable.ai_bot),
                            contentDescription = "AI",
                            modifier = Modifier.size(50.dp),
                            contentScale = ContentScale.Fit
                        )
                    }
                }
            }
        }
        
        item {
            ExpandableInfoSection(
                title = "ASTEROID 269 JUSTITIA",
                icon = Icons.Default.Science,
                color = NeonPurple,
                isExpanded = expandedSection == "science",
                onToggle = { expandedSection = if (expandedSection == "science") null else "science" }
            ) {
                Row(
                    modifier = Modifier.fillMaxWidth()
                ) {
                    Image(
                        painter = painterResource(id = R.drawable.asteroid_justitia),
                        contentDescription = "Justitia",
                        modifier = Modifier.size(100.dp),
                        contentScale = ContentScale.Fit
                    )
                    Spacer(modifier = Modifier.width(12.dp))
                    Column(modifier = Modifier.weight(1f), verticalArrangement = Arrangement.spacedBy(8.dp)) {
                        InfoContent(
                            items = listOf(
                                "DISCOVERY" to "Sept 21, 1887",
                                "DIAMETER" to "~53 km",
                                "ORBIT PERIOD" to "4.6 years",
                                "DISTANCE" to "2.8 AU from Sun",
                                "TYPE" to "X-type asteroid",
                                "SURFACE" to "Reddish (organic compounds)"
                            )
                        )
                        Spacer(modifier = Modifier.height(8.dp))
                        PixelDivider(color = NeonPurple.copy(alpha = 0.3f))
                        Spacer(modifier = Modifier.height(8.dp))
                        Text(
                            "Reddish surface indicates organic compounds - key to studying life's building blocks. ML algorithms optimize trajectory using solar wind & gravity data.",
                            style = MaterialTheme.typography.bodySmall,
                            color = StarWhite,
                            lineHeight = 16.sp
                        )
                    }
                }
            }
        }
        
        item {
            ExpandableInfoSection(
                title = "SPACECRAFT SPECS",
                icon = Icons.Default.Build,
                color = NeonOrange,
                isExpanded = expandedSection == "specs",
                onToggle = { expandedSection = if (expandedSection == "specs") null else "specs" }
            ) {
                Row(
                    modifier = Modifier.fillMaxWidth()
                ) {
                    Column(modifier = Modifier.weight(1f), verticalArrangement = Arrangement.spacedBy(12.dp)) {
                        SpecSection("PROPULSION", listOf(
                            "Ion Drive (2000s Isp)",
                            "12 Hydrazine RCS thrusters",
                            "450 kg fuel capacity"
                        ))
                        
                        SpecSection("POWER", listOf(
                            "15 kW solar arrays @ 1 AU",
                            "5 kWh Li-ion backup"
                        ))
                        
                        SpecSection("COMMS", listOf(
                            "X-band: 8 Mbps",
                            "Ka-band: 25 Mbps"
                        ))
                        
                        SpecSection("AI SYSTEMS", listOf(
                            "RL trajectory optimization",
                            "Real-time hazard detection",
                            "Multi-agent coordination"
                        ))
                    }
                    Spacer(modifier = Modifier.width(12.dp))
                    Column(
                        verticalArrangement = Arrangement.spacedBy(12.dp),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        Image(
                            painter = painterResource(id = R.drawable.booster),
                            contentDescription = "Booster",
                            modifier = Modifier.size(50.dp),
                            contentScale = ContentScale.Fit
                        )
                        Image(
                            painter = painterResource(id = R.drawable.mbr_explorer),
                            contentDescription = "Explorer",
                            modifier = Modifier.size(50.dp),
                            contentScale = ContentScale.Fit
                        )
                        Image(
                            painter = painterResource(id = R.drawable.comm),
                            contentDescription = "Comms",
                            modifier = Modifier.size(50.dp),
                            contentScale = ContentScale.Fit
                        )
                        Image(
                            painter = painterResource(id = R.drawable.ai_bot),
                            contentDescription = "AI",
                            modifier = Modifier.size(50.dp),
                            contentScale = ContentScale.Fit
                        )
                    }
                }
            }
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
fun SimulationButtonCard(context: android.content.Context) {
    PixelGlowBorder(
        modifier = Modifier.fillMaxWidth(),
        baseColor = NeonPurple
    ) {
        PixelButton(
            onClick = { 
                (context as? com.yourcompany.planetxnative.MainActivity)?.launchUnity(
                    com.yourcompany.planetxnative.MainActivity.SCENE_MBR_PART_3D
                )
            },
            modifier = Modifier
                .fillMaxWidth()
                .height(140.dp),
            backgroundColor = PixelContainer,
            textColor = NeonPurple,
            borderColor = NeonPurple
        ) {
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(8.dp)
            ) {
                androidx.compose.foundation.Image(
                    painter = androidx.compose.ui.res.painterResource(id = com.yourcompany.planetxnative.R.drawable.mbr_explorer),
                    contentDescription = "MBR Explorer",
                    modifier = Modifier.size(64.dp),
                    contentScale = androidx.compose.ui.layout.ContentScale.Fit
                )
                Spacer(modifier = Modifier.height(8.dp))
                Text(
                    "LAUNCH MBR EXPLORER",
                    fontSize = 18.sp,
                    fontWeight = FontWeight.Bold,
                    color = NeonPurple
                )
                Text(
                    "3D MODEL",
                    fontSize = 18.sp,
                    fontWeight = FontWeight.Bold,
                    color = NeonPurple
                )
                Spacer(modifier = Modifier.height(6.dp))
                Text(
                    "[ CLICK TO VIEW ]",
                    fontSize = 12.sp,
                    fontWeight = FontWeight.Bold,
                    color = StarYellow
                )
                Spacer(modifier = Modifier.height(4.dp))
                Text(
                    "ROTATE • INSPECT • LEARN",
                    fontSize = 10.sp,
                    fontWeight = FontWeight.Bold,
                    color = RocketSilver
                )
            }
        }
    }
}

@Composable
fun ExpandableInfoSection(
    title: String,
    icon: androidx.compose.ui.graphics.vector.ImageVector,
    color: Color,
    isExpanded: Boolean,
    onToggle: () -> Unit,
    content: @Composable () -> Unit
) {
    PixelCard(
        modifier = Modifier
            .fillMaxWidth()
            .clickable { onToggle() },
        backgroundColor = if (isExpanded) PixelSurface else SpaceDeepBlue,
        borderColor = color
    ) {
        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically
        ) {
            Row(verticalAlignment = Alignment.CenterVertically) {
                Icon(
                    icon,
                    contentDescription = null,
                    tint = color,
                    modifier = Modifier.size(28.dp)
                )
                Spacer(modifier = Modifier.width(12.dp))
                Text(
                    title,
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.Bold,
                    color = color
                )
            }
            Text(
                if (isExpanded) "[-]" else "[+]",
                style = MaterialTheme.typography.titleLarge,
                color = color,
                fontWeight = FontWeight.Bold
            )
        }
        
        if (isExpanded) {
            Spacer(modifier = Modifier.height(12.dp))
            PixelDivider(color = color)
            Spacer(modifier = Modifier.height(12.dp))
            content()
        }
    }
}

@Composable
fun InfoContent(items: List<Pair<String, String>>) {
    Column(verticalArrangement = Arrangement.spacedBy(6.dp)) {
        items.forEach { (label, value) ->
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                Text(
                    "$label:",
                    style = MaterialTheme.typography.bodySmall,
                    color = RocketSilver,
                    fontWeight = FontWeight.Bold
                )
                Text(
                    value,
                    style = MaterialTheme.typography.bodySmall,
                    color = StarWhite,
                    fontWeight = FontWeight.Bold
                )
            }
        }
    }
}

@Composable
fun BulletPoint(text: String) {
    Row(
        modifier = Modifier.fillMaxWidth(),
        verticalAlignment = Alignment.Top
    ) {
        Text(
            "[•]",
            style = MaterialTheme.typography.bodySmall,
            color = SuccessGreen,
            fontWeight = FontWeight.Bold
        )
        Spacer(modifier = Modifier.width(8.dp))
        Text(
            text,
            style = MaterialTheme.typography.bodySmall,
            color = StarWhite,
            lineHeight = 16.sp
        )
    }
}

@Composable
fun SpecSection(title: String, specs: List<String>) {
    Column {
        Text(
            "$title:",
            style = MaterialTheme.typography.labelMedium,
            color = NeonOrange,
            fontWeight = FontWeight.Bold
        )
        Spacer(modifier = Modifier.height(4.dp))
        specs.forEach { spec ->
            Row(verticalAlignment = Alignment.Top) {
                Text(
                    "[>]",
                    style = MaterialTheme.typography.bodySmall,
                    color = NeonOrange,
                    fontWeight = FontWeight.Bold
                )
                Spacer(modifier = Modifier.width(6.dp))
                Text(
                    spec,
                    style = MaterialTheme.typography.bodySmall,
                    color = StarWhite
                )
            }
        }
    }
}

