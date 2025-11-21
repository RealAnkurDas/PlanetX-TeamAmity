package com.yourcompany.planetxnative

import android.content.Intent
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.ui.Alignment
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.painterResource
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.navigation.NavDestination.Companion.hierarchy
import androidx.navigation.NavGraph.Companion.findStartDestination
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.currentBackStackEntryAsState
import androidx.navigation.compose.rememberNavController
import com.yourcompany.planetxnative.screens.DashboardScreen
import com.yourcompany.planetxnative.screens.EduScreen
import com.yourcompany.planetxnative.screens.VisualizerScreen
import com.yourcompany.planetxnative.ui.theme.PlanetXNativeTheme
import com.yourcompany.planetxnative.ui.theme.*
import com.yourcompany.planetxnative.data.TrajectoryManager

sealed class Screen(val route: String, val title: String, val icon: ImageVector) {
    object Dashboard : Screen("dashboard", "Dashboard", Icons.Default.Dashboard)
    object Edu : Screen("edu", "Learn", Icons.Default.School)
    object Visualizer : Screen("visualizer", "Explore", Icons.Default.ViewInAr)
}

class MainActivity : ComponentActivity() {
    companion object {
        const val SCENE_MENU = 0
        const val SCENE_SOLAR_SYSTEM_3D = 1
        const val SCENE_SOLAR_SYSTEM_AR = 2
        const val SCENE_MBR_PART_3D = 3
        const val SCENE_GA_TRAJECTORY = 4
        const val SCENE_CUSTOM_LAUNCH = 5
    }
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        
        // Initialize trajectory data
        TrajectoryManager.loadTrajectory(this)
        
        setContent {
            PlanetXNativeTheme {
                MainApp()
            }
        }
    }
    
    fun launchUnity(sceneIndex: Int) {
        Intent(this, UnityHolderActivity::class.java).apply {
            putExtra("targetSceneIndex", sceneIndex)
        }.also { startActivity(it) }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun MainApp() {
    val navController = rememberNavController()
    val items = listOf(Screen.Dashboard, Screen.Edu, Screen.Visualizer)
    
    Scaffold(
        modifier = Modifier.fillMaxSize(),
        topBar = {
            TopAppBar(
                title = {
                    Row(
                        verticalAlignment = Alignment.CenterVertically,
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        androidx.compose.foundation.Image(
                            painter = androidx.compose.ui.res.painterResource(id = R.drawable.app_logo_main_mbr_text),
                            contentDescription = "MBR++ Logo",
                            modifier = Modifier
                                .height(72.dp)
                                .padding(start = 0.dp),
                            contentScale = androidx.compose.ui.layout.ContentScale.Fit
                        )
                        Column(
                            horizontalAlignment = Alignment.CenterHorizontally,
                            modifier = Modifier
                                .weight(1f)
                                .padding(horizontal = 8.dp)
                        ) {
                            Text(
                                "Emirates Mission to the",
                                style = MaterialTheme.typography.bodySmall,
                                fontWeight = FontWeight.Bold,
                                color = NeonCyan
                            )
                            Text(
                                "Asteroid Belt",
                                style = MaterialTheme.typography.bodySmall,
                                fontWeight = FontWeight.Bold,
                                color = NeonCyan
                            )
                        }
                        androidx.compose.foundation.Image(
                            painter = androidx.compose.ui.res.painterResource(id = R.drawable.app_logo_main_v3),
                            contentDescription = "App Logo",
                            modifier = Modifier
                                .size(56.dp)
                                .padding(4.dp),
                            contentScale = androidx.compose.ui.layout.ContentScale.Fit
                        )
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = SpaceDeepBlue,
                    titleContentColor = NeonCyan
                )
            )
        },
        bottomBar = {
            NavigationBar(
                containerColor = SpaceDeepBlue
            ) {
                val navBackStackEntry by navController.currentBackStackEntryAsState()
                val currentDestination = navBackStackEntry?.destination
                
                items.forEach { screen ->
                    val iconRes = when(screen.route) {
                        "dashboard" -> R.drawable.dashboard
                        "edu" -> R.drawable.education
                        "visualizer" -> R.drawable.visualizer
                        else -> R.drawable.dashboard
                    }
                    
                    NavigationBarItem(
                        icon = { 
                            Image(
                                painter = painterResource(id = iconRes),
                                contentDescription = screen.title,
                                modifier = Modifier.size(48.dp),
                                contentScale = ContentScale.Fit
                            )
                        },
                        label = { 
                            Text(
                                screen.title.uppercase(),
                                fontWeight = FontWeight.ExtraBold,
                                style = MaterialTheme.typography.labelSmall
                            ) 
                        },
                        selected = currentDestination?.hierarchy?.any { it.route == screen.route } == true,
                        onClick = {
                            navController.navigate(screen.route) {
                                // Pop up to the start destination of the graph to
                                // avoid building up a large stack of destinations
                                popUpTo(navController.graph.findStartDestination().id) {
                                    saveState = true
                                }
                                // Avoid multiple copies of the same destination when
                                // reselecting the same item
                                launchSingleTop = true
                                // Restore state when reselecting a previously selected item
                                restoreState = true
                            }
                        },
                        colors = NavigationBarItemDefaults.colors(
                            selectedIconColor = NeonCyan,
                            selectedTextColor = NeonCyan,
                            indicatorColor = PixelContainer,
                            unselectedIconColor = RocketSilver,
                            unselectedTextColor = RocketSilver
                        )
                    )
                }
            }
        }
    ) { innerPadding ->
        NavHost(
            navController = navController,
            startDestination = Screen.Dashboard.route,
            modifier = Modifier
                .padding(innerPadding)
                .background(SpaceBlack)
        ) {
            composable(Screen.Dashboard.route) { DashboardScreen() }
            composable(Screen.Edu.route) { EduScreen() }
            composable(Screen.Visualizer.route) { VisualizerScreen() }
        }
    }
}