/*
 * Copyright 2013 ZXing authors
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

namespace FastReport.Barcode.Aztec
{
   /// <summary>
   /// Generates Aztec 2D barcodes.
   /// </summary>
   /// <author>Rustam Abdullaev</author>
   internal static class Encoder
   {
      public const int DEFAULT_EC_PERCENT = 33; // default minimal percentage of error check words
      public const int DEFAULT_AZTEC_LAYERS = 0;
      private const int MAX_NB_BITS = 32;
      private const int MAX_NB_BITS_COMPACT = 4;

      private static readonly int[] WORD_SIZE = {
                                                   4, 6, 6, 8, 8, 8, 8, 8, 8, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
                                                   12, 12, 12, 12, 12, 12, 12, 12, 12, 12
                                                };

      /// <summary>
      /// Encodes the given binary content as an Aztec symbol
      /// </summary>
      /// <param name="data">input data string</param>
      /// <returns>Aztec symbol matrix with metadata</returns>

      public static AztecCode encode(byte[] data)
      {
         return encode(data, DEFAULT_EC_PERCENT, DEFAULT_AZTEC_LAYERS);
      }

      /// <summary>
      /// Encodes the given binary content as an Aztec symbol
      /// </summary>
      /// <param name="data">input data string</param>
      /// <param name="minECCPercent">minimal percentage of error check words (According to ISO/IEC 24778:2008,
      /// a minimum of 23% + 3 words is recommended)</param>
      /// <param name="userSpecifiedLayers">if non-zero, a user-specified value for the number of layers</param>
      /// <returns>
      /// Aztec symbol matrix with metadata
      /// </returns>
      public static AztecCode encode(byte[] data, int minECCPercent, int userSpecifiedLayers)
      {
         // High-level encode
         BitArray bits = new HighLevelEncoder(data).encode();

         // stuff bits and choose symbol size
         int eccBits = bits.Size*minECCPercent/100 + 11;
         int totalSizeBits = bits.Size + eccBits;
         bool compact;
         int layers;
         int totalBitsInLayer;
         int wordSize;
         BitArray stuffedBits;

         if (userSpecifiedLayers != DEFAULT_AZTEC_LAYERS)
         {
            compact = userSpecifiedLayers < 0;
            layers = Math.Abs(userSpecifiedLayers);
            if (layers > (compact ? MAX_NB_BITS_COMPACT : MAX_NB_BITS))
            {
               throw new ArgumentException(
                  String.Format("Illegal value {0} for layers", userSpecifiedLayers));
            }
            totalBitsInLayer = TotalBitsInLayer(layers, compact);
            wordSize = WORD_SIZE[layers];
            int usableBitsInLayers = totalBitsInLayer - (totalBitsInLayer%wordSize);
            stuffedBits = stuffBits(bits, wordSize);
            if (stuffedBits.Size + eccBits > usableBitsInLayers)
            {
               throw new ArgumentException("Data to large for user specified layer");
            }
            if (compact && stuffedBits.Size > wordSize*64)
            {
               // Compact format only allows 64 data words, though C4 can hold more words than that
               throw new ArgumentException("Data to large for user specified layer");
            }
         }
         else
         {
            wordSize = 0;
            stuffedBits = null;
            // We look at the possible table sizes in the order Compact1, Compact2, Compact3,
            // Compact4, Normal4,...  Normal(i) for i < 4 isn't typically used since Compact(i+1)
            // is the same size, but has more data.
            for (int i = 0;; i++)
            {
               if (i > MAX_NB_BITS)
               {
                  throw new ArgumentException("Data too large for an Aztec code");
               }
               compact = i <= 3;
               layers = compact ? i + 1 : i;
               totalBitsInLayer = TotalBitsInLayer(layers, compact);
               if (totalSizeBits > totalBitsInLayer)
               {
                  continue;
               }
               // [Re]stuff the bits if this is the first opportunity, or if the
               // wordSize has changed
               if (wordSize != WORD_SIZE[layers])
               {
                  wordSize = WORD_SIZE[layers];
                  stuffedBits = stuffBits(bits, wordSize);
               }
               if (stuffedBits == null)
               {
                  continue;
               }
               int usableBitsInLayers = totalBitsInLayer - (totalBitsInLayer%wordSize);
               if (compact && stuffedBits.Size > wordSize*64)
               {
                  // Compact format only allows 64 data words, though C4 can hold more words than that
                  continue;
               }
               if (stuffedBits.Size + eccBits <= usableBitsInLayers)
               {
                  break;
               }
            }

         }

         BitArray messageBits = generateCheckWords(stuffedBits, totalBitsInLayer, wordSize);

         // generate mode message
         int messageSizeInWords = stuffedBits.Size / wordSize;
         BitArray modeMessage = generateModeMessage(compact, layers, messageSizeInWords);

         // allocate symbol
         int baseMatrixSize = compact ? 11 + layers*4 : 14 + layers*4; // not including alignment lines
         int[] alignmentMap = new int[baseMatrixSize];
         int matrixSize;
         if (compact)
         {
            // no alignment marks in compact mode, alignmentMap is a no-op
            matrixSize = baseMatrixSize;
            for (int i = 0; i < alignmentMap.Length; i++)
            {
               alignmentMap[i] = i;
            }
         }
         else
         {
            matrixSize = baseMatrixSize + 1 + 2*((baseMatrixSize/2 - 1)/15);
            int origCenter = baseMatrixSize/2;
            int center = matrixSize/2;
            for (int i = 0; i < origCenter; i++)
            {
               int newOffset = i + i/15;
               alignmentMap[origCenter - i - 1] = center - newOffset - 1;
               alignmentMap[origCenter + i] = center + newOffset + 1;
            }
         }
         BitMatrix matrix = new BitMatrix(matrixSize);

         // draw data bits
         for (int i = 0, rowOffset = 0; i < layers; i++)
         {
            int rowSize = compact ? (layers - i)*4 + 9 : (layers - i)*4 + 12;
            for (int j = 0; j < rowSize; j++)
            {
               int columnOffset = j*2;
               for (int k = 0; k < 2; k++)
               {
                  if (messageBits[rowOffset + columnOffset + k])
                  {
                     matrix[alignmentMap[i*2 + k], alignmentMap[i*2 + j]] = true;
                  }
                  if (messageBits[rowOffset + rowSize*2 + columnOffset + k])
                  {
                     matrix[alignmentMap[i*2 + j], alignmentMap[baseMatrixSize - 1 - i*2 - k]] = true;
                  }
                  if (messageBits[rowOffset + rowSize*4 + columnOffset + k])
                  {
                     matrix[alignmentMap[baseMatrixSize - 1 - i*2 - k], alignmentMap[baseMatrixSize - 1 - i*2 - j]] = true;
                  }
                  if (messageBits[rowOffset + rowSize*6 + columnOffset + k])
                  {
                     matrix[alignmentMap[baseMatrixSize - 1 - i*2 - j], alignmentMap[i*2 + k]] = true;
                  }
               }
            }
            rowOffset += rowSize*8;
         }

         // draw mode message
         drawModeMessage(matrix, compact, matrixSize, modeMessage);

         // draw alignment marks
         if (compact)
         {
            drawBullsEye(matrix, matrixSize/2, 5);
         }
         else
         {
            drawBullsEye(matrix, matrixSize/2, 7);
            for (int i = 0, j = 0; i < baseMatrixSize/2 - 1; i += 15, j += 16)
            {
               for (int k = (matrixSize/2) & 1; k < matrixSize; k += 2)
               {
                  matrix[matrixSize/2 - j, k] = true;
                  matrix[matrixSize/2 + j, k] = true;
                  matrix[k, matrixSize/2 - j] = true;
                  matrix[k, matrixSize/2 + j] = true;
               }
            }
         }

         AztecCode aztec = new AztecCode();
         aztec.isCompact = compact;
         aztec.Size = matrixSize;
         aztec.Layers = layers;
         aztec.CodeWords = messageSizeInWords;
         aztec.Matrix = matrix;
         return aztec;
      }

      private static void drawBullsEye(BitMatrix matrix, int center, int size)
      {
         for (int i = 0; i < size; i += 2)
         {
            for (int j = center - i; j <= center + i; j++)
            {
               matrix[j, center - i] = true;
               matrix[j, center + i] = true;
               matrix[center - i, j] = true;
               matrix[center + i, j] = true;
            }
         }
         matrix[center - size, center - size] = true;
         matrix[center - size + 1, center - size] = true;
         matrix[center - size, center - size + 1] = true;
         matrix[center + size, center - size] = true;
         matrix[center + size, center - size + 1] = true;
         matrix[center + size, center + size - 1] = true;
      }

      internal static BitArray generateModeMessage(bool compact, int layers, int messageSizeInWords)
      {
         BitArray modeMessage = new BitArray();
         if (compact)
         {
            modeMessage.appendBits(layers - 1, 2);
            modeMessage.appendBits(messageSizeInWords - 1, 6);
            modeMessage = generateCheckWords(modeMessage, 28, 4);
         }
         else
         {
            modeMessage.appendBits(layers - 1, 5);
            modeMessage.appendBits(messageSizeInWords - 1, 11);
            modeMessage = generateCheckWords(modeMessage, 40, 4);
         }
         return modeMessage;
      }

      private static void drawModeMessage(BitMatrix matrix, bool compact, int matrixSize, BitArray modeMessage)
      {
         int center = matrixSize / 2;

         if (compact)
         {
            for (int i = 0; i < 7; i++)
            {
               int offset = center - 3 + i;
               if (modeMessage[i])
               {
                  matrix[offset, center - 5] = true;
               }
               if (modeMessage[i + 7])
               {
                  matrix[center + 5, offset] = true;
               }
               if (modeMessage[20 - i])
               {
                  matrix[offset, center + 5] = true;
               }
               if (modeMessage[27 - i])
               {
                  matrix[center - 5, offset] = true;
               }
            }
         }
         else
         {
            for (int i = 0; i < 10; i++)
            {
               int offset = center - 5 + i + i / 5;
               if (modeMessage[i])
               {
                  matrix[offset, center - 7] = true;
               }
               if (modeMessage[i + 10])
               {
                  matrix[center + 7, offset] = true;
               }
               if (modeMessage[29 - i])
               {
                  matrix[offset, center + 7] = true;
               }
               if (modeMessage[39 - i])
               {
                  matrix[center - 7, offset] = true;
               }
            }
         }
      }

      private static BitArray generateCheckWords(BitArray bitArray, int totalBits, int wordSize)
      {
         if (bitArray.Size % wordSize != 0)
            throw new InvalidOperationException("size of bit array is not a multiple of the word size");

         // bitArray is guaranteed to be a multiple of the wordSize, so no padding needed
         int messageSizeInWords = bitArray.Size / wordSize;

         ReedSolomonEncoder rs = new ReedSolomonEncoder(getGF(wordSize));
         int totalWords = totalBits / wordSize;
         int[] messageWords = bitsToWords(bitArray, wordSize, totalWords);
         rs.encode(messageWords, totalWords - messageSizeInWords);

         int startPad = totalBits % wordSize;
         BitArray messageBits = new BitArray();
         messageBits.appendBits(0, startPad);
         foreach (int messageWord in messageWords)
         {
            messageBits.appendBits(messageWord, wordSize);
         }
         return messageBits;
      }

      private static int[] bitsToWords(BitArray stuffedBits, int wordSize, int totalWords)
      {
         int[] message = new int[totalWords];
         int i;
         int n;
         for (i = 0, n = stuffedBits.Size / wordSize; i < n; i++)
         {
            int value = 0;
            for (int j = 0; j < wordSize; j++)
            {
               value |= stuffedBits[i * wordSize + j] ? (1 << wordSize - j - 1) : 0;
            }
            message[i] = value;
         }
         return message;
      }

      private static GenericGF getGF(int wordSize)
      {
         switch (wordSize)
         {
            case 4:
               return GenericGF.AZTEC_PARAM;
            case 6:
               return GenericGF.AZTEC_DATA_6;
            case 8:
               return GenericGF.AZTEC_DATA_8;
            case 10:
               return GenericGF.AZTEC_DATA_10;
            case 12:
               return GenericGF.AZTEC_DATA_12;
            default:
               return null;
         }
      }

      internal static BitArray stuffBits(BitArray bits, int wordSize)
      {
         BitArray @out = new BitArray();

         int n = bits.Size;
         int mask = (1 << wordSize) - 2;
         for (int i = 0; i < n; i += wordSize)
         {
            int word = 0;
            for (int j = 0; j < wordSize; j++)
            {
               if (i + j >= n || bits[i + j])
               {
                  word |= 1 << (wordSize - 1 - j);
               }
            }
            if ((word & mask) == mask)
            {
               @out.appendBits(word & mask, wordSize);
               i--;
            }
            else if ((word & mask) == 0)
            {
               @out.appendBits(word | 1, wordSize);
               i--;
            }
            else
            {
               @out.appendBits(word, wordSize);
            }
         }

         return @out;
      }

      private static int TotalBitsInLayer(int layers, bool compact)
      {
         return ((compact ? 88 : 112) + 16 * layers) * layers;
      }
   }
}