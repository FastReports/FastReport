using FastReport.Utils;
using System;

namespace FastReport
{
    partial class CellularTextObject
    {
        private float GetCellWidthInternal(float fontHeight)
        {
            return (int)Math.Round((fontHeight + 10) / (0.25f * Units.Centimeters)) * (0.25f * Units.Centimeters);
        }
    }
}
