using System.Windows;
using System.Windows.Media;

namespace BIMPlugins.Bars.Extensions
{
    /// <summary>
    /// Extension methods for searching the WPF visual tree, based on <see cref="VisualTreeHelper"/>.
    /// </summary>
    public static class VisualTreeHelperUtils
    {
        /// <summary>
        /// Walks up the visual tree and returns the nearest ancestor of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the ancestor to find.</typeparam>
        /// <param name="child">Element to start from (not included in the search).</param>
        /// <returns>The nearest matching ancestor, or <see langword="null"/> if none is found.</returns>
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            if (child is null) return null;
            var parentElement = VisualTreeHelper.GetParent(child);

            if (parentElement is T parent)
                return parent;

            return parentElement.FindParent<T>();
        }

        /// <summary>
        /// Walks up the visual tree and returns the nearest ancestor of type <typeparamref name="T"/>
        /// whose <see cref="FrameworkElement.Name"/> equals <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">Type of the ancestor to find.</typeparam>
        /// <param name="child">Element to start from (not included in the search).</param>
        /// <param name="name">Required element name (<c>x:Name</c>).</param>
        /// <returns>The nearest matching ancestor, or <see langword="null"/> if none is found.</returns>
        public static T FindParent<T>(this DependencyObject child, string name) where T : FrameworkElement
        {
            if (child is null) return null;
            var parentElement = VisualTreeHelper.GetParent(child) as FrameworkElement;

            if (parentElement is T parent)
                if (parent.Name == name)
                    return parent;

            return parentElement.FindParent<T>(name);
        }

        /// <summary>
        /// Searches the descendants of <paramref name="parent"/> depth-first and returns the first
        /// element of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the descendant to find.</typeparam>
        /// <param name="parent">Element whose descendants are searched.</param>
        /// <returns>The first matching descendant, or <see langword="null"/> if none is found.</returns>
        public static T FindChild<T>(this DependencyObject parent) where T : DependencyObject
        {
            if (parent is null) return null;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var childElement = VisualTreeHelper.GetChild(parent, i);
                if (childElement is T child)
                    return child;

                if (childElement.FindChild<T>() is T result)
                    return result;

            }
            return null;
        }

        /// <summary>
        /// Searches the descendants of <paramref name="parent"/> depth-first and returns the first
        /// element of type <typeparamref name="T"/> whose <see cref="FrameworkElement.Name"/> equals <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">Type of the descendant to find.</typeparam>
        /// <param name="parent">Element whose descendants are searched.</param>
        /// <param name="name">Required element name (<c>x:Name</c>).</param>
        /// <returns>The first matching descendant, or <see langword="null"/> if none is found.</returns>
        public static T FindChild<T>(this DependencyObject parent, string name) where T : FrameworkElement
        {
            if (parent is null) return null;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var childElement = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                if (childElement is T child)
                    if (child.Name == name)
                        return child;

                if (childElement.FindChild<T>(name) is T result)
                    return result;

            }
            return null;
        }
    }
}
