using FastReport.Utils;
using System.Threading.Tasks;
using System.Threading;

namespace FastReport
{
    public abstract partial class BandBase
    {
        internal virtual Task<bool> IsEmptyAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(IsEmpty());
        }

        public override async Task GetDataAsync(CancellationToken cancellationToken)
        {
            await base.GetDataAsync(cancellationToken);

            FRCollectionBase list = new FRCollectionBase();
            Objects.CopyTo(list);
            foreach (ReportComponentBase obj in list)
            {
                await obj.GetDataAsync(cancellationToken);
                obj.OnAfterData();

                // break the component if it is of BreakableComponent an has non-empty BreakTo property
                if (obj is BreakableComponent && (obj as BreakableComponent).BreakTo != null &&
                  (obj as BreakableComponent).BreakTo.GetType() == obj.GetType())
                    (obj as BreakableComponent).Break((obj as BreakableComponent).BreakTo);
            }
            OnAfterData();
        }
    }
}
