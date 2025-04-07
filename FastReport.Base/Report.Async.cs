using FastReport.Code;
using FastReport.Data;
using FastReport.Engine;
using FastReport.Utils;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport
{
    public partial class Report
    {

        #region Script related

        internal async Task CompileAsync(CancellationToken token)
        {
            FillDataSourceCache();
            await CodeProvider.CompileAsync(token);
        }

        public Task<object> CalcAsync(string expression, CancellationToken token)
        {
            return CalcAsync(expression, 0, token);
        }

        public async Task<object> CalcAsync(string expression, Variant value, CancellationToken token)
        {
            if (TryCalc(expression, value, out var result))
                return result;

            return await CalcExpressionAsync(expression, value, token);
        }

        protected virtual async Task<object> CalcExpressionAsync(string expression, Variant value, CancellationToken token)
        {
            var expressionToLower = expression.ToLower();

            if (expressionToLower == "true" || expressionToLower == "false")
            {
                expression = expressionToLower;
            }

            return await CodeProvider.CalcExpressionAsync(expression, value, token);
        }

        /// <summary>
        /// Prepares the report asynchronously.
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <returns><b>true</b> if report was prepared successfully.</returns>
        public Task<bool> PrepareAsync(CancellationToken token = default)
        {
            return PrepareAsync(false, token);
        }

        public Task<bool> PrepareAsync(bool append, CancellationToken token = default)
        {
            return PrepareAsync(append, true, token);
        }

        public async Task<bool> PrepareAsync(bool append, bool resetDataState, CancellationToken token = default)
        {
            SetRunning(true);
            try
            {
                if (PreparedPages == null || !append)
                {
                    ClearPreparedPages();

                    SetPreparedPages(new Preview.PreparedPages(this));
                }
                engine = new ReportEngine(this);

                if (!Config.WebMode)
                    StartPerformanceCounter();

                try
                {
                    await CompileAsync(token).ConfigureAwait(false);
                    isParameterChanged = false;
                    return await Engine.RunAsync(true, append, resetDataState, token);
                }
                finally
                {
                    if (!Config.WebMode)
                        StopPerformanceCounter();
                }
            }
            finally
            {
                SetRunning(false);
            }
        }


        /// <summary>
        /// For internal use only.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task PreparePhase1Async(CancellationToken cancellationToken)
        {
            bool webDialog = false;
            SetRunning(true);
            if (preparedPages != null)
            {
                // if prepared pages are set before => it's call method again => it's web dialog
                webDialog = true;
                preparedPages.Clear();
            }
            SetPreparedPages(new Preview.PreparedPages(this));
            engine = new ReportEngine(this);
            await CompileAsync(cancellationToken);
            Engine.RunPhase1(true, webDialog);
        }

        #endregion Public Methods
    }
}