"""
Plot multiple trajectories from last 10 generations
Shows GA convergence with best solution highlighted
"""

import pandas as pd
import matplotlib.pyplot as plt
import matplotlib.animation as animation
from matplotlib.patches import Circle
import numpy as np

# Load trajectory data
print("Loading multi-trajectory data...")
traj_df = pd.read_csv('trajectory_justitia_multi.csv')

# Load full detailed data for celestial bodies (from single trajectory export)
try:
    full_df = pd.read_csv('trajectory_justitia_output.csv')
    has_bodies = True
    
    # Extract planets and asteroids
    venus_df = full_df[full_df['type'] == 'venus']
    earth_df = full_df[full_df['type'] == 'earth']
    mars_df = full_df[full_df['type'] == 'mars']
    jupiter_df = full_df[full_df['type'] == 'jupiter'] if 'jupiter' in full_df['type'].values else None
    
    # Asteroids
    westerwald_df = full_df[full_df['type'] == 'westerwald']
    chimaera_df = full_df[full_df['type'] == 'chimaera']
    rockox_df = full_df[full_df['type'] == 'rockox']
    moza_df = full_df[full_df['type'] == 'moza']
    ousha_df = full_df[full_df['type'] == 'ousha']
    ghaf_df = full_df[full_df['type'] == 'ghaf']
    justitia_df = full_df[full_df['type'] == 'justitia']
    
    print(f"Loaded celestial body data for visualization")
except:
    has_bodies = False
    print("Warning: Could not load celestial body data")

# Separate trajectories
trajectory_ids = traj_df['trajectory_id'].unique()
best_id = traj_df[traj_df['is_best'] == 1]['trajectory_id'].iloc[0]

print(f"Loaded {len(trajectory_ids)} spacecraft trajectories")
print(f"Best trajectory ID: {best_id}")

# Create figure
fig, ax = plt.subplots(figsize=(16, 14))
ax.set_aspect('equal')
ax.set_facecolor('#0a0a14')
fig.patch.set_facecolor('#0a0a14')

# Set limits
ax.set_xlim(-4, 4)
ax.set_ylim(-4, 4)

# Plot Sun
ax.plot(0, 0, 'o', color='#FDB813', markersize=30, label='Sun', zorder=10)

# Plot orbital reference circles
circle_earth = Circle((0, 0), 1.0, fill=False, color='#4A90E2', alpha=0.2, linestyle='--', linewidth=1)
circle_mars = Circle((0, 0), 1.52, fill=False, color='#E74C3C', alpha=0.2, linestyle='--', linewidth=1)
circle_belt = Circle((0, 0), 2.6, fill=False, color='#9B59B6', alpha=0.25, linestyle='--', linewidth=1)
ax.add_patch(circle_earth)
ax.add_patch(circle_mars)
ax.add_patch(circle_belt)

# Plot asteroid orbits if available
if has_bodies:
    asteroid_colors = {
        'westerwald': '#FF6B6B',
        'chimaera': '#4ECDC4',
        'rockox': '#FFE66D',
        'moza': '#95E1D3',
        'ousha': '#F38181',
        'ghaf': '#A8E6CF',
        'justitia': '#9B59B6'
    }
    
    ax.plot(westerwald_df['x_au'], westerwald_df['y_au'], color=asteroid_colors['westerwald'], 
            alpha=0.15, linewidth=0.5, linestyle=':')
    ax.plot(chimaera_df['x_au'], chimaera_df['y_au'], color=asteroid_colors['chimaera'], 
            alpha=0.15, linewidth=0.5, linestyle=':')
    ax.plot(rockox_df['x_au'], rockox_df['y_au'], color=asteroid_colors['rockox'], 
            alpha=0.15, linewidth=0.5, linestyle=':')
    ax.plot(moza_df['x_au'], moza_df['y_au'], color=asteroid_colors['moza'], 
            alpha=0.15, linewidth=0.5, linestyle=':')
    ax.plot(ousha_df['x_au'], ousha_df['y_au'], color=asteroid_colors['ousha'], 
            alpha=0.15, linewidth=0.5, linestyle=':')
    ax.plot(ghaf_df['x_au'], ghaf_df['y_au'], color=asteroid_colors['ghaf'], 
            alpha=0.15, linewidth=0.5, linestyle=':')
    ax.plot(justitia_df['x_au'], justitia_df['y_au'], color=asteroid_colors['justitia'], 
            alpha=0.2, linewidth=0.6, linestyle=':')

# Plot all non-best trajectories as faint lines
trajectory_lines = []
trajectory_dots = []
colors = []

# Get generation range for alpha scaling
all_gens = traj_df['generation'].unique()
min_gen = all_gens.min()
max_gen = all_gens.max()

