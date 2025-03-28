// Themes.js
// To add new themes, refer to app.css

// Blazor Server Specific: MutationObserver is needed to detect changes in the DOM and apply the theme

(function () {
    let currentTheme = null;

    // apply the selected theme to the application. Update this function when adding new themes
    function applyTheme(theme) {
        theme = theme || 'theme-light';
        currentTheme = theme;
        document.documentElement.classList.remove('theme-light', 'theme-dark', 'theme-hybrid');
        document.documentElement.classList.add(theme);
        if (document.body) {
            document.body.classList.remove('theme-light', 'theme-dark', 'theme-hybrid');
            document.body.classList.add(theme);
            const mainElements = document.querySelectorAll('main');
            mainElements.forEach(main => {
                main.classList.remove('theme-light', 'theme-dark', 'theme-hybrid');
                main.classList.add(theme);
            });
            forceRedraw();
        }
        return theme;
    }

    // force a redraw of the application components to apply the theme
    function forceRedraw() {
        const sidebar = document.querySelector('.rz-sidebar');
        const body = document.querySelector('.rz-body');
        const container = document.querySelector('.app-container');
        [sidebar, body, container].forEach(element => {
            if (element) { void element.offsetHeight; }
        });
    }

    //grab the theme from the cookie
    function getCurrentTheme() {
        const cookie = document.cookie.split(';').find(c => c.trim().startsWith('selectedTheme='));
        return cookie ? cookie.split('=')[1] : 'theme-light';
    }

    // init current theme
    currentTheme = getCurrentTheme();

    // apply the theme when the DOM is ready!
    document.addEventListener('DOMContentLoaded', function () { applyTheme(getCurrentTheme()); });

    // expose the AppTheme helpers to the window object
    window.ThemeHelpers = {
        getCurrentTheme: getCurrentTheme,
        applyTheme: applyTheme,
        forceRedraw: forceRedraw
    };

    // change the theme and save it to the cookie
    window.changeTheme = function (theme) {
        const expiry = new Date();
        expiry.setDate(expiry.getDate() + 365); //lasts a year
        document.cookie = `selectedTheme=${theme};expires=${expiry.toUTCString()};path=/;SameSite=Strict; Secure`;
        return applyTheme(theme);
    };

    // navigation handler
    const applyThemeAfterNavigation = () => {
        applyTheme(currentTheme);
        setTimeout(() => applyTheme(currentTheme), 5);
    };

    // needed for Blazor Server navigation
    document.addEventListener('blazor:enhancednavigation', applyThemeAfterNavigation);

    // watch for theme class removal
    const observer = new MutationObserver(() => {
        if (currentTheme && !document.documentElement.classList.contains(currentTheme)) {
            applyTheme(currentTheme);
        }
    });

    // start observing the document element's class attribute
    document.addEventListener('DOMContentLoaded', () => {
        observer.observe(document.documentElement, { attributes: true, attributeFilter: ['class'] });
    });
})();