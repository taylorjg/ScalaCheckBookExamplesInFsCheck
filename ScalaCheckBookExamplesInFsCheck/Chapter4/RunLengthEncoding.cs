using System;
using System.Collections.Generic;
using System.Linq;

namespace ScalaCheckBookExamplesInFsCheck.Chapter4
{
    internal static class RunLengthEncoding
    {
        public static IEnumerable<Tuple<int, T>> RunLengthEnc<T>(IEnumerable<T> xs)
        {
            var firstTimeThough = true;
            var previousElement = default(T);
            var previousElementRepeatCount = 0;
            var defaultEqualityComparer = EqualityComparer<T>.Default;

            using (var e = xs.GetEnumerator())
            {
                for (;;)
                {
                    if (!e.MoveNext())
                    {
                        if (previousElementRepeatCount > 0) yield return Tuple.Create(previousElementRepeatCount, previousElement);
                        yield break;
                    }

                    if (firstTimeThough)
                    {
                        previousElement = e.Current;
                        previousElementRepeatCount = 1;
                        firstTimeThough = false;
                        continue;
                    }

                    if (defaultEqualityComparer.Equals(e.Current, previousElement))
                    {
                        previousElementRepeatCount++;
                        continue;
                    }

                    System.Diagnostics.Debug.Assert(previousElementRepeatCount >= 1);
                    yield return Tuple.Create(previousElementRepeatCount, previousElement);

                    previousElement = e.Current;
                    previousElementRepeatCount = 1;
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
