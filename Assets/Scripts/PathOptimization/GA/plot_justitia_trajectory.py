"""
Quick plotter for trajectory_justitia_output.csv
Shows Earth to Justitia (asteroid) trajectory
"""

import pandas as pd
import matplotlib.pyplot as plt
import matplotlib.animation as animation
from matplotlib.patches import Circle
import numpy as np

# Load data
print("Loading Justitia trajectory data...")
df = pd.read_csv('trajectory_justitia_output.csv')

# Separate by type
spacecraft_df = df[df['type'] == 'spacecraft']

# Planets
venus_df = df[df['type'] == 'venus']
earth_df = df[df['type'] == 'earth']
mars_df = df[df['type'] == 'mars']
jupiter_df = df[df['type'] == 'jupiter'] if 'jupiter' in df['type'].values else None
saturn_df = df[df['type'] == 'saturn'] if 'saturn' in df['type'].values else None

# All 7 asteroids
westerwald_df = df[df['type'] == 'westerwald']
chimaera_df = df[df['type'] == 'chimaera']
rockox_df = df[df['type'] == 'rockox']
moza_df = df[df['type'] == 'moza']
ousha_df = df[df['type'] == 'ousha']
ghaf_df = df[df['type'] == 'ghaf']
justitia_df = df[df['type'] == 'justitia']

print(f"Loaded {len(spacecraft_df)} spacecraft positions")
print(f"\nPlanets:")
print(f"  Venus: {len(venus_df)}, Earth: {len(earth_df)}, Mars: {len(mars_df)}")
if jupiter_df is not None:
    print(f"  Jupiter: {len(jupiter_df)}, Saturn: {len(saturn_df) if saturn_df is not None else 0}")
print(f"\n7 Asteroids:")
print(f"  Westerwald: {len(westerwald_df)}, Chimaera: {len(chimaera_df)}, Rockox: {len(rockox_df)}")
print(f"  Moza: {len(moza_df)}, Ousha: {len(ousha_df)}, Ghaf: {len(ghaf_df)}")
print(f"  Justitia: {len(justitia_df)}")

# Create figure
fig, ax = plt.subplots(figsize=(14, 14))
ax.set_aspect('equal')
ax.set_facecolor('#0a0a14')
fig.patch.set_facecolor('#0a0a14')

# Set limits - show up to Saturn's orbit (~9.5 AU)
ax.set_xlim(-6, 6)
ax.set_ylim(-6, 6)

# Plot Sun
ax.plot(0, 0, 'o', color='#FDB813', markersize=30, label='Sun', zorder=10)

# Plot orbital paths
circle_venus = Circle((0, 0), 0.72, fill=False, color='#FFA500', alpha=0.2, linestyle='--', linewidth=0.8)
circle_earth = Circle((0, 0), 1.0, fill=False, color='#4A90E2', alpha=0.25, linestyle='--', linewidth=1)
circle_mars = Circle((0, 0), 1.52, fill=False, color='#E74C3C', alpha=0.2, linestyle='--', linewidth=0.8)
circle_justitia = Circle((0, 0), 2.6, fill=False, color='#9B59B6', alpha=0.25, linestyle='--', linewidth=1)
circle_jupiter = Circle((0, 0), 5.2, fill=False, color='#D35400', alpha=0.2, linestyle='--', linewidth=0.8)

ax.add_patch(circle_venus)
ax.add_patch(circle_earth)
ax.add_patch(circle_mars)
ax.add_patch(circle_justitia)
ax.add_patch(circle_jupiter)

# Plot complete spacecraft trajectory as thin line
ax.plot(spacecraft_df['x_au'], spacecraft_df['y_au'], 
        color='cyan', alpha=0.4, linewidth=1.5, label='Spacecraft Path', zorder=5)

# Plot all 7 asteroid orbits (faint)
asteroid_colors = {
    'westerwald': '#FF6B6B',
    'chimaera': '#4ECDC4',
    'rockox': '#FFE66D',
    'moza': '#95E1D3',
    'ousha': '#F38181',
    'ghaf': '#A8E6CF',
    'justitia': '#9B59B6'  # Final target - purple
}

ax.plot(westerwald_df['x_au'], westerwald_df['y_au'], color=asteroid_colors['westerwald'], 
        alpha=0.2, linewidth=0.6, linestyle=':', label='Westerwald orbit')
ax.plot(chimaera_df['x_au'], chimaera_df['y_au'], color=asteroid_colors['chimaera'], 
        alpha=0.2, linewidth=0.6, linestyle=':', label='Chimaera orbit')
ax.plot(rockox_df['x_au'], rockox_df['y_au'], color=asteroid_colors['rockox'], 
        alpha=0.2, linewidth=0.6, linestyle=':', label='Rockox orbit')
ax.plot(moza_df['x_au'], moza_df['y_au'], color=asteroid_colors['moza'], 
        alpha=0.2, linewidth=0.6, linestyle=':', label='Moza orbit')
