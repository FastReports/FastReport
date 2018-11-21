using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace FastReport.Barcode
{
    /// <summary>
    /// Generates the 2D MaxiCode barcode.
    /// </summary>
    public class BarcodeMaxiCode : Barcode2DBase
    {
        /// <summary>
        /// Sets the MaxiCode mode to use. Only modes 2 to 6 are supported.
        /// </summary>
        [DefaultValue(4)]
        public int Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
            }
        }

        //public int ECI { get; set; }

        const float FieldSizeFactor = 2.47f;
        const float PenSizeFactor = 4.5f;
        MaxiCodeImpl maxiCodeImpl;
        int mode;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeMaxiCode"/> class with default settings.
        /// </summary>
        public BarcodeMaxiCode()
        {
            Mode = 4;
        }

        internal override void Initialize(string text, bool showText, int angle, float zoom)
        {
            base.Initialize(text, showText, angle, zoom);

            maxiCodeImpl = new MaxiCodeImpl(base.text);
            maxiCodeImpl.setMode(Mode);
            maxiCodeImpl.encode();
        }

        internal override SizeF CalcBounds()
        {
            int textAdd = showText ? 18 : 0;
            SizeF s = new SizeF();

            foreach(MaxiCodeImpl.Hexagon hex in maxiCodeImpl.hexagons)
            {
                for (int i = 0; i < hex.pointX.Length; i++)
                {
                    float pointX = FieldSizeFactor * (float)hex.pointX[i];
                    float pointY = FieldSizeFactor * (float)hex.pointY[i];

                    if (pointX > s.Width)
                        s.Width = pointX;
                    if (pointY > s.Height)
                        s.Height = pointY;
                }
            }

            if (s.Width <= 0)
                s.Width = 100;
            if (s.Height <= 0)
                s.Height = 100;

            s.Height += textAdd;

            return s;
        }

        internal override void Draw2DBarcode(IGraphicsRenderer g, float kx, float ky)
        {
            Brush b = new SolidBrush(Color);
            Pen p = new Pen(Color, PenSizeFactor * ((kx + ky) / 2));

            foreach (MaxiCodeImpl.Hexagon hex in maxiCodeImpl.hexagons)
            {
                PointF[] points = new PointF[hex.pointX.Length];

                for (int i = 0; i < hex.pointX.Length; i++)
                    points[i] = new PointF(FieldSizeFactor * kx * (float)hex.pointX[i], FieldSizeFactor * ky * (float)hex.pointY[i]);

                g.FillPolygon(b, points);
            }
            
            foreach (MaxiCodeImpl.Ellipse circle in maxiCodeImpl.target)
            {
                g.DrawEllipse(p,
                    FieldSizeFactor * kx * (float)circle.x,
                    FieldSizeFactor * ky * (float)circle.y,
                    FieldSizeFactor * kx * ((float)circle.w - (float)circle.x),
                    FieldSizeFactor * ky * ((float)circle.h - (float)circle.y));
            }

            b.Dispose();
            p.Dispose();
        }

        /// <inheritdoc/>
        public override void Assign(BarcodeBase source)
        {
            base.Assign(source);
            BarcodeMaxiCode src = source as BarcodeMaxiCode;

            Mode = src.Mode;
        }

        internal override void Serialize(FastReport.Utils.FRWriter writer, string prefix, BarcodeBase diff)
        {
            base.Serialize(writer, prefix, diff);
            BarcodeMaxiCode c = diff as BarcodeMaxiCode;

            if (c == null || Mode != c.Mode)
                writer.WriteInt(prefix + "Mode", Mode);
        }
    }

    /*
     * Copyright 2014-2015 Robin Stuart, Daniel Gredler
     *
     * Licensed under the Apache License, Version 2.0 (the "License");
     * you may not use this file except in compliance with the License.
     * You may obtain a copy of the License at
     *
     *    http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    /**
     * Implements MaxiCode according to ISO 16023:2000.
     *
     * MaxiCode employs a pattern of hexagons around a central 'bulls-eye'
     * finder pattern. Encoding in several modes is supported, but encoding in
     * Mode 2 and 3 require primary messages to be set. Input characters can be
     * any from the ISO 8859-1 (Latin-1) character set.
     *
     * TODO: Add ECI functionality.
     *
     * @author <a href="mailto:rstuart114@gmail.com">Robin Stuart</a>
     * @author Daniel Gredler
     */
    class MaxiCodeImpl
    {
        public class ReedSolomon
        {
            private int logmod;
            private int rlen;

            private int[] logt;
            private int[] alog;
            private int[] rspoly;
            public int[] res;

            public int getResult(int count)
            {
                return res[count];
            }

            public void init_gf(int poly)
            {
                int m, b, p, v;

                // Find the top bit, and hence the symbol size
                for (b = 1, m = 0; b <= poly; b <<= 1)
                {
                    m++;
                }
                b >>= 1;
                m--;

                // Calculate the log/alog tables
                logmod = (1 << m) - 1;
                logt = new int[logmod + 1];
                alog = new int[logmod];

                for (p = 1, v = 0; v < logmod; v++)
                {
                    alog[v] = p;
                    logt[p] = v;
                    p <<= 1;
                    if ((p & b) != 0)
                    {
                        p ^= poly;
                    }
                }
            }

            public void init_code(int nsym, int index)
            {
                int i, k;

                rspoly = new int[nsym + 1];

                rlen = nsym;

                rspoly[0] = 1;
                for (i = 1; i <= nsym; i++)
                {
                    rspoly[i] = 1;
                    for (k = i - 1; k > 0; k--)
                    {
                        if (rspoly[k] != 0)
                        {
                            rspoly[k] = alog[(logt[rspoly[k]] + index) % logmod];
                        }
                        rspoly[k] ^= rspoly[k - 1];
                    }
                    rspoly[0] = alog[(logt[rspoly[0]] + index) % logmod];
                    index++;
                }
            }

            public void encode(int len, int[] data)
            {
                int i, k, m;

                res = new int[rlen];
                for (i = 0; i < rlen; i++)
                {
                    res[i] = 0;
                }
                for (i = 0; i < len; i++)
                {
                    m = res[rlen - 1] ^ data[i];
                    for (k = rlen - 1; k > 0; k--)
                    {
                        if ((m != 0) && (rspoly[k] != 0))
                        {
                            res[k] = res[k - 1] ^ alog[(logt[m] + logt[rspoly[k]]) % logmod];
                        }
                        else
                        {
                            res[k] = res[k - 1];
                        }
                    }
                    if ((m != 0) && (rspoly[0] != 0))
                    {
                        res[0] = alog[(logt[m] + logt[rspoly[0]]) % logmod];
                    }
                    else
                    {
                        res[0] = 0;
                    }
                }
            }
        }

        public struct Hexagon
        {
            const double INK_SPREAD = 1.25;

            static double[] OFFSET_X = new double[] { 0.0, 0.86, 0.86, 0.0, -0.86, -0.86 };
            static double[] OFFSET_Y = new double[] { 1.0, 0.5, -0.5, -1.0, -0.5, 0.5 };

            public double centreX;
            public double centreY;
            public double[] pointX;
            public double[] pointY;

            public Hexagon(double centreX, double centreY)
            {
                this.centreX = centreX;
                this.centreY = centreY;

                pointX = new double[6];
                pointY = new double[6];

                for (int i = 0; i < 6; i++)
                {
                    pointX[i] = centreX + (OFFSET_X[i] * INK_SPREAD);
                    pointY[i] = centreY + (OFFSET_Y[i] * INK_SPREAD);
                }
            }
        }

        public struct Ellipse
        {
#pragma warning disable FR0006 // Field name of struct must be longer than 2 characters.
            public double x;
            public double y;
            public double w;
            public double h;
#pragma warning restore FR0006 // Field name of struct must be longer than 2 characters.

            public Ellipse(double x, double y, double w, double h)
            {
                this.x = x;
                this.y = y;
                this.w = w;
                this.h = h;
            }
        }

        private int eciMode = 3;
        private byte[] inputBytes;
        //string error_msg;
        public List<Hexagon> hexagons = new List<Hexagon>();
        public List<Ellipse> target = new List<Ellipse>();
        private string text;

        public MaxiCodeImpl(string text)
        {
            this.text = text;
        }

        //========================================================================

        /** MaxiCode module sequence, from ISO/IEC 16023 Figure 5 (30 x 33 data grid). */
        static int[] MAXICODE_GRID = {
            122, 121, 128, 127, 134, 133, 140, 139, 146, 145, 152, 151, 158, 157, 164, 163, 170, 169, 176, 175, 182, 181, 188, 187, 194, 193, 200, 199, 0,   0,
            124, 123, 130, 129, 136, 135, 142, 141, 148, 147, 154, 153, 160, 159, 166, 165, 172, 171, 178, 177, 184, 183, 190, 189, 196, 195, 202, 201, 817, 0,
            126, 125, 132, 131, 138, 137, 144, 143, 150, 149, 156, 155, 162, 161, 168, 167, 174, 173, 180, 179, 186, 185, 192, 191, 198, 197, 204, 203, 819, 818,
            284, 283, 278, 277, 272, 271, 266, 265, 260, 259, 254, 253, 248, 247, 242, 241, 236, 235, 230, 229, 224, 223, 218, 217, 212, 211, 206, 205, 820, 0,
            286, 285, 280, 279, 274, 273, 268, 267, 262, 261, 256, 255, 250, 249, 244, 243, 238, 237, 232, 231, 226, 225, 220, 219, 214, 213, 208, 207, 822, 821,
            288, 287, 282, 281, 276, 275, 270, 269, 264, 263, 258, 257, 252, 251, 246, 245, 240, 239, 234, 233, 228, 227, 222, 221, 216, 215, 210, 209, 823, 0,
            290, 289, 296, 295, 302, 301, 308, 307, 314, 313, 320, 319, 326, 325, 332, 331, 338, 337, 344, 343, 350, 349, 356, 355, 362, 361, 368, 367, 825, 824,
            292, 291, 298, 297, 304, 303, 310, 309, 316, 315, 322, 321, 328, 327, 334, 333, 340, 339, 346, 345, 352, 351, 358, 357, 364, 363, 370, 369, 826, 0,
            294, 293, 300, 299, 306, 305, 312, 311, 318, 317, 324, 323, 330, 329, 336, 335, 342, 341, 348, 347, 354, 353, 360, 359, 366, 365, 372, 371, 828, 827,
            410, 409, 404, 403, 398, 397, 392, 391, 80,  79,  0,   0,   14,  13,  38,  37,  3,   0,   45,  44,  110, 109, 386, 385, 380, 379, 374, 373, 829, 0,
            412, 411, 406, 405, 400, 399, 394, 393, 82,  81,  41,  0,   16,  15,  40,  39,  4,   0,   0,   46,  112, 111, 388, 387, 382, 381, 376, 375, 831, 830,
            414, 413, 408, 407, 402, 401, 396, 395, 84,  83,  42,  0,   0,   0,   0,   0,   6,   5,   48,  47,  114, 113, 390, 389, 384, 383, 378, 377, 832, 0,
            416, 415, 422, 421, 428, 427, 104, 103, 56,  55,  17,  0,   0,   0,   0,   0,   0,   0,   21,  20,  86,  85,  434, 433, 440, 439, 446, 445, 834, 833,
            418, 417, 424, 423, 430, 429, 106, 105, 58,  57,  0,   0,   0,   0,   0,   0,   0,   0,   23,  22,  88,  87,  436, 435, 442, 441, 448, 447, 835, 0,
            420, 419, 426, 425, 432, 431, 108, 107, 60,  59,  0,   0,   0,   0,   0,   0,   0,   0,   0,   24,  90,  89,  438, 437, 444, 443, 450, 449, 837, 836,
            482, 481, 476, 475, 470, 469, 49,  0,   31,  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   1,   54,  53,  464, 463, 458, 457, 452, 451, 838, 0,
            484, 483, 478, 477, 472, 471, 50,  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   466, 465, 460, 459, 454, 453, 840, 839,
            486, 485, 480, 479, 474, 473, 52,  51,  32,  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   2,   0,   43,  468, 467, 462, 461, 456, 455, 841, 0,
            488, 487, 494, 493, 500, 499, 98,  97,  62,  61,  0,   0,   0,   0,   0,   0,   0,   0,   0,   27,  92,  91,  506, 505, 512, 511, 518, 517, 843, 842,
            490, 489, 496, 495, 502, 501, 100, 99,  64,  63,  0,   0,   0,   0,   0,   0,   0,   0,   29,  28,  94,  93,  508, 507, 514, 513, 520, 519, 844, 0,
            492, 491, 498, 497, 504, 503, 102, 101, 66,  65,  18,  0,   0,   0,   0,   0,   0,   0,   19,  30,  96,  95,  510, 509, 516, 515, 522, 521, 846, 845,
            560, 559, 554, 553, 548, 547, 542, 541, 74,  73,  33,  0,   0,   0,   0,   0,   0,   11,  68,  67,  116, 115, 536, 535, 530, 529, 524, 523, 847, 0,
            562, 561, 556, 555, 550, 549, 544, 543, 76,  75,  0,   0,   8,   7,   36,  35,  12,  0,   70,  69,  118, 117, 538, 537, 532, 531, 526, 525, 849, 848,
            564, 563, 558, 557, 552, 551, 546, 545, 78,  77,  0,   34,  10,  9,   26,  25,  0,   0,   72,  71,  120, 119, 540, 539, 534, 533, 528, 527, 850, 0,
            566, 565, 572, 571, 578, 577, 584, 583, 590, 589, 596, 595, 602, 601, 608, 607, 614, 613, 620, 619, 626, 625, 632, 631, 638, 637, 644, 643, 852, 851,
            568, 567, 574, 573, 580, 579, 586, 585, 592, 591, 598, 597, 604, 603, 610, 609, 616, 615, 622, 621, 628, 627, 634, 633, 640, 639, 646, 645, 853, 0,
            570, 569, 576, 575, 582, 581, 588, 587, 594, 593, 600, 599, 606, 605, 612, 611, 618, 617, 624, 623, 630, 629, 636, 635, 642, 641, 648, 647, 855, 854,
            728, 727, 722, 721, 716, 715, 710, 709, 704, 703, 698, 697, 692, 691, 686, 685, 680, 679, 674, 673, 668, 667, 662, 661, 656, 655, 650, 649, 856, 0,
            730, 729, 724, 723, 718, 717, 712, 711, 706, 705, 700, 699, 694, 693, 688, 687, 682, 681, 676, 675, 670, 669, 664, 663, 658, 657, 652, 651, 858, 857,
            732, 731, 726, 725, 720, 719, 714, 713, 708, 707, 702, 701, 696, 695, 690, 689, 684, 683, 678, 677, 672, 671, 666, 665, 660, 659, 654, 653, 859, 0,
            734, 733, 740, 739, 746, 745, 752, 751, 758, 757, 764, 763, 770, 769, 776, 775, 782, 781, 788, 787, 794, 793, 800, 799, 806, 805, 812, 811, 861, 860,
            736, 735, 742, 741, 748, 747, 754, 753, 760, 759, 766, 765, 772, 771, 778, 777, 784, 783, 790, 789, 796, 795, 802, 801, 808, 807, 814, 813, 862, 0,
            738, 737, 744, 743, 750, 749, 756, 755, 762, 761, 768, 767, 774, 773, 780, 779, 786, 785, 792, 791, 798, 797, 804, 803, 810, 809, 816, 815, 864, 863
        };

        /**
         * ASCII character to Code Set mapping, from ISO/IEC 16023 Appendix A.
         * 1 = Set A, 2 = Set B, 3 = Set C, 4 = Set D, 5 = Set E.
         * 0 refers to special characters that fit into more than one set (e.g. GS).
         */
        static int[] MAXICODE_SET = {
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 5, 0, 2, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 2,
            2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 4, 5, 5, 5, 5, 5, 5, 4, 5, 3, 4, 3, 5, 5, 4, 4, 3, 3, 3,
            4, 3, 5, 4, 4, 3, 3, 4, 3, 3, 3, 4, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4
        };

        /** ASCII character to symbol value, from ISO/IEC 16023 Appendix A. */
        static int[] MAXICODE_SYMBOL_CHAR = {
            0,  1,  2,  3,  4,  5,  6,  7,  8,  9,  10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
            20, 21, 22, 23, 24, 25, 26, 30, 28, 29, 30, 35, 32, 53, 34, 35, 36, 37, 38, 39,
            40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 37,
            38, 39, 40, 41, 52, 1,  2,  3,  4,  5,  6,  7,  8,  9,  10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 42, 43, 44, 45, 46, 0,  1,  2,  3,
            4,  5,  6,  7,  8,  9,  10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23,
            24, 25, 26, 32, 54, 34, 35, 36, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 47, 48,
            49, 50, 51, 52, 53, 54, 55, 56, 57, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 36,
            37, 37, 38, 39, 40, 41, 42, 43, 38, 44, 37, 39, 38, 45, 46, 40, 41, 39, 40, 41,
            42, 42, 47, 43, 44, 43, 44, 45, 45, 46, 47, 46, 0,  1,  2,  3,  4,  5,  6,  7,
            8,  9,  10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 32,
            33, 34, 35, 36, 0,  1,  2,  3,  4,  5,  6,  7,  8,  9,  10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 32, 33, 34, 35, 36
        };

        int mode;
        int structuredAppendPosition = 1;
        int structuredAppendTotal = 1;
        string primaryData = "";
        int[] codewords;
        int[] source;
        int[] set = new int[144];
        int[] character = new int[144];
        bool[,] grid = new bool[33,30];

        /**
         * Sets the MaxiCode mode to use. Only modes 2 to 6 are supported.
         *
         * @param mode the MaxiCode mode to use
         */
        public void setMode(int mode)
        {
            if (mode < 2 || mode > 6)
            {
                throw new ArgumentOutOfRangeException("Invalid MaxiCode mode: " + mode);
            }
            this.mode = mode;
        }

        /**
         * Returns the MaxiCode mode being used. Only modes 2 to 6 are supported.
         *
         * @return the MaxiCode mode being used
         */
        public int getMode()
        {
            return mode;
        }

        /**
         * If this MaxiCode symbol is part of a series of MaxiCode symbols appended in a structured format, this method sets the
         * position of this symbol in the series. Valid values are 1 through 8 inclusive.
         *
         * @param position the position of this MaxiCode symbol in the structured append series
         */
        public void setStructuredAppendPosition(int position)
        {
            if (position < 1 || position > 8)
            {
                throw new ArgumentOutOfRangeException("Invalid MaxiCode structured append position: " + position);
            }
            this.structuredAppendPosition = position;
        }

        /**
         * Returns the position of this MaxiCode symbol in a series of symbols using structured append. If this symbol is not part of
         * such a series, this method will return <code>1</code>.
         *
         * @return the position of this MaxiCode symbol in a series of symbols using structured append
         */
        public int getStructuredAppendPosition()
        {
            return structuredAppendPosition;
        }

        /**
         * If this MaxiCode symbol is part of a series of MaxiCode symbols appended in a structured format, this method sets the total
         * number of symbols in the series. Valid values are 1 through 8 inclusive. A value of 1 indicates that this symbol is not
         * part of a structured append series.
         *
         * @param total the total number of MaxiCode symbols in the structured append series
         */
        public void setStructuredAppendTotal(int total)
        {
            if (total < 1 || total > 8)
            {
                throw new ArgumentOutOfRangeException("Invalid MaxiCode structured append total: " + total);
            }
            this.structuredAppendTotal = total;
        }

        /**
         * Returns the size of the series of MaxiCode symbols using structured append that this symbol is part of. If this symbol is
         * not part of a structured append series, this method will return <code>1</code>.
         *
         * @return size of the series that this symbol is part of
         */
        public int getStructuredAppendTotal()
        {
            return structuredAppendTotal;
        }

        /**
         * Sets the primary data. Should only be used for modes 2 and 3. Must conform to the following structure:
         *
         * <table summary="Expected primary data structure.">
         *   <tr><th>Characters</th><th>Meaning</th></tr>
         *   <tr><td>1-9</td><td>Postal code data which can consist of up to 9 digits (for mode 2) or up to 6
         *                       alphanumeric characters (for mode 3). Remaining unused characters should be
         *                       filled with the SPACE character (ASCII 32).</td></tr>
         *   <tr><td>10-12</td><td>Three-digit country code according to ISO-3166.</td></tr>
         *   <tr><td>13-15</td><td>Three digit service code. This depends on your parcel courier.</td></tr>
         * </table>
         *
         * @param primary the primary data
         */
        public void setPrimary(String primary)
        {
            primaryData = primary;
        }

        /**
         * Returns the primary data for this MaxiCode symbol. Should only be used for modes 2 and 3.
         *
         * @return the primary data for this MaxiCode symbol
         */
        public String getPrimary()
        {
            return primaryData;
        }

        /** {@inheritDoc} */
        //@Override
        public bool encode()
        {
            inputBytes = Encoding.UTF8.GetBytes(text);

            // copy input data over into source
            int sourcelen = text.Length;
            source = new int[sourcelen];

            //eciProcess();

            for (int i = 0; i < sourcelen; i++)
            {
                source[i] = inputBytes[i] & 0xFF;
            }

            // mode 2 -> mode 3 if postal code isn't strictly numeric
            if (mode == 2)
            {
                for (int i = 0; i < 10 && i < primaryData.Length; i++)
                {
                    if ((primaryData[i] < '0') || (primaryData[i] > '9'))
                    {
                        mode = 3;
                        break;
                    }
                }
            }

            // initialize the set and character arrays
            if (!processText())
            {
                //error_msg = "Input data too long";
                //return false;
                throw new Exception("Input data too long");
            }

            // start building the codeword array, starting with a copy of the character data
            // insert primary message if this is a structured carrier message; insert mode otherwise
            codewords = new int[character.Length];
            character.CopyTo(codewords, 0);
            if (mode == 2 || mode == 3)
            {
                int[] _primary = getPrimaryCodewords();
                if (_primary == null)
                {
                    return false;
                }
                codewords = insert(codewords, 0, _primary);
            }
            else
            {
                codewords = insert(codewords, 0, new int[] { mode });
            }

            // insert structured append flag if necessary
            if (structuredAppendTotal > 1)
            {

                int[] flag = new int[2];
                flag[0] = 33; // padding
                flag[1] = ((structuredAppendPosition - 1) << 3) | (structuredAppendTotal - 1); // position + total

                int index;
                if (mode == 2 || mode == 3)
                {
                    index = 10; // first two data symbols in the secondary message
                }
                else
                {
                    index = 1; // first two data symbols in the primary message (first symbol at index 0 isn't a data symbol)
                }

                codewords = insert(codewords, index, flag);
            }

            int secondaryMax, secondaryECMax;
            if (mode == 5)
            {
                // 68 data codewords, 56 error corrections in secondary message
                secondaryMax = 68;
                secondaryECMax = 56;
            }
            else
            {
                // 84 data codewords, 40 error corrections in secondary message
                secondaryMax = 84;
                secondaryECMax = 40;
            }

            // truncate data codewords to maximum data space available
            int totalMax = secondaryMax + 10;
            if (codewords.Length > totalMax)
            {
                //codewords = codewords.Take(totalMax).ToArray();
                int[] __codewords = new int[totalMax];
                Array.Copy(codewords, __codewords, totalMax);
                codewords = __codewords;
            }

            // insert primary error correction between primary message and secondary message (always EEC)
            //int[] primary = codewords.Take(10).ToArray();
            int[] primary = new int[10];
            Array.Copy(codewords, 0, primary, 0, 10);
            int[] primaryCheck = getErrorCorrection(primary, 10);
            codewords = insert(codewords, 10, primaryCheck);

            // calculate secondary error correction
            //int[] secondary = codewords.Skip(20).ToArray();
            int[] secondary = new int[codewords.Length - 20];
            Array.Copy(codewords, 20, secondary, 0, secondary.Length);
            int[] secondaryOdd = new int[secondary.Length / 2];
            int[] secondaryEven = new int[secondary.Length / 2];
            for (int i = 0; i < secondary.Length; i++)
            {
                if ((i & 1) != 0)
                { // odd
                    secondaryOdd[(i - 1) / 2] = secondary[i];
                }
                else
                { // even
                    secondaryEven[i / 2] = secondary[i];
                }
            }
            int[] secondaryECOdd = getErrorCorrection(secondaryOdd, secondaryECMax / 2);
            int[] secondaryECEven = getErrorCorrection(secondaryEven, secondaryECMax / 2);

            // add secondary error correction after secondary message
            int[] _codewords = codewords;
            codewords = new int[codewords.Length + secondaryECOdd.Length + secondaryECEven.Length];
            Array.Copy(_codewords, codewords, _codewords.Length);
            //codewords = Arrays.copyOf(codewords, codewords.Length + secondaryECOdd.Length + secondaryECEven.Length);
            for (int i = 0; i < secondaryECOdd.Length; i++)
            {
                codewords[20 + secondaryMax + (2 * i) + 1] = secondaryECOdd[i];
            }
            for (int i = 0; i < secondaryECEven.Length; i++)
            {
                codewords[20 + secondaryMax + (2 * i)] = secondaryECEven[i];
            }

            //encodeInfo += "Mode: " + mode + "\n";
            //encodeInfo += "ECC Codewords: " + secondaryECMax + "\n";
            //encodeInfo += "Codewords: ";
            //for (int i = 0; i < codewords.Length; i++)
            //{
            //    encodeInfo += Integer.toString(codewords[i]) + " ";
            //}
            //encodeInfo += "\n";

            // copy data into symbol grid
            int[] bit_pattern = new int[7];
            for (int i = 0; i < 33; i++)
            {
                for (int j = 0; j < 30; j++)
                {

                    int block = (MAXICODE_GRID[(i * 30) + j] + 5) / 6;
                    int bit = (MAXICODE_GRID[(i * 30) + j] + 5) % 6;

                    if (block != 0)
                    {

                        bit_pattern[0] = (codewords[block - 1] & 0x20) >> 5;
                        bit_pattern[1] = (codewords[block - 1] & 0x10) >> 4;
                        bit_pattern[2] = (codewords[block - 1] & 0x8) >> 3;
                        bit_pattern[3] = (codewords[block - 1] & 0x4) >> 2;
                        bit_pattern[4] = (codewords[block - 1] & 0x2) >> 1;
                        bit_pattern[5] = (codewords[block - 1] & 0x1);

                        if (bit_pattern[bit] != 0)
                        {
                            grid[i,j] = true;
                        }
                        else
                        {
                            grid[i,j] = false;
                        }
                    }
                }
            }

            // add orientation markings
            grid[0,28] = true;  // top right filler
            grid[0, 29] = true;
            grid[9, 10] = true;  // top left marker
            grid[9, 11] = true;
            grid[10, 11] = true;
            grid[15, 7] = true;  // left hand marker
            grid[16, 8] = true;
            grid[16, 20] = true; // right hand marker
            grid[17, 20] = true;
            grid[22, 10] = true; // bottom left marker
            grid[23, 10] = true;
            grid[22, 17] = true; // bottom right marker
            grid[23, 17] = true;

            // the following is provided for compatibility, but the results are not useful
            //row_count = 33;
            //readable = "";
            //pattern = new String[33];
            //row_height = new int[33];
            //for (int i = 0; i < 33; i++)
            //{
            //    StringBuilder bin = new StringBuilder(30);
            //    for (int j = 0; j < 30; j++)
            //    {
            //        if (grid[i][j])
            //        {
            //            bin.append("1");
            //        }
            //        else
            //        {
            //            bin.append("0");
            //        }
            //    }
            //    pattern[i] = bin2pat(bin.toString());
            //    row_height[i] = 1;
            //}
            //symbol_height = 72;
            //symbol_width = 74;

            plotSymbol();

            return true;
        }

        /**
         * Extracts the postal code, country code and service code from the primary data and returns the corresponding primary message
         * codewords.
         *
         * @return the primary message codewords
         */
        int[] getPrimaryCodewords()
        {
            //assert mode == 2 || mode == 3;

            if (primaryData.Length != 15)
            {
                //error_msg = "Invalid Primary String";
                //return null;
                throw new Exception("Invalid Primary String");
            }

            for (int i = 9; i < 15; i++)
            { /* check that country code and service are numeric */
                if ((primaryData[i] < '0') || (primaryData[i] > '9'))
                {
                    //error_msg = "Invalid Primary String";
                    //return null;
                    throw new Exception("Invalid Primary String");
                }
            }

            String postcode;
            if (mode == 2)
            {
                postcode = primaryData.Substring(0, 9);
                int index = postcode.IndexOf(' ');
                if (index != -1)
                {
                    postcode = postcode.Substring(0, index);
                }
            }
            else
            {
                // if (mode == 3)
                postcode = primaryData.Substring(0, 6);
            }

            int country = int.Parse(primaryData.Substring(9, 3));
            int service = int.Parse(primaryData.Substring(12, 3));

            //if (debug)
            //{
            //    System.out.println("Using mode " + mode);
            //    System.out.println("     Postcode: " + postcode);
            //    System.out.println("     Country Code: " + country);
            //    System.out.println("     Service: " + service);
            //}

            if (mode == 2)
            {
                return getMode2PrimaryCodewords(postcode, country, service);
            }
            else
            { // mode == 3
                return getMode3PrimaryCodewords(postcode, country, service);
            }
        }

        /**
         * Returns the primary message codewords for mode 2.
         *
         * @param postcode the postal code
         * @param country the country code
         * @param service the service code
         * @return the primary message, as codewords
         */
        static int[] getMode2PrimaryCodewords(String postcode, int country, int service)
        {

            for (int i = 0; i < postcode.Length; i++)
            {
                if (postcode[i] < '0' || postcode[i] > '9')
                {
                    postcode = postcode.Substring(0, i);
                    break;
                }
            }

            int postcodeNum = int.Parse(postcode);

            int[] primary = new int[10];
            primary[0] = ((postcodeNum & 0x03) << 4) | 2;
            primary[1] = ((postcodeNum & 0xfc) >> 2);
            primary[2] = ((postcodeNum & 0x3f00) >> 8);
            primary[3] = ((postcodeNum & 0xfc000) >> 14);
            primary[4] = ((postcodeNum & 0x3f00000) >> 20);
            primary[5] = ((postcodeNum & 0x3c000000) >> 26) | ((postcode.Length & 0x3) << 4);
            primary[6] = ((postcode.Length & 0x3c) >> 2) | ((country & 0x3) << 4);
            primary[7] = (country & 0xfc) >> 2;
            primary[8] = ((country & 0x300) >> 8) | ((service & 0xf) << 2);
            primary[9] = ((service & 0x3f0) >> 4);

            return primary;
        }

        /**
         * Returns the primary message codewords for mode 3.
         *
         * @param postcode the postal code
         * @param country the country code
         * @param service the service code
         * @return the primary message, as codewords
         */
        static int[] getMode3PrimaryCodewords(String postcode, int country, int service)
        {

            int[] postcodeNums = new int[postcode.Length];

            postcode = postcode.ToUpper();
            for (int i = 0; i < postcodeNums.Length; i++)
            {
                postcodeNums[i] = postcode[i];
                if (postcode[i] >= 'A' && postcode[i] <= 'Z')
                {
                    // (Capital) letters shifted to Code Set A values
                    postcodeNums[i] -= 64;
                }
                if (postcodeNums[i] == 27 || postcodeNums[i] == 31 || postcodeNums[i] == 33 || postcodeNums[i] >= 59)
                {
                    // Not a valid postal code character, use space instead
                    postcodeNums[i] = 32;
                }
                // Input characters lower than 27 (NUL - SUB) in postal code are interpreted as capital
                // letters in Code Set A (e.g. LF becomes 'J')
            }

            int[] primary = new int[10];
            primary[0] = ((postcodeNums[5] & 0x03) << 4) | 3;
            primary[1] = ((postcodeNums[4] & 0x03) << 4) | ((postcodeNums[5] & 0x3c) >> 2);
            primary[2] = ((postcodeNums[3] & 0x03) << 4) | ((postcodeNums[4] & 0x3c) >> 2);
            primary[3] = ((postcodeNums[2] & 0x03) << 4) | ((postcodeNums[3] & 0x3c) >> 2);
            primary[4] = ((postcodeNums[1] & 0x03) << 4) | ((postcodeNums[2] & 0x3c) >> 2);
            primary[5] = ((postcodeNums[0] & 0x03) << 4) | ((postcodeNums[1] & 0x3c) >> 2);
            primary[6] = ((postcodeNums[0] & 0x3c) >> 2) | ((country & 0x3) << 4);
            primary[7] = (country & 0xfc) >> 2;
            primary[8] = ((country & 0x300) >> 8) | ((service & 0xf) << 2);
            primary[9] = ((service & 0x3f0) >> 4);

            return primary;
        }

        /**
         * Formats text according to Appendix A, populating the {@link #set} and {@link #character} arrays.
         *
         * @return true if the content fits in this symbol and was formatted; false otherwise
         */
        bool processText()
        {

            int Length = text.Length;
            int i, j, count, current_set;

            if (Length > 138)
            {
                return false;
            }

            for (i = 0; i < 144; i++)
            {
                set[i] = -1;
                character[i] = 0;
            }

            for (i = 0; i < Length; i++)
            {
                /* Look up characters in table from Appendix A - this gives
                 value and code set for most characters */
                set[i] = MAXICODE_SET[source[i]];
                character[i] = MAXICODE_SYMBOL_CHAR[source[i]];
            }

            // If a character can be represented in more than one code set, pick which version to use.
            if (set[0] == 0)
            {
                if (character[0] == 13)
                {
                    character[0] = 0;
                }
                set[0] = 1;
            }

            for (i = 1; i < Length; i++)
            {
                if (set[i] == 0)
                {
                    /* Special character that can be represented in more than one code set. */
                    if (character[i] == 13)
                    {
                        /* Carriage Return */
                        set[i] = bestSurroundingSet(i, Length, 1, 5);
                        if (set[i] == 5)
                        {
                            character[i] = 13;
                        }
                        else
                        {
                            character[i] = 0;
                        }
                    }
                    else if (character[i] == 28)
                    {
                        /* FS */
                        set[i] = bestSurroundingSet(i, Length, 1, 2, 3, 4, 5);
                        if (set[i] == 5)
                        {
                            character[i] = 32;
                        }
                    }
                    else if (character[i] == 29)
                    {
                        /* GS */
                        set[i] = bestSurroundingSet(i, Length, 1, 2, 3, 4, 5);
                        if (set[i] == 5)
                        {
                            character[i] = 33;
                        }
                    }
                    else if (character[i] == 30)
                    {
                        /* RS */
                        set[i] = bestSurroundingSet(i, Length, 1, 2, 3, 4, 5);
                        if (set[i] == 5)
                        {
                            character[i] = 34;
                        }
                    }
                    else if (character[i] == 32)
                    {
                        /* Space */
                        set[i] = bestSurroundingSet(i, Length, 1, 2, 3, 4, 5);
                        if (set[i] == 1)
                        {
                            character[i] = 32;
                        }
                        else if (set[i] == 2)
                        {
                            character[i] = 47;
                        }
                        else
                        {
                            character[i] = 59;
                        }
                    }
                    else if (character[i] == 44)
                    {
                        /* Comma */
                        set[i] = bestSurroundingSet(i, Length, 1, 2);
                        if (set[i] == 2)
                        {
                            character[i] = 48;
                        }
                    }
                    else if (character[i] == 46)
                    {
                        /* Full Stop */
                        set[i] = bestSurroundingSet(i, Length, 1, 2);
                        if (set[i] == 2)
                        {
                            character[i] = 49;
                        }
                    }
                    else if (character[i] == 47)
                    {
                        /* Slash */
                        set[i] = bestSurroundingSet(i, Length, 1, 2);
                        if (set[i] == 2)
                        {
                            character[i] = 50;
                        }
                    }
                    else if (character[i] == 58)
                    {
                        /* Colon */
                        set[i] = bestSurroundingSet(i, Length, 1, 2);
                        if (set[i] == 2)
                        {
                            character[i] = 51;
                        }
                    }
                }
            }

            for (i = Length; i < set.Length; i++)
            {
                /* Add the padding */
                if (set[Length - 1] == 2)
                {
                    set[i] = 2;
                }
                else
                {
                    set[i] = 1;
                }
                character[i] = 33;
            }

            /* Find candidates for number compression (not allowed in primary message in modes 2 and 3). */
            if (mode == 2 || mode == 3)
            {
                j = 9;
            }
            else
            {
                j = 0;
            }
            count = 0;
            for (i = j; i < 143; i++)
            {
                if (set[i] == 1 && character[i] >= 48 && character[i] <= 57)
                {
                    /* Character is a number */
                    count++;
                }
                else
                {
                    count = 0;
                }
                if (count == 9)
                {
                    /* Nine digits in a row can be compressed */
                    set[i] = 6;
                    set[i - 1] = 6;
                    set[i - 2] = 6;
                    set[i - 3] = 6;
                    set[i - 4] = 6;
                    set[i - 5] = 6;
                    set[i - 6] = 6;
                    set[i - 7] = 6;
                    set[i - 8] = 6;
                    count = 0;
                }
            }

            /* Add shift and latch characters */
            current_set = 1;
            i = 0;
            do
            {
                if ((set[i] != current_set) && (set[i] != 6))
                {
                    switch (set[i])
                    {
                        case 1:
                            if (i + 1 < set.Length && set[i + 1] == 1)
                            {
                                if (i + 2 < set.Length && set[i + 2] == 1)
                                {
                                    if (i + 3 < set.Length && set[i + 3] == 1)
                                    {
                                        /* Latch A */
                                        insert(i, 63);
                                        current_set = 1;
                                        Length++;
                                        i += 3;
                                    }
                                    else
                                    {
                                        /* 3 Shift A */
                                        insert(i, 57);
                                        Length++;
                                        i += 2;
                                    }
                                }
                                else
                                {
                                    /* 2 Shift A */
                                    insert(i, 56);
                                    Length++;
                                    i++;
                                }
                            }
                            else
                            {
                                /* Shift A */
                                insert(i, 59);
                                Length++;
                            }
                            break;
                        case 2:
                            if (i + 1 < set.Length && set[i + 1] == 2)
                            {
                                /* Latch B */
                                insert(i, 63);
                                current_set = 2;
                                Length++;
                                i++;
                            }
                            else
                            {
                                /* Shift B */
                                insert(i, 59);
                                Length++;
                            }
                            break;
                        case 3:
                            if (i + 3 < set.Length && set[i + 1] == 3 && set[i + 2] == 3 && set[i + 3] == 3)
                            {
                                /* Lock In C */
                                insert(i, 60);
                                insert(i, 60);
                                current_set = 3;
                                Length++;
                                i += 3;
                            }
                            else
                            {
                                /* Shift C */
                                insert(i, 60);
                                Length++;
                            }
                            break;
                        case 4:
                            if (i + 3 < set.Length && set[i + 1] == 4 && set[i + 2] == 4 && set[i + 3] == 4)
                            {
                                /* Lock In D */
                                insert(i, 61);
                                insert(i, 61);
                                current_set = 4;
                                Length++;
                                i += 3;
                            }
                            else
                            {
                                /* Shift D */
                                insert(i, 61);
                                Length++;
                            }
                            break;
                        case 5:
                            if (i + 3 < set.Length && set[i + 1] == 5 && set[i + 2] == 5 && set[i + 3] == 5)
                            {
                                /* Lock In E */
                                insert(i, 62);
                                insert(i, 62);
                                current_set = 5;
                                Length++;
                                i += 3;
                            }
                            else
                            {
                                /* Shift E */
                                insert(i, 62);
                                Length++;
                            }
                            break;
                        default:
                            throw new Exception("Unexpected set " + set[i] + " at index " + i + ".");
                    }
                    i++;
                }
                i++;
            } while (i < set.Length);

            /* Number compression has not been forgotten! It's handled below. */
            i = 0;
            do
            {
                if (set[i] == 6)
                {
                    /* Number compression */
                    int value = 0;
                    for (j = 0; j < 9; j++)
                    {
                        value *= 10;
                        value += (character[i + j] - '0');
                    }
                    character[i] = 31; /* NS */
                    character[i + 1] = (value & 0x3f000000) >> 24;
                    character[i + 2] = (value & 0xfc0000) >> 18;
                    character[i + 3] = (value & 0x3f000) >> 12;
                    character[i + 4] = (value & 0xfc0) >> 6;
                    character[i + 5] = (value & 0x3f);
                    i += 6;
                    for (j = i; j < 140; j++)
                    {
                        set[j] = set[j + 3];
                        character[j] = character[j + 3];
                    }
                    Length -= 3;
                }
                else
                {
                    i++;
                }
            } while (i < set.Length);

            /* Inject ECI codes to beginning of data, according to Table 3 */
            if (eciMode != 3)
            {
                insert(0, 27); // ECI

                if ((eciMode >= 0) && (eciMode <= 31))
                {
                    insert(1, eciMode & 0x1F);
                    Length += 2;
                }

                if ((eciMode >= 32) && (eciMode <= 1023))
                {
                    insert(1, 0x20 + (eciMode >> 6));
                    insert(2, eciMode & 0x3F);
                    Length += 3;
                }

                if ((eciMode >= 1024) && (eciMode <= 32767))
                {
                    insert(1, 0x30 + (eciMode >> 12));
                    insert(2, (eciMode >> 6) & 0x3F);
                    insert(3, eciMode & 0x3F);
                    Length += 4;
                }

                if ((eciMode >= 32768) && (eciMode <= 999999))
                {
                    insert(1, 0x38 + (eciMode >> 18));
                    insert(2, (eciMode >> 12) & 0x3F);
                    insert(3, (eciMode >> 6) & 0x3F);
                    insert(4, eciMode & 0x3F);
                    Length += 5;
                }
            }

            /* Make sure we haven't exceeded the maximum data Length. */
            if ((mode == 2 || mode == 3) && Length > 84)
            {
                return false;
            }
            if ((mode == 4 || mode == 6) && Length > 93)
            {
                return false;
            }
            if (mode == 5 && Length > 77)
            {
                return false;
            }

            return true;
        }

        /**
         * Guesses the best set to use at the specified index by looking at the surrounding sets. In general, characters in
         * lower-numbered sets are more common, so we choose them if we can. If no good surrounding sets can be found, the default
         * value returned is the first value from the valid set.
         *
         * @param index the current index
         * @param Length the maximum Length to look at
         * @param valid the valid sets for this index
         * @return the best set to use at the specified index
         */
        int bestSurroundingSet(int index, int Length, params int[] valid)
        {
            int option1 = set[index - 1];
            if (index + 1 < Length)
            {
                // we have two options to check
                int option2 = set[index + 1];
                if (contains(valid, option1) && contains(valid, option2))
                {
                    return Math.Min(option1, option2);
                }
                else if (contains(valid, option1))
                {
                    return option1;
                }
                else if (contains(valid, option2))
                {
                    return option2;
                }
                else
                {
                    return valid[0];
                }
            }
            else
            {
                // we only have one option to check
                if (contains(valid, option1))
                {
                    return option1;
                }
                else
                {
                    return valid[0];
                }
            }
        }

        /**
         * Moves everything up so that the specified shift or latch character can be inserted.
         *
         * @param position the position beyond which everything needs to be shifted
         * @param c the latch or shift character to insert at the specified position, after everything has been shifted
         */
        void insert(int position, int c)
        {
            for (int i = 143; i > position; i--)
            {
                set[i] = set[i - 1];
                character[i] = character[i - 1];
            }
            character[position] = c;
        }

        /**
         * Returns the error correction codewords for the specified data codewords.
         *
         * @param codewords the codewords that we need error correction codewords for
         * @param ecclen the number of error correction codewords needed
         * @return the error correction codewords for the specified data codewords
         */
        static int[] getErrorCorrection(int[] codewords, int ecclen)
        {

            ReedSolomon rs = new ReedSolomon();
            rs.init_gf(0x43);
            rs.init_code(ecclen, 1);
            rs.encode(codewords.Length, codewords);

            int[] results = new int[ecclen];
            for (int i = 0; i < ecclen; i++)
            {
                results[i] = rs.getResult(results.Length - 1 - i);
            }

            return results;
        }

        /** {@inheritDoc} */
        //@Override
        protected void plotSymbol()
        {
            // hexagons
            for (int row = 0; row < 33; row++)
            {
                for (int col = 0; col < 30; col++)
                {
                    if (grid[row,col])
                    {
                        double x = (2.46 * col) + 1.23;
                        if ((row & 1) != 0)
                        {
                            x += 1.23;
                        }
                        double y = (2.135 * row) + 1.43;
                        hexagons.Add(new Hexagon(x, y));
                    }
                }
            }

            // circles
            //double[] radii = { 10.85, 8.97, 7.10, 5.22, 3.31, 1.43 };
            double[] radii = { 9.91, 6.16, 2.37 };
            for (int i = 0; i < radii.Length; i++)
            {
                target.Add(new Ellipse(
                    35.76 - radii[i],
                    35.60 - radii[i],
                    35.76 + radii[i],
                    35.60 + radii[i]));
            }
        }

        /** {@inheritDoc} */
        //@Override
        protected int[] getCodewords()
        {
            return codewords;
        }

        //========================================================================

        int[] insert(int[] original, int index, int[] inserted)
        {
            int[] modified = new int[original.Length + inserted.Length];
            Array.Copy(original, 0, modified, 0, index);
            Array.Copy(inserted, 0, modified, index, inserted.Length);
            Array.Copy(original, index, modified, index + inserted.Length, modified.Length - index - inserted.Length);
            return modified;
        }

        bool contains(int[] values, int value)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == value)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
