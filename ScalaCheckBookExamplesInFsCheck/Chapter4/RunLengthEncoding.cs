using System;
using System.Collections.Generic;
using System.Linq;

namespace ScalaCheckBookExamplesInFsCheck.Chapter4
{
    internal static class RunLengthEncoding
    {
        public static IEnumerable<Tuple<int, T>> RunLengthEnc<T>(IEnumerable<T> xs)
        {
            var previousIsValid = false;
            var previous = default(T);
            var previousRepeatCount = 0;

            using (var e = xs.GetEnumerator())
            {
                for (;;)
                {
                    if (!e.MoveNext())
                    {
                        yield return Tuple.Create(previousRepeatCount, previous);
                        yield break;
                    }

                    if (!previousIsValid)
                    {
                        previous = e.Current;
                        previousRepeatCount = 1;
                        previousIsValid = true;
                    }
                    else
                    {
                        if (EqualityComparer<T>.Default.Equals(e.Current, previous))
                        {
                            previousRepeatCount++;
                        }
                        else
                        {
                            yield return Tuple.Create(previousRepeatCount, previous);
                            previous = e.Current;
                            previousRepeatCount = 1;
                        }
                    }
                }
            }
        }

        public static IEnumerable<T> RunLengthDec<T>(IEnumerable<Tuple<int, T>> r)
        {
            return r.SelectMany(tuple =>
            {
                var n = tuple.Item1;
                var x = tuple.Item2;
                return Enumerable.Repeat(x, n);
            });
        }
    }
}
