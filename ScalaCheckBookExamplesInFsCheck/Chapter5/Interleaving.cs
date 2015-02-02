using System.Collections.Generic;
using System.Linq;

namespace ScalaCheckBookExamplesInFsCheck.Chapter5
{
    internal static class Interleaving
    {
        public static IList<T> Interleave<T>(IList<T> xs, IList<T> ys)
        {
            if (xs.Count == 0) return ys;
            if (ys.Count == 0) return xs;
            return new[] {xs.First()}
                .Concat(new[] {ys.First()})
                .Concat(Interleave(xs.Skip(1).ToList(), ys.Skip(1).ToList()))
                .ToList();
        }
    }
}
