using System.Collections.Generic;
using System.Linq;
using FsCheck;
using Microsoft.FSharp.Core;
using NUnit.Framework;
using Flinq;

namespace ScalaCheckBookExamplesInFsCheck.Chapter5
{
    using Property = Gen<Rose<Result>>;

    [TestFixture]
    public class CollectingTestStatistics
    {
        private static readonly Config Config = Config.QuickThrowOnFailure;

        [Test]
        public void Test()
        {
            Check.One(Config, Prop.forAll(Arb.from<List<int>>(), FSharpFunc<List<int>, Property>.FromConverter(xs =>
            {
                return Prop.forAll(Arb.fromGen(Gen.choose(0, xs.Count - 1)), FSharpFunc<int, Property>.FromConverter(n =>
                {
                    return Prop.forAll(Arb.fromGen(Gen.choose(n, xs.Count)), FSharpFunc<int, Property>.FromConverter(m =>
                    {
                        var slice = xs.Slice(n, m).ToList();
                        string label;
                        switch (slice.Count)
                        {
                            case 0: label = "none"; break;
                            case 1: label = "one"; break;
                            default: label = (slice.Count == xs.Count) ? "whole" : "part"; break;
                        }
                        return Prop.collect<string, bool>(label).Invoke(xs.ContainsSlice(slice));
                    }));
                }));
            })));
        }

        [FsCheck.NUnit.Property]
        public Property Property(List<int> xs)
        {
            return Prop.forAll(Arb.fromGen(Gen.choose(0, xs.Count - 1)), FSharpFunc<int, Property>.FromConverter(n =>
            {
                return Prop.forAll(Arb.fromGen(Gen.choose(n, xs.Count)), FSharpFunc<int, Property>.FromConverter(m =>
                {
                    var slice = xs.Slice(n, m).ToList();
                    string label;
                    switch (slice.Count)
                    {
                        case 0: label = "none"; break;
                        case 1: label = "one"; break;
                        default: label = (slice.Count == xs.Count) ? "whole" : "part"; break;
                    }
                    return Prop.collect<string, bool>(label).Invoke(xs.ContainsSlice(slice));
                }));
            }));
        }
    }
}
