// Preloader.js
// This script simply hides the preloader after 1.5 seconds. Hiding the flash on app load!

addEventListener('DOMContentLoaded', () => {
    setTimeout(() => {
        const preload = document.getElementById('preloader');
        if (preload) {
            preload.classList.add('fade-out');
            preload.ontransitionend = () => preload.remove();
        }
    }, 1500);
});