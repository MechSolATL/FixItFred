# HeroFX Studio Sound Effects

This directory contains sound effect files for the HeroFX Studio.

## Available Effects

- `slam.mp3` - Powerful impact sound for SLAM effect
- `pop.mp3` - Quick bubble-like sound for POP effect  
- `yeet.mp3` - Explosive dismissal sound for YEET effect
- `glitch.mp3` - Digital distortion sound for GLITCH effect
- `stretch.mp3` - Elastic deformation sound for STRETCH effect

## Usage

These sound files are loaded by the HeroFXEngine.js and played automatically when effects are triggered.

Sound effects can be disabled via the admin interface or programmatically:
```javascript
HeroFXStudio.enableSound(false);
```

## Format Requirements

- Format: MP3 or WebM for broad browser compatibility
- Duration: 0.5-2 seconds recommended
- Volume: Pre-normalized to avoid clipping
- Sample Rate: 44.1kHz preferred