/*
* Copyright 2007 ZXing authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using System;
namespace FastReport.Barcode.QRCode
{
  
  /*/// <summary> <p>Represents a polynomial whose coefficients are elements of GF(256).
  /// Instances of this class are immutable.</p>
  /// 
  /// <p>Much credit is due to William Rucklidge since portions of this code are an indirect
  /// port of his C++ Reed-Solomon implementation.</p>
  /// 
  /// </summary>
  /// <author>  Sean Owen
  /// </author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
  /// </author>*/
  internal sealed class GF256Poly
  {
    internal int[] Coefficients
    {
      get
      {
        return coefficients;
      }
      
    }
    /*/// <returns> degree of this polynomial
    /// </returns>*/
    internal int Degree
    {
      get
      {
        return coefficients.Length - 1;
      }
      
    }
    /*/// <returns> true iff this polynomial is the monomial "0"
    /// </returns>*/
    internal bool Zero
    {
      get
      {
        return coefficients[0] == 0;
      }
      
    }
    
    //UPGRADE_NOTE: Final was removed from the declaration of 'field '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
    private GF256 field;
    //UPGRADE_NOTE: Final was removed from the declaration of 'coefficients '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
    private int[] coefficients;
    
    /*/// <param name="field">the {@link GF256} instance representing the field to use
    /// to perform computations
    /// </param>
    /// <param name="coefficients">coefficients as ints representing elements of GF(256), arranged
    /// from most significant (highest-power term) coefficient to least significant
    /// </param>
    /// <throws>  IllegalArgumentException if argument is null or empty, </throws>
    /// <summary> or if leading coefficient is 0 and this is not a
    /// constant polynomial (that is, it is not the monomial "0")
    /// </summary>*/
    internal GF256Poly(GF256 field, int[] coefficients)
    {
      if (coefficients == null || coefficients.Length == 0)
      {
        throw new System.ArgumentException();
      }
      this.field = field;
      int coefficientsLength = coefficients.Length;
      if (coefficientsLength > 1 && coefficients[0] == 0)
      {
        // Leading term must be non-zero for anything except the constant polynomial "0"
        int firstNonZero = 1;
        while (firstNonZero < coefficientsLength && coefficients[firstNonZero] == 0)
        {
          firstNonZero++;
        }
        if (firstNonZero == coefficientsLength)
        {
          this.coefficients = field.Zero.coefficients;
        }
        else
        {
          this.coefficients = new int[coefficientsLength - firstNonZero];
          Array.Copy(coefficients, firstNonZero, this.coefficients, 0, this.coefficients.Length);
        }
      }
      else
      {
        this.coefficients = coefficients;
      }
    }
    
    /*/// <returns> coefficient of x^degree term in this polynomial
    /// </returns>*/
    internal int getCoefficient(int degree)
    {
      return coefficients[coefficients.Length - 1 - degree];
    }
    
    internal GF256Poly addOrSubtract(GF256Poly other)
    {
      if (!field.Equals(other.field))
      {
        throw new System.ArgumentException("GF256Polys do not have same GF256 field");
      }
      if (Zero)
      {
        return other;
      }
      if (other.Zero)
      {
        return this;
      }
      
      int[] smallerCoefficients = this.coefficients;
      int[] largerCoefficients = other.coefficients;
      if (smallerCoefficients.Length > largerCoefficients.Length)
      {
        int[] temp = smallerCoefficients;
        smallerCoefficients = largerCoefficients;
        largerCoefficients = temp;
      }
      int[] sumDiff = new int[largerCoefficients.Length];
      int lengthDiff = largerCoefficients.Length - smallerCoefficients.Length;
      // Copy high-order terms only found in higher-degree polynomial's coefficients
      Array.Copy(largerCoefficients, 0, sumDiff, 0, lengthDiff);
      
      for (int i = lengthDiff; i < largerCoefficients.Length; i++)
      {
        sumDiff[i] = GF256.addOrSubtract(smallerCoefficients[i - lengthDiff], largerCoefficients[i]);
      }
      
      return new GF256Poly(field, sumDiff);
    }
    
    internal GF256Poly multiply(GF256Poly other)
    {
      if (!field.Equals(other.field))
      {
        throw new System.ArgumentException("GF256Polys do not have same GF256 field");
      }
      if (Zero || other.Zero)
      {
        return field.Zero;
      }
      int[] aCoefficients = this.coefficients;
      int aLength = aCoefficients.Length;
      int[] bCoefficients = other.coefficients;
      int bLength = bCoefficients.Length;
      int[] product = new int[aLength + bLength - 1];
      for (int i = 0; i < aLength; i++)
      {
        int aCoeff = aCoefficients[i];
        for (int j = 0; j < bLength; j++)
        {
          product[i + j] = GF256.addOrSubtract(product[i + j], field.multiply(aCoeff, bCoefficients[j]));
        }
      }
      return new GF256Poly(field, product);
    }
    
    internal GF256Poly multiplyByMonomial(int degree, int coefficient)
    {
      if (degree < 0)
      {
        throw new System.ArgumentException();
      }
      if (coefficient == 0)
      {
        return field.Zero;
      }
      int size = coefficients.Length;
      int[] product = new int[size + degree];
      for (int i = 0; i < size; i++)
      {
        product[i] = field.multiply(coefficients[i], coefficient);
      }
      return new GF256Poly(field, product);
    }
    
    internal GF256Poly[] divide(GF256Poly other)
    {
      if (!field.Equals(other.field))
      {
        throw new System.ArgumentException("GF256Polys do not have same GF256 field");
      }
      if (other.Zero)
      {
        throw new System.ArgumentException("Divide by 0");
      }
      
      GF256Poly quotient = field.Zero;
      GF256Poly remainder = this;
      
      int denominatorLeadingTerm = other.getCoefficient(other.Degree);
      int inverseDenominatorLeadingTerm = field.inverse(denominatorLeadingTerm);
      
      while (remainder.Degree >= other.Degree && !remainder.Zero)
      {
        int degreeDifference = remainder.Degree - other.Degree;
        int scale = field.multiply(remainder.getCoefficient(remainder.Degree), inverseDenominatorLeadingTerm);
        GF256Poly term = other.multiplyByMonomial(degreeDifference, scale);
        GF256Poly iterationQuotient = field.buildMonomial(degreeDifference, scale);
        quotient = quotient.addOrSubtract(iterationQuotient);
        remainder = remainder.addOrSubtract(term);
      }
      
      return new GF256Poly[]{quotient, remainder};
    }
  }
}