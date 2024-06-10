using System;
using System.Collections.Generic;

namespace Ametrin.Command
{
    public static class Extensions
    {
        public static bool StartsWith(this string str, ReadOnlySpan<char> value)
        {
            if (value.Length > str.Length)
            {
                return false;
            }

            return str.AsSpan(0, value.Length).SequenceEqual(value);
        }

        public static List<Range> Split(this ReadOnlySpan<char> span, char delimiter)
        {
            var start = 0;
            var result = new List<Range>();
            for (int i = 0; i < span.Length; i++)
            {
                if (span[i] == delimiter)
                {
                    if (i > start) result.Add(new Range(start, i));

                    start = i + 1;
                }
            }

            if (span.Length > start) result.Add(new Range(start, span.Length));
            return result;
        }
    }
}
