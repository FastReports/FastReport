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
namespace FastReport.Barcode.QRCode
{
  /*/// <summary>
  /// Contains conversion support elements such as classes, interfaces and static methods.
  /// </summary>*/
  internal class SupportClass
  {
    /*******************************/
    /*/// <summary>
    /// Performs an unsigned bitwise right shift with the specified number
    /// </summary>
    /// <param name="number">Number to operate on</param>
    /// <param name="bits">Ammount of bits to shift</param>
    /// <returns>The resulting number from the shift operation</returns>*/
    public static int URShift(int number, int bits)
    {
      if (number >= 0)
        return number >> bits;
      else
        return (number >> bits) + (2 << ~bits);
    }

    /*/// <summary>
    /// Performs an unsigned bitwise right shift with the specified number
    /// </summary>
    /// <param name="number">Number to operate on</param>
    /// <param name="bits">Ammount of bits to shift</param>
    /// <returns>The resulting number from the shift operation</returns>*/
    public static int URShift(int number, long bits)
    {
      return URShift(number, (int)bits);
    }

    /*/// <summary>
    /// Performs an unsigned bitwise right shift with the specified number
    /// </summary>
    /// <param name="number">Number to operate on</param>
    /// <param name="bits">Ammount of bits to shift</param>
    /// <returns>The resulting number from the shift operation</returns>*/
    public static long URShift(long number, int bits)
    {
      if (number >= 0)
        return number >> bits;
      else
        return (number >> bits) + (2L << ~bits);
    }

    /*/// <summary>
    /// Performs an unsigned bitwise right shift with the specified number
    /// </summary>
    /// <param name="number">Number to operate on</param>
    /// <param name="bits">Ammount of bits to shift</param>
    /// <returns>The resulting number from the shift operation</returns>*/
    public static long URShift(long number, long bits)
    {
      return URShift(number, (int)bits);
    }

    /*******************************/
    /*/// <summary>
    /// This method returns the literal value received
    /// </summary>
    /// <param name="literal">The literal to return</param>
    /// <returns>The received value</returns>*/
    public static long Identity(long literal)
    {
      return literal;
    }

    /*/// <summary>
    /// This method returns the literal value received
    /// </summary>
    /// <param name="literal">The literal to return</param>
    /// <returns>The received value</returns>*/
    public static ulong Identity(ulong literal)
    {
      return literal;
    }

    /*/// <summary>
    /// This method returns the literal value received
    /// </summary>
    /// <param name="literal">The literal to return</param>
    /// <returns>The received value</returns>*/
    public static float Identity(float literal)
    {
      return literal;
    }

    /*/// <summary>
    /// This method returns the literal value received
    /// </summary>
    /// <param name="literal">The literal to return</param>
    /// <returns>The received value</returns>*/
    public static double Identity(double literal)
    {
      return literal;
    }


    /*******************************/
    /*/// <summary>
    /// Receives a byte array and returns it transformed in an sbyte array
    /// </summary>
    /// <param name="byteArray">Byte array to process</param>
    /// <returns>The transformed array</returns>*/
    public static sbyte[] ToSByteArray(byte[] byteArray)
    {
      sbyte[] sbyteArray = null;
      if (byteArray != null)
      {
        sbyteArray = new sbyte[byteArray.Length];
        for (int index = 0; index < byteArray.Length; index++)
          sbyteArray[index] = (sbyte)byteArray[index];
      }
      return sbyteArray;
    }
  }
}