for traj_id in trajectory_ids:
    traj_df_single = traj_df[traj_df['trajectory_id'] == traj_id]
    is_best = traj_df_single['is_best'].iloc[0] == 1
    gen_num = traj_df_single['generation'].iloc[0]
    
    if is_best:
        # Best trajectory - bright cyan, thick
        line, = ax.plot(traj_df_single['x_au'], traj_df_single['y_au'], 
                       color='cyan', alpha=0.95, linewidth=3, label='✨ Best Solution', zorder=8)
        dot, = ax.plot([], [], 'o', color='white', markersize=12, 
                      markeredgecolor='cyan', markeredgewidth=3, zorder=20)
        colors.append('cyan')
    else:
        # Other trajectories - fade based on generation (older = more transparent)
        if max_gen > min_gen:
            gen_frac = (gen_num - min_gen) / (max_gen - min_gen)
        else:
            gen_frac = 1.0
        
        alpha = 0.15 + 0.35 * gen_frac  # Range: 0.15 to 0.50
        alpha = min(0.5, max(0.15, alpha))  # Clamp to valid range
        
        line, = ax.plot(traj_df_single['x_au'], traj_df_single['y_au'], 
                       color='orange', alpha=alpha, linewidth=1, zorder=3)
        dot, = ax.plot([], [], 'o', color='orange', markersize=6, alpha=alpha, zorder=10)
        colors.append('orange')
    
    trajectory_lines.append(line)
    trajectory_dots.append(dot)

# Add animated planet and asteroid dots
if has_bodies:
    venus_dot, = ax.plot([], [], 'o', color='#FFA500', markersize=9, label='Venus', zorder=12)
    earth_dot, = ax.plot([], [], 'o', color='#4A90E2', markersize=12, label='Earth', zorder=12)
    mars_dot, = ax.plot([], [], 'o', color='#E74C3C', markersize=10, label='Mars', zorder=12)
    
    westerwald_dot, = ax.plot([], [], 'o', color=asteroid_colors['westerwald'], markersize=5, zorder=13, markeredgecolor='white', markeredgewidth=0.5)
    chimaera_dot, = ax.plot([], [], 'o', color=asteroid_colors['chimaera'], markersize=5, zorder=13, markeredgecolor='white', markeredgewidth=0.5)
    rockox_dot, = ax.plot([], [], 'o', color=asteroid_colors['rockox'], markersize=5, zorder=13, markeredgecolor='white', markeredgewidth=0.5)
    moza_dot, = ax.plot([], [], 'o', color=asteroid_colors['moza'], markersize=5, zorder=13, markeredgecolor='white', markeredgewidth=0.5)
    ousha_dot, = ax.plot([], [], 'o', color=asteroid_colors['ousha'], markersize=5, zorder=13, markeredgecolor='white', markeredgewidth=0.5)
    ghaf_dot, = ax.plot([], [], 'o', color=asteroid_colors['ghaf'], markersize=5, zorder=13, markeredgecolor='white', markeredgewidth=0.5)
    justitia_dot, = ax.plot([], [], 'o', color=asteroid_colors['justitia'], markersize=9, label='🏁 Justitia', zorder=14, markeredgecolor='white', markeredgewidth=1.5)

# Info text
info_text = ax.text(0.02, 0.98, '', transform=ax.transAxes,
                    color='white', fontsize=11, verticalalignment='top',
                    fontfamily='monospace',
                    bbox=dict(boxstyle='round', facecolor='#1a1a2e', alpha=0.95, edgecolor='cyan'))

# Title
ax.set_xlabel('X (AU)', color='white', fontsize=13, fontweight='bold')
ax.set_ylabel('Y (AU)', color='white', fontsize=13, fontweight='bold')
ax.set_title('MBR Explorer - Genetic Algorithm Convergence\n' +
             f'Last 10 Generations | Best Solution Highlighted',
             color='white', fontsize=16, fontweight='bold', pad=20)

# Legend
legend = ax.legend(loc='upper right', facecolor='#1a1a2e', edgecolor='cyan',
                   labelcolor='white', fontsize=10, framealpha=0.95)

# Grid
ax.grid(True, alpha=0.15, color='white', linestyle=':', linewidth=0.5)
ax.tick_params(colors='white', labelsize=10)

# Add annotation
ax.text(0.02, 0.02, f'{len(trajectory_ids)} Trajectories | Sequential Display\n' +
                    f'Each fades in → holds → fades out\n' +
                    f'Best solution stays at the end',
        transform=ax.transAxes, color='white', fontsize=9,
        bbox=dict(boxstyle='round', facecolor='#1a1a2e', alpha=0.8, edgecolor='cyan'))

