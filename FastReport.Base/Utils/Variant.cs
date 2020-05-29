//**************************************
// Name: The Return of the Variant (ver. 6)
// Description:A Variant structure written in C#, ideal for VB.NET 2005 or C# 2002/2003/2005 developers who miss the old (and, in the eyes of many, ugly) VB6 Variant. A Variant is a type that contains any type of variable and can be worked with "automagically" without heavy typecasting procedures. (System.Object is not a variant equivalent because you cannot do things like addition with it.)
// One important difference about this structure: It becomes what you do to it. So if the value is a string and you add an integer of 12, the value becomes an integer. If you want it to remain the same, you should typecast first--for example, add "12" rather than (int)12.
// Changes for v2:
// - All type properties ("String", etc.) are now private.
// - Added implicit/explicit custom operators, so typecasts now work naturally. For example:
// string myString = (string)myVariant; // no longer myVariant.String ...
// float myFloat = (float)myVariant; // no longer myVariant.Float ...
// bool myBool = (bool)myVariant; // no longer myVariant.Boolean ....
// Changes for v3: Quite a few. Added DateTime support. Implemented IConvertible. Changed all the private type name properties to public "To[type]()" methods. Changed the naming convention to conform to IConvertible.
// Changes to v3.1: Removed many unnecessary if statements. Made some documentation updates.
// Changes to v4: Removed the "Type becomes what you do with it" rule for numeric operations. Example: adding an integer of 1 to a float of 1.1 results in a float of 2.1 rather than an integer of 2. Changed operator overloads to automatically adjust the type if the operation causes the resulting value to extend numerically beyond the value of the type. For example, if an integer (Int32) operation results in a value than can fit into an integer, the value is changed to a long (Int64).
// Changes to v5: Changed operator overloads so that if auto-converting to a compatible type, the new type is not smaller than the previous type; Made changes to CBln() to default to True if the object is a non-empty string; Added lots of documentation. Change to v5.1... it's IsNumberable, not IsNumerable. :) Also, To[Type]() should default to the specified type. And SmartMath() method name changed to VariantArithmetic(). Changes in v6... Added TimeSpan type support; added IsDate, IsTimeSpan; updated operator overloads, including equality (==).
// By: Jon Davis
//
//
// Inputs:// Here is an example of use:
// start with a string of "1"
// Variant da = new Variant("1");

// add an integer of 2, becomes 3
// da += 2;

// add a float of 1.3, becomes 4.3
// da += 1.3F;

// add a string, becomes "4.3wow"
// da += "wow";

// writes "4.3wow"
// Console.WriteLine(da);

//
// Returns:None
//
// Assumes:None
//
// Side Effects:Note that as with all typecasting rules, the Variant can be
// dangerous if you are unsure of what you are doing. In this case, the
// Variant class remains strongly typed, while making the most of the CLR's
// intrinsic type conversion features. Conversion of a float (1.5) to an
// integer, for instance, will of course result in data loss. In some cases,
// conversion will simply fail, such as extracting the integer value of the
// string "wow".
// This code is copyrighted and has limited warranties.
// Please see http://www.Planet-Source-Code.com/xq/ASP/txtCodeId.2854/lngWId.10/qx/vb/scripts/ShowCode.htm
// for details.
//**************************************

using System;

