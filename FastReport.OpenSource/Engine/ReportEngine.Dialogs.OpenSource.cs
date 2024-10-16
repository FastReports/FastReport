using FastReport.Dialog;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FastReport.Engine
{
    public partial class ReportEngine
    {
        #region Private Methods


        private bool RunDialogs()
        {
            return true;
        }

        private Task<bool> RunDialogsAsync()
        {
            return Task.FromResult(true);
        }

        #endregion Private Methods
    }
}
