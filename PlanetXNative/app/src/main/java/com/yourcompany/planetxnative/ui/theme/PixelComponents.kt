package com.yourcompany.planetxnative.ui.theme

import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.Canvas
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.Path
import androidx.compose.ui.graphics.StrokeCap
import androidx.compose.ui.graphics.drawscope.Stroke
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import kotlin.math.cos
import kotlin.math.sin
import kotlin.random.Random

/**
 * Pixel Art themed Card with retro border styling
 */
@Composable
fun PixelCard(
    modifier: Modifier = Modifier,
    backgroundColor: Color = SpaceDeepBlue,
    borderColor: Color = NeonCyan,
    borderWidth: Dp = 3.dp,
    content: @Composable ColumnScope.() -> Unit
) {
    Box(
        modifier = modifier
            .border(
                width = borderWidth,
                color = borderColor,
                shape = RoundedCornerShape(0.dp) // Sharp corners for pixel art
            )
            .background(backgroundColor)
            .padding(16.dp)
    ) {
        Column(content = content)
    }
}

/**
 * Pixel Art themed Button with retro styling
 */
@Composable
fun PixelButton(
    onClick: () -> Unit,
    modifier: Modifier = Modifier,
    backgroundColor: Color = NeonCyan,
    textColor: Color = SpaceBlack,
    borderColor: Color = NeonCyan,
    enabled: Boolean = true,
    content: @Composable RowScope.() -> Unit
) {
    Button(
        onClick = onClick,
        modifier = modifier.border(3.dp, borderColor, RoundedCornerShape(0.dp)),
        enabled = enabled,
        colors = ButtonDefaults.buttonColors(
            containerColor = backgroundColor,
            contentColor = textColor,
            disabledContainerColor = SpaceMidnight,
            disabledContentColor = RocketSilver
        ),
        shape = RoundedCornerShape(0.dp), // Sharp corners
        elevation = ButtonDefaults.buttonElevation(0.dp, 0.dp, 0.dp),
        content = content
    )
}

/**
 * Animated pixel star background
 */
@Composable
fun PixelStarField(
    modifier: Modifier = Modifier,
    starCount: Int = 50,
    animate: Boolean = true
) {
    val stars = remember {
        List(starCount) {
            Star(
                x = Random.nextFloat(),
                y = Random.nextFloat(),
                size = Random.nextFloat() * 3f + 1f,
                twinkleSpeed = Random.nextFloat() * 2f + 1f
            )
        }
    }
    
    var time by remember { mutableStateOf(0f) }
    
    if (animate) {
        LaunchedEffect(Unit) {
            while (true) {
                kotlinx.coroutines.delay(50)
                time += 0.05f
            }
        }
    }
    
    Canvas(modifier = modifier) {
        val width = size.width
        val height = size.height
        
        stars.forEach { star ->
            val alpha = (sin(time * star.twinkleSpeed) + 1f) / 2f * 0.5f + 0.5f
            drawCircle(
                color = StarWhite.copy(alpha = alpha),
                radius = star.size,
                center = Offset(star.x * width, star.y * height)
            )
        }
    }
}

private data class Star(
    val x: Float,
    val y: Float,
    val size: Float,
    val twinkleSpeed: Float
)

/**
 * Animated pixel asteroids flying across the screen
 */
