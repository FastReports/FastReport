using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

#pragma warning disable FR0005 // Field must be texted in lowerCamelCase.

namespace FastReport.CrossView
{
    /// <summary>
    /// 
    /// </summary>
    public struct CrossViewMeasureCell
    {

        /// <summary>
        /// 
        /// </summary>
        public string Text;

    }
    /// <summary>
    /// 
    /// </summary>
    public struct CrossViewAxisDrawCell
    {
        /// <summary>
        /// 
        /// </summary>
        public int Level;
        /// <summary>
        /// 
        /// </summary>
        public int SizeLevel;
        /// <summary>
        /// 
        /// </summary>
        public int Cell;
        /// <summary>
        /// 
        /// </summary>
        public int SizeCell;
        /// <summary>
        /// 
        /// </summary>
        public int MeasureIndex;
        /// <summary>
        /// 
        /// </summary>
        public string Text;
        //    public PropertyOfCellAxis CellProperties;
        //    public int TotalIndex;
        //    public int NodeLevel;
        //    public int NodeIndex;
        //    public HorizontalAlignment Alignment;
        //    public int ValueIndex;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="crossViewAxisDrawCell"></param>
    /// <returns></returns>
    public delegate bool CrossViewAxisDrawCellHandler(CrossViewAxisDrawCell crossViewAxisDrawCell);
    /// <summary>
    /// Represents interface of the source for <see cref="CrossView"/> object.
    /// </summary>
    public interface IBaseCubeLink
    {
        /// <summary>
        /// 
        /// </summary>
        int XAxisFieldsCount { get; }
        /// <summary>
        /// 
        /// </summary>
        int YAxisFieldsCount { get; }
        /// <summary>
        /// 
        /// </summary>
        int MeasuresCount { get; }
        /// <summary>
        /// 
        /// </summary>
        int MeasuresLevel { get; }
        /// <summary>
        /// 
        /// </summary>
        bool MeasuresInXAxis { get; }
        /// <summary>
        /// 
        /// </summary>
        bool MeasuresInYAxis { get; }
        /// <summary>
        /// 
        /// </summary>
        int DataColumnCount { get; }
        /// <summary>
        /// 
        /// </summary>
        int DataRowCount { get; }
        /// <summary>
        /// 
        /// </summary>
        bool SourceAssigned { get; }
        /// <summary>
        /// 
        /// </summary>
        CrossViewMeasureCell GetMeasureCell(int colIndex, int rowIndex);
        /// <summary>
        /// 
        /// </summary>
        void TraverseXAxis(CrossViewAxisDrawCellHandler crossViewAxisDrawCellHandler);
        /// <summary>
        /// 
        /// </summary>
        void TraverseYAxis(CrossViewAxisDrawCellHandler crossViewAxisDrawCellHandler);
        /// <summary>
        /// 
        /// </summary>
        string GetXAxisFieldName(int fieldIndex);
        /// <summary>
        /// 
        /// </summary>
        string GetYAxisFieldName(int fieldIndex);
        /// <summary>
        /// 
        /// </summary>
        string GetMeasureName(int measureIndex);
    }

}
#pragma warning restore FR0005 // Field must be texted in lowerCamelCase.