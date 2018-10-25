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

namespace FastReport.Barcode.Aztec
{
   /// <summary>
   /// Aztec 2D code representation
   /// </summary>
   /// <author>Rustam Abdullaev</author>
   internal sealed class AztecCode
   {
       private bool is_Compact;
       private int size;
       private int layers;
       private int codeWords;
       private BitMatrix matrix;

      /// <summary>
      /// Compact or full symbol indicator
      /// </summary>
      public bool isCompact 
      {
          get { return is_Compact; }
          set { is_Compact = value; }
      }

      /// <summary>
      /// Size in pixels (width and height)
      /// </summary>
      public int Size 
      {
          get { return size; }
          set { size = value; }
      }

      /// <summary>
      /// Number of levels
      /// </summary>
      public int Layers 
      {
          get { return layers; }
          set { layers = value; }
      }

      /// <summary>
      /// Number of data codewords
      /// </summary>
      public int CodeWords 
      {
          get { return codeWords; }
          set { codeWords = value; }
      }

      /// <summary>
      /// The symbol image
      /// </summary>
      public BitMatrix Matrix 
      {
          get { return matrix; }
          set { matrix = value; }
      }
   }
}