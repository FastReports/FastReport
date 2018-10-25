//
// In order to convert some functionality to Visual C#, the Java Language Conversion Assistant
// creates "support classes" that duplicate the original functionality.  
//
// Support classes replicate the functionality of the original code, but in some cases they are 
// substantially different architecturally. Although every effort is made to preserve the 
// original architecture of the application in the converted project, the user should be aware that 
// the primary goal of these support classes is to replicate functionality, and that at times 
// the architecture of the resulting solution may differ somewhat.
//

using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Barcode.Aztec
{
   /// <summary>
   /// Contains conversion support elements such as classes, interfaces and static methods.
   /// </summary>
   internal static class SupportClass
   {
      /*******************************/
      /// <summary>
      /// Copies an array of chars obtained from a String into a specified array of chars
      /// </summary>
      /// <param name="sourceString">The String to get the chars from</param>
      /// <param name="sourceStart">Position of the String to start getting the chars</param>
      /// <param name="sourceEnd">Position of the String to end getting the chars</param>
      /// <param name="destinationArray">Array to return the chars</param>
      /// <param name="destinationStart">Position of the destination array of chars to start storing the chars</param>
      /// <returns>An array of chars</returns>
      public static void GetCharsFromString(System.String sourceString, int sourceStart, int sourceEnd, char[] destinationArray, int destinationStart)
      {
         int sourceCounter = sourceStart;
         int destinationCounter = destinationStart;
         while (sourceCounter < sourceEnd)
         {
            destinationArray[destinationCounter] = (char)sourceString[sourceCounter];
            sourceCounter++;
            destinationCounter++;
         }
      }

      /*******************************/
      /// <summary>
      /// Sets the capacity for the specified List
      /// </summary>
      /// <param name="vector">The List which capacity will be set</param>
      /// <param name="newCapacity">The new capacity value</param>
      public static void SetCapacity<T>(System.Collections.Generic.IList<T> vector, int newCapacity) where T : new()
      {
         while (newCapacity > vector.Count)
            vector.Add(new T());
         while (newCapacity < vector.Count)
            vector.RemoveAt(vector.Count - 1);
      }

      /// <summary>
      /// Converts a string-Collection to an array
      /// </summary>
      /// <param name="strings">The strings.</param>
      /// <returns></returns>
      public static String[] toStringArray(ICollection<string> strings)
      {
         String[] result = new String[strings.Count];
         strings.CopyTo(result, 0);
         return result;
      }

      /// <summary>
      /// Joins all elements to one string.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="separator">The separator.</param>
      /// <param name="values">The values.</param>
      /// <returns></returns>
      public static string Join<T>(string separator, IEnumerable<T> values)
      {
         StringBuilder builder = new StringBuilder();
         separator = separator ?? String.Empty;
         if (values != null)
         {
            foreach (T value in values)
            {
               builder.Append(value);
               builder.Append(separator);
            }
            if (builder.Length > 0)
               builder.Length -= separator.Length;
         }

         return builder.ToString();
      }

      /// <summary>
      /// Fills the specified array.
      /// (can't use extension method because of .Net 2.0 support)
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="array">The array.</param>
      /// <param name="value">The value.</param>
      public static void Fill<T>(T[] array, T value)
      {
         for (int i = 0; i < array.Length; i++)
         {
            array[i] = value;
         }
      }

      /// <summary>
      /// Fills the specified array.
      /// (can't use extension method because of .Net 2.0 support)
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="array">The array.</param>
      /// <param name="startIndex">The start index.</param>
      /// <param name="endIndex">The end index.</param>
      /// <param name="value">The value.</param>
      public static void Fill<T>(T[] array, int startIndex, int endIndex, T value)
      {
         for (int i = startIndex; i < endIndex; i++)
         {
            array[i] = value;
         }
      }

      public static string ToBinaryString(int x)
      {
         char[] bits = new char[32];
         int i = 0;

         while (x != 0)
         {
            bits[i++] = (x & 1) == 1 ? '1' : '0';
            x >>= 1;
         }

         Array.Reverse(bits, 0, i);
         return new string(bits);
      }

      public static int bitCount(int n)
      {
         int ret = 0;
         while (n != 0)
         {
            n &= (n - 1);
            ret++;
         }
         return ret;
      }

      ///// <summary>
      ///// Savely gets the value of a decoding hint
      ///// if hints is null the default is returned
      ///// </summary>
      ///// <typeparam name="T"></typeparam>
      ///// <param name="hints">The hints.</param>
      ///// <param name="hintType">Type of the hint.</param>
      ///// <param name="default">The @default.</param>
      ///// <returns></returns>
      //public static T GetValue<T>(IDictionary<DecodeHintType, object> hints, DecodeHintType hintType, T @default)
      //{
      //   // can't use extension method because of .Net 2.0 support

      //   if (hints == null)
      //      return @default;
      //   if (!hints.ContainsKey(hintType))
      //      return @default;

      //   return (T)hints[hintType];
      //}
   }
}