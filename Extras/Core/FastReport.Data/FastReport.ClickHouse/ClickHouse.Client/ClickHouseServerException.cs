using System;
using System.Data.Common;
using System.Runtime.Serialization;

namespace ClickHouse.Client
{
    /// <summary>
    /// Exception class representing error which happened on ClickHouse server
    /// </summary>
    [Serializable]
    public class ClickHouseServerException : DbException
    {
        public ClickHouseServerException()
        {
        }

        public ClickHouseServerException(string error, string query, int errorCode)
            : base(error, errorCode)
        {
            Query = query;
        }

        public string Query { get; } = null;

        public static ClickHouseServerException FromServerResponse(string error, string query)
        {
            var errorCode = ParseErrorCode(error) ?? -1;
            return new ClickHouseServerException(error, query, errorCode);
        }

        protected ClickHouseServerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static int? ParseErrorCode(string error)
        {
            int start = -1;
            int end = error.Length - 1;

            for (int i = 0; i < error.Length; i++)
            {
                if (char.IsDigit(error[i]))
                {
                    start = i;
                    break;
                }
            }

            if (start == -1)
            {
                return null;
            }

            for (int i = start; i < error.Length; i++)
            {
                if (!char.IsDigit(error[i]))
                {
                    end = i;
                    break;
                }
            }

            return int.TryParse(error.Substring(start, end - start), out int result) ? result : (int?)null;
        }
    }
}
