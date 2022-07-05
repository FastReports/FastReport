using FastReport.Export.PdfSimple.PdfObjects;
using System;
using System.Globalization;

namespace FastReport.Export.PdfSimple.PdfCore
{
    /// <summary>
    /// The numberic object for real or int value
    /// </summary>
    public class PdfNumeric : PdfObjectBase
    {
        #region Internal Fields

        #endregion Internal Fields

        #region Private Fields

        private int precision;
        private double realValue;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets precision for the numeric, set value to zero for working with int
        /// </summary>
        public int Precision
        {
            get
            {
                return precision;
            }
            set
            {
                precision = value;
            }
        }

        /// <summary>
        /// Gets or sets the int value, when value is set, numeric percision sets to 0
        /// </summary>
        public int ValueInt
        {
            get
            {
                return (int)realValue;
            }
            set
            {
                realValue = value;
                precision = 0;
            }
        }

        /// <summary>
        /// Gets or sets real value, when value is set, numberic precision is not updated
        /// </summary>
        public double ValueReal
        {
            get
            {
                return realValue;
            }
            set
            {
                realValue = value;
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new instance and set the real value with precision 2
        /// </summary>
        /// <param name="value">real value</param>
        public PdfNumeric(double value)
        {
            this.realValue = value;
            this.precision = 2;
        }

        /// <summary>
        /// Initialize a new instance and set the int value
        /// </summary>
        /// <param name="value"></param>
        public PdfNumeric(int value)
        {
            this.realValue = value;
        }

        /// <summary>
        /// Initialize a new instance, set the real value and set the percision
        /// </summary>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        public PdfNumeric(double value, int precision)
        {
            this.realValue = value;
            this.precision = precision;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc/>
        public override void Write(PdfWriter writer)
        {
            if (precision <= 0)
            {
                writer.Write((int)realValue);
            }
            else
            {
                writer.Write(ExportUtils.FloatToString(realValue, precision));
            }
        }

        #endregion Public Methods
    }
}