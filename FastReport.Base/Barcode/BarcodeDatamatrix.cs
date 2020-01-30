//
// Copyright 2007 by Paulo Soares.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1
// (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
// Software distributed under the License is distributed on an "AS IS" basis,
// WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
// for the specific language governing rights and limitations under the License.
//
// The Original Code is 'iText, a free JAVA-PDF library'.
//
// The Initial Developer of the Original Code is Bruno Lowagie. Portions created by
// the Initial Developer are Copyright (C) 1999, 2000, 2001, 2002 by Bruno Lowagie.
// All Rights Reserved.
// Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
// are Copyright (C) 2000, 2001, 2002 by Paulo Soares. All Rights Reserved.
// Modifications: Alexander Tzyganenko
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Barcode
{
  /// <summary>
  /// Specifies the Datamatrix encoding.
  /// </summary>
  public enum DatamatrixEncoding
  {
    /// <summary>
    /// Specifies the auto encoding.
    /// </summary>
    Auto,

    /// <summary>
    /// Specifies the ASCII encoding.
    /// </summary>
    Ascii,

    /// <summary>
    /// Specifies the C40 encoding.
    /// </summary>
    C40,

    /// <summary>
    /// Specifies the text encoding.
    /// </summary>
    Text,

    /// <summary>
    /// Specifies the binary encoding.
    /// </summary>
    Base256,

    /// <summary>
    /// Specifies the X12 encoding.
    /// </summary>
    X12,

    /// <summary>
    /// Specifies the Edifact encoding.
    /// </summary>
    Edifact
  }

  /// <summary>
  /// Specifies the Datamatrix symbol size.
  /// </summary>
  public enum DatamatrixSymbolSize
  {
    /// <summary>
    /// Specifies the auto size.
    /// </summary>
    Auto,
    
    /// <summary>
    /// Specifies the 10x10 size.
    /// </summary>
    Size10x10,

    /// <summary>
    /// Specifies the 12x12 size.
    /// </summary>
    Size12x12,

    /// <summary>
    /// Specifies the 8x8 size.
    /// </summary>
    Size8x18,

    /// <summary>
    /// Specifies the 14x14 size.
    /// </summary>
    Size14x14,

    /// <summary>
    /// Specifies the 8x32 size.
    /// </summary>
    Size8x32,

    /// <summary>
    /// Specifies the 16x16 size.
    /// </summary>
    Size16x16,

    /// <summary>
    /// Specifies the 12x26 size.
    /// </summary>
    Size12x26,

    /// <summary>
    /// Specifies the 18x18 size.
    /// </summary>
    Size18x18,

    /// <summary>
    /// Specifies the 20x20 size.
    /// </summary>
    Size20x20,

    /// <summary>
    /// Specifies the 12x36 size.
    /// </summary>
    Size12x36,

    /// <summary>
    /// Specifies the 22x22 size.
    /// </summary>
    Size22x22,

    /// <summary>
    /// Specifies the 16x36 size.
    /// </summary>
    Size16x36,

    /// <summary>
    /// Specifies the 24x24 size.
    /// </summary>
    Size24x24,

    /// <summary>
    /// Specifies the 26x26 size.
    /// </summary>
    Size26x26,

    /// <summary>
    /// Specifies the 16x48 size.
    /// </summary>
    Size16x48,

    /// <summary>
    /// Specifies the 32x32 size.
    /// </summary>
    Size32x32,

    /// <summary>
    /// Specifies the 36x36 size.
    /// </summary>
    Size36x36,

    /// <summary>
    /// Specifies the 40x40 size.
    /// </summary>
    Size40x40,

    /// <summary>
    /// Specifies the 44x44 size.
    /// </summary>
    Size44x44,

    /// <summary>
    /// Specifies the 48x48 size.
    /// </summary>
    Size48x48,

    /// <summary>
    /// Specifies the 52x52 size.
    /// </summary>
    Size52x52,

    /// <summary>
    /// Specifies the 64x64 size.
    /// </summary>
    Size64x64,

    /// <summary>
    /// Specifies the 72x72 size.
    /// </summary>
    Size72x72,

    /// <summary>
    /// Specifies the 80x80 size.
    /// </summary>
    Size80x80,

    /// <summary>
    /// Specifies the 88x88 size.
    /// </summary>
    Size88x88,

    /// <summary>
    /// Specifies the 96x96 size.
    /// </summary>
    Size96x96,

    /// <summary>
    /// Specifies the 104x104 size.
    /// </summary>
    Size104x104,

    /// <summary>
    /// Specifies the 120x120 size.
    /// </summary>
    Size120x120,

    /// <summary>
    /// Specifies the 132x132 size.
    /// </summary>
    Size132x132,

    /// <summary>
    /// Specifies the 144x144 size.
    /// </summary>
    Size144x144
  }

  /// <summary>
  /// Generates the 2D Data Matrix barcode.
  /// </summary>
  public sealed class BarcodeDatamatrix : Barcode2DBase
  {
    #region Fields
    private static readonly DmParams[] dmSizes = {
            new DmParams(10, 10, 10, 10, 3, 3, 5),
            new DmParams(12, 12, 12, 12, 5, 5, 7),
            new DmParams(8, 18, 8, 18, 5, 5, 7),
            new DmParams(14, 14, 14, 14, 8, 8, 10),
            new DmParams(8, 32, 8, 16, 10, 10, 11),
            new DmParams(16, 16, 16, 16, 12, 12, 12),
            new DmParams(12, 26, 12, 26, 16, 16, 14),
            new DmParams(18, 18, 18, 18, 18, 18, 14),
            new DmParams(20, 20, 20, 20, 22, 22, 18),
            new DmParams(12, 36, 12, 18, 22, 22, 18),
            new DmParams(22, 22, 22, 22, 30, 30, 20),
            new DmParams(16, 36, 16, 18, 32, 32, 24),
            new DmParams(24, 24, 24, 24, 36, 36, 24),
            new DmParams(26, 26, 26, 26, 44, 44, 28),
            new DmParams(16, 48, 16, 24, 49, 49, 28),
            new DmParams(32, 32, 16, 16, 62, 62, 36),
            new DmParams(36, 36, 18, 18, 86, 86, 42),
            new DmParams(40, 40, 20, 20, 114, 114, 48),
            new DmParams(44, 44, 22, 22, 144, 144, 56),
            new DmParams(48, 48, 24, 24, 174, 174, 68),
            new DmParams(52, 52, 26, 26, 204, 102, 42),
            new DmParams(64, 64, 16, 16, 280, 140, 56),
            new DmParams(72, 72, 18, 18, 368, 92, 36),
            new DmParams(80, 80, 20, 20, 456, 114, 48),
            new DmParams(88, 88, 22, 22, 576, 144, 56),
            new DmParams(96, 96, 24, 24, 696, 174, 68),
            new DmParams(104, 104, 26, 26, 816, 136, 56),
            new DmParams(120, 120, 20, 20, 1050, 175, 68),
            new DmParams(132, 132, 22, 22, 1304, 163, 62),
            new DmParams(144, 144, 24, 24, 1558, 156, 62)};

    private const String x12 = "\r*> 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private short[] place;
    private byte[] image;
    private int height;
    private int width;
    private DatamatrixSymbolSize symbolSize;
    private DatamatrixEncoding encoding;
    private int codePage;
    private int pixelSize;
        private bool autoEncode;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the symbol size.
    /// </summary>
    [DefaultValue(DatamatrixSymbolSize.Auto)]
    public DatamatrixSymbolSize SymbolSize
    {
      get { return symbolSize; }
      set { symbolSize = value; }
    }
    
    /// <summary>
    /// Gets or sets the encoding mode.
    /// </summary>
    [DefaultValue(DatamatrixEncoding.Auto)]
    public DatamatrixEncoding Encoding
    {
      get { return encoding; }
      set { encoding = value; }
    }

    /// <summary>
    /// Gets or sets the code page used for text conversion.
    /// </summary>
    /// <remarks>
    /// Use this property to encode non-ASCII characters. For example, set this 
    /// property to <b>1251</b> to use Window CP1251.
    /// </remarks>
    [DefaultValue(1252)]
    public int CodePage
    {
      get { return codePage; }
      set { codePage = value; }
    }

    /// <summary>
    /// Gets or sets the size of the pixel.
    /// </summary>
    [DefaultValue(3)]
    public int PixelSize
    {
      get { return pixelSize; }
      set { pixelSize = value; }
    }

        [DefaultValue(true)]
        public bool AutoEncode {
            get { return autoEncode; }
            set { autoEncode = value; }
        }
    #endregion

    #region Private Methods
    private void SetBit(int x, int y, int xByte)
    {
      image[y * xByte + x / 8] |= (byte)(128 >> (x & 7));
    }

    private void Draw(byte[] data, int dataSize, DmParams dm)
    {
      int i, j, p, x, y, xs, ys, z;
      int xByte = (dm.width + 7) / 8;
      for (int k = 0; k < image.Length; ++k)
        image[k] = 0;
      //alignment patterns
      //dotted horizontal line
      for (i = 0; i < dm.height; i += dm.heightSection)
      {
        for (j = 0; j < dm.width; j += 2)
        {
          SetBit(j, i, xByte);
        }
      }
      //solid horizontal line
      for (i = dm.heightSection - 1; i < dm.height; i += dm.heightSection)
      {
        for (j = 0; j < dm.width; ++j)
        {
          SetBit(j, i, xByte);
        }
      }
      //solid vertical line
      for (i = 0; i < dm.width; i += dm.widthSection)
      {
        for (j = 0; j < dm.height; ++j)
        {
          SetBit(i, j, xByte);
        }
      }
      //dotted vertical line
      for (i = dm.widthSection - 1; i < dm.width; i += dm.widthSection)
      {
        for (j = 1; j < dm.height; j += 2)
        {
          SetBit(i, j, xByte);
        }
      }
      p = 0;
      for (ys = 0; ys < dm.height; ys += dm.heightSection)
      {
        for (y = 1; y < dm.heightSection - 1; ++y)
        {
          for (xs = 0; xs < dm.width; xs += dm.widthSection)
          {
            for (x = 1; x < dm.widthSection - 1; ++x)
            {
              z = place[p++];
              if (z == 1 || (z > 1 && ((data[z / 8 - 1] & 0xff) & (128 >> (z % 8))) != 0))
                SetBit(x + xs, y + ys, xByte);
            }
          }
        }
      }
    }

    private static void MakePadding(byte[] data, int position, int count)
    {
      //already in ascii mode
      if (count <= 0)
        return;
      data[position++] = (byte)129;
      while (--count > 0)
      {
        int t = 129 + (((position + 1) * 149) % 253) + 1;
        if (t > 254)
          t -= 254;
        data[position++] = (byte)t;
      }
    }

    private static bool IsDigit(int c)
    {
      return c >= '0' && c <= '9';
    }

    private static int AsciiEncodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset, int dataLength)
    {
      int ptrIn, ptrOut, c;
      ptrIn = textOffset;
      ptrOut = dataOffset;
      textLength += textOffset;
      dataLength += dataOffset;
      while (ptrIn < textLength)
      {
        if (ptrOut >= dataLength)
          return -1;
        c = text[ptrIn++] & 0xff;
        if (IsDigit(c) && ptrIn < textLength && IsDigit(text[ptrIn] & 0xff))
        {
          data[ptrOut++] = (byte)((c - '0') * 10 + (text[ptrIn++] & 0xff) - '0' + 130);
        }
        else if (c > 127)
        {
          if (ptrOut + 1 >= dataLength)
            return -1;
          data[ptrOut++] = (byte)235;
          data[ptrOut++] = (byte)(c - 128 + 1);
        }
        else
        {
          data[ptrOut++] = (byte)(c + 1);
        }
      }
      return ptrOut - dataOffset;
    }

    private static int B256Encodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset, int dataLength)
    {
      int k, j, prn, tv, c;
      if (textLength == 0)
        return 0;
      if (textLength < 250 && textLength + 2 > dataLength)
        return -1;
      if (textLength >= 250 && textLength + 3 > dataLength)
        return -1;
      data[dataOffset] = (byte)231;
      if (textLength < 250)
      {
        data[dataOffset + 1] = (byte)textLength;
        k = 2;
      }
      else
      {
        data[dataOffset + 1] = (byte)(textLength / 250 + 249);
        data[dataOffset + 2] = (byte)(textLength % 250);
        k = 3;
      }
      System.Array.Copy(text, textOffset, data, k + dataOffset, textLength);
      k += textLength + dataOffset;
      for (j = dataOffset + 1; j < k; ++j)
      {
        c = data[j] & 0xff;
        prn = ((149 * (j + 1)) % 255) + 1;
        tv = c + prn;
        if (tv > 255)
          tv -= 256;
        data[j] = (byte)tv;

      }
      return k - dataOffset;
    }

    private static int X12Encodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset, int dataLength)
    {
      int ptrIn, ptrOut, count, k, n, ci;
      byte c;
      if (textLength == 0)
        return 0;
      ptrIn = 0;
      ptrOut = 0;
      byte[] x = new byte[textLength];
      count = 0;
      for (; ptrIn < textLength; ++ptrIn)
      {
        int i = x12.IndexOf((char)text[ptrIn + textOffset]);
        if (i >= 0)
        {
          x[ptrIn] = (byte)i;
          ++count;
        }
        else
        {
          x[ptrIn] = 100;
          if (count >= 6)
            count -= (count / 3) * 3;
          for (k = 0; k < count; ++k)
            x[ptrIn - k - 1] = 100;
          count = 0;
        }
      }
      if (count >= 6)
        count -= (count / 3) * 3;
      for (k = 0; k < count; ++k)
        x[ptrIn - k - 1] = 100;
      ptrIn = 0;
      c = 0;
      for (; ptrIn < textLength; ++ptrIn)
      {
        c = x[ptrIn];
        if (ptrOut >= dataLength)
          break;
        if (c < 40)
        {
          if (ptrIn == 0 || (ptrIn > 0 && x[ptrIn - 1] > 40))
            data[dataOffset + ptrOut++] = (byte)238;
          if (ptrOut + 2 > dataLength)
            break;
          n = 1600 * x[ptrIn] + 40 * x[ptrIn + 1] + x[ptrIn + 2] + 1;
          data[dataOffset + ptrOut++] = (byte)(n / 256);
          data[dataOffset + ptrOut++] = (byte)n;
          ptrIn += 2;
        }
        else
        {
          if (ptrIn > 0 && x[ptrIn - 1] < 40)
            data[dataOffset + ptrOut++] = (byte)254;
          ci = text[ptrIn + textOffset] & 0xff;
          if (ci > 127)
          {
            data[dataOffset + ptrOut++] = (byte)235;
            ci -= 128;
          }
          if (ptrOut >= dataLength)
            break;
          data[dataOffset + ptrOut++] = (byte)(ci + 1);
        }
      }
      c = 100;
      if (textLength > 0)
        c = x[textLength - 1];
      if (ptrIn != textLength || (c < 40 && ptrOut >= dataLength))
        return -1;
      if (c < 40)
        data[dataOffset + ptrOut++] = (byte)(254);
      return ptrOut;
    }

    private static int EdifactEncodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset, int dataLength)
    {
        int ptrIn, ptrOut, edi, pedi, c;
        if (textLength == 0)
            return 0;
        ptrIn = 0;
        ptrOut = 0;
        edi = 0;
        pedi = 18;
        bool ascii = true;
        for (; ptrIn < textLength; ++ptrIn)
        {
            c = text[ptrIn + textOffset] & 0xff;
            if (((c & 0xe0) == 0x40 || (c & 0xe0) == 0x20) && c != '_')
            {
                if (ascii)
                {
                    if (ptrOut + 1 > dataLength)
                        break;
                    data[dataOffset + ptrOut++] = (byte)240;
                    ascii = false;
                }
                c &= 0x3f;
                edi |= c << pedi;
                if (pedi == 0)
                {
                    if (ptrOut + 3 > dataLength)
                        break;
                    data[dataOffset + ptrOut++] = (byte)(edi >> 16);
                    data[dataOffset + ptrOut++] = (byte)(edi >> 8);
                    data[dataOffset + ptrOut++] = (byte)edi;
                    edi = 0;
                    pedi = 18;
                }
                else
                    pedi -= 6;
            }
            else
            {
                if (!ascii)
                {
                    edi |= ('_' & 0x3f) << pedi;
                    if (ptrOut + 3 - pedi / 8 > dataLength)
                        break;
                    data[dataOffset + ptrOut++] = (byte)(edi >> 16);
                    if (pedi <= 12)
                        data[dataOffset + ptrOut++] = (byte)(edi >> 8);
                    if (pedi <= 6)
                        data[dataOffset + ptrOut++] = (byte)edi;
                    ascii = true;
                    pedi = 18;
                    edi = 0;
                }
                if (c > 127)
                {
                    if (ptrOut >= dataLength)
                        break;
                    data[dataOffset + ptrOut++] = (byte)235;
                    c -= 128;
                }
                if (ptrOut >= dataLength)
                    break;
                data[dataOffset + ptrOut++] = (byte)(c + 1);
            }
        }
        if (ptrIn != textLength)
            return -1;
        int dataSize = int.MaxValue;
        for (int i = 0; i < dmSizes.Length; ++i)
        {
            if (dmSizes[i].dataSize >= dataOffset + ptrOut + (3 - pedi / 6))
            {
                dataSize = dmSizes[i].dataSize;
                break;
            }
        }

        if (dataSize - dataOffset - ptrOut <= 2 && pedi >= 6)
        {
            //have to write up to 2 bytes and up to 2 symbols
            if (pedi <= 12)
            {
                byte val = (byte)((edi >> 18) & 0x3F);
                if ((val & 0x20) == 0)
                    val |= 0x40;
                data[dataOffset + ptrOut++] = (byte)(val + 1);
            }
            if (pedi <= 6)
            {
                byte val = (byte)((edi >> 12) & 0x3F);
                if ((val & 0x20) == 0)
                    val |= 0x40;
                data[dataOffset + ptrOut++] = (byte)(val + 1);
            }
        }
        else if (!ascii)
        {
            edi |= ('_' & 0x3f) << pedi;
            if (ptrOut + 3 - pedi / 8 > dataLength)
                return -1;
            data[dataOffset + ptrOut++] = (byte)(edi >> 16);
            if (pedi <= 12)
                data[dataOffset + ptrOut++] = (byte)(edi >> 8);
            if (pedi <= 6)
                data[dataOffset + ptrOut++] = (byte)edi;
        }
        return ptrOut;
    }

    private static int C40OrTextEncodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset, int dataLength, bool c40)
    {
      int ptrIn, ptrOut, encPtr, last0, last1, i, a, c;
      String basic, shift2, shift3;
      if (textLength == 0)
        return 0;
      ptrIn = 0;
      ptrOut = 0;
      if (c40)
        data[dataOffset + ptrOut++] = (byte)230;
      else
        data[dataOffset + ptrOut++] = (byte)239;
      shift2 = "!\"#$%&'()*+,-./:;<=>?@[\\]^_";
      if (c40)
      {
        basic = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        shift3 = "`abcdefghijklmnopqrstuvwxyz{|}~\u007f";
      }
      else
      {
        basic = " 0123456789abcdefghijklmnopqrstuvwxyz";
        shift3 = "`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~\u007f";
      }
      int[] enc = new int[textLength * 4 + 10];
      encPtr = 0;
      last0 = 0;
      last1 = 0;
      while (ptrIn < textLength)
      {
        if ((encPtr % 3) == 0)
        {
          last0 = ptrIn;
          last1 = encPtr;
        }
        c = text[textOffset + ptrIn++] & 0xff;
        if (c > 127)
        {
          c -= 128;
          enc[encPtr++] = 1;
          enc[encPtr++] = 30;
        }
        int idx = basic.IndexOf((char)c);
        if (idx >= 0)
        {
          enc[encPtr++] = idx + 3;
        }
        else if (c < 32)
        {
          enc[encPtr++] = 0;
          enc[encPtr++] = c;
        }
        else if ((idx = shift2.IndexOf((char)c)) >= 0)
        {
          enc[encPtr++] = 1;
          enc[encPtr++] = idx;
        }
        else if ((idx = shift3.IndexOf((char)c)) >= 0)
        {
          enc[encPtr++] = 2;
          enc[encPtr++] = idx;
        }
      }
      if ((encPtr % 3) != 0)
      {
        ptrIn = last0;
        encPtr = last1;
      }
      if (encPtr / 3 * 2 > dataLength - 2)
      {
        return -1;
      }
      i = 0;
      for (; i < encPtr; i += 3)
      {
        a = 1600 * enc[i] + 40 * enc[i + 1] + enc[i + 2] + 1;
        data[dataOffset + ptrOut++] = (byte)(a / 256);
        data[dataOffset + ptrOut++] = (byte)a;
      }
      data[ptrOut++] = (byte)254;
      i = AsciiEncodation(text, ptrIn, textLength - ptrIn, data, ptrOut, dataLength - ptrOut);
      if (i < 0)
        return i;
      return ptrOut + i;
    }

    private int GetEncodation(byte[] text, int textOffset, int textSize, byte[] data, int dataOffset, int dataSize, bool firstMatch)
    {
      int e, j, k;
      int[] e1 = new int[6];
      if (dataSize < 0)
        return -1;
      e = -1;

      if (encoding == DatamatrixEncoding.Auto)
      {
        e1[0] = AsciiEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
        if (firstMatch && e1[0] >= 0)
          return e1[0];
        e1[1] = C40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, false);
        if (firstMatch && e1[1] >= 0)
          return e1[1];
        e1[2] = C40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, true);
        if (firstMatch && e1[2] >= 0)
          return e1[2];
        e1[3] = B256Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
        if (firstMatch && e1[3] >= 0)
          return e1[3];
        e1[4] = X12Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
        if (firstMatch && e1[4] >= 0)
          return e1[4];
        e1[5] = EdifactEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
        if (firstMatch && e1[5] >= 0)
          return e1[5];
        if (e1[0] < 0 && e1[1] < 0 && e1[2] < 0 && e1[3] < 0 && e1[4] < 0 && e1[5] < 0)
        {
          return -1;
        }
        j = 0;
        e = 99999;
        for (k = 0; k < 6; ++k)
        {
          if (e1[k] >= 0 && e1[k] < e)
          {
            e = e1[k];
            j = k;
          }
        }
        if (j == 0)
          e = AsciiEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
        else if (j == 1)
          e = C40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, false);
        else if (j == 2)
          e = C40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, true);
        else if (j == 3)
          e = B256Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
        else if (j == 4)
          e = X12Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
        return e;
      }

      switch (encoding)
      {
        case DatamatrixEncoding.Ascii:
          return AsciiEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
        case DatamatrixEncoding.C40:
          return C40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, true);
        case DatamatrixEncoding.Text:
          return C40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, false);
        case DatamatrixEncoding.Base256:
          return B256Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
        case DatamatrixEncoding.X12:
          return X12Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
        case DatamatrixEncoding.Edifact:
          return EdifactEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
      }
      return -1;
    }

    private static int GetNumber(byte[] text, int ptrIn, int n)
    {
      int v, j, c;
      v = 0;
      for (j = 0; j < n; ++j)
      {
        c = text[ptrIn++] & 0xff;
        if (c < '0' || c > '9')
          return -1;
        v = v * 10 + c - '0';
      }
      return v;
    }

    private string ReplaceControlCodes(string text)
    {
            if(AutoEncode)
            {
                if (text.StartsWith("&1;"))
                    text = ((char)232).ToString() + text.Remove(0, 3);
                text = text.Replace("&1;", ((char)0x1d).ToString());
            }
            return text;
    }
    
    private void Generate(String text)
    {
      text = ReplaceControlCodes(text);
      byte[] t = System.Text.Encoding.GetEncoding(CodePage).GetBytes(text);
      Generate(t, 0, t.Length);
    }

    private void Generate(byte[] text, int textOffset, int textSize)
    {
      int e, k, full;
      DmParams dm, last;
      byte[] data = new byte[2500];
      e = -1;
      int extCount = 0;
      if (text.Length > 0 && text[0] == 232)
      {
        data[0] = (byte)232;
        textOffset++;
        textSize--;
        extCount = 1;
      }
      
      if (height == 0 || width == 0)
      {
        last = dmSizes[dmSizes.Length - 1];
        e = GetEncodation(text, textOffset, textSize, data, extCount, last.dataSize, false);
        if (e < 0)
        {
          throw new Exception("The text is too big.");
        }
        e += extCount;
        for (k = 0; k < dmSizes.Length; ++k)
        {
          if (dmSizes[k].dataSize >= e)
            break;
        }
        dm = dmSizes[k];
        height = dm.height;
        width = dm.width;
      }
      else
      {
        for (k = 0; k < dmSizes.Length; ++k)
        {
          if (height == dmSizes[k].height && width == dmSizes[k].width)
            break;
        }
        if (k == dmSizes.Length)
        {
          throw new Exception("Invalid symbol size.");
        }
        dm = dmSizes[k];
        e = GetEncodation(text, textOffset, textSize, data, extCount, dm.dataSize, true);
        if (e < 0)
        {
          throw new Exception("The text is too big.");
        }
        e += extCount;
      }
      
      image = new byte[((dm.width + 7) / 8) * dm.height];
      MakePadding(data, e, dm.dataSize - e);
      place = Placement.DoPlacement(dm.height - (dm.height / dm.heightSection * 2), dm.width - (dm.width / dm.widthSection * 2));
      full = dm.dataSize + ((dm.dataSize + 2) / dm.dataBlock) * dm.errorBlock;
      ReedSolomon.GenerateECC(data, dm.dataSize, dm.dataBlock, dm.errorBlock);
      Draw(data, full, dm);
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(BarcodeBase source)
    {
      base.Assign(source);
      BarcodeDatamatrix src = source as BarcodeDatamatrix;

      SymbolSize = src.SymbolSize;
      Encoding = src.Encoding;
      CodePage = src.CodePage;
      PixelSize = src.PixelSize;
            AutoEncode = src.AutoEncode;
    }

    internal override void Serialize(FastReport.Utils.FRWriter writer, string prefix, BarcodeBase diff)
    {
      base.Serialize(writer, prefix, diff);
      BarcodeDatamatrix c = diff as BarcodeDatamatrix;

            if (c == null || SymbolSize != c.SymbolSize)
                writer.WriteValue(prefix + "SymbolSize", SymbolSize);
            if (c == null || Encoding != c.Encoding)
                writer.WriteValue(prefix + "Encoding", Encoding);
            if (c == null || CodePage != c.CodePage)
                writer.WriteInt(prefix + "CodePage", CodePage);
            if (c == null || PixelSize != c.PixelSize)
                writer.WriteInt(prefix + "PixelSize", PixelSize);
            if (c == null || AutoEncode != c.AutoEncode)
                writer.WriteBool(prefix + "AutoEncode", AutoEncode);
    }

    internal override void Initialize(string text, bool showText, int angle, float zoom)
    {
      base.Initialize(text, showText, angle, zoom);
      
      if (SymbolSize == DatamatrixSymbolSize.Auto)
      {
        width = 0;
        height = 0;
      }
      else
      {
        DmParams dmParams = dmSizes[(int)SymbolSize - 1];
        width = dmParams.width;
        height = dmParams.height;
      }

            Generate(base.text);
    }

    internal override SizeF CalcBounds()
    {
      int textAdd = showText ? 18 : 0;
      return new SizeF(width * PixelSize, height * PixelSize + textAdd);
    }

    internal override string StripControlCodes(string data)
    {
            if(AutoEncode)
            {
                if (data.StartsWith("&1;"))
                    data = data.Remove(0, 3);
                data = data.Replace("&1;", " ");
            }
            return data;
    }

    internal override void Draw2DBarcode(IGraphicsRenderer g, float kx, float ky)
    {
      if (image == null)
        return;
      
      Brush light = Brushes.White;
      Brush dark = new SolidBrush(Color);
      int stride = (width + 7) / 8;

      for (int k = 0; k < height; ++k)
      {
        int p = k * stride;
        for (int j = 0; j < width; ++j)
        {
          int b = image[p + (j / 8)] & 0xff;
          b <<= j % 8;

          Brush brush = /*(b & 0x80) == 0 ? light :*/ dark;
          if ((b & 0x80) != 0)
            g.FillRectangle(brush, j * PixelSize * kx, k * PixelSize * ky,
            PixelSize * kx, PixelSize * ky);
        }
      }

      dark.Dispose();
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="BarcodeDatamatrix"/> class with default settings.
    /// </summary>
    public BarcodeDatamatrix()
    {
      CodePage = 1252;
      PixelSize = 3;
            AutoEncode = true;
#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
    }


    private class DmParams
    {
      internal DmParams(int height, int width, int heightSection, int widthSection, int dataSize, int dataBlock, int errorBlock)
      {
        this.height = height;
        this.width = width;
        this.heightSection = heightSection;
        this.widthSection = widthSection;
        this.dataSize = dataSize;
        this.dataBlock = dataBlock;
        this.errorBlock = errorBlock;
      }

      internal int height;
      internal int width;
      internal int heightSection;
      internal int widthSection;
      internal int dataSize;
      internal int dataBlock;
      internal int errorBlock;
    };

    private class Placement
    {
      private int nrow;
      private int ncol;
      private short[] array;
      private static Hashtable cache = Hashtable.Synchronized(new Hashtable());

      private Placement()
      {
      }

      internal static short[] DoPlacement(int nrow, int ncol)
      {
        int key = nrow * 1000 + ncol;
        short[] pc = (short[])cache[key];
        if (pc != null)
          return pc;
        Placement p = new Placement();
        p.nrow = nrow;
        p.ncol = ncol;
        p.array = new short[nrow * ncol];
        p.Ecc200();
        cache[key] = p.array;
        return p.array;
      }

      /* "module" places "chr+bit" with appropriate wrapping within array[] */
      private void Module(int row, int col, int chr, int bit)
      {
        if (row < 0) { row += nrow; col += 4 - ((nrow + 4) % 8); }
        if (col < 0) { col += ncol; row += 4 - ((ncol + 4) % 8); }
        array[row * ncol + col] = (short)(8 * chr + bit);
      }
      /* "utah" places the 8 bits of a utah-shaped symbol character in ECC200 */
      private void Utah(int row, int col, int chr)
      {
        Module(row - 2, col - 2, chr, 0);
        Module(row - 2, col - 1, chr, 1);
        Module(row - 1, col - 2, chr, 2);
        Module(row - 1, col - 1, chr, 3);
        Module(row - 1, col, chr, 4);
        Module(row, col - 2, chr, 5);
        Module(row, col - 1, chr, 6);
        Module(row, col, chr, 7);
      }
      /* "cornerN" places 8 bits of the four special corner cases in ECC200 */
      private void Corner1(int chr)
      {
        Module(nrow - 1, 0, chr, 0);
        Module(nrow - 1, 1, chr, 1);
        Module(nrow - 1, 2, chr, 2);
        Module(0, ncol - 2, chr, 3);
        Module(0, ncol - 1, chr, 4);
        Module(1, ncol - 1, chr, 5);
        Module(2, ncol - 1, chr, 6);
        Module(3, ncol - 1, chr, 7);
      }
      private void Corner2(int chr)
      {
        Module(nrow - 3, 0, chr, 0);
        Module(nrow - 2, 0, chr, 1);
        Module(nrow - 1, 0, chr, 2);
        Module(0, ncol - 4, chr, 3);
        Module(0, ncol - 3, chr, 4);
        Module(0, ncol - 2, chr, 5);
        Module(0, ncol - 1, chr, 6);
        Module(1, ncol - 1, chr, 7);
      }
      private void Corner3(int chr)
      {
        Module(nrow - 3, 0, chr, 0);
        Module(nrow - 2, 0, chr, 1);
        Module(nrow - 1, 0, chr, 2);
        Module(0, ncol - 2, chr, 3);
        Module(0, ncol - 1, chr, 4);
        Module(1, ncol - 1, chr, 5);
        Module(2, ncol - 1, chr, 6);
        Module(3, ncol - 1, chr, 7);
      }
      private void Corner4(int chr)
      {
        Module(nrow - 1, 0, chr, 0);
        Module(nrow - 1, ncol - 1, chr, 1);
        Module(0, ncol - 3, chr, 2);
        Module(0, ncol - 2, chr, 3);
        Module(0, ncol - 1, chr, 4);
        Module(1, ncol - 3, chr, 5);
        Module(1, ncol - 2, chr, 6);
        Module(1, ncol - 1, chr, 7);
      }
      /* "ECC200" fills an nrow x ncol array with appropriate values for ECC200 */
      private void Ecc200()
      {
        int row, col, chr;
        /* First, fill the array[] with invalid entries */
        for (int k = 0; k < array.Length; ++k)
          array[k] = (short)0;
        /* Starting in the correct location for character #1, bit 8,... */
        chr = 1; row = 4; col = 0;
        do
        {
          /* repeatedly first check for one of the special corner cases, then... */
          if ((row == nrow) && (col == 0)) Corner1(chr++);
          if ((row == nrow - 2) && (col == 0) && (ncol % 4 != 0)) Corner2(chr++);
          if ((row == nrow - 2) && (col == 0) && (ncol % 8 == 4)) Corner3(chr++);
          if ((row == nrow + 4) && (col == 2) && (ncol % 8 == 0)) Corner4(chr++);
          /* sweep upward diagonally, inserting successive characters,... */
          do
          {
            if ((row < nrow) && (col >= 0) && array[row * ncol + col] == 0)
              Utah(row, col, chr++);
            row -= 2; col += 2;
          } while ((row >= 0) && (col < ncol));
          row += 1; col += 3;
          /* & then sweep downward diagonally, inserting successive characters,... */

          do
          {
            if ((row >= 0) && (col < ncol) && array[row * ncol + col] == 0)
              Utah(row, col, chr++);
            row += 2; col -= 2;
          } while ((row < nrow) && (col >= 0));
          row += 3; col += 1;
          /* ... until the entire array is scanned */
        } while ((row < nrow) || (col < ncol));
        /* Lastly, if the lower righthand corner is untouched, fill in fixed pattern */
        if (array[nrow * ncol - 1] == 0)
        {
          array[nrow * ncol - 1] = array[nrow * ncol - ncol - 2] = 1;
        }
      }
    }

    private class ReedSolomon
    {

      private static readonly int[] log = {
                0, 255,   1, 240,   2, 225, 241,  53,   3,  38, 226, 133, 242,  43,  54, 210,
                4, 195,  39, 114, 227, 106, 134,  28, 243, 140,  44,  23,  55, 118, 211, 234,
                5, 219, 196,  96,  40, 222, 115, 103, 228,  78, 107, 125, 135,   8,  29, 162,
                244, 186, 141, 180,  45,  99,  24,  49,  56,  13, 119, 153, 212, 199, 235,  91,
                6,  76, 220, 217, 197,  11,  97, 184,  41,  36, 223, 253, 116, 138, 104, 193,
                229,  86,  79, 171, 108, 165, 126, 145, 136,  34,   9,  74,  30,  32, 163,  84,
                245, 173, 187, 204, 142,  81, 181, 190,  46,  88, 100, 159,  25, 231,  50, 207,
                57, 147,  14,  67, 120, 128, 154, 248, 213, 167, 200,  63, 236, 110,  92, 176,
                7, 161,  77, 124, 221, 102, 218,  95, 198,  90,  12, 152,  98,  48, 185, 179,
                42, 209,  37, 132, 224,  52, 254, 239, 117, 233, 139,  22, 105,  27, 194, 113,
                230, 206,  87, 158,  80, 189, 172, 203, 109, 175, 166,  62, 127, 247, 146,  66,
                137, 192,  35, 252,  10, 183,  75, 216,  31,  83,  33,  73, 164, 144,  85, 170,
                246,  65, 174,  61, 188, 202, 205, 157, 143, 169,  82,  72, 182, 215, 191, 251,
                47, 178,  89, 151, 101,  94, 160, 123,  26, 112, 232,  21,  51, 238, 208, 131,
                58,  69, 148,  18,  15,  16,  68,  17, 121, 149, 129,  19, 155,  59, 249,  70,
                214, 250, 168,  71, 201, 156,  64,  60, 237, 130, 111,  20,  93, 122, 177, 150
            };

      private static readonly int[] alog = {
                1,   2,   4,   8,  16,  32,  64, 128,  45,  90, 180,  69, 138,  57, 114, 228,
                229, 231, 227, 235, 251, 219, 155,  27,  54, 108, 216, 157,  23,  46,  92, 184,
                93, 186,  89, 178,  73, 146,   9,  18,  36,  72, 144,  13,  26,  52, 104, 208,
                141,  55, 110, 220, 149,   7,  14,  28,  56, 112, 224, 237, 247, 195, 171, 123,
                246, 193, 175, 115, 230, 225, 239, 243, 203, 187,  91, 182,  65, 130,  41,  82,
                164, 101, 202, 185,  95, 190,  81, 162, 105, 210, 137,  63, 126, 252, 213, 135,
                35,  70, 140,  53, 106, 212, 133,  39,  78, 156,  21,  42,  84, 168, 125, 250,
                217, 159,  19,  38,  76, 152,  29,  58, 116, 232, 253, 215, 131,  43,  86, 172,
                117, 234, 249, 223, 147,  11,  22,  44,  88, 176,  77, 154,  25,  50, 100, 200,
                189,  87, 174, 113, 226, 233, 255, 211, 139,  59, 118, 236, 245, 199, 163, 107,
                214, 129,  47,  94, 188,  85, 170, 121, 242, 201, 191,  83, 166,  97, 194, 169,
                127, 254, 209, 143,  51, 102, 204, 181,  71, 142,  49,  98, 196, 165, 103, 206,
                177,  79, 158,  17,  34,  68, 136,  61, 122, 244, 197, 167,  99, 198, 161, 111,
                222, 145,  15,  30,  60, 120, 240, 205, 183,  67, 134,  33,  66, 132,  37,  74,
                148,   5,  10,  20,  40,  80, 160, 109, 218, 153,  31,  62, 124, 248, 221, 151,
                3,   6,  12,  24,  48,  96, 192, 173, 119, 238, 241, 207, 179,  75, 150,   1
            };

      private static readonly int[] poly5 = {
                228,  48,  15, 111,  62
            };

      private static readonly int[] poly7 = {
                23,  68, 144, 134, 240,  92, 254
            };

      private static readonly int[] poly10 = {
                28,  24, 185, 166, 223, 248, 116, 255, 110,  61
            };

      private static readonly int[] poly11 = {
                175, 138, 205,  12, 194, 168,  39, 245,  60,  97, 120
            };

      private static readonly int[] poly12 = {
                41, 153, 158,  91,  61,  42, 142, 213,  97, 178, 100, 242
            };

      private static readonly int[] poly14 = {
                156,  97, 192, 252,  95,   9, 157, 119, 138,  45,  18, 186,  83, 185
            };

      private static readonly int[] poly18 = {
                83, 195, 100,  39, 188,  75,  66,  61, 241, 213, 109, 129,  94, 254, 225,  48,
                90, 188
            };

      private static readonly int[] poly20 = {
                15, 195, 244,   9, 233,  71, 168,   2, 188, 160, 153, 145, 253,  79, 108,  82,
                27, 174, 186, 172
            };

      private static readonly int[] poly24 = {
                52, 190,  88, 205, 109,  39, 176,  21, 155, 197, 251, 223, 155,  21,   5, 172,
                254, 124,  12, 181, 184,  96,  50, 193
            };

      private static readonly int[] poly28 = {
                211, 231,  43,  97,  71,  96, 103, 174,  37, 151, 170,  53,  75,  34, 249, 121,
                17, 138, 110, 213, 141, 136, 120, 151, 233, 168,  93, 255
            };

      private static readonly int[] poly36 = {
                245, 127, 242, 218, 130, 250, 162, 181, 102, 120,  84, 179, 220, 251,  80, 182,
                229,  18,   2,   4,  68,  33, 101, 137,  95, 119, 115,  44, 175, 184,  59,  25,
                225,  98,  81, 112
            };

      private static readonly int[] poly42 = {
                77, 193, 137,  31,  19,  38,  22, 153, 247, 105, 122,   2, 245, 133, 242,   8,
                175,  95, 100,   9, 167, 105, 214, 111,  57, 121,  21,   1, 253,  57,  54, 101,
                248, 202,  69,  50, 150, 177, 226,   5,   9,   5
            };

      private static readonly int[] poly48 = {
                245, 132, 172, 223,  96,  32, 117,  22, 238, 133, 238, 231, 205, 188, 237,  87,
                191, 106,  16, 147, 118,  23,  37,  90, 170, 205, 131,  88, 120, 100,  66, 138,
                186, 240,  82,  44, 176,  87, 187, 147, 160, 175,  69, 213,  92, 253, 225,  19
            };

      private static readonly int[] poly56 = {
                175,   9, 223, 238,  12,  17, 220, 208, 100,  29, 175, 170, 230, 192, 215, 235,
                150, 159,  36, 223,  38, 200, 132,  54, 228, 146, 218, 234, 117, 203,  29, 232,
                144, 238,  22, 150, 201, 117,  62, 207, 164,  13, 137, 245, 127,  67, 247,  28,
                155,  43, 203, 107, 233,  53, 143,  46
            };

      private static readonly int[] poly62 = {
                242,  93, 169,  50, 144, 210,  39, 118, 202, 188, 201, 189, 143, 108, 196,  37,
                185, 112, 134, 230, 245,  63, 197, 190, 250, 106, 185, 221, 175,  64, 114,  71,
                161,  44, 147,   6,  27, 218,  51,  63,  87,  10,  40, 130, 188,  17, 163,  31,
                176, 170,   4, 107, 232,   7,  94, 166, 224, 124,  86,  47,  11, 204
            };

      private static readonly int[] poly68 = {
                220, 228, 173,  89, 251, 149, 159,  56,  89,  33, 147, 244, 154,  36,  73, 127,
                213, 136, 248, 180, 234, 197, 158, 177,  68, 122,  93, 213,  15, 160, 227, 236,
                66, 139, 153, 185, 202, 167, 179,  25, 220, 232,  96, 210, 231, 136, 223, 239,
                181, 241,  59,  52, 172,  25,  49, 232, 211, 189,  64,  54, 108, 153, 132,  63,
                96, 103,  82, 186
            };

      private static int[] GetPoly(int nc)
      {
        switch (nc)
        {
          case 5:
            return poly5;
          case 7:
            return poly7;
          case 10:
            return poly10;
          case 11:
            return poly11;
          case 12:
            return poly12;
          case 14:
            return poly14;
          case 18:
            return poly18;
          case 20:
            return poly20;
          case 24:
            return poly24;
          case 28:
            return poly28;
          case 36:
            return poly36;
          case 42:
            return poly42;
          case 48:
            return poly48;
          case 56:
            return poly56;
          case 62:
            return poly62;
          case 68:
            return poly68;
        }
        return null;
      }

      private static void ReedSolomonBlock(byte[] wd, int nd, byte[] ncout, int nc, int[] c)
      {
        int i, j, k;

        for (i = 0; i <= nc; i++) ncout[i] = 0;
        for (i = 0; i < nd; i++)
        {
          k = (ncout[0] ^ wd[i]) & 0xff;
          for (j = 0; j < nc; j++)
          {
            ncout[j] = (byte)(ncout[j + 1] ^ (k == 0 ? (byte)0 : (byte)alog[(log[k] + log[c[nc - j - 1]]) % (255)]));
          }
        }
      }

      internal static void GenerateECC(byte[] wd, int nd, int datablock, int nc)
      {
        int blocks = (nd + 2) / datablock;
        int b;
        byte[] buf = new byte[256];
        byte[] ecc = new byte[256];
        int[] c = GetPoly(nc);
        if (c == null)
            return;
        for (b = 0; b < blocks; b++)
        {
          int n, p = 0;
          for (n = b; n < nd; n += blocks)
            buf[p++] = wd[n];
          ReedSolomonBlock(buf, p, ecc, nc, c);
          p = 0;
          for (n = b; n < nc * blocks; n += blocks)
            wd[nd + n] = ecc[p++];
        }
      }
    }
  }
}
