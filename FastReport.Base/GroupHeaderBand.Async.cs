using System.Threading;
using System.Threading.Tasks;

namespace FastReport
{
    public partial class GroupHeaderBand
    {
        #region Public Methods

        internal async Task InitDataSourceAsync(CancellationToken cancellationToken)
        {
            DataBand dataBand = GroupDataBand;
            GroupHeaderBand group = this;
            int index = 0;
            // insert group sort to the databand
            while (group != null)
            {
                if (group.SortOrder != SortOrder.None)
                {
                    dataBand.Sort.Insert(index, new Sort(group.Condition, group.SortOrder == SortOrder.Descending));
                    index++;
                }
                group = group.NestedGroup;
            }

            await dataBand.InitDataSourceAsync(cancellationToken);
        }

        internal override async Task<bool> IsEmptyAsync(CancellationToken cancellationToken)
        {
            if (NestedGroup != null)
                return await NestedGroup.IsEmptyAsync(cancellationToken);
            else if (Data != null)
                return await Data.IsEmptyAsync(cancellationToken);
            return await base.IsEmptyAsync(cancellationToken);
        }
        #endregion
    }
}