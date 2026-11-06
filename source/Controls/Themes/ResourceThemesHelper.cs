using BIMPlugins.Bars.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BIMPlugins.Bars.Controls.Themes
{
    /// <summary>
    /// Applies the library's theme resource dictionaries (<c>Controls/Themes/*.xaml</c>) to WPF elements.
    /// </summary>
    public static class ResourceThemesHelper
    {
        /// <summary>
        /// Merges all theme resource dictionaries of this assembly into the resources of
        /// <paramref name="window"/>. The dictionary matching the selected theme is merged last,
        /// so its values take precedence. Dictionaries that were already merged are replaced.
        /// </summary>
        /// <param name="window">Element whose <see cref="FrameworkElement.Resources"/> receive the themes.</param>
        /// <param name="defaultThemeName">
        /// Theme to prefer: <c>"default"</c>, <c>"light"</c> or <c>"dark"</c>.
        /// When <see langword="null"/>, the theme is detected from Revit: <c>"dark"</c> or <c>"light"</c>
        /// for Revit 2024 and later (depending on <c>UIThemeManager.CurrentTheme</c>), <c>"default"</c> for older versions.
        /// </param>
        /// <remarks>Errors are swallowed: a missing or invalid dictionary never breaks the caller.</remarks>
        public static void AddResourceThemes(this FrameworkElement window, string defaultThemeName = null)
        {
            try
            {
                if (defaultThemeName is null)
                    defaultThemeName = GetRevitThemeName();

                var resourceNames = GetResourceThemes()
                    .OrderBy(e => e.Contains(defaultThemeName));

                foreach (var resourceName in resourceNames)
                {
                    try
                    {
                        var resource = new ResourceDictionary
                        {
                            Source = ResourceHelper.GetAbsoluteUri(resourceName)
                        };

                        var similarResource = window.Resources.MergedDictionaries.FirstOrDefault(e => e.SimilarSource(resource));
                        if (similarResource is not null)
                            window.Resources.MergedDictionaries.Remove(similarResource);

                        window.Resources.MergedDictionaries.Add(resource);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch { }
        }

        private static int RevitVersion { get; } = typeof(Autodesk.Revit.UI.UIThemeManager).Assembly.GetName().Version.Major;

        private static string GetRevitThemeName()
        {
            var revitThemeName = "default";
            try
            {
                if (RevitVersion >= 24)
                {
                    revitThemeName = "light";
                    var currentTheme = Autodesk.Revit.UI.UIThemeManager.CurrentTheme;
                    if (currentTheme == Autodesk.Revit.UI.UITheme.Dark)
                    {
                        revitThemeName = "dark";
                    }
                }

            }
            catch { }
            return revitThemeName;
        }

        private static IEnumerable<string> GetResourceThemes()
        {
            return ResourceHelper.GetResourceNames()
                .Where(e => e.Contains("/themes/"))
                .Where(e => e.EndsWith(".xaml"));
        }
    }
}
