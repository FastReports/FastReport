using System.ComponentModel;

namespace FastReport.Data
{
    /// <summary>
    /// Query parameter for request to stored procedure.
    /// </summary>
    public class ProcedureParameter : CommandParameter
    {
        /// <inheritdoc/>
        [ReadOnlyAttribute(true)]
        public override string Name { get => base.Name; set => base.Name = value; }

        /// <inheritdoc/>
        [ReadOnlyAttribute(true)]
        public override int DataType { get => base.DataType; set => base.DataType = value; }

        /// <inheritdoc/>
        [Browsable(false)]
        public override int Size { get => base.Size; set => base.Size = value; }

        /// <inheritdoc/>
        [DisplayName("Value")]
        public override string DefaultValue { get => base.DefaultValue; set => base.DefaultValue = value; }
    }
}