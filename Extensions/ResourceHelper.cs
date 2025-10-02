using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace BIMPlugins.ProgressBar.Extensions
{
    /// <summary>
    /// ResourceHelper
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// GetResourceNames
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
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
        /// GetAbsoluteUri
        /// </summary>
        /// <param name="path"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
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
        /// SimilarSource
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool SimilarSource(this ResourceDictionary resource, ResourceDictionary other)
        {
            var resourceSource = resource.Source.ToString();
            var otherSource = other.Source.ToString();
            return resourceSource.EndsWith(otherSource, StringComparison.InvariantCultureIgnoreCase) || otherSource.EndsWith(resourceSource, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}