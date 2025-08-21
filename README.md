
# 🕹️ Grid Tactics Prototype

Welcome to the Grid Tactics prototype—a simple, turn-based strategy game built in Unity, perfect for learning how grid movement, simple pathfinding, and event-driven AI work together! This project is beginner-friendly and uses clear, organized scripts.

---

## What Is This Game?

- You control a player unit on a 10x10 grid.
- The grid has obstacles that block movement.
- There’s an enemy on the grid too—it chases after you!
- Click anywhere to move your player—the game finds the shortest path around barriers.
- After you move, the enemy gets its turn.
- The game ends if you get "boxed in"—with no open tiles to move to.

---

## How to Play

1. **Start the Game**
   - Click "Play" in the Unity Editor to begin.

2. **Moving Your Player**
   - Left-click any open (walkable) square within the grid.
   - Your player will find the shortest possible path and walk there, one tile at a time.

3. **Enemy’s Turn**
   - After your move, the enemy will try to get next to you, moving around obstacles if needed.

4. **Obstacles**
   - Some tiles can’t be walked on. You can set these up in the Unity Editor using an obstacle grid tool (super easy—just click checkboxes!).

5. **Game Over**
   - If you get completely surrounded (no free squares next to you), a “Game Over: Out of Room!” message appears on screen.

---

## How It Works (in Plain English)

- **Grid Creation:** The game sets up a 10x10 board, with every tile keeping track of its own position.
- **Player Moves:** When you click, the player uses a pathfinding algorithm (BFS) to pick the best way to the destination, avoiding walls and other units.
- **Enemy AI:** The enemy listens for your move, then paths toward you—avoiding the same obstacles.
- **Keeping Track:** A UnitManager keeps track of where every piece is, so you and the enemy can never accidentally "walk through" or "stack on" one another.
- **Turn Flow:** You and the enemy alternate moves (the enemy can’t "interrupt" you).
- **No Moves Left?** The game instantly recognizes when you can’t move anywhere, displays a message, and pauses gameplay.

---

## Getting Started (for Beginners)

1. **Import the Project** into Unity (2022 LTS or newer works best).
2. **Add Scripts:** Drag all the script files into your `Assets/Scripts` folder.
3. **Create the UnitManager:** Add an empty GameObject named `UnitManager` to your scene and attach the UnitManager.cs script.
4. **Create Obstacles:**
    - Use the provided obstacle tool to set which tiles should be blocked.
    - Place your obstacle data asset in the proper place and connect it in the inspector.
5. **UI Setup:**
    - Add a Canvas, then add a Text object called `GameOverText` (set it inactive at first).
    - Drag that `Text` into the `gameOverText` field in the PlayerMovement script on your Player object.

6. **Press Play and Try It Out!**
    - Click anywhere to move, watch your player animate, and see how the enemy reacts.

---

## Files Included

- **PlayerMovement.cs** – handles your clicks, movement, pathfinding, and game over checks
- **EnemyAI.cs** – makes the enemy react after you move
- **UnitManager.cs** – keeps units from overlapping or crashing
- **GridGenerator.cs** – sets up the board
- **TileInfo.cs** – tracks grid positions for each tile
- **ObstacleManager.cs, ObstacleData.cs, ObstacleTool.cs** – manage obstacles and make editing easy
- **AI.cs** – interface for enemy intelligence

---

## Tips for Extending the Game

- Try changing the grid size (just adjust a variable!)
- Add more enemies or different types of obstacles
- Swap out the basic Text UI for something custom
- Implement a win condition (maybe have the player reach a goal?)

---

## AI Use Statement

Some parts of this project’s code and documentation were assisted by generative AI tools for faster troubleshooting and clearer explanations. All game logic and tuning were tested and integrated by the project author.

---

**Have fun and experiment! Don’t be afraid to break things—it’s the best way to learn. 😊**
