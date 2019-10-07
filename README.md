# Final Project for Microcontroller Interfacing Lab (ECE 3730)

A game controller interface linking the PIC32 microcontroller to events in Unity3D.

The process flow is as follows:
- The user inputs data via a joystick or a button
- The PIC32 microcontroller gets analog input from the joystick and/or digital input from the button
- The PIC32 transmits data to a PC via a USB-to-TTL converter
- A running Unity3D instance receives the data via an open port using the Ardity package
- The Unity3D scene interprets data and translates to in-scene events

The process flow is exemplified with a basic Unity game. The user translates a cube in the XY plane using the joystick and discretely along the Z axis using the button. The user manipulates the game controller so that the cube collides with and collects randomly placed spheres as a game objective.