@Composable
fun PixelAsteroidField(
    modifier: Modifier = Modifier,
    maxAsteroids: Int = 5,
    spawnIntervalMs: Long = 3500L,
    speedRange: ClosedFloatingPointRange<Float> = 2f..5f,
    sizeRange: ClosedFloatingPointRange<Float> = 16f..32f
) {
    val asteroidColors = listOf(
        NeonOrange,
        StarYellow,
        RocketSilver,
        NeonPink,
        DangerRed,
        WarningYellow,
        NeonCyan,
        SuccessGreen
    )
    
    var asteroids by remember { mutableStateOf<List<Asteroid>>(emptyList()) }
    var lastSpawnTime by remember { mutableStateOf(0L) }
    var screenSize by remember { mutableStateOf(androidx.compose.ui.geometry.Size(1080f, 1920f)) }
    
    LaunchedEffect(Unit) {
        while (true) {
            kotlinx.coroutines.delay(16) // ~60 FPS
            val currentTime = System.currentTimeMillis()
            
            // Update asteroid positions
            val margin = sizeRange.endInclusive * 2
            asteroids = asteroids.map { asteroid ->
                asteroid.copy(
                    x = asteroid.x + asteroid.velocityX,
                    y = asteroid.y + asteroid.velocityY
                )
            }.filter { asteroid ->
                // Keep asteroids that are still on screen (with some margin)
                asteroid.x > -margin && asteroid.x < screenSize.width + margin &&
                asteroid.y > -margin && asteroid.y < screenSize.height + margin
            }
            
            // Spawn new asteroid if enough time has passed and we're under the limit
            if (asteroids.size < maxAsteroids && 
                (currentTime - lastSpawnTime) >= spawnIntervalMs) {
                val width = screenSize.width
                val height = screenSize.height
                
                // Don't spawn if screen size is not initialized
                if (width > 0 && height > 0) {
                    // Random spawn side: 0=top, 1=right, 2=bottom, 3=left
                    val side = Random.nextInt(4)
                    val asteroidSize = Random.nextFloat() * (sizeRange.endInclusive - sizeRange.start) + sizeRange.start
                    val speed = Random.nextFloat() * (speedRange.endInclusive - speedRange.start) + speedRange.start
                    
                    val startX: Float
                    val startY: Float
                    val velocityX: Float
                    val velocityY: Float
                    
                    when (side) {
                        0 -> { // Top
                            startX = Random.nextFloat() * width
                            startY = -asteroidSize
                            // Strong diagonal movement: -speed to +speed horizontally
                            velocityX = (Random.nextFloat() - 0.5f) * speed * 1.5f
                            velocityY = speed
                        }
                        1 -> { // Right
                            startX = width + asteroidSize
                            startY = Random.nextFloat() * height
                            velocityX = -speed
                            // Strong diagonal movement: -speed to +speed vertically
                            velocityY = (Random.nextFloat() - 0.5f) * speed * 1.5f
                        }
                        2 -> { // Bottom
                            startX = Random.nextFloat() * width
                            startY = height + asteroidSize
                            // Strong diagonal movement: -speed to +speed horizontally
                            velocityX = (Random.nextFloat() - 0.5f) * speed * 1.5f
                            velocityY = -speed
                        }
                        else -> { // Left
                            startX = -asteroidSize
                            startY = Random.nextFloat() * height
                            velocityX = speed
                            // Strong diagonal movement: -speed to +speed vertically
                            velocityY = (Random.nextFloat() - 0.5f) * speed * 1.5f
                        }
                    }
                    
                    asteroids = asteroids + Asteroid(
                        x = startX,
                        y = startY,
                        velocityX = velocityX,
                        velocityY = velocityY,
                        size = asteroidSize,
                        color = asteroidColors.random(),
                        spawnTime = currentTime
                    )
                    lastSpawnTime = currentTime
                }
            }
        }
    }
    
    Canvas(modifier = modifier) {
        screenSize = size
        
        // Draw all asteroids
        asteroids.forEach { asteroid ->
            // Draw pixel square asteroid
            drawRect(
                color = asteroid.color,
                topLeft = Offset(asteroid.x, asteroid.y),
                size = androidx.compose.ui.geometry.Size(asteroid.size, asteroid.size)
            )
        }
    }
}

private data class Asteroid(
    val x: Float,
    val y: Float,
    val velocityX: Float,
    val velocityY: Float,
    val size: Float,
    val color: Color,
    val spawnTime: Long
)

/**
 * Pixel art progress bar with blocky aesthetic
 */
@Composable
fun PixelProgressBar(
    progress: Float,
    modifier: Modifier = Modifier,
    backgroundColor: Color = SpaceMidnight,
    fillColor: Color = NeonCyan,
    borderColor: Color = NeonCyan,
    height: Dp = 24.dp,
    showPercentage: Boolean = true
) {
    Box(
        modifier = modifier
            .height(height)
            .border(2.dp, borderColor, RoundedCornerShape(0.dp))
            .background(backgroundColor)
    ) {
        Box(
            modifier = Modifier
                .fillMaxHeight()
                .fillMaxWidth(progress.coerceIn(0f, 1f))
                .background(fillColor)
        )
        
        if (showPercentage) {
            Text(
                text = "${(progress * 100).toInt()}%",
                modifier = Modifier.align(Alignment.Center),
                style = MaterialTheme.typography.labelSmall,
                color = if (progress > 0.5f) SpaceBlack else StarWhite
            )
        }
    }
}

/**
 * Retro pixel art badge/tag
 */
@Composable
fun PixelBadge(
    text: String,
    modifier: Modifier = Modifier,
    backgroundColor: Color = PixelContainer,
    textColor: Color = NeonCyan,
    borderColor: Color = NeonCyan
) {
    Box(
        modifier = modifier
            .border(2.dp, borderColor, RoundedCornerShape(0.dp))
            .background(backgroundColor)
            .padding(horizontal = 12.dp, vertical = 6.dp)
    ) {
        Text(
            text = text,
            style = MaterialTheme.typography.labelSmall,
            color = textColor
        )
    }
}

/**
 * Pixel art divider with dashed line effect
 */
@Composable
fun PixelDivider(
    modifier: Modifier = Modifier,
    color: Color = NeonCyan,
    thickness: Dp = 2.dp,
    dashWidth: Dp = 8.dp,
    gapWidth: Dp = 4.dp
) {
    Canvas(
        modifier = modifier
            .fillMaxWidth()
            .height(thickness)
    ) {
        val totalWidth = size.width
        val dashPx = dashWidth.toPx()
        val gapPx = gapWidth.toPx()
        val segmentWidth = dashPx + gapPx
        
        var x = 0f
        while (x < totalWidth) {
            drawLine(
                color = color,
                start = Offset(x, 0f),
                end = Offset((x + dashPx).coerceAtMost(totalWidth), 0f),
                strokeWidth = thickness.toPx(),
                cap = StrokeCap.Square
            )
            x += segmentWidth
        }
    }
}

