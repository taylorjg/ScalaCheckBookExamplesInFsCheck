using System;
using System.Collections.Generic;
using System.Linq;

namespace ScalaCheckBookExamplesInFsCheck.Chapter4.RunLengthEncoding
{
    internal static class RunLengthEncoding
    {
        public static IEnumerable<Tuple<int, T>> RunLengthEnc<T>(IEnumerable<T> xs)
        {
            var defaultEqualityComparer = EqualityComparer<T>.Default;
            var currTuple = null as Tuple<int, T>;

            using (var e = xs.GetEnumerator())
            {
                for (;;)
                {
                    if (!e.MoveNext())
                    {
                        if (currTuple != null) yield return currTuple;
                        yield break;
                    }

                    if (currTuple == null)
                    {
                        currTuple = Tuple.Create(1, e.Current);
                        continue;
                    }

                    if (defaultEqualityComparer.Equals(e.Current, currTuple.Item2))
                    {
                        currTuple = Tuple.Create(currTuple.Item1 + 1, currTuple.Item2);
                        continue;
                    }

                    System.Diagnostics.Debug.Assert(currTuple.Item1 >= 1);
                    yield return currTuple;

                    currTuple = Tuple.Create(1, e.Current);
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
