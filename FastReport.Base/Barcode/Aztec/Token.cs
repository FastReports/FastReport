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
   internal abstract class Token
   {
      public static Token EMPTY = new SimpleToken(null, 0, 0);

      private readonly Token previous;

      protected Token(Token previous)
      {
         this.previous = previous;
      }

      public Token Previous
      {
         get { return previous; }
      }

      public Token add(int value, int bitCount)
      {
         return new SimpleToken(this, value, bitCount);
      }

      public Token addBinaryShift(int start, int byteCount)
      {
         int bitCount = (byteCount*8) + (byteCount <= 31 ? 10 : byteCount <= 62 ? 20 : 21);
         return new BinaryShiftToken(this, start, byteCount);
      }

      public abstract void appendTo(BitArray bitArray, byte[] text);
   }
}
