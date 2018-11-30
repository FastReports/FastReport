using System;
using System.Collections.Generic;
using System.Text;


namespace FastReport.Utils
{
    /// <summary>
    /// Fast alternative of StringBuilder.
    /// </summary>
    public class FastString
    {
        #region Constants

        private const int INIT_CAPACITY = 32;

        #endregion Constants

        #region Fields

        /*        private char[] chars = null;
                private int count = 0;
                private int capacity = 0;
                private List<char> repList = null;
                */
        private string result = "";
        StringBuilder sb;
        private bool done = false;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the Length of string.
        /// </summary>
        public int Length
        {
            get { return sb.Length;  }
            set
            {
                sb.Length = value;
                done = false;
            }
        }

        /// <summary>
        /// Gets or sets the chars of string.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Char value</returns>
        public char this[int index]
        {
            get
            {
                if (index >= 0 && index < sb.Length)
                    return sb[index];
                else
                    throw new IndexOutOfRangeException();
            }
            set
            {
                if (index >= 0 && index < sb.Length)
                    sb[index] = value;
                else
                    throw new IndexOutOfRangeException();
                done = false;
            }
        }

        /// <summary>
        /// Gets StringBuilder
        /// </summary>
        public StringBuilder StringBuilder
        {
            get { return sb;  }
        }

        #endregion Properties

        #region Private Methods

/*        /// <summary>
        /// Reallocate internal array.
        /// </summary>
        /// <param name="addLength">Additional length.</param>
        private void ReAlloc(int addLength)
        {
            if (count + addLength > capacity)
            {
                capacity = (capacity + addLength) > capacity * 2 ? capacity + addLength : capacity * 2;
                //Array.Resize<char>(ref chars, capacity);
                char[] newChars = new char[capacity];
                Array.Copy(chars, newChars, chars.Length);
                chars = newChars;
            }
        }*/

        /// <summary>
        /// Initialize the new array for chars.
        /// </summary>
        /// <param name="iniCapacity">Length of initial array.</param>
        private void Init(int iniCapacity)
        {
            sb = new StringBuilder(iniCapacity);
            //chars = new char[iniCapacity];
            //capacity = iniCapacity;
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Checks the empty array.
        /// </summary>
        /// <returns>True if string is empty.</returns>
        public bool IsEmpty()
        {
            return sb.Length == 0;
        }

        /// <summary>
        /// Converts the array in string.
        /// </summary>
        /// <returns>String value.</returns>
        public override string ToString()
        {
            if (!done)
            {
                result = sb.ToString();
                //result = new string(chars, 0, count);
                done = true;
            }
            return result;
        }

        /// <summary>
        /// Clears the string.
        /// </summary>
        /// <returns>FastString object.</returns>
        public FastString Clear()
        {
            //count = 0;
            sb.Length = 0; //Clear();
            done = false;
            return this;
        }

        /// <summary>
        /// Appends the string by string value.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <returns>FastString object.</returns>
        public FastString Append(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                /*                ReAlloc(value.Length);
                                value.CopyTo(0, chars, count, value.Length);
                                count += value.Length;*/
                sb.Append(value);
                done = false;
            }
            return this;
        }

        /// <summary>
        /// Appends the string by string value.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <returns>FastString object.</returns>
        public FastString AppendLine(string value)
        {
            /*            ReAlloc(value.Length + 2);
                        value.CopyTo(0, chars, count, value.Length);
                        count += value.Length + 2;
                        chars[count - 2] = '\r';
                        chars[count - 1] = '\n';*/
            sb.AppendLine(value);
            done = false;
            return this;
        }

        /// <summary>
        /// Append formatted string.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public FastString AppendFormat(String format, params Object[] args)
        {
            /*            string value = String.Format(format, args);
                        ReAlloc(value.Length);
                        value.CopyTo(0, chars, count, value.Length);
                        count += value.Length;*/
            sb.AppendFormat(format, args);
            done = false;
            return this;
        }

        /// <summary>
        /// Appends new line.
        /// </summary>
        /// <returns>FastString object.</returns>
        public FastString AppendLine()
        {
            /*            ReAlloc(2);
                        count += 2;
                        chars[count - 2] = '\r';
                        chars[count - 1] = '\n';*/
            sb.AppendLine();
            done = false;
            return this;
        }

        /// <summary>
        /// Appends the string by char value.
        /// </summary>
        /// <param name="value">Char value.</param>
        /// <returns>FastString object.</returns>
        public FastString Append(char value)
        {
            //ReAlloc(1);
            //chars[count++] = value;
            sb.Append(value);
            done = false;
            return this;
        }

        /// <summary>
        /// Appends the another FastString object.
        /// </summary>
        /// <param name="fastString">FastString object.</param>
        /// <returns>FastString object.</returns>
        public FastString Append(FastString fastString)
        {
            if (fastString != null)
            {
                sb.Append(fastString.StringBuilder);
                done = false;
            }
            return this;
        }

        /// <summary>
        /// Appends the string by object data.
        /// </summary>
        /// <param name="value">Object value.</param>
        /// <returns>FastString object.</returns>
        public FastString Append(object value)
        {
            sb.Append(value);
            done = false;
            //Append(value.ToString());
            return this;
        }

        /// <summary>
        /// Copies the substring in char array.
        /// </summary>
        /// <param name="sourceIndex">Start index in source.</param>
        /// <param name="destination">Destination array.</param>
        /// <param name="destinationIndex">Destination index.</param>
        /// <param name="count">Count of chars</param>
        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            sb.CopyTo(sourceIndex, destination, destinationIndex, count);
            //Array.Copy(chars, sourceIndex, destination, destinationIndex, count);
        }

