using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Fluent;
using FsCheckUtils;
using Microsoft.FSharp.Core;
using NUnit.Framework;

namespace ScalaCheckBookExamplesInFsCheck.Chapter5
{
    using Property = Gen<Rose<Result>>;

    [TestFixture]
    public class InterleavingTests
    {
        private static readonly Config Config = Config.VerboseThrowOnFailure;
        private static readonly Configuration Configuration = Config.ToConfiguration();

        [Test]
        public void InterleaveTestFluent()
        {
            Spec
                .For(Any.OfType<IList<int>>(), Any.OfType<IList<int>>(), (xs, ys) =>
                {
                    var res = Interleaving.Interleave(xs, ys);
                    return xs.Count + ys.Count == res.Count();
                }).Label("length")
                .And((xs, ys) =>
                {
                    var res = Interleaving.Interleave(xs, ys);
                    var idxs = Enumerable.Range(0, Math.Min(xs.Count, ys.Count)).ToList();
                    return xs.SequenceEqual(idxs.Select(idx => res[2 * idx]).Concat(res.Skip(2 * ys.Count)));
                }, "zip xs")
                .And((xs, ys) =>
                {
                    var res = Interleaving.Interleave(xs, ys);
                    var idxs = Enumerable.Range(0, Math.Min(xs.Count, ys.Count)).ToList();
                    return ys.SequenceEqual(idxs.Select(idx => res[2 * idx + 1]).Concat(res.Skip(2 * xs.Count)));
                }, "zip ys")
                .Check(Configuration);
        }

        [Test]
        public void InterleaveTest()
        {
            var body = FSharpFunc<IList<int>, FSharpFunc<IList<int>, Property>>.FromConverter(xs =>
                FSharpFunc<IList<int>, Property>.FromConverter(ys =>
                {
                    var res = Interleaving.Interleave(xs, ys);
                    var idxs = Enumerable.Range(0, Math.Min(xs.Count, ys.Count)).ToList();
                    return PropExtensions.AndAll(
                        PropExtensions.Label(xs.Count + ys.Count == res.Count(), "length"),
                        PropExtensions.Label(xs.SequenceEqual(idxs.Select(idx => res[2 * idx]).Concat(res.Skip(2 * ys.Count))), "zip xs"),
                        PropExtensions.Label(ys.SequenceEqual(idxs.Select(idx => res[2 * idx + 1]).Concat(res.Skip(2 * xs.Count))), "zip ys"));
                }));
            Check.One(Config, body);
        }

        [FsCheck.NUnit.Property(Verbose = true)]
        public Property InterleavePropertyFluent(IList<int> xsParam, IList<int> ysParam)
        {
            return Spec
                .For(Any.Value(xsParam), Any.Value(ysParam), (xs, ys) =>
                {
                    var res = Interleaving.Interleave(xs, ys);
                    return xs.Count + ys.Count == res.Count();
                }).Label("length")
                .And((xs, ys) =>
                {
                    var res = Interleaving.Interleave(xs, ys);
                    var idxs = Enumerable.Range(0, Math.Min(xs.Count, ys.Count)).ToList();
                    return xs.SequenceEqual(idxs.Select(idx => res[2*idx]).Concat(res.Skip(2*ys.Count)));
                }, "zip xs")
                .And((xs, ys) =>
                {
                    var res = Interleaving.Interleave(xs, ys);
                    var idxs = Enumerable.Range(0, Math.Min(xs.Count, ys.Count)).ToList();
                    return ys.SequenceEqual(idxs.Select(idx => res[2*idx + 1]).Concat(res.Skip(2*xs.Count)));
                }, "zip ys")
                .Build();
        }

        [FsCheck.NUnit.Property(Verbose = true)]
        public Property InterleaveProperty(IList<int> xs, IList<int> ys)
        {
            var res = Interleaving.Interleave(xs, ys);
            var idxs = Enumerable.Range(0, Math.Min(xs.Count, ys.Count)).ToList();
            return PropExtensions.AndAll(
                PropExtensions.Label(xs.Count + ys.Count == res.Count(), "length"),
                PropExtensions.Label(xs.SequenceEqual(idxs.Select(idx => res[2 * idx]).Concat(res.Skip(2 * ys.Count))), "zip xs"),
                PropExtensions.Label(ys.SequenceEqual(idxs.Select(idx => res[2 * idx + 1]).Concat(res.Skip(2 * xs.Count))), "zip ys"));
        }
    }
}
