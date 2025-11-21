package com.yourcompany.planetxnative.ui.theme

import android.app.Activity
import android.os.Build
import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.dynamicDarkColorScheme
import androidx.compose.material3.dynamicLightColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable
import androidx.compose.ui.platform.LocalContext

// Pixel Art Space Theme - Always Dark Mode
private val PixelSpaceColorScheme = darkColorScheme(
    primary = NeonCyan,
    onPrimary = SpaceBlack,
    primaryContainer = PixelContainer,
    onPrimaryContainer = NeonCyan,
    
    secondary = NeonPink,
    onSecondary = SpaceBlack,
    secondaryContainer = PixelSurfaceVariant,
    onSecondaryContainer = NeonPink,
    
    tertiary = NeonPurple,
    onTertiary = SpaceBlack,
    tertiaryContainer = PixelSurface,
    onTertiaryContainer = NeonPurple,
    
    background = SpaceBlack,
    onBackground = StarWhite,
    
    surface = SpaceDeepBlue,
    onSurface = StarWhite,
    surfaceVariant = PixelSurfaceVariant,
    onSurfaceVariant = StarWhite,
    
    error = DangerRed,
    onError = StarWhite,
    
    outline = NeonCyan,
    outlineVariant = SpaceMidnight
)

@Composable
fun PlanetXNativeTheme(
    darkTheme: Boolean = true, // Always use dark theme for pixel art space aesthetic
    dynamicColor: Boolean = false, // Disable dynamic colors to maintain pixel art theme
    content: @Composable () -> Unit
) {
    // Always use our custom pixel space color scheme
    val colorScheme = PixelSpaceColorScheme

    MaterialTheme(
        colorScheme = colorScheme,
        typography = Typography,
        content = content
    )
}