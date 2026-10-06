# Coin Collector

GAME 360 - Jeremy Gurzynski

 

## How to play

WASD / Up, Down, Left, Right keys for movement
Mouse movement for camera controls
Left Click for Attack

 

## Singleton

Class: ScoreManager.cs

What it holds: Holds player's score 

Why it's a singleton: ScoreManager is a singleton because every script accesses the same exact score, while duplicate ScoreManger scripts are prevented when scene reloads.

 

## Observer

Event: OnScoreChanged in ScoreManager.cs>

Listener 1: ScoreUI.cs - Updates the score text whenever the score changes.

Listener 2: GameUI.cs - Checks if the player has reached the score required to win.

 

## Help I used

https://www.youtube.com/watch?v=QPJHY6MPag4     -   For Dialogue Editor
https://youtu.be/9U--F5rSGGs?si=XIk-2Vwu3xTExkCa   -  For Animations
https://youtu.be/o7O28SFGWS4?si=REyiMKmKgGdU_2NJ     -   For Cinemachine & movement
