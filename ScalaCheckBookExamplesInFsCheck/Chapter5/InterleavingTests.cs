using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Fluent;
using FsCheckUtils;
using NUnit.Framework;

namespace ScalaCheckBookExamplesInFsCheck.Chapter5
{
    [TestFixture]
    public class InterleavingTests
    {
        private static readonly Config Config = Config.VerboseThrowOnFailure;
        private static readonly Configuration Configuration = Config.ToConfiguration();

        [Test]
        public void InterleavePropertyFluent()
        {
            Spec
                .For(Any.OfType<IList<int>>(), Any.OfType<IList<int>>(), (xs, ys) =>
                {
                    var res = Interleaving.Interleave(xs, ys);
                    return xs.Count + ys.Count == res.Count();
                })
                .Label("length")
                .And((xs, ys) =>
                {
                    var res = Interleaving.Interleave(xs, ys);
                    var idxs = Enumerable.Range(0, Math.Min(xs.Count, ys.Count)).ToList();
                    return xs.SequenceEqual(idxs.Select(idx => res[2 * idx]).Concat(res.Skip(2 * ys.Count)));
                })
                .Label("zip xs")
                .And((xs, ys) =>
                {
                    var res = Interleaving.Interleave(xs, ys);
                    var idxs = Enumerable.Range(0, Math.Min(xs.Count, ys.Count)).ToList();
                    return ys.SequenceEqual(idxs.Select(idx => res[2 * idx + 1]).Concat(res.Skip(2 * xs.Count)));
                })
                .Label("zip ys")
                .Check(Configuration);
        }
    }
}
