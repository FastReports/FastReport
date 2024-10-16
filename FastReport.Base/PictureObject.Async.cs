using System;
using System.Drawing;
using FastReport.Utils;
using System.Threading.Tasks;
using System.Threading;

namespace FastReport
{
    public partial class PictureObject
    {
        #region Public Methods

        public override async Task LoadImageAsync(CancellationToken cancellationToken)
        {
            if (!String.IsNullOrEmpty(ImageLocation))
            {
                try
                {
                    Uri uri = CalculateUri();
                    byte[] bytes;
                    if (uri.IsFile)
                        bytes = await ImageHelper.LoadAsync(uri.LocalPath, cancellationToken);
                    else
                        bytes = await ImageHelper.LoadURLAsync(uri, cancellationToken);
                    SetImageData(bytes);
                }
                catch
                {
                    Image = null;
                }

                ShouldDisposeImage = true;
            }
        }

#endregion

        #region Report Engine

        public override async Task GetDataAsync(CancellationToken cancellationToken)
        {
            await base.GetDataAsync(cancellationToken);

            if (!String.IsNullOrEmpty(DataColumn))
            {
                // reset the image
                Image = null;
                imageData = null;

                object data = Report.GetColumnValueNullable(DataColumn);
                if (data is byte[])
                {
                    SetImageData((byte[])data);
                }
                else if (data is Image)
                {
                    Image = data as Image;
                }
                else if (data is string dataStr)
                {
                    await SetImageLocationAsync(dataStr, true, cancellationToken);
                }
            }
            else
            {
                // no other data received
                await UpdateImageLocationAsync(cancellationToken);
            }
        }

        #endregion
    }
}