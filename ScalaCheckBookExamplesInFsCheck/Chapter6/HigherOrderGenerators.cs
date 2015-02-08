using System;
using FsCheck;
using FsCheck.Fluent;
using FsCheckUtils;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using NUnit.Framework;
using ScalaCheckBookExamplesInFsCheck.Utils;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6
{
    using Property = Gen<Rose<Result>>;
    
    [TestFixture]
    public class HigherOrderGenerators
    {
        private static readonly Config Config = Config.QuickThrowOnFailure;

        [Test]
        public void SequenceSample()
        {
            var numbers = Gen.sequence(ListModule.OfSeq(new[]
            {
                Gen.choose(1, 10),
                Gen.constant(20),
                Gen.constant(30)
            }));
            numbers.DumpSamples();
        }

        [Test]
        public void FrequencyTest()
        {
            var evenNumbersGen = from n in Gen.choose(2, 100000) select 2*n;
            var oddNumbersGen = from n in Gen.choose(0, 100000) select 2*n + 1;
            var numberGen = Gen.frequency(new[]
            {
                Tuple.Create(1, oddNumbersGen),
                Tuple.Create(2, evenNumbersGen),
                Tuple.Create(4, Gen.constant(0))
            });

            var property = Prop.forAll(Arb.fromGen(numberGen), FSharpFunc<int, Property>.FromConverter(n =>
            {
                string l;
                if (n == 0) l = "one";
                else if (n%2 == 0) l = "even";
                else l = "odd";
                return Prop.collect<string, bool>(l).Invoke(true);
            }));

            Check.One(Config, property);
            Check.One(Config.WithMaxTest(1000), property);
            Check.One(Config.WithMaxTest(10000), property);
        }

        [Test]
        public void OneOfSample()
        {
            var genNotZero = Gen.oneof(new[] {Gen.choose(-10, -1), Gen.choose(1, 10)});
            genNotZero.DumpSamples();

            var genVowel = Gen.oneof(new[]
            {
                Gen.constant('a'),
                Gen.constant('e'),
                Gen.constant('i'),
                Gen.constant('o'),
                Gen.constant('u'),
                Gen.constant('y')
            });
            genVowel.DumpSamples();
        }
    }
}