ax.plot(ousha_df['x_au'], ousha_df['y_au'], color=asteroid_colors['ousha'], 
        alpha=0.2, linewidth=0.6, linestyle=':', label='Ousha orbit')
ax.plot(ghaf_df['x_au'], ghaf_df['y_au'], color=asteroid_colors['ghaf'], 
        alpha=0.2, linewidth=0.6, linestyle=':', label='Ghaf orbit')
ax.plot(justitia_df['x_au'], justitia_df['y_au'], color=asteroid_colors['justitia'], 
        alpha=0.3, linewidth=0.8, linestyle=':', label='Justitia orbit')

# Initialize animated elements
spacecraft_dot, = ax.plot([], [], 'o', color='white', markersize=10, 
                          label='Spacecraft', zorder=15, markeredgecolor='cyan', markeredgewidth=2)
spacecraft_trail, = ax.plot([], [], '-', color='cyan', linewidth=2.5, alpha=0.9)

venus_dot, = ax.plot([], [], 'o', color='#FFA500', markersize=9, label='Venus', zorder=12)
earth_dot, = ax.plot([], [], 'o', color='#4A90E2', markersize=12, label='Earth', zorder=12)
mars_dot, = ax.plot([], [], 'o', color='#E74C3C', markersize=10, label='Mars', zorder=12)
jupiter_dot, = ax.plot([], [], 'o', color='#D35400', markersize=18, label='Jupiter', zorder=12)
saturn_dot, = ax.plot([], [], 'o', color='#F4A460', markersize=16, label='Saturn', zorder=12)

# All 7 asteroid dots
westerwald_dot, = ax.plot([], [], 'o', color=asteroid_colors['westerwald'], markersize=6, label='Westerwald', zorder=13, markeredgecolor='white', markeredgewidth=0.5)
chimaera_dot, = ax.plot([], [], 'o', color=asteroid_colors['chimaera'], markersize=6, label='Chimaera', zorder=13, markeredgecolor='white', markeredgewidth=0.5)
rockox_dot, = ax.plot([], [], 'o', color=asteroid_colors['rockox'], markersize=6, label='Rockox', zorder=13, markeredgecolor='white', markeredgewidth=0.5)
moza_dot, = ax.plot([], [], 'o', color=asteroid_colors['moza'], markersize=6, label='Moza', zorder=13, markeredgecolor='white', markeredgewidth=0.5)
ousha_dot, = ax.plot([], [], 'o', color=asteroid_colors['ousha'], markersize=6, label='Ousha', zorder=13, markeredgecolor='white', markeredgewidth=0.5)
ghaf_dot, = ax.plot([], [], 'o', color=asteroid_colors['ghaf'], markersize=6, label='Ghaf', zorder=13, markeredgecolor='white', markeredgewidth=0.5)
justitia_dot, = ax.plot([], [], 'o', color=asteroid_colors['justitia'], markersize=10, label='🏁 Justitia', zorder=14, markeredgecolor='white', markeredgewidth=1.5)

# Info text
info_text = ax.text(0.02, 0.98, '', transform=ax.transAxes,
                    color='white', fontsize=11, verticalalignment='top',
                    fontfamily='monospace',
                    bbox=dict(boxstyle='round', facecolor='#1a1a2e', alpha=0.95, edgecolor='cyan'))

# Title and labels
ax.set_xlabel('X (AU)', color='white', fontsize=13, fontweight='bold')
ax.set_ylabel('Y (AU)', color='white', fontsize=13, fontweight='bold')
ax.set_title('MBR Explorer - 7-Asteroid Grand Tour Mission\n' +
             'UAE Asteroid Belt Mission | Gravity Assists: Venus→Earth→Mars',
             color='white', fontsize=15, fontweight='bold', pad=20)

# Legend (compact, three columns for all bodies - top left to avoid clutter)
legend = ax.legend(loc='upper left', ncol=3, facecolor='#1a1a2e', edgecolor='cyan',
                   labelcolor='white', fontsize=7, framealpha=0.9)

# Grid
ax.grid(True, alpha=0.15, color='white', linestyle=':', linewidth=0.5)
ax.tick_params(colors='white', labelsize=10)

# Add text annotation
ax.text(0.02, 0.02, '7 Asteroids | 3 Planetary Flybys | 6 Deep-Space Maneuvers\n' +
                    'Mission Duration: 7 years (2028-2035)',
        transform=ax.transAxes, color='cyan', fontsize=8,
        bbox=dict(boxstyle='round', facecolor='#0a0a14', alpha=0.8, edgecolor='cyan'))

# Animation parameters
trail_length = 100  # Number of points to show in trail
speed_factor = 8    # Skip frames for faster playback

