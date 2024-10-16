using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;


namespace FastReport.Data
{
    internal static class PostgresTypesParsers
    {
        static readonly Regex pointRegex = new Regex(@"\((-?\d+.?\d*),(-?\d+.?\d*)\)", RegexOptions.Compiled);
        static readonly Regex lineRegex = new Regex(@"\{(-?\d+.?\d*),(-?\d+.?\d*),(-?\d+.?\d*)\}", RegexOptions.Compiled);
        static readonly Regex lSegRegex = new Regex(@"\[\((-?\d+.?\d*),(-?\d+.?\d*)\),\((-?\d+.?\d*),(-?\d+.?\d*)\)\]", RegexOptions.Compiled);
        static readonly Regex boxRegex = new Regex(@"\((-?\d+.?\d*),(-?\d+.?\d*)\),\((-?\d+.?\d*),(-?\d+.?\d*)\)", RegexOptions.Compiled);
        static readonly Regex circleRegex = new Regex(@"<\((-?\d+.?\d*),(-?\d+.?\d*)\),(\d+.?\d*)>", RegexOptions.Compiled);

        public static NpgsqlPoint ParsePoint(string s)
        {
            var m = pointRegex.Match(s);
            if (!m.Success)
            {
                throw new FormatException("Not a valid point: " + s);
            }
            return new NpgsqlPoint(double.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                double.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat));
        }

        public static NpgsqlLine ParseLine(string s)
        {
            var m = lineRegex.Match(s);
            if (!m.Success)
                throw new FormatException("Not a valid line: " + s);
            return new NpgsqlLine(
                double.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                double.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                double.Parse(m.Groups[3].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)
            );
        }

        public static NpgsqlLSeg ParseLSeg(string s)
        {
            var m = lSegRegex.Match(s);
            if (!m.Success)
            {
                throw new FormatException("Not a valid line: " + s);
            }
            return new NpgsqlLSeg(
                double.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                double.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                double.Parse(m.Groups[3].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                double.Parse(m.Groups[4].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)
            );

        }

        public static NpgsqlBox ParseBox(string s)
        {
            var m = boxRegex.Match(s);
            return new NpgsqlBox(
                new NpgsqlPoint(double.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                    double.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)),
                new NpgsqlPoint(double.Parse(m.Groups[3].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                    double.Parse(m.Groups[4].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat))
            );
        }

        public static NpgsqlPath ParsePath(string s)
        {
            if (s[0] != '[' && s[0] != '(')
                throw new Exception("Invalid path string: " + s);

            var open = s[0] == '[';

            var result = new NpgsqlPath(open);
            var i = 1;
            while (true)
            {
                var i2 = s.IndexOf(')', i);
                result.Add(ParsePoint(s.Substring(i, i2 - i + 1)));
                if (s[i2 + 1] != ',')
                    break;
                i = i2 + 2;
            }
            return result;
        }

        public static NpgsqlPolygon ParsePolygon(string s)
        {
            var points = new List<NpgsqlPoint>();
            var i = 1;
            while (true)
            {
                var i2 = s.IndexOf(')', i);
                points.Add(ParsePoint(s.Substring(i, i2 - i + 1)));
                if (s[i2 + 1] != ',')
                    break;
                i = i2 + 2;
            }
            return new NpgsqlPolygon(points);
        }

        public static NpgsqlCircle ParseCircle(string s)
        {
            var m = circleRegex.Match(s);
            if (!m.Success)
                throw new FormatException("Not a valid circle: " + s);

            return new NpgsqlCircle(
                double.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                double.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                double.Parse(m.Groups[3].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)
            );
        }

    }
}