# Animation parameters
frames_per_trajectory = 80  # How many frames to animate each trajectory
frames_per_bodies = 40  # How many frames to show planets/asteroids moving

# Sort trajectories by generation for sequential display
trajectory_list = []
for traj_id in trajectory_ids:
    single_traj = traj_df[traj_df['trajectory_id'] == traj_id]
    is_best = single_traj['is_best'].iloc[0] == 1
    gen_num = single_traj['generation'].iloc[0]
    trajectory_list.append((traj_id, gen_num, is_best))

# Sort: best always last, others by generation
trajectory_list.sort(key=lambda x: (x[2], x[1]))  # (is_best, gen_num)

def init():
    for line in trajectory_lines:
        line.set_alpha(0)  # Hide all initially
    for dot in trajectory_dots:
        dot.set_data([], [])
    info_text.set_text('')
    
    if has_bodies:
        venus_dot.set_data([], [])
        earth_dot.set_data([], [])
        mars_dot.set_data([], [])
        westerwald_dot.set_data([], [])
        chimaera_dot.set_data([], [])
        rockox_dot.set_data([], [])
        moza_dot.set_data([], [])
        ousha_dot.set_data([], [])
        ghaf_dot.set_data([], [])
        justitia_dot.set_data([], [])
        return trajectory_lines + trajectory_dots + [info_text, venus_dot, earth_dot, mars_dot, 
                                  westerwald_dot, chimaera_dot, rockox_dot, moza_dot, ousha_dot, ghaf_dot, justitia_dot]
    
    return trajectory_lines + trajectory_dots + [info_text]

