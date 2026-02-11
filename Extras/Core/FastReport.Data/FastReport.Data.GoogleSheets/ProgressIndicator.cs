#if !FRCORE
using FastReport.Forms;
using FastReport.Utils;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FastReport.Data
{
    /// <summary>
    /// Implements <see cref="IProgressIndicator"/> using WinForms. Shows a modal progress window
    /// and executes a background task. Provides a cancel button to signal cancellation
    /// via a <see cref="CancellationToken"/> and safely marshals the result or exceptions back to the calling thread.
    /// </summary>
    internal class ProgressIndicator : IProgressIndicator
    {
        /// <inheritdoc/>
        public DataSet ShowAndRun(Func<CancellationToken, DataSet> work)
        {
            DataSet result = null;
            Exception exception = null;

            using (var progressForm = new ProgressForm(null, true))
            {
                // ensures the progress form is not always on top, allowing the browser window (for OAuth) to be visible
                progressForm.TopMost = false;

                using (var cts = new CancellationTokenSource())
                {
                    if (progressForm.Controls.Find("panel1", true).FirstOrDefault() is Panel panel)
                    {
                        if (panel.Controls.Find("lblProgress", true).FirstOrDefault() is Label originalLabel)
                        {
                            originalLabel.Text = Res.Get("ConnectionEditors,GoogleSheets,ProgressLabelText");

                            originalLabel.Location = new Point(
                            (panel.ClientSize.Width - originalLabel.Width) / 2,
                            (panel.ClientSize.Height - originalLabel.Height) / 2 - 10
                        );
                        }

                        if (progressForm.Controls.Find("btnCancel", true).FirstOrDefault() is Button buttonCancel)
                        {
                            buttonCancel.Location = new Point(
                            (panel.ClientSize.Width - buttonCancel.Width) / 2,
                            panel.ClientSize.Height - buttonCancel.Height - 20 // bottom margin
                            );
                            buttonCancel.Click += (s, e) =>
                            {
                                cts.Cancel();
                                progressForm.Close();
                            };
                        }
                    }

                    progressForm.Shown += (s, e) =>
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                result = work(cts.Token);
                            }
                            catch (Exception ex)
                            {
                                exception = ex;
                            }
                            finally
                            {
                                try
                                {
                                    progressForm.Invoke((Action)progressForm.Close);
                                }
                                catch (Exception ex) when (ex is ObjectDisposedException || ex is InvalidOperationException)
                                {
                                    // ignores errors if the form is already disposed or unavailable
                                }
                            }
                        }, cts.Token);
                    };

                    progressForm.ShowDialog();
                }
            }

            if (exception != null)
            {
                // do not throw for cancellation, just return null
                if (exception is OperationCanceledException)
                    return null;

                // re-throw any other exception to be handled by the caller
                ExceptionDispatchInfo.Capture(exception).Throw();
            }

            return result;
        }
    }
}
#endif
