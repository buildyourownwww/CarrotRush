# Development Decisions

## Week 1 — November 25, 2026 (Sound Pass)

- **What I asked the AI assistant:** Help me implement and test the game's audio requirements, including sound effects, background music, Audio Mixer groups, volume sliders, saving volume settings, and UI audio.
- **What the AI got wrong:** Some early troubleshooting suggestions for the pause-audio snapshot issue did not solve the problem and led to unnecessary Audio Mixer changes. The pause panel and controls worked, but the music did not change volume when pausing.
- **What I changed:** I tested the features in Unity, assigned Audio Sources and sound clips in the Inspector, corrected button event connections, and updated scripts to trigger sounds at the appropriate moments.
- **Why I made these changes:** I wanted sound effects to play only when their corresponding game events happen, and I wanted volume sliders and saved settings to work without breaking existing gameplay systems.
- **Current status:** Most sound effects and volume settings were implemented and tested. The pause-audio snapshot transition remains unresolved.