namespace FastReport
{
  /// <summary>
  /// A strongly typed object that readily casts an intrinsic
  /// object to the other intrinsic types when possible.
  /// </summary>
  /// <remarks>
  /// <para>The Variant class is an intrinsic object container structure
  /// inspired by Visual Basic 6.0's Variant. The key features
  /// of a Variant class include the ability to perform typecasts and
  /// arithmetic between types that are not normally considered compatible.
  /// For example, if a Variant class contains a string describing a
  /// number, such as "1.1", then floating point arithmetic can be
  /// performed on it.</para>
  /// <para>Variants are normally considered dangerous because they
  /// strip away the effectiveness of type safety, which is the
  /// reason why the Visual Basic 6.0 Variant was left out of
  /// Visual Basic .NET. However, this implementation restores the
  /// Variant structure, both as a proof of concept and as a
  /// restoration of the utility and positive merits of the Variant
  /// where it can be used responsibly.</para>
  /// </remarks>
  public struct Variant : System.IConvertible, IComparable {
    /// <summary>
    /// Creates a strongly typed object that readily casts a primitive
    /// object to the other primitive types when possible.
    /// </summary>
    public Variant(object value) {
      if (value == DBNull.Value)
        value = null;
      if (value != null && value.GetType() == typeof(Variant))
        _value = ((Variant)value).Value;
      else
        _value = value;
    }
    private object _value;
    /// <summary>
    /// The actual value being stored in its original <see cref="System.Type"/>,
    /// returned as an <see cref="Object"/>.
    /// </summary>
    public object Value {
      get {
        return this._value;
      }
    }
    /// <summary>
    /// The <see cref="System.Type"/> of the <see cref="Value"/> property.
    /// </summary>
    public Type Type {
      get {
        if (_value == null) return null;
        return _value.GetType();
      }
    }
    /// <summary>
    /// Returns the <see cref="System.TypeCode"/> for this instance.
    /// </summary>
    /// <returns>The enumerated constant that is the <see cref="System.TypeCode"/>
    /// of the class or value type that implements this interface.</returns>
    public TypeCode GetTypeCode() {
      return System.Type.GetTypeCode(this.Type);
    }
    /// <summary>
    /// Returns the string equivalent of the <see cref="Value"/> property.
    /// </summary>
    private string String {
      get {
        return _value.ToString();
      }
    }
    /// <summary>
    /// Attempts to convert or typecast to the specified type.
    /// </summary>
    /// <param name="type">The type to convert or cast to.</param>
    /// <returns>The object after typecasting.</returns>
    public object ToType(Type type) {
      return ToType(type, false, null);
    }
    /// <summary>
    /// Attempts to convert or typecast to the specified type.
    /// </summary>
    /// <param name="type">The type to convert or cast to.</param>
    /// <param name="provider">An <see cref="IFormatProvider"/>
    /// interface implementation that supplies culture-specific formatting information.</param>
    /// <returns>The object after typecasting.</returns>
    public object ToType(Type type, IFormatProvider provider) {
      return ToType(type, false, provider);
    }
    private object ToType(Type type, bool nomap) {
      return ToType(type, nomap, null);
    }
    private object ToType(Type type, bool nomap, IFormatProvider formatProvider) {
      if (type == this.Type) return _value;
      if (!nomap && formatProvider == null) {
        return TypeMap(type);
      }
      try {
        if (formatProvider != null) {
          return System.Convert.ChangeType(_value, type, formatProvider);
        } else {
          return System.Convert.ChangeType(_value, type);
        }
      } catch {}
      System.Reflection.MethodInfo[] mis = this.Type.GetMethods();
      foreach (System.Reflection.MethodInfo mi in mis) {
        if (mi.Name.ToLower() == "to" + type.Name.ToLower()) {
          try {
            if (formatProvider != null && mi.GetParameters().Length == 1) {
              if (mi.GetParameters()[0].ParameterType.IsSubclassOf(typeof(IFormatProvider)))
                return mi.Invoke(_value, new object[] {formatProvider});
            }
            return mi.Invoke(_value, new object[] {});
          } catch {}
        }
      }
      if (!nomap) return TypeMap(type);
      throw new InvalidCastException(
        "Cannot determine conversion method.");
    }
    private object TypeMap(Type type) {
      Type tt = type;
      if (tt == typeof(System.Boolean))
        return this.ToBoolean();
      if (tt == typeof(System.String))
        return this.ToString();
      if (tt == typeof(System.Char))
        return this.ToChar();
      if (tt == typeof(System.Byte))
        return this.ToByte();
      if (tt == typeof(System.Int16))
        return this.ToInt16();
      if (tt == typeof(System.Int32))
        return this.ToInt32();
      if (tt == typeof(System.Int64))
        return this.ToInt64();
      if (tt == typeof(System.SByte))
        return this.ToSByte();
      if (tt == typeof(System.UInt16))
        return this.ToUInt16();
      if (tt == typeof(System.UInt32))
        return this.ToUInt32();
      if (tt == typeof(System.UInt64))
        return this.ToUInt64();
      if (tt == typeof(System.Single))
        return this.ToSingle();
      if (tt == typeof(System.Double))
        return this.ToDouble();
      if (tt == typeof(System.Decimal))
        return this.ToDecimal();
      if (tt == typeof(System.DateTime))
        return this.ToDateTime();
      if (tt == typeof(System.TimeSpan))
        return this.ToTimeSpan();
      if (tt == typeof(System.DateTimeOffset))
        return this.ToDateTimeOffset();
      return ToType(type, true);
    }
    /// <summary>
    /// Returns true if the <see cref="Value"/> property implements <see cref="IConvertible"/>
    /// </summary>
    public bool ImplementsIConvertible {
      get {
        return Type.IsSubclassOf(typeof(IConvertible));
      }
    }
    /// <summary>
    /// Returns true if the <see cref="Value"/> property
    /// is a numeric intrinsic value.
    /// </summary>
    public bool IsNumeric {
      get {
        Type tt = this.Type;
        return (tt == typeof(byte) ||
          tt == typeof(sbyte) ||
          tt == typeof(short) ||
          tt == typeof(int) ||
          tt == typeof(long) ||
          tt == typeof(ushort) ||
          tt == typeof(uint) ||
          tt == typeof(ulong) ||
          tt == typeof(float) ||
          tt == typeof(double) ||
          tt == typeof(decimal));
      }
    }
    private static char decimalSep = ((float)1.1).ToString().ToCharArray()[1];
    /// <summary>
    /// Returns true if the <see cref="Value"/> property
    /// is a numeric intrinsic value or else can be parsed into
    /// a numeric intrinsic value.
    /// </summary>
    public bool IsNumberable {
      get {
        if (IsNumeric) return true;
        Type tt = this.Type;
        if (tt == typeof(bool)) return true;
        if (tt == typeof(string) ||
          tt == typeof(char)) {
          try {
            foreach (char c in this.ToString().ToCharArray()) {
              if (!char.IsDigit(c) && c != decimalSep) return false;
            }
            //double d = (this.ToDouble() + (double)0.1);
            return true;
          } catch {
            return false;
          }
        }
        return false;
      }
    }
    /// <summary>
    /// Returns true if the value is a date or can be parsed into a date.
    /// </summary>
    public bool IsDate {
      get {
        Type tt = this.Type;
        if (tt == typeof(DateTime)) return true;
        if (tt == typeof(string)) {
          DateTime dt = new DateTime();
          if (DateTime.TryParse(this.ToString(), out dt))
            return true;
        }
        return false;
      }
    }
    /// <summary>
    /// Returns true if the value is a TimeSpan.
    /// </summary>
    public bool IsTimeSpan {
      get {
        return this.Type == typeof(TimeSpan);
      }
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Boolean"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance to
    /// an equivalent <see cref="System.Boolean"/> value.
    /// </summary>
    public bool ToBoolean() {
      if (_value == null) return false;
      if (this.Type == typeof(System.Boolean))
        return (bool)_value;
      return CBln(_value);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Boolean"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance to
    /// an equivalent <see cref="System.Boolean"/> value using the specified culture-specific
    /// formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    public bool ToBoolean(IFormatProvider formatProvider) {
      return (bool)ToType(typeof(bool), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Byte"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 8-bit unsigned integer.
    /// </summary>
    public byte ToByte() {
      if (_value == null) return 0;
      Type tt = this.Type;
      if (tt == typeof(System.Byte))
        return (byte)_value;
      if (tt == typeof(System.String) || tt == typeof(System.Char))
        return byte.Parse(_value.ToString());
      if (tt == typeof(System.Boolean))
        if ((bool)_value)
          return (Byte)1;
        else
          return (Byte)0;
      if (tt == typeof(System.DateTime))
        return (byte)((DateTime)_value).Ticks;
      if (tt == typeof(System.TimeSpan))
        return (byte)((TimeSpan)_value).Ticks;
      return (byte)ToType(typeof(byte), true);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Byte"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 8-bit unsigned integer using the specified
    /// culture-specific formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    public byte ToByte(IFormatProvider formatProvider) {
      return (byte)ToType(typeof(byte), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Int16"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 16-bit signed integer.
    /// </summary>
    public short ToInt16() {
      if (_value == null) return 0;
      Type tt = this.Type;
      if (tt == typeof(System.Int16))
        return (short)_value;
      if (tt == typeof(System.String) || tt == typeof(System.Char))
        return short.Parse(this._value.ToString());
      if (tt == typeof(System.Boolean))
        if ((bool)_value)
          return (short)1;
        else
          return (short)0;
      if (tt == typeof(System.DateTime))
        return (short)((DateTime)_value).Ticks;
      if (tt == typeof(System.TimeSpan))
        return (short)((TimeSpan)_value).Ticks;
      return (short)ToType(typeof(short), true);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Boolean"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance to
    /// an equivalent 16-bit signed integer using the specified
    /// culture-specific formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    public short ToInt16(IFormatProvider formatProvider) {
      return (short)ToType(typeof(short), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Int32"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 32-bit signed integer.
    /// </summary>
    public int ToInt32() {
      if (_value == null) return 0;
      Type tt = this.Type;
      if (tt == typeof(System.Int32))
        return (int)_value;
      if (tt == typeof(System.String) || tt == typeof(System.Char))
        return int.Parse(_value.ToString());
      if (tt == typeof(System.Boolean))
        if ((bool)_value)
          return (int)1;
        else
          return (int)0;
      if (tt == typeof(System.DateTime))
        return (int)((DateTime)_value).Ticks;
      if (tt == typeof(System.TimeSpan))
        return (int)((TimeSpan)_value).Ticks;
      return (int)ToType(typeof(int), true);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Int32"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 32-bit signed integer using the specified
    /// culture-specific formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    public int ToInt32(IFormatProvider formatProvider) {
      return (int)ToType(typeof(int), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Int64"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 64-bit signed integer.
    /// </summary>
    public long ToInt64() {
      if (_value == null) return 0;
      Type tt = this.Type;
      if (tt == typeof(System.Int64))
        return (long)_value;
      if (tt == typeof(System.String) || tt == typeof(System.Char))
        return long.Parse(_value.ToString());
      if (tt == typeof(System.Boolean))
        if ((bool)_value)
          return (long)1;
        else
          return (long)0;
      if (tt == typeof(System.DateTime))
        return (long)((DateTime)_value).Ticks;
      if (tt == typeof(System.TimeSpan))
        return (long)((TimeSpan)_value).Ticks;
      return (long)ToType(typeof(long), true);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Int64"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 64-bit signed integer using the specified
    /// culture-specific formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    public long ToInt64(IFormatProvider formatProvider) {
      return (long)ToType(typeof(long), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Double"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent double-precision floating-point number.
    /// </summary>
    public double ToDouble() {
      if (_value == null) return 0;
      Type tt = this.Type;
      if (tt == typeof(System.Double))
        return (double)_value;
      if (tt == typeof(System.String) || tt == typeof(System.Char))
        return double.Parse(
          _value.ToString());
      if (tt == typeof(System.Boolean))
        if ((bool)_value)
          return (double)1;
        else
          return (double)0;
      if (tt == typeof(System.DateTime))
        return (double)((DateTime)_value).Ticks;
      if (tt == typeof(System.TimeSpan))
        return (double)((TimeSpan)_value).Ticks;
      return (double)ToType(typeof(double), true);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Double"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent double-precision floating-point number using the
    /// specified culture-specific formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    public double ToDouble(IFormatProvider formatProvider) {
      return (double)ToType(typeof(double), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Single"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent single-precision floating-point number.
    /// </summary>
    public float ToSingle() {
      if (_value == null) return 0;
      Type tt = this.Type;
      if (tt == typeof(System.Single))
        return (float)_value;
      if (tt == typeof(System.String) || tt == typeof(System.Char))
        return float.Parse(_value.ToString());
      if (tt == typeof(System.Boolean))
        if ((bool)_value)
          return (float)1;
        else
          return (float)0;
      if (tt == typeof(System.DateTime))
        return (float)((DateTime)_value).Ticks;
      if (tt == typeof(System.TimeSpan))
        return (float)((TimeSpan)_value).Ticks;
      return (float)ToType(typeof(float), true);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Single"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent single-precision floating-point number using the
    /// specified culture-specific formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    public float ToSingle(IFormatProvider formatProvider) {
      return (float)ToType(typeof(float), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Decimal"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance to
    /// an equivalent Decimal number.
    /// </summary>
    public decimal ToDecimal() {
      if (_value == null) return 0;
      Type tt = this.Type;
      if (tt == typeof(System.Decimal))
        return (decimal)_value;
      if (tt == typeof(System.String) || tt == typeof(System.Char))
        return decimal.Parse(_value.ToString());
      if (tt == typeof(System.Boolean))
        if ((bool)_value)
          return (decimal)1;
        else
          return (decimal)0;
      if (tt == typeof(System.DateTime))
        return (decimal)((DateTime)_value).Ticks;
      if (tt == typeof(System.TimeSpan))
        return (decimal)((TimeSpan)_value).Ticks;
      return (decimal)ToType(typeof(decimal), true);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Decimal"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance to
    /// an equivalent Decimal number using the specified culture-specific
    /// formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    public decimal ToDecimal(IFormatProvider formatProvider) {
      return (decimal)ToType(typeof(decimal), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.SByte"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 8-bit signed integer.
    /// </summary>
    [CLSCompliant(false)]
    public sbyte ToSByte()
    {
      if (_value == null) return 0;
      Type tt = this.Type;
      if (tt == typeof(System.SByte))
        return (sbyte)_value;
      if (tt == typeof(System.String) || tt == typeof(System.Char))
        return sbyte.Parse(_value.ToString());
      if (tt == typeof(System.Boolean))
        if ((bool)_value)
          return (SByte)1;
        else
          return (SByte)0;
      if (tt == typeof(System.DateTime))
        return (sbyte)((DateTime)_value).Ticks;
      if (tt == typeof(System.TimeSpan))
        return (sbyte)((TimeSpan)_value).Ticks;
      return (sbyte)ToType(typeof(SByte), true);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.SByte"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 8-bit signed integer using the specified
    /// culture-specific formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    [CLSCompliant(false)]
    public sbyte ToSByte(IFormatProvider formatProvider)
    {
      return (sbyte)ToType(typeof(sbyte), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.UInt16"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 16-bit unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    public ushort ToUInt16()
    {
      if (_value == null) return 0;
      Type tt = this.Type;
      if (tt == typeof(System.UInt16))
        return (ushort)_value;
      if (tt == typeof(System.String) || tt == typeof(System.Char))
        return ushort.Parse(_value.ToString());
      if (tt == typeof(System.Boolean))
        if ((bool)_value)
          return (ushort)1;
        else
          return (ushort)0;
      if (tt == typeof(System.DateTime))
        return (ushort)((DateTime)_value).Ticks;
      if (tt == typeof(System.TimeSpan))
        return (ushort)((TimeSpan)_value).Ticks;
      return (ushort)ToType(typeof(ushort), true);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.UInt16"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 16-bit unsigned integer using the specified
    /// culture-specific formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    [CLSCompliant(false)]
    public ushort ToUInt16(IFormatProvider formatProvider)
    {
      return (ushort)ToType(typeof(ushort), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.UInt32"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 32-bit unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    public uint ToUInt32()
    {
      if (_value == null) return 0;
      Type tt = this.Type;
      if (tt == typeof(System.UInt32))
        return (uint)_value;
      if (tt == typeof(System.String) || tt == typeof(System.Char))
        return uint.Parse(_value.ToString());
      if (tt == typeof(System.Boolean))
        if ((bool)_value)
          return (uint)1;
        else
          return (uint)0;
      if (tt == typeof(System.DateTime))
        return (uint)((DateTime)_value).Ticks;
      if (tt == typeof(System.TimeSpan))
        return (uint)((TimeSpan)_value).Ticks;
      return (uint)ToType(typeof(uint), true);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.UInt32"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 32-bit unsigned integer using the specified
    /// culture-specific formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    [CLSCompliant(false)]
    public uint ToUInt32(IFormatProvider formatProvider)
    {
      return (uint)ToType(typeof(uint), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.UInt64"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 64-bit unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    public ulong ToUInt64()
    {
      if (_value == null) return 0;
      Type tt = this.Type;
      if (tt == typeof(System.UInt64))
        return (ulong)_value;
      if (tt == typeof(System.String) || tt == typeof(System.Char))
        return ulong.Parse(_value.ToString());
      if (tt == typeof(System.Boolean))
        if ((bool)_value)
          return (ulong)1;
        else
          return (ulong)0;
      if (tt == typeof(System.DateTime))
        return (ulong)((DateTime)_value).Ticks;
      if (tt == typeof(System.TimeSpan))
        return (ulong)((TimeSpan)_value).Ticks;
      return (ulong)ToType(typeof(ulong), true);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.UInt64"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent 64-bit unsigned integer using the specified
    /// culture-specific formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    [CLSCompliant(false)]
    public ulong ToUInt64(IFormatProvider formatProvider)
    {
      return (ulong)ToType(typeof(ulong), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.DateTime"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance to
    /// an equivalent DateTime.
    /// </summary>
    public DateTime ToDateTime() {
      if (_value == null) return DateTime.MinValue;
      Type tt = this.Type;
      if (tt == typeof(System.DateTime))
        return (DateTime)_value;
      if (tt == typeof(System.TimeSpan))
        return new DateTime(((TimeSpan)_value).Ticks);
      if (tt == typeof(System.String) || tt == typeof(System.Char))
        if (this.IsDate)
          return DateTime.Parse(_value.ToString());
//      if (tt == typeof(System.Boolean))
//        throw new InvalidCastException();
      if (this.IsNumberable)
        return new DateTime(this.ToInt64());
      return (DateTime)ToType(typeof(DateTime), true);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.DateTime"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance to
    /// an equivalent DateTime using the specified culture-specific
    /// formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    public DateTime ToDateTime(IFormatProvider formatProvider) {
      return (DateTime)ToType(typeof(DateTime), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.TimeSpan"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance to
    /// an equivalent TimeSpan.
    /// </summary>
    public TimeSpan ToTimeSpan() {
      if (_value == null) return TimeSpan.MinValue;
      Type tt = this.Type;
      if (tt == typeof(System.TimeSpan))
        return (TimeSpan)_value;
      if (tt == typeof(System.DateTime))
        return new TimeSpan(((DateTime)_value).Ticks);
      if (tt == typeof(System.String) || tt == typeof(System.Char))
        if (this.IsDate)
          return new TimeSpan(DateTime.Parse(_value.ToString()).Ticks);
//      if (tt == typeof(System.Boolean))
//        throw new InvalidCastException();
      if (this.IsNumberable)
        return new TimeSpan(this.ToInt64());
      return (TimeSpan)ToType(typeof(TimeSpan), true);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.TimeSpan"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance to
    /// an equivalent TimeSpan using the specified culture-specific
    /// formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    public TimeSpan ToTimeSpan(IFormatProvider formatProvider) {
      return (TimeSpan)ToType(typeof(TimeSpan), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.DateTimeOffset"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance to
    /// an equivalent DateTimeOffset.
    /// </summary>
    public DateTimeOffset ToDateTimeOffset() {
      if (_value == null) return DateTimeOffset.MinValue;
      Type tt = this.Type;
      if (tt == typeof(DateTimeOffset))
        return (DateTimeOffset)_value;
      DateTimeOffset result = new DateTimeOffset();
      if (DateTimeOffset.TryParse(_value.ToString(), out result))
        return result;
      return (DateTimeOffset)ToType(typeof(DateTimeOffset), true);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.DateTimeOffset"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance to
    /// an equivalent DateTimeOffset using the specified culture-specific
    /// formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    public DateTimeOffset ToDateTimeOffset(IFormatProvider formatProvider) {
      return (DateTimeOffset)ToType(typeof(DateTimeOffset), false, formatProvider);
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Char"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent Unicode character.
    /// </summary>
    public char ToChar() {
      if (_value == null) return (char)0;
      Type tt = this.Type;
      if (tt == typeof(System.Char))
        return (char)_value;
      if (tt == typeof(float) ||
        tt == typeof(double) ||
        tt == typeof(decimal)) {
        return (char)this.ToInt32();
      }
      if (tt == typeof(System.String))
        return char.Parse((string)_value);
      if (tt == typeof(System.Boolean))
        if ((bool)_value)
          return '1';
        else
          return '0';
      try {
        return (char)ToType(typeof(char), true);
      } catch {
        try {
          return char.Parse(_value.ToString());
        } catch {
          try {
            return _value.ToString().ToCharArray()[0];
          } catch {}
        }
      }
      throw new InvalidCastException();
    }
    /// <summary>
    /// If <see cref="Value" /> is a <see cref="System.Char"/>, returns
    /// as-is. Otherwise, attempts to convert the value of this instance
    /// to an equivalent Unicode character using the specified
    /// culture-specific formatting information.
    /// </summary>
    /// <param name="formatProvider">The culture-specific formatting information.</param>
    public char ToChar(IFormatProvider formatProvider) {
      return (Char)ToType(typeof(Char), false, formatProvider);
    }
    private enum MathAction {
      Add,
      Subtract,
      Multiply,
      Divide,
      Modulus,
      BitAnd,
      BitOr
    }
    private static object VariantArithmetic(MathAction action, object target, object value) {
      object ret = null;
      Variant sv = new Variant(target);
      Variant av = new Variant(value);
      if (sv.IsNumberable && av.IsNumberable) {
        Double sd = sv.ToDouble();
        Double ad = av.ToDouble();
        long lsd = (long)sd;
        long lad = (long)ad;
        System.Type dt = sv.Type;
        switch (action) {
          case MathAction.Add:
            sd += ad;
            break;
          case MathAction.Subtract:
            sd -= ad;
            break;
          case MathAction.Multiply:
            sd *= ad;
            break;
          case MathAction.Divide:
            sd /= ad;
            break;
          case MathAction.Modulus:
            sd %= ad;
            break;
          case MathAction.BitAnd:
            lsd &= lad;
            sd = (double)lsd;
            break;
          case MathAction.BitOr:
            lsd |= lad;
            sd = (double)lsd;
            break;
        }
        if (sv.IsNumeric) {
          bool isfloat = (sd != Math.Round(sd, 0));
          bool expandminmax = false;
          bool expandfloat = false;
          bool signed = (
            dt == typeof(sbyte) ||
            dt == typeof(short) ||
            dt == typeof(int) ||
            dt == typeof(long) ||
            dt == typeof(float) ||
            dt == typeof(double) ||
            dt == typeof(decimal));
          if ((dt == typeof(byte) && (sd < byte.MinValue || sd > byte.MaxValue)) ||
            (dt == typeof(sbyte) && (sd < sbyte.MinValue || sd > sbyte.MaxValue)) ||
            (dt == typeof(short) && (sd < short.MinValue || sd > short.MaxValue)) ||
            (dt == typeof(ushort) && (sd < ushort.MinValue || sd > ushort.MaxValue)) ||
            (dt == typeof(int) && (sd < int.MinValue || sd > int.MaxValue)) ||
            (dt == typeof(uint) && (sd < uint.MinValue || sd > uint.MaxValue)) ||
            (dt == typeof(long) && (sd < long.MinValue || sd > long.MaxValue)) ||
            (dt == typeof(ulong) && (sd < ulong.MinValue || sd > ulong.MaxValue)) ||
            (dt == typeof(float) && (sd < float.MinValue || sd > float.MaxValue)) ||
            (dt == typeof(decimal) && (sd < (double)decimal.MinValue || sd > (double)decimal.MaxValue)))
            expandminmax = true;

          if (isfloat && (
            dt == typeof(byte) ||
            dt == typeof(sbyte) ||
            dt == typeof(short) ||
            dt == typeof(ushort) ||
            dt == typeof(int) ||
            dt == typeof(uint) ||
            dt == typeof(long) ||
            dt == typeof(ulong)))
            expandfloat = true;
          if (expandfloat) {
            /*if (sd < (double)decimal.MinValue || sd > (double)decimal.MaxValue) {
              ret = sd;
            } else if ((float)sd < (float)float.MinValue || sd > (float)float.MaxValue) {
              ret = (decimal)sd;
            } else {
              ret = (float)sd;
            }*/
            // do we really need all that stuff above?
            // converting to decimal from double makes no sense
            // and converting to float from double causing losing precision
            // so we just assign value
            ret = sd;
          } else if (expandminmax) {
            if (dt == typeof(byte) ||
              dt == typeof(sbyte) ||
              dt == typeof(short) ||
              dt == typeof(ushort) ||
              dt == typeof(int) ||
              dt == typeof(uint) ||
              dt == typeof(long) ||
              dt == typeof(ulong)) {
              if (sd < 0 || signed) {
                long lmin = long.MinValue;
                long lmax = long.MaxValue;
                if (dt == typeof(sbyte)) {
                  lmin = sbyte.MinValue;
                  lmax = sbyte.MaxValue;
                }
                if (dt == typeof(short)) {
                  lmin = short.MinValue;
                  lmax = short.MaxValue;
                }
                if (dt == typeof(int)) {
                  lmin = int.MinValue;
                  lmax = int.MaxValue;
                }
                if (sd < long.MinValue || sd > long.MaxValue ||
                  lmin < long.MinValue || lmax > long.MaxValue) {
                  ret = sd;
                } else if (sd < int.MinValue || sd > int.MaxValue ||
                  lmin < int.MinValue || lmax > int.MaxValue) {
                  ret = (long)sd;
                } else if (sd < short.MinValue || sd > short.MaxValue ||
                  lmin < short.MinValue || lmax > short.MaxValue) {
                  ret = (int)sd;
                } else if (sd < sbyte.MinValue || sd > sbyte.MaxValue ||
                  lmin < sbyte.MinValue || lmax > sbyte.MaxValue) {
                  ret = (short)sd;
                }
              } else {
                ulong lmax = ulong.MaxValue;
                if (dt == typeof(byte)) lmax = byte.MaxValue;
                if (dt == typeof(ushort)) lmax = ushort.MaxValue;
                if (dt == typeof(uint)) lmax = uint.MaxValue;
                if (sd < ulong.MinValue || sd > ulong.MaxValue ||
                  lmax > ulong.MaxValue) {
                  ret = sd;
                } else if (sd < uint.MinValue || sd > uint.MaxValue ||
                  lmax > uint.MaxValue) {
                  ret = (ulong)sd;
                } else if (sd < ushort.MinValue || sd > ushort.MaxValue ||
                  lmax > ushort.MaxValue) {
                  ret = (uint)sd;
                } else if (sd < byte.MinValue || sd > byte.MaxValue ||
                  lmax > byte.MaxValue) {
                  ret = (ushort)sd;
                }
              }
            } else {
              ret = sd;
            }
          } else { // Not expandfloat and not expandminmax,
             // so revert to original type!
            ret = System.Convert.ChangeType(sd, sv.Type);
          }
        } else { // not numeric
          Variant v = new Variant(sd);
          ret = v.ToType(sv.Type);
        }
      }
      return ret;
    }
    /// <summary>
    /// Addition operator.
    /// </summary>
    /// <remarks>
    /// If the value on the right is a <see cref="System.String"/>
    /// or a <see cref="System.Char"/>,
    /// the Variant is converted to a string and appended.
    /// If the value on the right or the Variant
    /// is a <see cref="System.DateTime"/>, arithmetic
    /// is performed on the <see cref="DateTime.Ticks"/> property and the
    /// resulting value is set to the DateTime type.
    /// Otherwise, if the value on the right is a number, both
    /// the Variant and the value on the right are
    /// converted to a <see cref="System.Double"/>, the arithmetic
    /// is performed, and the resulting value is converted back to the
    /// original type that the Variant previously represented.
    /// If the type that the Variant previously represented
    /// cannot contain the resulting value--such as if the type is a
    /// <see cref="System.UInt32"/> and the value is <c>-12</c>--then the
    /// type will be converted to a type that can contain
    /// the value, such as <see cref="System.Int32"/>.
    /// </remarks>
    /// <param name="subjectVariant"></param>
    /// <param name="value"></param>
    /// <returns>A new <see cref="Variant"/> containing the resulting value.</returns>
    public static Variant operator +(Variant subjectVariant, object value) {
      if (value.GetType() == typeof(Variant))
        value = ((Variant)value).Value;
      Variant sv = subjectVariant; // smaller var name :)
      Variant dv = new Variant(value);
      Type tt = value.GetType();
      if (tt == typeof(bool) && sv.IsNumeric) {
        if ((bool)value) value = 1;
        else value = 0;
        tt = value.GetType();
      }
      if (((sv.Type == typeof(string) || sv.Type == typeof(char)) &&
        !sv.IsNumberable) ||
        (tt == typeof(System.String) || tt == typeof(System.Char)))
        return new Variant(sv.ToString() + value.ToString());
      if ((sv.IsDate && dv.IsDate) ||
        (sv.IsDate && dv.IsTimeSpan) || (sv.IsTimeSpan && dv.IsDate) ||
        (sv.IsDate && dv.IsNumberable)) {
        return new Variant(new DateTime(sv.ToTimeSpan().Ticks + dv.ToTimeSpan().Ticks));
      }
      if (sv.Type == typeof(TimeSpan) && tt == typeof(TimeSpan))
        return new Variant(sv.ToTimeSpan() + dv.ToTimeSpan());
      if (tt == typeof(System.Boolean)) { // change to boolean and toggle if true
        Variant ret = new Variant(sv.ToBoolean());
        if ((bool)value) {
          ret = new Variant(!ret.ToBoolean());
        }
        return ret;
      }
      object retobj = VariantArithmetic(MathAction.Add, sv, value);
      if (retobj != null) return new Variant(retobj);
      throw new InvalidOperationException(
        "Cannot implicitly add value to unidentified type.");
    }
    /// <summary>
    /// Subtraction operator.
    /// </summary>
    /// <remarks>
    /// If the value on the right or the Variant
    /// is a <see cref="System.DateTime"/>, arithmetic
    /// is performed on the <see cref="DateTime.Ticks"/> property and the
    /// resulting value is set to the DateTime type.
    /// Otherwise, if the value on the right is a number, both
    /// the Variant and the value on the right are
    /// converted to a <see cref="System.Double"/>, the arithmetic
    /// is performed, and the resulting value is converted back to the
    /// original type that the Variant previously represented.
    /// If the type that the Variant previously represented
    /// cannot contain the resulting value--such as if the type is a
    /// <see cref="System.UInt32"/> and the value is <c>-12</c>--then the
    /// type will be converted to a type that can contain
    /// the value, such as <see cref="System.Int32"/>.
    /// </remarks>
    /// <param name="subjectVariant"></param>
    /// <param name="value"></param>
    /// <returns>A new <see cref="Variant"/> containing the resulting value.</returns>
    public static Variant operator -(Variant subjectVariant, object value) {
      if (value.GetType() == typeof(Variant))
        value = ((Variant)value).Value;
      Variant sv = subjectVariant; // smaller var name :)
      Variant dv = new Variant(value);
      Type tt = value.GetType();
      if (tt == typeof(bool) && sv.IsNumeric) {
        if ((bool)value) value = 1;
        else value = 0;
        tt = value.GetType();
      }
      if ((sv.IsDate && dv.IsDate) ||
        (sv.IsDate && dv.IsTimeSpan) || (sv.IsTimeSpan && dv.IsDate) ||
        (sv.IsDate && dv.IsNumberable)) {
        return new Variant(new DateTime(sv.ToTimeSpan().Ticks - dv.ToTimeSpan().Ticks));
      }
      if (sv.Type == typeof(TimeSpan) && tt == typeof(TimeSpan))
        return new Variant(sv.ToTimeSpan() - dv.ToTimeSpan());
      if (tt == typeof(System.String))
        throw new InvalidOperationException("Objects of type System.ToString() "
          + "cannot subtract.");
      if (tt == typeof(System.Char))
        throw new InvalidOperationException("Objects of type System.ToChar() "
          + "cannot subtract.");
      if (tt == typeof(System.Boolean)) { // change to boolean and toggle if false
        Variant ret = new Variant(sv.ToBoolean());
        if (!(bool)value) {
          ret = new Variant(!ret.ToBoolean());
        }
        return ret;
      }
      object retobj = VariantArithmetic(MathAction.Subtract, sv, value);
      if (retobj != null) return new Variant(retobj);
      throw new InvalidOperationException(
        "Cannot implicitly add value to unidentified type.");
    }

    // AlexTZ
    /// <summary>
    /// Unary minus operator.
    /// </summary>
    public static Variant operator -(Variant subjectVariant)
    {
      return 0 - subjectVariant;
    }

    /// <summary>
    /// Greater than operator.
    /// </summary>
    public static bool operator >(Variant subjectVariant, object value)
    {
      if (value.GetType() == typeof(Variant))
        value = ((Variant)value).Value;
      Variant sv = subjectVariant;
      Variant dv = new Variant(value);
      if (sv.IsNumeric && dv.IsNumeric)
        return sv.ToDouble() > dv.ToDouble();
      if (sv.IsDate && dv.IsDate)
        return sv.ToDateTime() > dv.ToDateTime();
      if (sv.IsTimeSpan && dv.IsTimeSpan)
        return sv.ToTimeSpan() > dv.ToTimeSpan();
      return false;
    }

    /// <summary>
    /// Greater than or equal operator.
    /// </summary>
    public static bool operator >=(Variant subjectVariant, object value)
    {
      return subjectVariant > value || subjectVariant == value;
    }

    /// <summary>
    /// Less than operator.
    /// </summary>
    public static bool operator <(Variant subjectVariant, object value)
    {
      return !(subjectVariant > value || subjectVariant == value);
    }

    /// <summary>
    /// Less than or equal operator.
    /// </summary>
    public static bool operator <=(Variant subjectVariant, object value)
    {
      return !(subjectVariant > value);
    }
    /// <summary>
    /// Multiplication operator.
    /// </summary>
    /// <remarks>
    /// If the value on the right or the Variant
    /// is a <see cref="System.DateTime"/>, arithmetic
    /// is performed on the <see cref="DateTime.Ticks"/> property and the
    /// resulting value is set to the DateTime type.
    /// Otherwise, if the value on the right is a number, both
    /// the Variant and the value on the right are
    /// converted to a <see cref="System.Double"/>, the arithmetic
    /// is performed, and the resulting value is converted back to the
    /// original type that the Variant previously represented.
    /// If the type that the Variant previously represented
    /// cannot contain the resulting value--such as if the type is a
    /// <see cref="System.UInt32"/> and the value is <c>-12</c>--then the
    /// type will be converted to a type that can contain
    /// the value, such as <see cref="System.Int32"/>.
    /// </remarks>
    /// <param name="subjectVariant"></param>
    /// <param name="value"></param>
    /// <returns>A new <see cref="Variant"/> containing the resulting value.</returns>
    public static Variant operator *(Variant subjectVariant, object value) {
      if (value.GetType() == typeof(Variant))
        value = ((Variant)value).Value;
      Variant sv = subjectVariant; // smaller var name :)
      Variant dv = new Variant(value);
      Type tt = value.GetType();
      if (tt == typeof(bool) && sv.IsNumeric) {
        if ((bool)value) value = 1;
        else value = 0;
        tt = value.GetType();
      }
      if ((sv.IsDate && dv.IsDate) ||
        (sv.IsDate && dv.IsTimeSpan) || (sv.IsTimeSpan && dv.IsDate) ||
        (sv.IsDate && dv.IsNumberable)) {
        return new Variant(new DateTime(sv.ToTimeSpan().Ticks * dv.ToTimeSpan().Ticks));
      }
      if (sv.Type == typeof(TimeSpan) && tt == typeof(TimeSpan))
        return new Variant(new TimeSpan(sv.ToTimeSpan().Ticks * dv.ToTimeSpan().Ticks));
      if (tt == typeof(System.String))
        throw new InvalidOperationException("Objects of type System.ToString() "
          + "cannot multiply.");
      if (tt == typeof(System.Char))
        throw new InvalidOperationException("Objects of type System.ToChar() "
          + "cannot multiply.");
      if (tt == typeof(System.Boolean)) { // change to boolean and multiply by 1 (true) or 0 (false)
        Variant ret = new Variant(sv.ToBoolean());
        if (!(bool)value) {
          ret = new Variant(!ret.ToBoolean());
        }
        return ret;
      }
      object retobj = VariantArithmetic(MathAction.Multiply, sv, value);
      if (retobj != null) return new Variant(retobj);
      throw new InvalidOperationException(
        "Cannot implicitly add value to unidentified type.");
    }
    /// <summary>
    /// Division operator.
    /// </summary>
    /// <remarks>
    /// If the value on the right or the Variant
    /// is a <see cref="System.DateTime"/>, arithmetic
    /// is performed on the <see cref="DateTime.Ticks"/> property and the
    /// resulting value is set to the DateTime type.
    /// Otherwise, if the value on the right is a number, both
    /// the Variant and the value on the right are
    /// converted to a <see cref="System.Double"/>, the arithmetic
    /// is performed, and the resulting value is converted back to the
    /// original type that the Variant previously represented.
    /// If the type that the Variant previously represented
    /// cannot contain the resulting value--such as if the type is a
    /// <see cref="System.UInt32"/> and the value is <c>-12</c>--then the
    /// type will be converted to a type that can contain
    /// the value, such as <see cref="System.Int32"/>.
    /// </remarks>
    /// <param name="subjectVariant"></param>
    /// <param name="value"></param>
    /// <returns>A new <see cref="Variant"/> containing the resulting value.</returns>
    public static Variant operator /(Variant subjectVariant, object value) {
      if (value.GetType() == typeof(Variant))
        value = ((Variant)value).Value;
      Variant sv = subjectVariant; // smaller var name :)
      Variant dv = new Variant(value);
      Type tt = value.GetType();
      if (tt == typeof(bool) && sv.IsNumeric) {
        if ((bool)value) value = 1;
        else value = 0;
        tt = value.GetType();
      }
      if ((sv.IsDate && dv.IsDate) ||
        (sv.IsDate && dv.IsTimeSpan) || (sv.IsTimeSpan && dv.IsDate) ||
        (sv.IsDate && dv.IsNumberable)) {
        return new Variant(new DateTime(sv.ToTimeSpan().Ticks / dv.ToTimeSpan().Ticks));
      }
      if (sv.Type == typeof(TimeSpan) && tt == typeof(TimeSpan))
        return new Variant(new TimeSpan(sv.ToTimeSpan().Ticks / dv.ToTimeSpan().Ticks));
      if (tt == typeof(System.String))
        throw new InvalidOperationException("Objects of type System.ToString() "
          + "cannot divide.");
      if (tt == typeof(System.Char))
        throw new InvalidOperationException("Objects of type System.ToChar() "
          + "cannot divide.");
      if (tt == typeof(System.Boolean)) {
        throw new InvalidOperationException("Objects of type System.ToChar() "
          + "cannot apply division.");
      }

      object retobj = VariantArithmetic(MathAction.Divide, sv, value);
      if (retobj != null) return new Variant(retobj);
      throw new InvalidOperationException(
        "Cannot implicitly add value to unidentified type.");
    }
    /// <summary>
    /// Modulus operator.
    /// </summary>
    /// <remarks>
    /// If the value on the right or the Variant
    /// is a <see cref="System.DateTime"/>, arithmetic
    /// is performed on the <see cref="DateTime.Ticks"/> property and the
    /// resulting value is set to the DateTime type.
    /// Otherwise, if the value on the right is a number, both
    /// the Variant and the value on the right are
    /// converted to a <see cref="System.Double"/>, the arithmetic
    /// is performed, and the resulting value is converted back to the
    /// original type that the Variant previously represented.
    /// If the type that the Variant previously represented
    /// cannot contain the resulting value--such as if the type is a
    /// <see cref="System.UInt32"/> and the value is <c>-12</c>--then the
    /// type will be converted to a type that can contain
    /// the value, such as <see cref="System.Int32"/>.
    /// </remarks>
    /// <param name="subjectVariant"></param>
    /// <param name="value"></param>
    /// <returns>A new <see cref="Variant"/> containing the resulting value.</returns>
    public static Variant operator %(Variant subjectVariant, object value) {
      if (value.GetType() == typeof(Variant))
        value = ((Variant)value).Value;
      Variant sv = subjectVariant; // smaller var name :)
      Variant dv = new Variant(value);
      Type tt = value.GetType();
      if (tt == typeof(bool) && sv.IsNumeric) {
        if ((bool)value) value = 1;
        else value = 0;
        tt = value.GetType();
      }
      if ((sv.IsDate && dv.IsDate) ||
        (sv.IsDate && dv.IsTimeSpan) || (sv.IsTimeSpan && dv.IsDate) ||
        (sv.IsDate && dv.IsNumberable)) {
        return new Variant(new DateTime(sv.ToTimeSpan().Ticks / dv.ToTimeSpan().Ticks));
      }
      if (sv.Type == typeof(TimeSpan) && tt == typeof(TimeSpan))
        return new Variant(new TimeSpan(sv.ToTimeSpan().Ticks % dv.ToTimeSpan().Ticks));
      if (tt == typeof(System.String))
        throw new InvalidOperationException("Objects of type System.ToString() "
          + "cannot apply modulus.");
      if (tt == typeof(System.Char))
        throw new InvalidOperationException("Objects of type System.ToChar() "
          + "cannot apply modulus.");
      if (tt == typeof(System.Boolean))
        throw new InvalidOperationException("Objects of type System.Boolean "
          + "cannot apply modulus.");
      object retobj = VariantArithmetic(MathAction.Modulus, sv, value);
      if (retobj != null) return new Variant(retobj);
      throw new InvalidOperationException(
        "Cannot implicitly add value to unidentified type.");
    }
    /// <summary>
    /// Bitwise And operator.
    /// </summary>
    /// <remarks>
    /// If the value on the right or the Variant
    /// is a <see cref="System.DateTime"/>, arithmetic
    /// is performed on the <see cref="DateTime.Ticks"/> property and the
    /// resulting value is set to the DateTime type.
    /// Otherwise, if the value on the right is a number, both
    /// the Variant and the value on the right are
    /// converted to a <see cref="System.Double"/>, the arithmetic
    /// is performed, and the resulting value is converted back to the
    /// original type that the Variant previously represented.
    /// If the type that the Variant previously represented
    /// cannot contain the resulting value--such as if the type is a
    /// <see cref="System.UInt32"/> and the value is <c>-12</c>--then the
    /// type will be converted to a type that can contain
    /// the value, such as <see cref="System.Int32"/>.
    /// </remarks>
    /// <param name="subjectVariant"></param>
    /// <param name="value"></param>
    /// <returns>A new <see cref="Variant"/> containing the resulting value.</returns>
    public static Variant operator &(Variant subjectVariant, object value) {
      if (value.GetType() == typeof(Variant))
        value = ((Variant)value).Value;
      Variant sv = subjectVariant; // smaller var name :)
      Variant dv = new Variant(value);
      Type tt = value.GetType();
      if (tt == typeof(bool) && sv.IsNumeric) {
        if ((bool)value) value = 1;
        else value = 0;
        tt = value.GetType();
      }
      if (tt == typeof(System.Single))
        throw new InvalidOperationException("Operator '&' cannot "
          + "be applied to operands of type 'float' and 'float'.");
      if (tt == typeof(System.Double))
        throw new InvalidOperationException("Operator '&' cannot "
          + "be applied to operands of type 'double' and 'double'.");
      if (tt == typeof(System.Decimal))
        throw new InvalidOperationException("Operator '&' cannot "
          + "be applied to operands of type 'decimal' and 'decimal'.");
      if ((sv.IsDate && dv.IsDate) ||
        (sv.IsDate && dv.IsTimeSpan) || (sv.IsTimeSpan && dv.IsDate) ||
        (sv.IsDate && dv.IsNumberable)) {
        return new Variant(new DateTime(sv.ToTimeSpan().Ticks & dv.ToTimeSpan().Ticks));
      }
      if (sv.Type == typeof(TimeSpan) && tt == typeof(TimeSpan))
        return new Variant(new TimeSpan(sv.ToTimeSpan().Ticks & dv.ToTimeSpan().Ticks));
      if (tt == typeof(System.String))
        throw new InvalidOperationException("Objects of type System.ToString() "
          + "cannot apply '&' operator.");
      if (tt == typeof(System.Char))
        throw new InvalidOperationException("Objects of type System.ToChar() "
          + "cannot apply '&' operator.");
      if (tt == typeof(System.Boolean))
        return new Variant(sv.ToBoolean() & (bool)value);
      object retobj = VariantArithmetic(MathAction.BitAnd, sv, value);
      if (retobj != null) return new Variant(retobj);
      throw new InvalidOperationException(
        "Cannot implicitly add value to unidentified type.");
    }
    /// <summary>
    /// Bitwise Or operator.
    /// </summary>
    /// <remarks>
    /// If the value on the right or the Variant
    /// is a <see cref="System.DateTime"/>, arithmetic
    /// is performed on the <see cref="DateTime.Ticks"/> property and the
    /// resulting value is set to the DateTime type.
    /// Otherwise, if the value on the right is a number, both
    /// the Variant and the value on the right are
    /// converted to a <see cref="System.Double"/>, the arithmetic
    /// is performed, and the resulting value is converted back to the
    /// original type that the Variant previously represented.
    /// If the type that the Variant previously represented
    /// cannot contain the resulting value--such as if the type is a
    /// <see cref="System.UInt32"/> and the value is <c>-12</c>--then the
    /// type will be converted to a type that can contain
    /// the value, such as <see cref="System.Int32"/>.
    /// </remarks>
    /// <param name="subjectVariant"></param>
    /// <param name="value"></param>
    /// <returns>A new <see cref="Variant"/> containing the resulting value.</returns>
    public static Variant operator |(Variant subjectVariant, object value) {
      if (value.GetType() == typeof(Variant))
        value = ((Variant)value).Value;
      Variant sv = subjectVariant; // smaller var name :)
      Variant dv = new Variant(value);
      Type tt = value.GetType();
      if (tt == typeof(bool) && sv.IsNumeric) {
        if ((bool)value) value = 1;
        else value = 0;
        tt = value.GetType();
      }
      if (tt == typeof(System.Single))
        throw new InvalidOperationException("Operator '|' cannot "
          + "be applied to operands of type 'float' and 'float'.");
      if (tt == typeof(System.Double))
        throw new InvalidOperationException("Operator '|' cannot "
          + "be applied to operands of type 'double' and 'double'.");
      if (tt == typeof(System.Decimal))
        throw new InvalidOperationException("Operator '|' cannot "
          + "be applied to operands of type 'decimal' and 'decimal'.");
      if ((sv.IsDate && dv.IsDate) ||
        (sv.IsDate && dv.IsTimeSpan) || (sv.IsTimeSpan && dv.IsDate) ||
        (sv.IsDate && dv.IsNumberable)) {
        return new Variant(new DateTime(sv.ToTimeSpan().Ticks | dv.ToTimeSpan().Ticks));
      }
      if ((sv.Type == typeof(TimeSpan) && tt == typeof(TimeSpan)) ||
        (sv.IsNumberable && tt == typeof(TimeSpan)) ||
        (sv.Type == typeof(TimeSpan) && dv.IsNumberable))
        return new Variant(new TimeSpan(sv.ToTimeSpan().Ticks | dv.ToTimeSpan().Ticks));
      if (tt == typeof(System.String))
        throw new InvalidOperationException("Objects of type System.ToString() "
          + "cannot apply '|' operator.");
      if (tt == typeof(System.Char))
        throw new InvalidOperationException("Objects of type System.ToChar() "
          + "cannot apply '|' operator.");
      if (tt == typeof(System.Boolean))
        return new Variant(sv.ToBoolean() | (bool)value);
      object retobj = VariantArithmetic(MathAction.BitOr, sv, value);
      if (retobj != null) return new Variant(retobj);
      throw new InvalidOperationException(
        "Cannot implicitly add value to unidentified type.");
    }
    /// <summary>
    /// Inequality operator.
    /// </summary>
    /// <param name="subjectVariant"></param>
    /// <param name="value"></param>
    /// <returns>
    /// The opposite of ==
    /// </returns>
    public static bool operator !=(Variant subjectVariant, object value) {
      return !(subjectVariant == value);
    }

    /// <summary>
    /// <para>Equality operator.</para>
    /// <para>First attempts to compare the left value after
    /// temporarily converting it to the type of the right value.
    /// If the conversion cannot occur, such as if the value is not an
    /// intrinsic value type, the comparison occurs at the <see cref="System.Object"/>
    /// level using <b>Object.Equals</b>.</para>
    /// </summary>
    /// <param name="subjectVariant"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool operator ==(Variant subjectVariant, object value) {
      if (value.GetType() == typeof(Variant))
        value = ((Variant)value).Value;
      Variant sv = subjectVariant; // smaller var name :)
      Variant dv = new Variant(value);
      Variant dvv = dv;
      if (sv.IsNumberable && dv.IsNumberable) {
        sv = new Variant(sv.ToDouble());
        dvv = new Variant(dv.ToDouble());
      }
      //if (sv.Type != dvv.Type) return false;
      if (sv.IsDate && dv.IsDate)
        return sv.ToDateTime() == dv.ToDateTime();
      if (sv.IsTimeSpan && dv.IsTimeSpan)
        return sv.ToTimeSpan() == dvv.ToTimeSpan();
      Type tt = dvv.Type;
      if (tt == typeof(System.String) || tt == typeof(System.Char))
        if (sv.ToString() == dvv.ToString()) return true;
      if (dv.IsNumeric)
        return sv.ToDouble() == dv.ToDouble();
      if (dv.Type == typeof(System.Boolean))
        return sv.ToBoolean() == (bool)value;
      return sv.Value == dv.Value ;//Object.Equals(subjectVariant.Value, value);
    }

    /// <summary>
    /// <para>Equality operator.</para>
    /// </summary>
    /// <param name="subjectVariant"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool operator ==(Variant subjectVariant, string value)
    {
      return subjectVariant.ToString() == value;
    }

    /// <summary>
    /// <para>Equality operator.</para>
    /// </summary>
    /// <param name="subjectVariant"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool operator !=(Variant subjectVariant, string value)
    {
      return subjectVariant.ToString() != value;
    }

    /// <summary>
    /// Returns <see cref="String"/> property unless the value on the right
    /// is null. If the value on the right is null, returns "".
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        if (_value == null) return "";
        return this.String;
    }
    /// <summary>
    /// Converts the value of this instance to an equivalent <see cref="String"/>
    /// using the specified culture-specific formatting information.
    /// </summary>
    /// <param name="formatProvider"></param>
    /// <returns></returns>
    public string ToString(IFormatProvider formatProvider) {
      return (string)ToType(typeof(string), false, formatProvider);
    }
    /// <summary>
    /// See <see cref="Object.GetHashCode()"/>.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() {
      return base.GetHashCode ();
    }
    /// <summary>
    /// See <see cref="Object.Equals(object)"/>.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj) {
      return base.Equals (obj);
    }
    /// <summary>
    /// Converts an object to a boolean.
    /// For any type, if null, returns false.
    /// For Boolean: true/false.
    /// For String: "", "false", "0", etc. == false;
    ///   "1", "true", etc. == true, else true.
    /// For numeric intrinsics: 0 == false, else true.
    /// For any other non-null object, returns true.
    /// </summary>
    /// <param name="bln">The string to be converted</param>
    /// <returns>The boolean value of this string.</returns>
    public static bool CBln(object bln) {
      if (bln == null) return false;
      if (bln.GetType() == typeof(bool)) {
        return (bool)bln;
      } else if (bln.GetType() == typeof(string)) {
        string val = (string)bln;
        bool ret = true;
        val = val.ToLower().Trim();
        string sTrue = true.ToString().ToLower();
        string sFalse = false.ToString().ToLower();
        if (val == "" ||
          val == "false" ||
          val == "f" ||
          val == "0" ||
          val == "no" ||
          val == "n" ||
          val == "off" ||
          val == "negative" ||
          val == "neg" ||
          val == "disabled" ||
          val == "incorrect" ||
          val == "wrong" ||
          val == "left" ||
          val == sFalse) {
          ret = false;
          return ret;
        }
        if (val == "true" ||
          val == "t" ||
          val == "1" ||
          val == "-1" ||
          val == "yes" ||
          val == "y" ||
          val == "positive" ||
          val == "pos" ||
          val == "on" ||
          val == "enabled" ||
          val == "correct" ||
          val == "right" ||
          val == sTrue) {
          ret = true;
          return ret;
        }
        try {
          ret = bool.Parse(val);
        } catch {}
        return ret;
      } else if (bln.GetType() == typeof(byte) ||
        bln.GetType() == typeof(ushort) ||
        bln.GetType() == typeof(decimal) ||
        bln.GetType() == typeof(sbyte) ||
        bln.GetType() == typeof(ulong) ||
        bln.GetType() == typeof(int) ||
        bln.GetType() == typeof(uint) ||
        bln.GetType() == typeof(long) ||
        bln.GetType() == typeof(short) ||
        bln.GetType() == typeof(double) ||
        bln.GetType() == typeof(float)) {
        if (bln.ToString() != "0" && bln.ToString() != "0.0") {
          return true;
        } else {
          return false;
        }
      }
      return true;
    }

        public int CompareTo(object other)
        {
            if (this > other) return 1;
            else if (this == other) return 0;
            else return -1;
        }

// AlexTZ
    ///
    public static implicit operator string(Variant v) {
      return v.ToString();
    }
    ///
    public static implicit operator Variant(string v)
    {
      return new Variant(v);
    }

    ///
    public static implicit operator char(Variant v)
    {
      return v.ToChar();
    }
    ///
    public static implicit operator Variant(char v)
    {
      return new Variant(v);
    }

    ///
    public static implicit operator byte(Variant v)
    {
      return v.ToByte();
    }
    ///
    public static implicit operator Variant(byte v)
    {
      return new Variant(v);
    }

    ///
    [CLSCompliant(false)]
    public static implicit operator sbyte(Variant v)
    {
      return v.ToSByte();
    }
    ///
    [CLSCompliant(false)]
    public static implicit operator Variant(sbyte v)
    {
      return new Variant(v);
    }

    ///
    public static implicit operator short(Variant v)
    {
      return v.ToInt16();
    }
    ///
    public static implicit operator Variant(short v)
    {
      return new Variant(v);
    }

    ///
    [CLSCompliant(false)]
    public static implicit operator ushort(Variant v)
    {
      return v.ToUInt16();
    }
    ///
    [CLSCompliant(false)]
    public static implicit operator Variant(ushort v)
    {
      return new Variant(v);
    }

    ///
    public static implicit operator int(Variant v)
    {
      return v.ToInt32();
    }
    ///
    public static implicit operator Variant(int v)
    {
      return new Variant(v);
    }

    ///
    [CLSCompliant(false)]
    public static implicit operator uint(Variant v)
    {
      return v.ToUInt32();
    }
    ///
    [CLSCompliant(false)]
    public static implicit operator Variant(uint v)
    {
      return new Variant(v);
    }

    ///
    public static implicit operator long(Variant v)
    {
      return v.ToInt64();
    }
    ///
    public static implicit operator Variant(long v)
    {
      return new Variant(v);
    }

    ///
    [CLSCompliant(false)]
    public static implicit operator ulong(Variant v)
    {
      return v.ToUInt64();
    }
    ///
    [CLSCompliant(false)]
    public static implicit operator Variant(ulong v)
    {
      return new Variant(v);
    }

    ///
    public static implicit operator float(Variant v)
    {
      return v.ToSingle();
    }
    ///
    public static implicit operator Variant(float v)
    {
      return new Variant(v);
    }

    ///
    public static implicit operator double(Variant v)
    {
      return v.ToDouble();
    }
    ///
    public static implicit operator Variant(double v)
    {
      return new Variant(v);
    }

    ///
    public static implicit operator decimal(Variant v)
    {
      return v.ToDecimal();
    }
    ///
    public static implicit operator Variant(decimal v)
    {
      return new Variant(v);
    }

    ///
    public static implicit operator bool(Variant v)
    {
      return v.ToBoolean();
    }
    ///
    public static implicit operator Variant(bool v)
    {
      return new Variant(v);
    }

    ///
    public static implicit operator DateTime(Variant v)
    {
      return v.ToDateTime();
    }
    ///
    public static implicit operator Variant(DateTime v)
    {
      return new Variant(v);
    }
        ///
    public static implicit operator TimeSpan(Variant v)
    {
        return v.ToTimeSpan();
    }
    ///
    public static implicit operator Variant(TimeSpan v)
    {
        return new Variant(v);
    }
    }
}

