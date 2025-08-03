/**
 * HeroFX Studio JavaScript Engine - Sprint127_HeroFX_StudioDivision
 * Empowers admins to control visual energy with "KAPOW" moments
 * VoiceFX integration for narrator pops and comic-style visuals
 */
window.HeroFXStudio = (function() {
    'use strict';

    // Configuration
    const config = {
        apiEndpoint: '/api/herofx',
        soundEnabled: true,
        voiceEnabled: true,
        debugMode: false
    };

    // Available sound effects
    const soundEffects = {
        slam: '/Resources/FX/Animations/sounds/slam.mp3',
        pop: '/Resources/FX/Animations/sounds/pop.mp3',
        yeet: '/Resources/FX/Animations/sounds/yeet.mp3',
        glitch: '/Resources/FX/Animations/sounds/glitch.mp3',
        stretch: '/Resources/FX/Animations/sounds/stretch.mp3'
    };

    // Voice FX configuration
    const voiceFx = {
        enabled: false,
        context: null,
        voices: {}
    };

    // Initialize Web Audio API for voice effects
    function initVoiceFX() {
        if ('speechSynthesis' in window) {
            voiceFx.enabled = true;
            voiceFx.voices = speechSynthesis.getVoices();
            
            // Update voices when they load
            speechSynthesis.onvoiceschanged = () => {
                voiceFx.voices = speechSynthesis.getVoices();
            };
            
            log('VoiceFX initialized with ' + voiceFx.voices.length + ' voices');
        }
    }

    // Logging utility
    function log(message, level = 'info') {
        if (config.debugMode) {
            console[level]('[HeroFX]', message);
        }
    }

    // Play sound effect
    function playSound(soundName) {
        if (!config.soundEnabled || !soundEffects[soundName]) return;

        try {
            const audio = new Audio(soundEffects[soundName]);
            audio.volume = 0.3;
            audio.play().catch(e => {
                // Gracefully handle audio play failures (common in demos)
                log('Sound play failed (this is normal in demos): ' + e.message, 'warn');
            });
        } catch (e) {
            log('Sound error (this is normal in demos): ' + e.message, 'warn');
        }
    }

    // Play voice effect
    function playVoice(text, voiceType = 'calm') {
        if (!config.voiceEnabled || !voiceFx.enabled || !text) return;

        try {
            const utterance = new SpeechSynthesisUtterance(text);
            
            // Configure voice based on type
            if (voiceType === 'chaos') {
                utterance.pitch = 1.5;
                utterance.rate = 1.3;
                utterance.volume = 0.7;
            } else {
                utterance.pitch = 1.0;
                utterance.rate = 1.0;
                utterance.volume = 0.5;
            }

            // Find appropriate voice
            const voice = voiceFx.voices.find(v => 
                v.lang.startsWith('en') && (voiceType === 'chaos' ? v.name.includes('Google') : true)
            );
            if (voice) utterance.voice = voice;

            speechSynthesis.speak(utterance);
            log(`VoiceFX: "${text}" (${voiceType})`);
        } catch (e) {
            log('Voice error: ' + e.message, 'error');
        }
    }

    // Add voice indicator to element
    function addVoiceIndicator(element) {
        const indicator = document.createElement('div');
        indicator.className = 'hero-fx-voice-indicator';
        element.style.position = 'relative';
        element.appendChild(indicator);
        
        setTimeout(() => {
            if (indicator.parentNode) {
                indicator.parentNode.removeChild(indicator);
            }
        }, 1000);
    }

    // Core effect triggering function
    function triggerEffect(effectName, element, options = {}) {
        if (!element) {
            log('No element provided for effect: ' + effectName, 'warn');
            return false;
        }

        const {
            voiceText = null,
            voiceType = 'calm',
            triggerEvent = 'manual',
            userId = null,
            userRole = null,
            deviceType = getDeviceType()
        } = options;

        try {
            // Add effect class
            element.classList.add('hero-fx-trigger');
            element.classList.add(`hero-fx-${effectName}`);

            // Play sound effect
            playSound(effectName);

            // Play voice effect if configured
            if (voiceText) {
                playVoice(voiceText, voiceType);
                addVoiceIndicator(element);
            }

            // Remove effect class after animation
            const duration = getEffectDuration(effectName);
            setTimeout(() => {
                element.classList.remove(`hero-fx-${effectName}`);
                element.classList.remove('hero-fx-trigger');
            }, duration);

            // Log analytics
            logAnalytics(effectName, triggerEvent, userId, userRole, deviceType);

            log(`Effect triggered: ${effectName} on ${element.tagName}`);
            return true;
        } catch (e) {
            log('Effect error: ' + e.message, 'error');
            return false;
        }
    }

    // Get effect duration in milliseconds
    function getEffectDuration(effectName) {
        const durations = {
            slam: 800,
            pop: 600,
            yeet: 1200,
            glitch: 1000,
            stretch: 800
        };
        return durations[effectName] || 800;
    }

    // Get device type
    function getDeviceType() {
        return window.innerWidth <= 768 ? 'mobile' : 'desktop';
    }

    // Log analytics to server
    async function logAnalytics(effectName, triggerEvent, userId, userRole, deviceType) {
        try {
            const response = await fetch(`${config.apiEndpoint}/trigger`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    effectName,
                    triggerEvent,
                    userId,
                    userRole,
                    deviceType
                })
            });

            if (!response.ok) {
                log('Analytics logging failed: ' + response.statusText, 'warn');
            }
        } catch (e) {
            log('Analytics error: ' + e.message, 'warn');
        }
    }

    // Auto-trigger effects based on events
    function setupAutoTriggers() {
        // Dispatch success
        document.addEventListener('dispatch-success', (e) => {
            const element = e.target;
            triggerEffect('slam', element, {
                voiceText: 'KAPOW!',
                voiceType: 'chaos',
                triggerEvent: 'dispatch'
            });
        });

        // Login success
        document.addEventListener('login-success', (e) => {
            const element = e.target;
            triggerEffect('pop', element, {
                voiceText: 'Welcome back!',
                voiceType: 'calm',
                triggerEvent: 'login'
            });
        });

        // Job completion
        document.addEventListener('job-complete', (e) => {
            const element = e.target;
            triggerEffect('yeet', element, {
                voiceText: 'Another one done!',
                voiceType: 'chaos',
                triggerEvent: 'complete'
            });
        });

        // Error events
        document.addEventListener('error-occurred', (e) => {
            const element = e.target;
            triggerEffect('glitch', element, {
                voiceText: 'Whoops!',
                voiceType: 'calm',
                triggerEvent: 'error'
            });
        });

        // Update events
        document.addEventListener('data-updated', (e) => {
            const element = e.target;
            triggerEffect('stretch', element, {
                triggerEvent: 'update'
            });
        });
    }

    // Random effect generator
    function randomEffect(element, persona = null, role = null, mood = null) {
        const effects = ['slam', 'pop', 'yeet', 'glitch', 'stretch'];
        const voices = {
            slam: { text: 'BOOM!', type: 'chaos' },
            pop: { text: 'Pop!', type: 'calm' },
            yeet: { text: 'YEET!', type: 'chaos' },
            glitch: { text: 'Error!', type: 'calm' },
            stretch: { text: 'Stretch!', type: 'calm' }
        };

        // Filter effects based on role/mood (simplified logic)
        let availableEffects = effects;
        if (role === 'CSR') {
            availableEffects = ['pop', 'stretch']; // Clean fades for CSR
        } else if (role === 'Tech') {
            availableEffects = ['slam', 'yeet', 'glitch']; // Explosions for Tech
        }

        if (mood === 'calm') {
            availableEffects = availableEffects.filter(e => ['pop', 'stretch'].includes(e));
        } else if (mood === 'celebration') {
            availableEffects = availableEffects.filter(e => ['slam', 'yeet'].includes(e));
        }

        const selectedEffect = availableEffects[Math.floor(Math.random() * availableEffects.length)];
        const voice = voices[selectedEffect];

        triggerEffect(selectedEffect, element, {
            voiceText: voice.text,
            voiceType: voice.type,
            triggerEvent: 'random'
        });
    }

    // Live preview for admin interface
    function previewEffect(effectName, previewElement) {
        if (!previewElement) {
            previewElement = document.getElementById('fx-preview') || document.body;
        }

        triggerEffect(effectName, previewElement, {
            voiceText: `${effectName.toUpperCase()}!`,
            voiceType: effectName === 'slam' || effectName === 'yeet' ? 'chaos' : 'calm',
            triggerEvent: 'preview'
        });
    }

    // Configuration methods
    function enableSound(enabled = true) {
        config.soundEnabled = enabled;
        log('Sound ' + (enabled ? 'enabled' : 'disabled'));
    }

    function enableVoice(enabled = true) {
        config.voiceEnabled = enabled;
        log('Voice ' + (enabled ? 'enabled' : 'disabled'));
    }

    function setDebugMode(enabled = true) {
        config.debugMode = enabled;
        log('Debug mode ' + (enabled ? 'enabled' : 'disabled'));
    }

    // Initialize HeroFX Studio
    function init() {
        log('HeroFX Studio initializing...');
        
        initVoiceFX();
        setupAutoTriggers();
        
        // Load CSS if not already loaded
        if (!document.querySelector('link[href*="HeroFXAnimations.css"]')) {
            const link = document.createElement('link');
            link.rel = 'stylesheet';
            link.href = '/Resources/FX/Animations/HeroFXAnimations.css';
            document.head.appendChild(link);
        }

        log('HeroFX Studio ready! 🎆');
    }

    // Public API
    return {
        // Core functions
        init,
        triggerEffect,
        randomEffect,
        previewEffect,
        
        // Configuration
        enableSound,
        enableVoice,
        setDebugMode,
        
        // Utilities
        playSound,
        playVoice,
        getDeviceType,
        
        // For admin interface
        logAnalytics,
        
        // Available effects
        effects: Object.keys(soundEffects)
    };
})();

// Auto-initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', HeroFXStudio.init);
} else {
    HeroFXStudio.init();
}

// Global shortcuts for convenience
window.triggerHeroFX = HeroFXStudio.triggerEffect;
window.randomHeroFX = HeroFXStudio.randomEffect;
window.previewHeroFX = HeroFXStudio.previewEffect;