def animate(frame):
    # Sequential reveal: show bodies moving first, then add trajectories one by one
    
    # Phase 1: Show planets/asteroids moving (first 40 frames)
    if frame < frames_per_bodies and has_bodies:
        body_idx = int((frame / frames_per_bodies) * len(venus_df))
        
        venus_dot.set_data([venus_df.iloc[body_idx]['x_au']], [venus_df.iloc[body_idx]['y_au']])
        earth_dot.set_data([earth_df.iloc[body_idx]['x_au']], [earth_df.iloc[body_idx]['y_au']])
        mars_dot.set_data([mars_df.iloc[body_idx]['x_au']], [mars_df.iloc[body_idx]['y_au']])
        
        westerwald_dot.set_data([westerwald_df.iloc[body_idx]['x_au']], [westerwald_df.iloc[body_idx]['y_au']])
        chimaera_dot.set_data([chimaera_df.iloc[body_idx]['x_au']], [chimaera_df.iloc[body_idx]['y_au']])
        rockox_dot.set_data([rockox_df.iloc[body_idx]['x_au']], [rockox_df.iloc[body_idx]['y_au']])
        moza_dot.set_data([moza_df.iloc[body_idx]['x_au']], [moza_df.iloc[body_idx]['y_au']])
        ousha_dot.set_data([ousha_df.iloc[body_idx]['x_au']], [ousha_df.iloc[body_idx]['y_au']])
        ghaf_dot.set_data([ghaf_df.iloc[body_idx]['x_au']], [ghaf_df.iloc[body_idx]['y_au']])
        justitia_dot.set_data([justitia_df.iloc[body_idx]['x_au']], [justitia_df.iloc[body_idx]['y_au']])
        
        info_text.set_text(f'Initializing solar system...\nFrame {frame}/{frames_per_bodies}')
        
        if has_bodies:
            return trajectory_lines + trajectory_dots + [info_text, venus_dot, earth_dot, mars_dot, 
                                      westerwald_dot, chimaera_dot, rockox_dot, moza_dot, ousha_dot, ghaf_dot, justitia_dot]
    
    # Phase 2: Show trajectories sequentially (ONE AT A TIME)
    adjusted_frame = frame - frames_per_bodies
    
    # Each trajectory gets: 8 frames fade-in, 12 frames hold, 8 frames fade-out = 28 frames total
    frames_per_cycle = 28
    
    # Which trajectory to show
    current_traj_index = min(adjusted_frame // frames_per_cycle, len(trajectory_list) - 1)
    cycle_frame = adjusted_frame % frames_per_cycle
    
    # Clear all trajectory alphas first
    for i in range(len(trajectory_lines)):
        trajectory_lines[i].set_alpha(0)
        trajectory_dots[i].set_data([], [])
    
    # Show only the current trajectory
    traj_id, gen_num, is_best = trajectory_list[current_traj_index]
    traj_idx = list(trajectory_ids).index(traj_id)
    
    if cycle_frame < 8:
        # Fade in
        fade = cycle_frame / 8.0
    elif cycle_frame < 20:
        # Hold (fully visible)
        fade = 1.0
    else:
        # Fade out
        fade = 1.0 - ((cycle_frame - 20) / 8.0)
    
    if is_best:
        trajectory_lines[traj_idx].set_alpha(0.95 * fade)
    else:
        original_alpha = 0.45  # Make non-best more visible when they're the focus
        trajectory_lines[traj_idx].set_alpha(original_alpha * fade)
    
    # If we're at the last trajectory (best), keep it visible after fade cycle
    if current_traj_index == len(trajectory_list) - 1 and adjusted_frame >= frames_per_cycle * len(trajectory_list):
        trajectory_lines[traj_idx].set_alpha(0.95)  # Best stays visible
    
    # Animate spacecraft dot along current trajectory
    current_traj_data = traj_df[traj_df['trajectory_id'] == traj_id]
    
    # Dot moves along trajectory during hold phase
    if cycle_frame >= 8 and cycle_frame < 20:
        dot_progress = (cycle_frame - 8) / 12.0
        traj_idx_pos = int(dot_progress * len(current_traj_data))
        if traj_idx_pos < len(current_traj_data):
            x = current_traj_data.iloc[traj_idx_pos]['x_au']
            y = current_traj_data.iloc[traj_idx_pos]['y_au']
            trajectory_dots[traj_idx].set_data([x], [y])
    
    # Update planet and asteroid positions (continue animation)
    if has_bodies and len(venus_df) > 0:
        body_idx = min(adjusted_frame * 2, len(venus_df) - 1)
        
        venus_dot.set_data([venus_df.iloc[body_idx]['x_au']], [venus_df.iloc[body_idx]['y_au']])
        earth_dot.set_data([earth_df.iloc[body_idx]['x_au']], [earth_df.iloc[body_idx]['y_au']])
        mars_dot.set_data([mars_df.iloc[body_idx]['x_au']], [mars_df.iloc[body_idx]['y_au']])
        
        westerwald_dot.set_data([westerwald_df.iloc[body_idx]['x_au']], [westerwald_df.iloc[body_idx]['y_au']])
        chimaera_dot.set_data([chimaera_df.iloc[body_idx]['x_au']], [chimaera_df.iloc[body_idx]['y_au']])
        rockox_dot.set_data([rockox_df.iloc[body_idx]['x_au']], [rockox_df.iloc[body_idx]['y_au']])
        moza_dot.set_data([moza_df.iloc[body_idx]['x_au']], [moza_df.iloc[body_idx]['y_au']])
        ousha_dot.set_data([ousha_df.iloc[body_idx]['x_au']], [ousha_df.iloc[body_idx]['y_au']])
        ghaf_dot.set_data([ghaf_df.iloc[body_idx]['x_au']], [ghaf_df.iloc[body_idx]['y_au']])
        justitia_dot.set_data([justitia_df.iloc[body_idx]['x_au']], [justitia_df.iloc[body_idx]['y_au']])
    
    # Info text
    status = "🌟 BEST SOLUTION" if is_best else f"Generation {gen_num}"
    
    if cycle_frame < 8:
        phase = "Fading in..."
    elif cycle_frame < 20:
        phase = "Analyzing..."
    else:
        phase = "Fading out..."
    
    info_text.set_text(
        f'Trajectory {current_traj_index + 1}/{len(trajectory_list)}\n'
        f'{status} | {phase}\n'
        f'Cycle: {cycle_frame}/28\n'
        f'Watching GA converge...'
    )
    
    if has_bodies:
        return trajectory_lines + trajectory_dots + [info_text, venus_dot, earth_dot, mars_dot, 
                                  westerwald_dot, chimaera_dot, rockox_dot, moza_dot, ousha_dot, ghaf_dot, justitia_dot]
    
    return trajectory_lines + trajectory_dots + [info_text]

# Create animation
# Total frames = bodies intro + (28 frames per trajectory: 8 fade-in + 12 hold + 8 fade-out)
frames_per_cycle = 28
num_frames = frames_per_bodies + (len(trajectory_list) * frames_per_cycle) + 60  # Extra 60 for final best display
print(f"\nCreating sequential animation with {num_frames} frames...")
print(f"Will show {len(trajectory_ids)} trajectories ONE AT A TIME")
print(f"  Phase 1: Solar system setup ({frames_per_bodies} frames)")
print(f"  Phase 2: Each trajectory: fade-in → hold → fade-out (28 frames each)")
print(f"  Best solution (CYAN) will appear LAST and stay visible!\n")

anim = animation.FuncAnimation(
    fig, animate, init_func=init,
    frames=num_frames, interval=30,
    blit=True, repeat=True
)

print("Starting sequential trajectory animation...")
print("Watch as each solution appears, then fades away")
print("The BEST solution (cyan) will appear last and stay visible!\n")
plt.tight_layout()
plt.show()

