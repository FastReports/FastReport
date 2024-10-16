using System;
using System.Threading;
using System.Threading.Tasks;
using FastReport.Utils;

namespace FastReport
{
    public abstract partial class PictureObjectBase
    {
        internal async Task SetImageLocationAsync(string value, bool forceUpdate, CancellationToken cancellationToken)
        {
            if (!String.IsNullOrEmpty(Config.ReportSettings.ImageLocationRoot))
                imageLocation = value.Replace(Config.ReportSettings.ImageLocationRoot, "");
            else
                imageLocation = value;

            if (forceUpdate)
                await UpdateImageLocationAsync(cancellationToken);
        }

        internal async Task UpdateImageLocationAsync(CancellationToken cancellationToken)
        {
            await LoadImageAsync(cancellationToken);
            ResetImageIndex();
        }

        #region Public Methods

        /// <summary>
        /// Loads image asynchronously
        /// </summary>
        /// <remarks>
        /// You mustn't call this method when override it in nested class because it will call synchronous implementation
        /// </remarks>
        public virtual Task LoadImageAsync(CancellationToken cancellationToken)
        {
            // fallback if this method wasn't overridden
            LoadImage();
            return Task.CompletedTask;
        }

        #endregion
    }
}