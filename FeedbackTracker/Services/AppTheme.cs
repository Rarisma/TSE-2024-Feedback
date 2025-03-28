// Theme Service (named AppTheme to prevent ambiguous errors)
// To add new themes, please refer to app.css

using Microsoft.JSInterop;

namespace FeedbackTracker.Services
{
    public class AppTheme
    {
        private readonly IJSRuntime _js;
        private string _colourTheme = "theme-light"; //default theme
        public event Action? OnChange;
        public AppTheme(IJSRuntime js) { _js = js; }

        //dictionary of available themes. Add new theme definitions here, add themes to app.css
        public static readonly Dictionary<string, string> AvailableThemes = new()
        {
            { "theme-light", "Light Mode" },
            { "theme-dark", "Dark Mode" },
            { "theme-hybrid", "Hybrid Mode" }
        };

        //get, set current theme
        public string ColourTheme
        {
            get => _colourTheme; set
            {
                if (_colourTheme != value && AvailableThemes.ContainsKey(value))
                {
                    _colourTheme = value;
                    _ = SetThemeAsync(value);
                    OnChange?.Invoke();
                }
            }
        }

        //cycle through themes - if adding new themes, update this method
        public void CycleTheme()
        {
            ColourTheme = _colourTheme switch
            {
                "theme-light" => "theme-dark",
                "theme-dark" => "theme-hybrid",
                "theme-hybrid" => "theme-light",
                _ => "theme-light"
            };
        }

        //initialize theme
        public async Task InitializeAsync()
        {
            string? savedTheme = await GetSavedThemeAsync();
            _colourTheme = savedTheme != null && AvailableThemes.ContainsKey(savedTheme) ? savedTheme : "theme-light";
            await SetThemeAsync(_colourTheme);
        }

        //get saved theme
        private async Task<string?> GetSavedThemeAsync()
        {
            return await _js.InvokeAsync<string?>("ThemeHelpers.getCurrentTheme");
        }

        //use theme helpers to set the theme
        private async Task SetThemeAsync(string colourTheme)
        {
            await _js.InvokeVoidAsync("ThemeHelpers.applyTheme", colourTheme);
            await _js.InvokeVoidAsync("ThemeHelpers.forceRedraw");
            await _js.InvokeVoidAsync("changeTheme", colourTheme);
        }
    }
}