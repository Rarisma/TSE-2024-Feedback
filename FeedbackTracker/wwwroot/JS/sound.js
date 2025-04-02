/*
   feature settings
*/

// volume
var EffectVolume = 1.0

// mute
var effectmute = false

// sound effect
// src = audio file
window.PlaySoundEffect = (src) => {

    // don't play if muted
    if (effectmute) {
        return
    }

    var audio = document.getElementById('effectplayer');
    if (audio != null) {
        var audioSource = document.getElementById('effectplayerSource');
        if (audioSource != null) {
            audioSource.src = src;
            audio.volume = window.EffectVolume;
            audio.muted = window.effectmute;
            audio.load();
            let playPromise = audio.play();
            if (playPromise !== undefined) {
                playPromise.catch(error => {
                    console.log("Autoplay prevented: User interaction required. Waiting for user input...");

                    // Listen for first user interaction to play audio
                    document.addEventListener("click", () => {
                        audio.play();
                    }, { once: true });
                });
            }
        }
    } else {
        console.error("effect audio player not found.");
    }
};

// assign listerners to buttons (assumed btn class for all buttons)
// classname = every tag with classname gets event
// sound = sound
// event = type of event to trigger sound
window.attachListener = function (classname, sound = "sounds/notification.wav", event = "onload") {
    //console.log("Attaching event listener to .btn buttons");

    // Remove existing event listeners (to avoid duplicates)
    document.querySelectorAll("." + classname).forEach(element => {
        element.replaceWith(element.cloneNode(true)); // Remove old listeners
    });

    // Attach new click event listener
    document.querySelectorAll("." + classname).forEach(element => {
        element.addEventListener(event, function () {
            PlaySoundEffect(sound);
        });
    });
};