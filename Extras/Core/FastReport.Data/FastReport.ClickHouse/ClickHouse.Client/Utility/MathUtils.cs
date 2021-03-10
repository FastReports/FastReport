using System;

namespace ClickHouse.Client.Utility
{
    public static class MathUtils
    {
        public static long ToPower(int value, int power)
        {
            long result = 1;
            while (power > 0)
            {
                if ((power & 1) == 1)
                {
                    result *= value;
                }

                power >>= 1;
                if (power <= 0)
                {
                    break;
                }

                value *= value;
            }
            return result;
        }

        public static long ShiftDecimalPlaces(long value, int places)
        {
            if (places == 0)
            {
                return value;
            }

            var factor = ToPower(10, Math.Abs(places));
            return places < 0 ? value / factor : value * factor;
        }

        public static decimal ShiftDecimalPlaces(decimal value, int places)
        {
            if (places == 0)
            {
                return value;
            }

            var factor = ToPower(10, Math.Abs(places));
            return places < 0 ? value / factor : value * factor;
        }
    }
}
