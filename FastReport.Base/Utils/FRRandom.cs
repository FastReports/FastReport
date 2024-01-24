using System;
using System.Text;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
using FastReport.Data;

namespace FastReport.Utils
{
    /// <summary>
    /// The pseudo-random generator.
    /// </summary>
    public class FRRandom
    {
        #region Fields

        private readonly Random random;

        private static readonly char[] lowerLetters =
        {
          'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
          'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

        private static readonly char[] upperLetters =
        {
          'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
          'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };

        #endregion Fields

        #region Public Methods

        /// <summary>
        /// Gets a random letter in same case that source character.
        /// </summary>
        /// <param name="source">The source character.</param>
        /// <returns>The random character.</returns>
        public char NextLetter(char source)
        {
            if (char.IsLower(source))
                return lowerLetters[random.Next(lowerLetters.Length)];
            else if (char.IsUpper(source))
                return upperLetters[random.Next(upperLetters.Length)];

            return source;
        }

        /// <summary>
        /// Gets random int value from <b>0</b> to <b>9</b>.
        /// </summary>
        /// <returns>Random int value.</returns>
        public int NextDigit()
        {
            return random.Next(10);
        }

        /// <summary>
        /// Gets random int value from <b>0</b> to <b>max</b>.
        /// </summary>
        /// <param name="max">The maximum for random digit.</param>
        /// <returns>Random int value.</returns>
        public int NextDigit(int max)
        {
            return random.Next(max + 1);
        }

        /// <summary>
        /// Gets random int value from <b>min</b> to <b>max</b>.
        /// </summary>
        /// <param name="min">The minimum for random digit.</param>
        /// <param name="max">The maximum for random digit.</param>
        /// <returns>Random int value.</returns>
        public int NextDigit(int min, int max)
        {
            return random.Next(min, max + 1);
        }

        /// <summary>
        /// Gets number of random digits from <b>0</b> to <b>9</b>.
        /// </summary>
        /// <param name="number">The number of digits.</param>
        /// <returns>Number of random digits.</returns>
        public string NextDigits(int number)
        {
            if (number <= 0)
                return "";

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < number; i++)
            {
                sb.Append(NextDigit());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the random byte value.
        /// </summary>
        /// <returns>Random byte value.</returns>
        public byte NextByte()
        {
            return (byte)random.Next(byte.MaxValue);
        }

        /// <summary>
        /// Gets random byte array with specified number of elements.
        /// </summary>
        /// <param name="number">The number of elements in array.</param>
        /// <returns>Random byte array.</returns>
        public byte[] NextBytes(int number)
        {
            byte[] bytes = new byte[number];
            random.NextBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// Gets the randomized char value.
        /// </summary>
        /// <returns>Random char value.</returns>
        public char NextChar()
        {
            return Convert.ToChar(random.Next(char.MaxValue));
        }

        /// <summary>
        /// Gets the random day from start to DataTime.Today.
        /// </summary>
        /// <param name="start">The starting DateTime value.</param>
        /// <returns>Random DateTime value.</returns>
        public DateTime NextDay(DateTime start)
        {
            if (start > DateTime.Today)
                return DateTime.Today;

            int range = (DateTime.Today - start).Days;
            return start.AddDays(random.Next(range));
        }

        /// <summary>
        /// Gets the randomized TimeSpan value beetwin specified hours.
        /// </summary>
        /// <param name="start">The starting hour (0 - 24).</param>
        /// <param name="end">The ending hour (0 - 24).</param>
        /// <returns>Random TimeSpan value.</returns>
        public TimeSpan NextTimeSpanBetweenHours(int start, int end)
        {
            if (start < 0)
                start = 0;
            if (end > 24)
                end = 24;
            if (start > end)
            {
                int temp = start;
                start = end;
                end = temp;
            }

            TimeSpan startTs = TimeSpan.FromHours(start);
            TimeSpan endTs = TimeSpan.FromHours(end);

            int maxMinutes = (int)(endTs - startTs).TotalMinutes;
            int randomMinutes = random.Next(maxMinutes);

            TimeSpan result = startTs.Add(TimeSpan.FromMinutes(randomMinutes));
            return result;
        }

        /// <summary>
        /// Gets the randomized decimal value with same number of digits that in source value.
        /// </summary>
        /// <param name="source">The source decimal value.</param>
        /// <returns>Random decimal value based on source.</returns>
        public decimal RandomizeDecimal(decimal source)
        {
            StringBuilder sb = new StringBuilder();

            string[] parts = source.ToString(CultureInfo.InvariantCulture).ToUpper().Split('E');
            string e = "";
            if (parts.Length > 1)
                e = "E" + parts[1];

            parts = parts[0].Split('.');
            if (parts.Length > 0)
            {
                int length = parts[0].Length;
                if (source < 0.0m)
                {
                    sb.Append("-");
                    length--;
                }
                sb.Append(NextDigit(1, 9));
                sb.Append(NextDigits(length - 1));
            }

            if (parts.Length > 1)
            {
                sb.Append(".");
                sb.Append(NextDigits(parts[1].Length - 1));
                sb.Append(NextDigit(1, 9));
            }

            sb.Append(e);

            decimal result;
            bool parsed = decimal.TryParse(sb.ToString(), NumberStyles.Float,
                CultureInfo.InvariantCulture, out result);
            if (parsed)
                return result;

            return source;
        }

        /// <summary>
        /// Gets the randomized double value with same number of digits that in source value.
        /// </summary>
        /// <param name="source">The source double value.</param>
        /// <returns>Random double value based on source.</returns>
        public double RandomizeDouble(double source)
        {
            return (double)RandomizeDecimal((decimal)source);
        }

        /// <summary>
        /// Gets the randomized Int16 value with same number of digits that in source value.
        /// </summary>
        /// <param name="source">The source Int16 value.</param>
        /// <returns>Random Int16 value based on source.</returns>
        public Int16 RandomizeInt16(Int16 source)
        {
            StringBuilder sb = new StringBuilder();

            int length = source.ToString(CultureInfo.InvariantCulture).Length;
            if (source < 0)
            {
                sb.Append('-');
                length--;
            }
            int maxLength = Int16.MaxValue.ToString(CultureInfo.InvariantCulture).Length;
            if (length < maxLength)
            {
                sb.Append(NextDigit(1, 9));
                sb.Append(NextDigits(length - 1));
            }
            else // Guarantee a value less than 32 000.
            {
                int next = NextDigit(1, 3);
                sb.Append(next);
                if (next < 3)
                    sb.Append(NextDigits(maxLength - 1));
                else
                {
                    sb.Append(NextDigit(1));
                    sb.Append(NextDigits(maxLength - 2));
                }
            }

            Int16 result;
            bool parsed = Int16.TryParse(sb.ToString(), out result);
            if (parsed)
                return result;

            return source;
        }

        /// <summary>
        /// Gets the randomized Int32 value with same number of digits that in source value.
        /// </summary>
        /// <param name="source">The source Int32 value.</param>
        /// <returns>Random Int32 value based on source.</returns>
        public Int32 RandomizeInt32(Int32 source)
        {
            StringBuilder sb = new StringBuilder();

            int length = source.ToString(CultureInfo.InvariantCulture).Length;
            if (source < 0)
            {
                sb.Append('-');
                length--;
            }
            int maxLength = Int32.MaxValue.ToString(CultureInfo.InvariantCulture).Length;
            if (length < maxLength)
            {
                sb.Append(NextDigit(1, 9));
                sb.Append(NextDigits(length - 1));
            }
            else // Guarantee a value less than 2 200 000 000.
            {
                int next = NextDigit(1, 2);
                sb.Append(next);
                if (next < 2)
                    sb.Append(NextDigits(maxLength - 1));
                else
                {
                    sb.Append(NextDigit(1));
                    sb.Append(NextDigits(maxLength - 2));
                }
            }

            Int32 result;
            bool parsed = Int32.TryParse(sb.ToString(), out result);
            if (parsed)
                return result;

            return source;
        }

        /// <summary>
        /// Gets the randomized Int64 value with same number of digits that in source value.
        /// </summary>
        /// <param name="source">The source Int64 value.</param>
        /// <returns>Random Int64 value based on source.</returns>
        public Int64 RandomizeInt64(Int64 source)
        {
            StringBuilder sb = new StringBuilder();

            int length = source.ToString(CultureInfo.InvariantCulture).Length;
            if (source < 0)
            {
                sb.Append('-');
                length--;
            }
            int maxLength = Int64.MaxValue.ToString(CultureInfo.InvariantCulture).Length;
            if (length < maxLength)
            {
                sb.Append(NextDigit(1, 9));
                sb.Append(NextDigits(length - 1));
            }
            else // Guarantee a value less than 9 200 000 000 000 000 000.
            {
                int next = NextDigit(1, 9);
                sb.Append(next);
                if (next < 9)
                    sb.Append(NextDigits(maxLength - 1));
                else
                {
                    sb.Append(NextDigit(1));
                    sb.Append(NextDigits(maxLength - 2));
                }
            }

            Int64 result;
            bool parsed = Int64.TryParse(sb.ToString(), out result);
            if (parsed)
                return result;
            return source;
        }

        /// <summary>
        /// Gets the randomized SByte value with same number of digits that in source value.
        /// </summary>
        /// <param name="source">The source SByte value.</param>
        /// <returns>Random SByte value based on source.</returns>
        public SByte RandomizeSByte(SByte source)
        {
            StringBuilder sb = new StringBuilder();

            int length = source.ToString(CultureInfo.InvariantCulture).Length;
            if (source < 0)
            {
                sb.Append('-');
                length--;
            }
            int maxLength = SByte.MaxValue.ToString(CultureInfo.InvariantCulture).Length;
            if (length < maxLength)
                sb.Append(NextDigits(length));
            else // Guarantee a value less than 128.
            {
                int next = NextDigit(1);
                sb.Append(next);
                if (next < 1)
                    sb.Append(NextDigits(maxLength - 1));
                else
                {
                    sb.Append(NextDigit(2));
                    sb.Append(NextDigit(7));
                }
            }

            SByte result;
            bool parsed = SByte.TryParse(sb.ToString(), out result);
            if (parsed)
                return result;

            return source;
        }

        /// <summary>
        /// Gets the randomized Single value with same number of digits that in source value.
        /// </summary>
        /// <param name="source">The source Single value.</param>
        /// <returns>Random Single value based on source.</returns>
        public float RandomizeFloat(float source)
        {
            return (float)RandomizeDecimal((decimal)source);
        }

        /// <summary>
        /// Gets the randomized string with same length and same whitespaces that in source string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>Random string based on source string.</returns>
        public string RandomizeString(string source)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in source)
            {
                if (char.IsWhiteSpace(c))
                    sb.Append(c);
                else if (char.IsLetter(c))
                    sb.Append(NextLetter(c));
                else if (char.IsDigit(c))
                    sb.Append(NextDigit());
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the randomized UInt16 value with same number of digits that in source value.
        /// </summary>
        /// <param name="source">The source UInt16 value.</param>
        /// <returns>Random UInt16 value based on source.</returns>
        public UInt16 RandomizeUInt16(UInt16 source)
        {
            StringBuilder sb = new StringBuilder();

            int length = source.ToString(CultureInfo.InvariantCulture).Length;
            int maxLength = UInt16.MaxValue.ToString(CultureInfo.InvariantCulture).Length;
            if (length < maxLength)
            {
                sb.Append(NextDigit(1, 9));
                sb.Append(NextDigits(length - 1));
            }
            else // Guarantee a value less than 65 000.
            {
                int next = NextDigit(1, 6);
                sb.Append(next);
                if (next < 6)
                    sb.Append(NextDigits(maxLength - 1));
                else
                {
                    sb.Append(NextDigit(4));
                    sb.Append(NextDigits(maxLength - 2));
                }
            }

            UInt16 result;
            bool parsed = UInt16.TryParse(sb.ToString(), out result);
            if (parsed)
                return result;

            return source;
        }

        /// <summary>
        /// Gets the randomized UInt32 value with same number of digits that in source value.
        /// </summary>
        /// <param name="source">The source UInt32 value.</param>
        /// <returns>Random UInt32 value based on source.</returns>
        public UInt32 RandomizeUInt32(UInt32 source)
        {
            StringBuilder sb = new StringBuilder();

            int length = source.ToString(CultureInfo.InvariantCulture).Length;
            int maxLength = UInt32.MaxValue.ToString(CultureInfo.InvariantCulture).Length;
            if (length < maxLength)
            {
                sb.Append(NextDigit(1, 9));
                sb.Append(NextDigits(length - 1));
            }
            else // Guarantee a value less than 4 200 000 000.
            {
                int next = NextDigit(1, 4);
                sb.Append(next);
                if (next < 4)
                    sb.Append(NextDigits(maxLength - 1));
                else
                {
                    sb.Append(NextDigit(1));
                    sb.Append(NextDigits(maxLength - 2));
                }
            }

            UInt32 result;
            bool parsed = UInt32.TryParse(sb.ToString(), out result);
            if (parsed)
                return result;

            return source;
        }

        /// <summary>
        /// Gets the randomized UInt64 value with same number of digits that in source value.
        /// </summary>
        /// <param name="source">The source UInt64 value.</param>
        /// <returns>Random UInt64 value based on source.</returns>
        public UInt64 RandomizeUInt64(UInt64 source)
        {
            StringBuilder sb = new StringBuilder();

            int length = source.ToString(CultureInfo.InvariantCulture).Length;
            int maxLength = UInt64.MaxValue.ToString(CultureInfo.InvariantCulture).Length;
            if (length < maxLength)
            {
                sb.Append(NextDigit(1, 9));
                sb.Append(NextDigits(length - 1));
            }
            else // Guarantee a value less than 18 400 000 000 000 000 000.
            {
                sb.Append(1);
                int next = NextDigit(8);
                sb.Append(next);
                if (next < 8)
                    sb.Append(NextDigits(maxLength - 2));
                else
                {
                    sb.Append(NextDigit(3));
                    sb.Append(NextDigits(maxLength - 3));
                }
            }

            UInt64 result;
            bool parsed = UInt64.TryParse(sb.ToString(), out result);
            if (parsed)
                return result;
            return source;
        }

        /// <summary>
        /// Gets randomized object based on the source object.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="type">The type of object.</param>
        /// <returns>Random object based on source.</returns>
        public object GetRandomObject(object source, Type type)
        {
            try
            {
                if (type == typeof(string))
                    return RandomizeString((string)source);
                else if (type == typeof(Int32))
                    return RandomizeInt32((Int32)source);
                else if (type == typeof(double))
                    return RandomizeDouble((double)source);
                else if (type == typeof(DateTime))
                    return NextDay(new DateTime(1990, 1, 1));
                else if (type == typeof(Int64))
                    return RandomizeInt64((Int64)source);
                else if (type == typeof(decimal))
                    return RandomizeDecimal((decimal)source);
                else if (type == typeof(Int16))
                    return RandomizeInt16((Int16)source);
                else if (type == typeof(float))
                    return RandomizeFloat((float)source);
                else if (type == typeof(char))
                    return NextChar();
                else if (type == typeof(byte))
                    return NextByte();
                else if (type == typeof(UInt32))
                    return RandomizeUInt32((UInt32)source);
                else if (type == typeof(UInt64))
                    return RandomizeUInt64((UInt64)source);
                else if (type == typeof(UInt16))
                    return RandomizeUInt16((UInt16)source);
                else if (type == typeof(byte[]))
                    return NextBytes(((byte[])source).Length);
                else if (type == typeof(SByte))
                    return RandomizeSByte((sbyte)source);
                else if (type == typeof(TimeSpan))
                    return NextTimeSpanBetweenHours(0, 24);
            }
            catch
            {
                return source;
            }

            return source;
        }

        /// <summary>
        /// Randomizes datasources.
        /// </summary>
        /// <param name="datasources">Collection of datasources.</param>
        public void RandomizeDataSources(DataSourceCollection datasources)
        {
            Dictionary<string, FRColumnInfo> uniquesAndRelations = new Dictionary<string, FRColumnInfo>();

            // Get list of related columns and columns with unique values with their type and length.
            foreach (DataSourceBase datasource in datasources)
            {
                if (datasource is TableDataSource)
                {
                    DataTable table = (datasource as TableDataSource).Table;
                    DataSet ds = table.DataSet;
                    int length = table.Rows.Count;
                    for (int c = 0; c < table.Columns.Count; c++)
                        foreach (DataColumn column in table.Columns)
                        {
                            if (column.Unique)
                            {
                                if (!uniquesAndRelations.ContainsKey(column.ColumnName))
                                    uniquesAndRelations.Add(column.ColumnName, new FRColumnInfo(column.DataType, length));
                            }
                        }
                    foreach (DataRelation dr in ds.Relations)
                    {
                        foreach (DataColumn dc in dr.ParentColumns)
                        {
                            if (!uniquesAndRelations.ContainsKey(dc.ColumnName))
                                uniquesAndRelations.Add(dc.ColumnName, new FRColumnInfo(dc.DataType, length));
                        }
                        foreach (DataColumn dc in dr.ChildColumns)
                        {
                            if (!uniquesAndRelations.ContainsKey(dc.ColumnName))
                                uniquesAndRelations.Add(dc.ColumnName, new FRColumnInfo(dc.DataType, length));
                        }
                    }
                }
            }

            Dictionary<string, FRRandomFieldValueCollection> dict = new Dictionary<string, FRRandomFieldValueCollection>();
            foreach (KeyValuePair<string, FRColumnInfo> pair in uniquesAndRelations)
            {
                dict.Add(pair.Key, new FRRandomFieldValueCollection());
            }

            // Get values for related columns and columns with unique values.
            foreach (DataSourceBase datasource in datasources)
            {
                if (datasource is TableDataSource)
                {
                    DataTable table = (datasource as TableDataSource).Table;
                    for (int c = 0; c < table.Columns.Count; c++)
                    {
                        DataColumn column = table.Columns[c];
                        if (!uniquesAndRelations.ContainsKey(column.ColumnName))
                            continue;

                        Type type = uniquesAndRelations[column.ColumnName].Type;
                        for (int r = 0; r < table.Rows.Count; r++)
                        {
                            object val = table.Rows[r][c];
                            if (val != null && !(val is System.DBNull) && !dict[column.ColumnName].ContainsOrigin(val))
                            {
                                object randomVal;
                                do
                                {
                                    randomVal = GetRandomObject(val, type);
                                }
                                while (dict[column.ColumnName].ContainsRandom(new FRRandomFieldValue(val, randomVal)));
                                dict[column.ColumnName].Add(new FRRandomFieldValue(val, randomVal));
                            }
                        }
                    }
                }
            }

            // Randomize all table datasources.
            foreach (DataSourceBase datasource in datasources)
            {
                if (datasource is TableDataSource)
                {
                    (datasource as TableDataSource).StoreData = true;
                    DataTable table = (datasource as TableDataSource).Table;
                    for (int c = 0; c < table.Columns.Count; c++)
                    {
                        if (table.Columns[c].ReadOnly)
                            continue;

                        Type type = table.Columns[c].DataType;
                        for (int r = 0; r < table.Rows.Count; r++)
                        {
                            object val = table.Rows[r][c];
                            if (val != null && !(val is System.DBNull))
                            {
                                if (uniquesAndRelations.ContainsKey(table.Columns[c].ColumnName))
                                    table.Rows[r][c] = dict[table.Columns[c].ColumnName].GetRandom(val);
                                else
                                    table.Rows[r][c] = GetRandomObject(val, type);
                            }
                        }
                    }
                }
            }
        }

        #endregion Public Methods

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FRRandom"/> class.
        /// </summary>
        public FRRandom()
        {
            random = new Random();
        }

        #endregion Constructors
    }

    /// <summary>
    /// Represents information about column.
    /// </summary>
    public class FRColumnInfo
    {
        #region Fields

        private Type type;
        private int length;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the type of column.
        /// </summary>
        public Type Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets or sets the length of column.
        /// </summary>
        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FRColumnInfo"/> class.
        /// </summary>
        /// <param name="type">The type of column.</param>
        /// <param name="length">The lenght of column.</param>
        public FRColumnInfo(Type type, int length)
        {
            this.type = type;
            this.length = length;
        }

        #endregion Constructors
    }

    /// <summary>
    /// Represents random value of field.
    /// </summary>
    public class FRRandomFieldValue
    {
        #region Fields

        private object origin;
        private object random;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the original value of field.
        /// </summary>
        public object Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        /// <summary>
        /// Gets or sets the random value of field.
        /// </summary>
        public object Random
        {
            get { return random; }
            set { random = value; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FRRandomFieldValue"/> class.
        /// </summary>
        /// <param name="origin">The original value of field.</param>
        /// <param name="random">The random value of field.</param>
        public FRRandomFieldValue(object origin, object random)
        {
            this.origin = origin;
            this.random = random;
        }

        #endregion Constructors
    }

    /// <summary>
    /// Represents collection of random values of field.
    /// </summary>
    public class FRRandomFieldValueCollection
    {
        #region Fields

        private readonly List<FRRandomFieldValue> list;

        #endregion Fields

        #region Properties
        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FRRandomFieldValueCollection"/> class.
        /// </summary>
        public FRRandomFieldValueCollection()
        {
            list = new List<FRRandomFieldValue>();
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Adds an object to the end of this collection.
        /// </summary>
        /// <param name="value">Object to add.</param>
        public void Add(FRRandomFieldValue value)
        {
            list.Add(value);
        }

        /// <summary>
        /// Determines whether an element with the same origin value is in the collection.
        /// </summary>
        /// <param name="origin">The object to locate in the collection.</param>
        /// <returns><b>true</b> if object is found in the collection; otherwise, <b>false</b>.</returns>
        public bool ContainsOrigin(object origin)
        {
            foreach (FRRandomFieldValue value in list)
            {
                if (value.Origin == origin)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether an element with the same random value is in the collection.
        /// </summary>
        /// <param name="random">The object to locate in the collection.</param>
        /// <returns><b>true</b> if object is found in the collection; otherwise, <b>false</b>.</returns>
        public bool ContainsRandom(object random)
        {
            foreach (FRRandomFieldValue value in list)
            {
                if (value.Random == random)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the random value for specified origin.
        /// </summary>
        /// <param name="origin">The origin value.</param>
        /// <returns>The random value.</returns>
        public object GetRandom(object origin)
        {
            foreach (FRRandomFieldValue value in list)
            {
                if (value.Origin == origin)
                    return value.Random;
            }

            return origin;
        }

        #endregion Public Methods
    }
}