        /// <summary>
        /// Removes substring.
        /// </summary>
        /// <param name="startIndex">Start index of removed string.</param>
        /// <param name="length">Length of removed string.</param>
        /// <returns>FastString object.</returns>
        public FastString Remove(int startIndex, int length)
        {
            sb.Remove(startIndex, length);
            /*         
            if (startIndex >= 0 && (length + startIndex) <= count)
            {
                int len = count - length - startIndex;                
                
                for (int i = 0; i < len; i++)
                    chars[startIndex + i] = chars[startIndex + i + length];

                count -= length;
                done = false;
            }*/
            done = false;
            return this;
        }

        /// <summary>
        /// Inserts string.
        /// </summary>
        /// <param name="startIndex">Start index in existing string.</param>
        /// <param name="value">Value of inserting string.</param>
        /// <returns>FastString object.</returns>
        public FastString Insert(int startIndex, string value)
        {
            sb.Insert(startIndex, value);
            /*
            if (value != null && value.Length > 0 && startIndex >= 0 && startIndex <= count)
            {
                int countToMove = count - startIndex;
                ReAlloc(value.Length);
                count += value.Length;
                for (int i = count - 1; i > count - countToMove - 1; i--)
                    chars[i] = chars[i - value.Length];
                value.CopyTo(0, chars, startIndex, value.Length);
            }*/
            done = false;
            return this;
        }

        /// <summary>
        /// Replacing the substring on other.
        /// </summary>
        /// <param name="oldValue">Old string value.</param>
        /// <param name="newValue">New string value.</param>
        /// <returns>FastString object.</returns>
        public FastString Replace(string oldValue, string newValue)
        {
            sb.Replace(oldValue, newValue);
            /*
            if (count == 0)
                return this;

            if (repList == null)
                repList = new List<char>();

            for (int i = 0; i < count; i++)
            {
                bool isRep = false;
                if (chars[i] == oldValue[0]) 
                {
                    int k = 1;
                    while (k < oldValue.Length && chars[i + k] == oldValue[k])
                        k++;
                    isRep = (k >= oldValue.Length);
                }
                if (isRep) 
                {
                    i += oldValue.Length - 1;
                    if (newValue != null)
                        for (int k = 0; k < newValue.Length; k++)
                            repList.Add(newValue[k]);
                }
                else 
                    repList.Add(chars[i]);
            }

            ReAlloc(repList.Count - count);
            for (int k = 0; k < repList.Count; k++)
                chars[k] = repList[k];
            count = repList.Count;
            repList.Clear();
            */
	    done = false;
            return this;
        }

        /// <summary>
        /// Index of substring.
        /// </summary>
        /// <param name="value">Substring for search.</param>
        /// <param name="startIndex">Sarting position for search.</param>
        /// <returns>Position of substring.</returns>
        public int IndexOf(string value, int startIndex)
        {
            if (!String.IsNullOrEmpty(value) && 
                startIndex >= 0 && 
                startIndex < sb.Length)
            {
                int valueIndex = 0;
                for (int i = startIndex; i < sb.Length; i++)
                {
                    if (sb[i] == value[valueIndex])
                    {
                        valueIndex++;
                        if (valueIndex == value.Length)
                            return i;
                    }
                    else
                        valueIndex = 0;
                }
            }
            return -1;
        }

        /// <summary>
        /// Compare of substring in position.
        /// </summary>
        /// <param name="startIndex">Starting index for comparsion.</param>
        /// <param name="value">Value for compare.</param>
        /// <returns>True if substring is identical in position.</returns>
        public bool SubstringCompare(int startIndex, string value)
        {
            if (!String.IsNullOrEmpty(value) && 
                startIndex >= 0 && 
                startIndex < sb.Length)
            {
                int valueIndex = 0;
                for (int i = 0; i < value.Length; i++)
                {
                    if (sb[startIndex + i] == value[i])
                    {
                        valueIndex++;
                        if (valueIndex == value.Length)
                            return true;
                    }
                    else
                        valueIndex = 0;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the substring.
        /// </summary>
        /// <param name="startIndex">Starting index.</param>
        /// <param name="length">Length of substring.</param>
        /// <returns>Substring.</returns>
        public string Substring(int startIndex, int length)
        {
            char[] result = new char[length];
            sb.CopyTo(startIndex, result, 0, length);
            return new string(result);
 /*           if (startIndex + length > count)
                return new string(chars, startIndex, count - startIndex);
            else
                return new string(chars, startIndex, length);*/
        }

        #endregion Public Methods

        #region Constructors

        /// <summary>
        /// Creates the new FastString object with initial capacity.
        /// </summary>
        /// <param name="initCapacity">Initial capacity.</param>
        public FastString(int initCapacity)
        {
            Init(initCapacity);
        }

        /// <summary>
        /// Creates the new FastString object with default capacity.
        /// </summary>
        public FastString()
        {
            Init(INIT_CAPACITY);
        }

        /// <summary>
        /// Creates the new FastString object from initial string.
        /// </summary>
        /// <param name="initValue"></param>
        public FastString(string initValue)
        {
            if (!string.IsNullOrEmpty(initValue))
            {
                Init(initValue.Length);
                Append(initValue);
            }
            else
                Init(0);
        }

        #endregion Constructors
    }

    public class FastStringWithPool : FastString
    {
        Dictionary<string, string> pool;

        public FastStringWithPool(Dictionary<string, string> pool): base()
        {
            this.pool = pool;
        }

        public override string ToString()
        {
            string baseResult = base.ToString();
            string result;
            if (pool.TryGetValue(baseResult, out result))
                return result;
            return pool[baseResult] = baseResult;
        }
    }
}