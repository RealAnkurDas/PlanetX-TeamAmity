"""
Interactive Waypoint Extractor for Mission Trajectory
Click points on the trajectory image to extract waypoints for each mission phase
"""

import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
from matplotlib.widgets import Button
import numpy as np
import csv
from datetime import datetime

# Mission phases with dates and colors
MISSION_PHASES = [
    {"name": "Launch", "date": "2028-03-03", "color": "#FF0000"},
    {"name": "Venus Gravity Assist", "date": "2028-07-10", "color": "#FF8800"},
    {"name": "Earth Gravity Assist", "date": "2029-05-24", "color": "#FFFF00"},
    {"name": "Westerwald", "date": "2030-02-18", "color": "#88FF00"},
    {"name": "Chimaera", "date": "2030-06-14", "color": "#00FF00"},
    {"name": "Rockox", "date": "2031-01-14", "color": "#00FFFF"},
    {"name": "Mars Gravity Assist", "date": "2031-09-23", "color": "#0088FF"},
    {"name": "2000 VA28", "date": "2032-07-24", "color": "#4B0082"},
    {"name": "1998 RC76", "date": "2032-12-15", "color": "#8800FF"},
    {"name": "1999 SG6", "date": "2033-09-02", "color": "#FF00FF"},
    {"name": "Justitia Arrival", "date": "2034-10-30", "color": "#FF0088"},
    {"name": "Deploy Lander", "date": "2035-05-01", "color": "#880000"}
]

