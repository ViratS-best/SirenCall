# Siren's Curse

A first-person horror game built with Unity. The goal is to survive a terrifying experience while navigating through a dynamic environment.

## Features

* **First-Person Player Controller:** Standard movement including walking, sprinting, crouching, and jumping.
* **Rebindable Keybinds:** Fully customizable keybinds for all player actions, with a default setup for standard WASD controls.
* **Settings Menu:** A functional settings panel with a graphics quality dropdown and a mouse sensitivity slider.
* **Win/Lose Condition:** The game includes a win condition that transitions the player to a credits scene.
* **Key/Door System:** A basic system for interacting with and opening locked doors.
* **Multi-Platform:** Built for both Windows and WebGL.

## How to Play

### Controls

The default controls are as follows, but can be changed in the settings menu:

* **Move Forward:** `W`
* **Move Backward:** `S`
* **Move Left:** `A`
* **Move Right:** `D`
* **Sprint:** `Left Shift`
* **Crouch:** `Left Ctrl`
* **Jump:** `Space`

## Technical Details

This project uses a standard Unity setup with a few key features:

* **Scenes:** The project is structured with a **`MainMenu`**, a **`GameScene`**, and a **`CreditsScene`**.
* **Singleton Pattern:** The `KeybindManager` uses a singleton pattern with `DontDestroyOnLoad` to persist keybind settings across different scenes.
* **Builds:** The game is configured to be built for Windows (with an Inno Setup installer) and WebGL for in-browser play.

## Credits

**Developer:** VirateyBoy (GitHub: [ViratS-best](https://github.com/ViratS-best/SirenCall))
**Platform:** [itch.io](https://virateyboy.itch.io/)
