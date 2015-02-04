using System.Collections.Generic;
using System.Linq;
using FsCheck;
//using FsCheck.Fluent;
//using FsCheckUtils;
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
        //private static readonly Configuration Configuration = Config.ToConfiguration();

        [Test]
        public void PropertyFluent()
        {
            Check.One(Config, Prop.forAll(Arb.from<List<int>>(), FSharpFunc<List<int>, Property>.FromConverter(xs =>
            {
                return Prop.forAll(Arb.fromGen(Gen.choose(0, xs.Count - 1)), FSharpFunc<int, Property>.FromConverter(n =>
                {
                    return Prop.forAll(Arb.fromGen(Gen.choose(n, xs.Count)), FSharpFunc<int, Property>.FromConverter(m =>
                    {
                        var slice1 = xs.Slice(n, m).ToList();
                        string label1;
                        switch (slice1.Count)
                        {
                            case 0:
                                label1 = "none";
                                break;
                            case 1:
                                label1 = "one";
                                break;
                            default:
                                label1 = (slice1.Count == xs.Count) ? "whole" : "part";
                                break;
                        }
                        return Prop.collect<string, bool>(label1).Invoke(xs.ContainsSlice(slice1));
                    }));
                }));
            })));
        }
    }
}
