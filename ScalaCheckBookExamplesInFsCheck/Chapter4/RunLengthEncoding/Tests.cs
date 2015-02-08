using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Fluent;
using FsCheckUtils;
using Microsoft.FSharp.Core;
using NUnit.Framework;

namespace ScalaCheckBookExamplesInFsCheck.Chapter4.RunLengthEncoding
{
    using Property = Gen<Rose<Result>>;
    using RLE = RunLengthEncoding;

    [TestFixture]
    public class Tests
    {
        private static readonly Config Config = Config.VerboseThrowOnFailure;
        private static readonly Configuration Configuration = Config.ToConfiguration();

        [Test]
        public void RunLengthEncodingTestFluent()
        {
            Spec
                .For(GenOutput, r =>
                {
                    var original = r.ToList();
                    var actual = RLE.RunLengthEnc(RLE.RunLengthDec(original));
                    return actual.SequenceEqual(original);
                })
                .Check(Configuration);
        }

        [Test]
        public void RunLengthEncodingTest()
        {
            var arb = Arb.fromGen(GenOutput);
            var body = FSharpFunc<IEnumerable<Tuple<int, char>>, bool>.FromConverter(r =>
            {
                var original = r.ToList();
                var actual = RLE.RunLengthEnc(RLE.RunLengthDec(original));
                return actual.SequenceEqual(original);
            });
            Check.One(Config, Prop.forAll(arb, body));
        }

        [FsCheck.NUnit.Property(Verbose = true, Arbitrary = new[] { typeof(LocalArbitraties) })]
        public Property RunLengthEncodingPropertyFluent(IEnumerable<Tuple<int, char>> rParam)
        {
            return Spec
                .For(Any.Value(rParam), r =>
                {
                    var original = r.ToList();
                    var actual = RLE.RunLengthEnc(RLE.RunLengthDec(original));
                    return actual.SequenceEqual(original);
                })
                .Build();
        }

        [FsCheck.NUnit.Property(Verbose = true, Arbitrary = new[] { typeof(LocalArbitraties) })]
        public bool RunLengthEncodingProperty(IEnumerable<Tuple<int, char>> r)
        {
            var original = r.ToList();
            var actual = RLE.RunLengthEnc(RLE.RunLengthDec(original));
            return actual.SequenceEqual(original);
        }

        private static Gen<IEnumerable<Tuple<int, char>>> GenOutput
        {
            get
            {
                return Gen.sized(FSharpFunc<int, Gen<IEnumerable<Tuple<int, char>>>>.FromConverter(RleList));
            }
        }

        private static readonly Converter<int, Gen<IEnumerable<Tuple<int, char>>>> RleList = size =>
        {
            if (size <= 1)
            {
                return RleItem.Select(t => Enumerable.Repeat(t, 1));
            }

            return from tail in RleList(size - 1)
                   let c1 = tail.First().Item2
                   from head in RleItem.RetryUntil(t => t.Item2 != c1)
                   select Enumerable.Repeat(head, 1).Concat(tail);
        };

        private static Gen<Tuple<int, char>> RleItem
        {
            get
            {
                return from n in Gen.choose(1, 20)
                       from c in GenExtensions.AlphaNumChar
                       select Tuple.Create(n, c);
            }
        }

        private class LocalArbitraties
        {
            // ReSharper disable once UnusedMember.Local
            public static Arbitrary<IEnumerable<Tuple<int, char>>> ArbitraryOutput
            {
                get
                {
                    return new ArbOutput();
                }
            }
        }

        private class ArbOutput : Arbitrary<IEnumerable<Tuple<int, char>>>
        {
            public override Gen<IEnumerable<Tuple<int, char>>> Generator
            {
                get { return GenOutput; }
            }
        }
    }
}
