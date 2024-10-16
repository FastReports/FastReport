using System;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport
{
    /// <summary>
    /// Container object that may contain child objects.
    /// </summary>
    public partial class ContainerObject : ReportComponentBase, IParent
    {
        #region Report engine

        /// <inheritdoc/>
        public override async Task GetDataAsync(CancellationToken cancellationToken)
        {
            await base.GetDataAsync(cancellationToken);
            foreach (ReportComponentBase obj in Objects)
            {
                await obj.GetDataAsync(cancellationToken);
                obj.OnAfterData();

                // break the component if it is of BreakableComponent an has non-empty BreakTo property
                if (obj is BreakableComponent && (obj as BreakableComponent).BreakTo != null &&
                    (obj as BreakableComponent).BreakTo.GetType() == obj.GetType())
                    (obj as BreakableComponent).Break((obj as BreakableComponent).BreakTo);
            }
        }

        #endregion
    }
}
