// Colour Changer

window.changeTheme = (theme) => {
    // Store theme in localStorage
    if (localStorage.getItem('selectedTheme') !== theme) {
        localStorage.setItem('selectedTheme', theme);
    }

    // apply theme
    const mainElement = document.querySelector('main');
    if (mainElement) {
        mainElement.classList.remove('blue', 'red', 'green');
        mainElement.classList.add(theme);
    }
};

document.addEventListener('DOMContentLoaded', () => {
    window.changeTheme(localStorage.getItem('selectedTheme') || 'blue');
});