class WaypointExtractor:
    def __init__(self, image_path):
        self.image_path = image_path
        self.waypoints = []
        self.current_phase_idx = 0
        self.waypoint_counter = 1
        
        # Set up the figure
        self.fig, self.ax = plt.subplots(figsize=(14, 10))
        plt.subplots_adjust(left=0.05, right=0.95, top=0.95, bottom=0.25)
        
        # Load and display image
        img = plt.imread(image_path)
        self.ax.imshow(img)
        self.ax.set_title("Click to add waypoints | Right-click to undo | Press 'Enter' when done", 
                         fontsize=12, fontweight='bold')
        self.ax.axis('off')
        
        # Create phase selection buttons
        self.create_phase_buttons()
        
        # Create control buttons
        self.create_control_buttons()
        
        # Add instructions
        self.add_instructions()
        
        # Connect events
        self.fig.canvas.mpl_connect('button_press_event', self.on_click)
        self.fig.canvas.mpl_connect('key_press_event', self.on_key)
        
        # Store plot objects
        self.waypoint_plots = []
        self.waypoint_texts = []
        
        print("\n=== Waypoint Extractor Started ===")
        print("1. Click a phase button to select it")
        print("2. Click on the image to add waypoints")
        print("3. Right-click to undo last waypoint")
        print("4. Press Enter to finish and save")
        print("================================\n")
    
    def create_phase_buttons(self):
        """Create buttons for each mission phase"""
        button_height = 0.03
        button_width = 0.075
        start_y = 0.15
        spacing = 0.01
        
        self.phase_buttons = []
        
        for i, phase in enumerate(MISSION_PHASES):
            # Create button position (2 rows of 6)
            row = i // 6
            col = i % 6
            ax_btn = plt.axes([0.05 + col * (button_width + spacing), 
                              start_y - row * (button_height + spacing), 
                              button_width, button_height])
            
            button = Button(ax_btn, phase['name'][:12], color=phase['color'], 
                          hovercolor=self.lighten_color(phase['color']))
            button.label.set_fontsize(8)
            button.on_clicked(lambda event, idx=i: self.select_phase(idx))
            self.phase_buttons.append(button)
        
        # Highlight first phase
        self.update_phase_highlight()
    
    def create_control_buttons(self):
        """Create save and clear buttons"""
        ax_save = plt.axes([0.75, 0.05, 0.08, 0.04])
        ax_clear = plt.axes([0.85, 0.05, 0.08, 0.04])
        
        self.btn_save = Button(ax_save, 'Save', color='#90EE90', hovercolor='#7CFC00')
        self.btn_clear = Button(ax_clear, 'Clear All', color='#FFB6C6', hovercolor='#FF69B4')
        
        self.btn_save.on_clicked(self.save_waypoints)
        self.btn_clear.on_clicked(self.clear_waypoints)
    
    def add_instructions(self):
        """Add text with current phase info"""
        self.phase_text = plt.text(0.05, 0.04, self.get_phase_info(), 
                                   transform=self.fig.transFigure, 
                                   fontsize=10, verticalalignment='bottom',
                                   bbox=dict(boxstyle='round', facecolor='wheat', alpha=0.5))
    
    def get_phase_info(self):
        """Get current phase information text"""
        phase = MISSION_PHASES[self.current_phase_idx]
        return f"Current Phase: {phase['name']} | Date: {phase['date']} | Waypoints: {len(self.waypoints)}"
    
    def lighten_color(self, color, amount=0.3):
        """Lighten a hex color"""
        import matplotlib.colors as mc
        import colorsys
        try:
            c = mc.cnames[color]
        except:
            c = color
        c = colorsys.rgb_to_hls(*mc.to_rgb(c))
        return colorsys.hls_to_rgb(c[0], 1 - amount * (1 - c[1]), c[2])
    
    def select_phase(self, phase_idx):
        """Select a mission phase"""
        self.current_phase_idx = phase_idx
        self.update_phase_highlight()
        self.phase_text.set_text(self.get_phase_info())
        self.fig.canvas.draw()
        print(f"Selected phase: {MISSION_PHASES[phase_idx]['name']}")
    
    def update_phase_highlight(self):
        """Update button highlights to show current phase"""
        for i, button in enumerate(self.phase_buttons):
            if i == self.current_phase_idx:
                for spine in button.ax.spines.values():
                    spine.set_linewidth(3)
                    spine.set_edgecolor('black')
            else:
                for spine in button.ax.spines.values():
                    spine.set_linewidth(1)
                    spine.set_edgecolor('gray')
    
    def on_click(self, event):
        """Handle mouse clicks on the image"""
        if event.inaxes != self.ax:
            return
        
        if event.button == 1:  # Left click - add waypoint
            x, y = event.xdata, event.ydata
            phase = MISSION_PHASES[self.current_phase_idx]
            
            waypoint = {
                'id': self.waypoint_counter,
                'x': x,
                'y': y,
                'phase': phase['name'],
                'date': phase['date'],
                'color': phase['color']
            }
            
            self.waypoints.append(waypoint)
            
            # Plot waypoint
            plot, = self.ax.plot(x, y, 'o', color=phase['color'], 
                               markersize=10, markeredgecolor='white', markeredgewidth=2)
            text = self.ax.text(x, y-30, str(self.waypoint_counter), 
                              color='white', fontsize=9, fontweight='bold',
                              ha='center', va='center',
                              bbox=dict(boxstyle='circle', facecolor=phase['color'], 
                                      edgecolor='white', linewidth=2))
            
            self.waypoint_plots.append(plot)
            self.waypoint_texts.append(text)
            
            print(f"Added waypoint #{self.waypoint_counter} at ({x:.1f}, {y:.1f}) - {phase['name']}")
            
            self.waypoint_counter += 1
            self.phase_text.set_text(self.get_phase_info())
            self.fig.canvas.draw()
            
        elif event.button == 3:  # Right click - undo last
            self.undo_last()
    
    def undo_last(self):
        """Remove the last waypoint"""
        if self.waypoints:
            removed = self.waypoints.pop()
            self.waypoint_plots[-1].remove()
            self.waypoint_texts[-1].remove()
            self.waypoint_plots.pop()
            self.waypoint_texts.pop()
            self.waypoint_counter -= 1
            
            print(f"Removed waypoint #{removed['id']}")
            self.phase_text.set_text(self.get_phase_info())
            self.fig.canvas.draw()
    
    def clear_waypoints(self, event=None):
        """Clear all waypoints"""
        if self.waypoints:
            confirm = input("\nAre you sure you want to clear all waypoints? (yes/no): ")
            if confirm.lower() == 'yes':
                for plot in self.waypoint_plots:
                    plot.remove()
                for text in self.waypoint_texts:
                    text.remove()
                
                self.waypoints.clear()
                self.waypoint_plots.clear()
                self.waypoint_texts.clear()
                self.waypoint_counter = 1
                
                self.phase_text.set_text(self.get_phase_info())
                self.fig.canvas.draw()
                print("All waypoints cleared!")
    
    def on_key(self, event):
        """Handle keyboard events"""
        if event.key == 'enter':
            self.save_waypoints()
    
    def save_waypoints(self, event=None):
        """Save waypoints to CSV"""
        if not self.waypoints:
            print("\nNo waypoints to save!")
            return
        
        filename = 'spacecraft_trajectory.csv'
        
        with open(filename, 'w', newline='') as csvfile:
            fieldnames = ['waypoint_id', 'x_pixels', 'y_pixels', 'phase_name', 'phase_date', 'color']
            writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
            
            writer.writeheader()
            for wp in self.waypoints:
                writer.writerow({
                    'waypoint_id': wp['id'],
                    'x_pixels': f"{wp['x']:.2f}",
                    'y_pixels': f"{wp['y']:.2f}",
                    'phase_name': wp['phase'],
                    'phase_date': wp['date'],
                    'color': wp['color']
                })
        
        print(f"\n✓ Saved {len(self.waypoints)} waypoints to {filename}")
        print("\nPhase breakdown:")
        
        # Print summary by phase
        phase_counts = {}
        for wp in self.waypoints:
            phase_counts[wp['phase']] = phase_counts.get(wp['phase'], 0) + 1
        
        for phase_name, count in phase_counts.items():
            print(f"  {phase_name}: {count} waypoints")
        
        print("\nYou can now use this CSV in the Android app!")
        print("Press any key to close...")
    
    def show(self):
        """Display the interactive window"""
        plt.show()


if __name__ == "__main__":
    # Path to the trajectory image
    image_path = "EMA trajectory.jpg"
    
    try:
        extractor = WaypointExtractor(image_path)
        extractor.show()
    except FileNotFoundError:
        print(f"Error: Could not find '{image_path}'")
        print("Please ensure the trajectory image is in the same directory as this script.")
    except Exception as e:
        print(f"Error: {e}")

