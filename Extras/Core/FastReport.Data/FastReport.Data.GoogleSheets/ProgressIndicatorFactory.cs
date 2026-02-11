#if !FRCORE
using System;

namespace FastReport.Data
{
    /// <summary>
    /// A factory class for creating <see cref="IProgressIndicator"/> instances.
    /// Uses a static Lazy singleton to ensure only one instance is created.
    /// Currently, it always returns the WinForms implementation.
    /// </summary>
    public static class ProgressIndicatorFactory
    {
        private static readonly Lazy<IProgressIndicator> _indicator = new Lazy<IProgressIndicator>(CreateIndicator);

        /// <summary>
        /// Creates and returns an <see cref="IProgressIndicator"/> instance.
        /// </summary>
        /// <returns>The <see cref="IProgressIndicator"/> instance.</returns>
        public static IProgressIndicator Create()
        {
            return _indicator.Value;
        }

        /// <summary>
        /// Creates the specific <see cref="IProgressIndicator"/> implementation.
        /// </summary>
        /// <returns>A new instance of <see cref="IProgressIndicator"/>.</returns>
        private static IProgressIndicator CreateIndicator()
        {
            return new ProgressIndicator();
        }
    }
}
#endif