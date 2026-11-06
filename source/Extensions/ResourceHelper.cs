using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace BIMPlugins.Bars.Extensions
{
    /// <summary>
    /// Helpers for working with WPF resources embedded in an assembly.
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// Returns the names of all WPF resources embedded in the assembly's <c>&lt;AssemblyName&gt;.g.resources</c>.
        /// </summary>
        /// <param name="assembly">Assembly to inspect. Defaults to this library's assembly.</param>
        /// <returns>Resource names (lower-case relative paths), sorted alphabetically.</returns>
        public static string[] GetResourceNames(Assembly assembly = null)
        {
            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();

            string resName = assembly.GetName().Name + ".g.resources";
            using (var stream = assembly.GetManifestResourceStream(resName))
            using (var reader = new System.Resources.ResourceReader(stream))
            {
                var resources = reader.Cast<DictionaryEntry>().Select(entry => (string)entry.Key).OrderBy(e => e).ToArray();
                return resources;
            }
        }

        /// <summary>
        /// Builds a pack URI (<c>/&lt;AssemblyName&gt;;component/&lt;path&gt;</c>) for a resource of the assembly.
        /// </summary>
        /// <param name="path">Relative path of the resource, for example <c>controls/themes/colordefault.xaml</c>.</param>
        /// <param name="assembly">Assembly that contains the resource. Defaults to this library's assembly.</param>
        /// <returns>
        /// The pack URI; if it cannot be built, <paramref name="path"/> as a plain URI;
        /// <see langword="null"/> if both attempts fail.
        /// </returns>
        public static Uri GetAbsoluteUri(string path, Assembly assembly = null)
        {
            try
            {
                if (assembly == null)
                    assembly = Assembly.GetExecutingAssembly();

                var assemblyName = assembly.GetName().Name;
                return new Uri($"/{assemblyName};component/{path}", UriKind.RelativeOrAbsolute);
            }
            catch { }
            try
            {
                return new Uri(path, UriKind.RelativeOrAbsolute);
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Checks whether two resource dictionaries point to the same source, i.e. one
        /// <see cref="ResourceDictionary.Source"/> ends with the other (case-insensitive).
        /// </summary>
        /// <param name="resource">First dictionary.</param>
        /// <param name="other">Second dictionary.</param>
        /// <returns><see langword="true"/> if the sources match.</returns>
        public static bool SimilarSource(this ResourceDictionary resource, ResourceDictionary other)
        {
            var resourceSource = resource.Source.ToString();
            var otherSource = other.Source.ToString();
            return resourceSource.EndsWith(otherSource, StringComparison.InvariantCultureIgnoreCase) || otherSource.EndsWith(resourceSource, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
