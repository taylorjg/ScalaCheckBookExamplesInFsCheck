using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Fluent;
using FsCheckUtils;
using Microsoft.FSharp.Core;
using NUnit.Framework;

namespace ScalaCheckBookExamplesInFsCheck.Chapter4
{
    using RLE = RunLengthEncoding;

    [TestFixture]
    public class RunLengthEncodingTests
    {
        private static readonly Config Config = Config.VerboseThrowOnFailure;
        private static readonly Configuration Configuration = Config.ToConfiguration();

        [Test]
        public void RunLengthEncodingPropertyFluent()
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
        public void RunLengthEncodingProperty()
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
    }
}
