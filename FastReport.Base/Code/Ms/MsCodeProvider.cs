namespace FastReport.Code.Ms
{
    /// <summary>
    /// Represents code provider backed by MS CodeDom/Roslyn.
    /// </summary>
    public class MsCodeProvider : CodeProvider
    {
        /// <inheritdoc/>
        public override AssemblyDescriptor CreateAssemblyDescriptor(string scriptText)
        {
            return new MsAssemblyDescriptor(Report, scriptText);
        }

        /// <summary>
        /// Initializes a new instance of MsCodeProvider class.
        /// </summary>
        public MsCodeProvider(Report report) : base(report)
        {
        }
    }
}