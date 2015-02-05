using FsCheck;
using FsCheck.Fluent;
using FsCheckUtils;
using Microsoft.FSharp.Core;
using NUnit.Framework;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6
{
    using Property = Gen<Rose<Result>>;
    
    [TestFixture]
    public class NumberGenerators
    {
        private static readonly Config Config = Config.VerboseThrowOnFailure;
        private static readonly Configuration Configuration = Config.ToConfiguration();

        [Test]
        public void TestFluent()
        {
            var g = Any.IntBetween(-2, 5);
            Spec.For(g, n => n >= -2 && n <= 5).Check(Configuration);
        }

        [Test]
        public void Test()
        {
            var g = Gen.choose(-2, 5);
            var property = Prop.forAll(Arb.fromGen(g), FSharpFunc<int, bool>.FromConverter(n => n >= -2 && n <= 5));
            Check.One(Config, property);
        }

        [FsCheck.NUnit.Property(Verbose = true)]
        public Property Property()
        {
            var g = Gen.choose(-2, 5);
            return Prop.forAll(Arb.fromGen(g), FSharpFunc<int, bool>.FromConverter(n => n >= -2 && n <= 5));
        }

        [FsCheck.NUnit.Property(Verbose = true)]
        public Property PropertyFluent()
        {
            var g = Any.IntBetween(-2, 5);
            return Spec.For(g, n => n >= -2 && n <= 5).Build();
        }
    }
}
