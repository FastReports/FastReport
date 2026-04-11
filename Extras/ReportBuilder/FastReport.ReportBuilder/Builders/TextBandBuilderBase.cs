using System.Drawing;

namespace FastReport.ReportBuilder
{
    /// <summary>
    /// Provides shared fluent configuration for text-based report bands.
    /// </summary>
    /// <typeparam name="TBuilder">The concrete builder type.</typeparam>
    /// <typeparam name="T">The report row type.</typeparam>
    public abstract class TextBandBuilderBase<TBuilder, T>
        where TBuilder : TextBandBuilderBase<TBuilder, T>
    {
        /// <summary>
        /// The owning report builder.
        /// </summary>
        protected readonly ReportBuilder<T> Report;

        /// <summary>
        /// The backing definition being configured by the fluent API.
        /// </summary>
        protected readonly TextBandDefinition Definition;

        /// <summary>
        /// Initializes a new text-band builder.
        /// </summary>
        /// <param name="report">The owning report builder.</param>
        /// <param name="definition">The backing band definition to update.</param>
        protected TextBandBuilderBase(ReportBuilder<T> report, TextBandDefinition definition)
        {
            Report = report;
            Definition = definition;
        }

        /// <summary>
        /// Sets the displayed text for the band.
        /// </summary>
        /// <param name="text">The text or expression to render.</param>
        /// <returns>The current band builder.</returns>
        public TBuilder Text(string text)
        {
            Definition.Text = text;
            Definition.Visible = true;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the band font family, size, and style.
        /// </summary>
        /// <param name="familyName">The font family name.</param>
        /// <param name="emSize">The font size in points.</param>
        /// <param name="style">The font style to apply.</param>
        /// <returns>The current band builder.</returns>
        public TBuilder Font(string familyName, float emSize, FontStyle style)
        {
            Definition.Font = new Font(familyName, emSize, style);
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the band font family and size.
        /// </summary>
        /// <param name="familyName">The font family name.</param>
        /// <param name="emSize">The font size in points.</param>
        /// <returns>The current band builder.</returns>
        public TBuilder Font(string familyName, float emSize)
        {
            return Font(familyName, emSize, FontStyle.Regular);
        }

        /// <summary>
        /// Sets the band font family using the default size and style.
        /// </summary>
        /// <param name="familyName">The font family name.</param>
        /// <returns>The current band builder.</returns>
        public TBuilder Font(string familyName)
        {
            return Font(familyName, 10.0f, FontStyle.Regular);
        }

        /// <summary>
        /// Sets whether the band is visible in the prepared report.
        /// </summary>
        /// <param name="visible"><see langword="true"/> to show the band; otherwise, <see langword="false"/>.</param>
        /// <returns>The current band builder.</returns>
        public TBuilder Visible(bool visible)
        {
            Definition.Visible = visible;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the text color for the band.
        /// </summary>
        /// <param name="color">The text color.</param>
        /// <returns>The current band builder.</returns>
        public TBuilder TextColor(Color color)
        {
            Definition.TextColor = color;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the background fill color for the band.
        /// </summary>
        /// <param name="color">The fill color.</param>
        /// <returns>The current band builder.</returns>
        public TBuilder FillColor(Color color)
        {
            Definition.FillColor = color;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the vertical text alignment for the band.
        /// </summary>
        /// <param name="vertAlign">The vertical alignment.</param>
        /// <returns>The current band builder.</returns>
        public TBuilder VertAlign(VertAlign vertAlign)
        {
            Definition.VertAlign = vertAlign;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the horizontal text alignment for the band.
        /// </summary>
        /// <param name="horzAlign">The horizontal alignment.</param>
        /// <returns>The current band builder.</returns>
        public TBuilder HorzAlign(HorzAlign horzAlign)
        {
            Definition.HorzAlign = horzAlign;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the band height in centimeters.
        /// </summary>
        /// <param name="centimeters">The desired band height in centimeters.</param>
        /// <returns>The current band builder.</returns>
        public TBuilder Height(float centimeters)
        {
            Definition.Height = centimeters;
            return (TBuilder)this;
        }
    }
}