def init():
    spacecraft_dot.set_data([], [])
    spacecraft_trail.set_data([], [])
    venus_dot.set_data([], [])
    earth_dot.set_data([], [])
    mars_dot.set_data([], [])
    jupiter_dot.set_data([], [])
    saturn_dot.set_data([], [])
    westerwald_dot.set_data([], [])
    chimaera_dot.set_data([], [])
    rockox_dot.set_data([], [])
    moza_dot.set_data([], [])
    ousha_dot.set_data([], [])
    ghaf_dot.set_data([], [])
    justitia_dot.set_data([], [])
    info_text.set_text('')
    return (spacecraft_dot, spacecraft_trail, venus_dot, earth_dot, mars_dot, jupiter_dot, saturn_dot,
            westerwald_dot, chimaera_dot, rockox_dot, moza_dot, ousha_dot, ghaf_dot, justitia_dot, info_text)

def animate(frame):
    idx = frame * speed_factor
    if idx >= len(spacecraft_df):
        idx = len(spacecraft_df) - 1
    
    # Spacecraft position
    sc_x = spacecraft_df.iloc[idx]['x_au']
    sc_y = spacecraft_df.iloc[idx]['y_au']
    spacecraft_dot.set_data([sc_x], [sc_y])
    
    # Spacecraft trail
    start_idx = max(0, idx - trail_length)
    trail_x = spacecraft_df.iloc[start_idx:idx]['x_au']
    trail_y = spacecraft_df.iloc[start_idx:idx]['y_au']
    spacecraft_trail.set_data(trail_x, trail_y)
    
    # Body positions
    if idx < len(earth_df):
        # Planets
        venus_dot.set_data([venus_df.iloc[idx]['x_au']], [venus_df.iloc[idx]['y_au']])
        earth_dot.set_data([earth_df.iloc[idx]['x_au']], [earth_df.iloc[idx]['y_au']])
        mars_dot.set_data([mars_df.iloc[idx]['x_au']], [mars_df.iloc[idx]['y_au']])
        
        if jupiter_df is not None and idx < len(jupiter_df):
            jupiter_dot.set_data([jupiter_df.iloc[idx]['x_au']], [jupiter_df.iloc[idx]['y_au']])
        
        if saturn_df is not None and idx < len(saturn_df):
            saturn_dot.set_data([saturn_df.iloc[idx]['x_au']], [saturn_df.iloc[idx]['y_au']])
        
        # All 7 asteroids
        if idx < len(westerwald_df):
            westerwald_dot.set_data([westerwald_df.iloc[idx]['x_au']], [westerwald_df.iloc[idx]['y_au']])
        if idx < len(chimaera_df):
            chimaera_dot.set_data([chimaera_df.iloc[idx]['x_au']], [chimaera_df.iloc[idx]['y_au']])
        if idx < len(rockox_df):
            rockox_dot.set_data([rockox_df.iloc[idx]['x_au']], [rockox_df.iloc[idx]['y_au']])
        if idx < len(moza_df):
            moza_dot.set_data([moza_df.iloc[idx]['x_au']], [moza_df.iloc[idx]['y_au']])
        if idx < len(ousha_df):
            ousha_dot.set_data([ousha_df.iloc[idx]['x_au']], [ousha_df.iloc[idx]['y_au']])
        if idx < len(ghaf_df):
            ghaf_dot.set_data([ghaf_df.iloc[idx]['x_au']], [ghaf_df.iloc[idx]['y_au']])
        if idx < len(justitia_df):
            justitia_dot.set_data([justitia_df.iloc[idx]['x_au']], [justitia_df.iloc[idx]['y_au']])
        
        # Calculate distance to Justitia
        jus_x = justitia_df.iloc[idx]['x_au']
        jus_y = justitia_df.iloc[idx]['y_au']
        dist_to_jus = np.sqrt((sc_x - jus_x)**2 + (sc_y - jus_y)**2)
        
        # Info text
        time_str = spacecraft_df.iloc[idx]['time']
        days_elapsed = idx * 1  # ~1 day per point
        
        info_text.set_text(
            f'Time: {time_str}\n'
            f'Mission Day: {days_elapsed:.0f} ({days_elapsed/365:.2f} years)\n'
            f'Distance to Justitia: {dist_to_jus:.4f} AU\n'
            f'                      ({dist_to_jus * 1.496e8:.0f} km)'
        )
    
    return (spacecraft_dot, spacecraft_trail, venus_dot, earth_dot, mars_dot, jupiter_dot, saturn_dot,
            westerwald_dot, chimaera_dot, rockox_dot, moza_dot, ousha_dot, ghaf_dot, justitia_dot, info_text)

# Create animation
num_frames = len(spacecraft_df) // speed_factor
print(f"\nCreating animation with {num_frames} frames...")

anim = animation.FuncAnimation(
    fig, animate, init_func=init,
    frames=num_frames, interval=30,
    blit=True, repeat=True
)

print("Showing animation... (Close window when done)")
print("The animation will loop continuously.\n")

plt.tight_layout()
plt.show()