/**
 * Retro scanning line effect overlay
 */
@Composable
fun PixelScanlineEffect(
    modifier: Modifier = Modifier,
    lineColor: Color = NeonCyan.copy(alpha = 0.05f)
) {
    Canvas(modifier = modifier) {
        val height = size.height
        val lineHeight = 4f
        var y = 0f
        
        while (y < height) {
            drawLine(
                color = lineColor,
                start = Offset(0f, y),
                end = Offset(size.width, y),
                strokeWidth = lineHeight
            )
            y += lineHeight * 2
        }
    }
}

/**
 * Animated pixel art border that glows
 */
@Composable
fun PixelGlowBorder(
    modifier: Modifier = Modifier,
    baseColor: Color = NeonCyan,
    content: @Composable () -> Unit
) {
    var glowIntensity by remember { mutableStateOf(0f) }
    
    LaunchedEffect(Unit) {
        while (true) {
            kotlinx.coroutines.delay(50)
            glowIntensity = (glowIntensity + 0.1f) % (2f * kotlin.math.PI.toFloat())
        }
    }
    
    val alpha = (sin(glowIntensity) + 1f) / 2f * 0.5f + 0.5f
    
    Box(
        modifier = modifier
            .border(3.dp, baseColor.copy(alpha = alpha), RoundedCornerShape(0.dp))
            .padding(2.dp)
            .border(1.dp, baseColor, RoundedCornerShape(0.dp))
    ) {
        content()
    }
}

/**
 * Retro radar/sonar sweep animation
 */
@Composable
fun PixelRadarSweep(
    modifier: Modifier = Modifier,
    color: Color = NeonCyan,
    backgroundColor: Color = SpaceDeepBlue
) {
    var angle by remember { mutableStateOf(0f) }
    
    LaunchedEffect(Unit) {
        while (true) {
            kotlinx.coroutines.delay(30)
            angle = (angle + 3f) % 360f
        }
    }
    
    Canvas(modifier = modifier.size(120.dp)) {
        val centerX = size.width / 2
        val centerY = size.height / 2
        val radius = size.minDimension / 2
        
        // Background circle
        drawCircle(
            color = backgroundColor,
            radius = radius,
            center = Offset(centerX, centerY)
        )
        
        // Outer border
        drawCircle(
            color = color,
            radius = radius,
            center = Offset(centerX, centerY),
            style = Stroke(width = 3f)
        )
        
        // Concentric circles
        for (i in 1..3) {
            drawCircle(
                color = color.copy(alpha = 0.3f),
                radius = radius * i / 4,
                center = Offset(centerX, centerY),
                style = Stroke(width = 1f)
            )
        }
        
        // Sweep line
        val radians = Math.toRadians(angle.toDouble())
        val endX = centerX + (radius * cos(radians)).toFloat()
        val endY = centerY + (radius * sin(radians)).toFloat()
        
        drawLine(
            color = color.copy(alpha = 0.8f),
            start = Offset(centerX, centerY),
            end = Offset(endX, endY),
            strokeWidth = 2f,
            cap = StrokeCap.Round
        )
        
        // Sweep arc (fading trail)
        val path = Path().apply {
            moveTo(centerX, centerY)
            val sweepAngle = 45f
            arcTo(
                rect = androidx.compose.ui.geometry.Rect(
                    centerX - radius,
                    centerY - radius,
                    centerX + radius,
                    centerY + radius
                ),
                startAngleDegrees = angle - sweepAngle,
                sweepAngleDegrees = sweepAngle,
                forceMoveTo = false
            )
            lineTo(centerX, centerY)
            close()
        }
        
        drawPath(
            path = path,
            color = color.copy(alpha = 0.2f)
        )
    }
}

/**
 * Pixel art status indicator (like LED)
 */
@Composable
fun PixelStatusLED(
    isActive: Boolean,
    modifier: Modifier = Modifier,
    activeColor: Color = SuccessGreen,
    inactiveColor: Color = SpaceMidnight
) {
    var pulse by remember { mutableStateOf(0f) }
    
    if (isActive) {
        LaunchedEffect(Unit) {
            while (true) {
                kotlinx.coroutines.delay(50)
                pulse = (pulse + 0.1f) % (2f * kotlin.math.PI.toFloat())
            }
        }
    }
    
    val alpha = if (isActive) (sin(pulse) + 1f) / 2f * 0.5f + 0.5f else 1f
    val color = if (isActive) activeColor.copy(alpha = alpha) else inactiveColor
    
    Box(
        modifier = modifier
            .size(12.dp)
            .border(1.dp, if (isActive) activeColor else RocketSilver, RoundedCornerShape(0.dp))
            .background(color)
    )
}

