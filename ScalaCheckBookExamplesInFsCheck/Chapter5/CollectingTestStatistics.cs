using System;
using FsCheck;
using FsCheck.Fluent;
using FsCheckUtils;
using Microsoft.FSharp.Core;
using NUnit.Framework;

namespace ScalaCheckBookExamplesInFsCheck.Chapter5
{
    using Property = Gen<Rose<Result>>;

    [TestFixture]
    public class CollectingTestStatistics
    {
        private static readonly Config Config = Config.QuickThrowOnFailure;
        private static readonly Configuration Configuration = Config.ToConfiguration();

        [Test]
        public void TestFluent()
        {
            Spec
                .ForAny((int n) => n + n == 2*n)
                .Classify(x => x%2 == 0, "even")
                .Classify(x => x%2 != 0, "odd")
                .Classify(x => x < 0, "neg")
                .Classify(x => x >= 0, "pos")
                .Classify(x => Math.Abs(x) > 50, "large")
                .Check(Configuration);
        }

        [Test]
        public void Test()
        {
            var body = FSharpFunc<int, Property>.FromConverter(n =>
                Prop.classify<Property>(n%2 == 0, "even")
                    .Invoke(Prop.classify<Property>(n%2 != 0, "odd")
                        .Invoke(Prop.classify<Property>(n < 0, "neg")
                            .Invoke(Prop.classify<Property>(n >= 0, "pos")
                                .Invoke(Prop.classify<Property>(Math.Abs(n) > 50, "large")
                                    .Invoke(Prop.ofTestable(n + n == 2*n)))))));
            Check.One(Config, body);
        }

        [FsCheck.NUnit.Property]
        public Property PropertyFluent(int n)
        {
            return Spec
                .For(Any.Value(n), i => n + n == 2*n)
                .Classify(x => x%2 == 0, "even")
                .Classify(x => x%2 != 0, "odd")
                .Classify(x => x < 0, "neg")
                .Classify(x => x >= 0, "pos")
                .Classify(x => Math.Abs(x) > 50, "large")
                .Build();
        }

        [FsCheck.NUnit.Property]
        public Property Property(int n)
        {
            return Prop.classify<Property>(n%2 == 0, "even")
                .Invoke(Prop.classify<Property>(n%2 != 0, "odd")
                    .Invoke(Prop.classify<Property>(n < 0, "neg")
                        .Invoke(Prop.classify<Property>(n >= 0, "pos")
                            .Invoke(Prop.classify<Property>(Math.Abs(n) > 50, "large")
                                .Invoke(Prop.ofTestable(n + n == 2*n))))));
        }
    }
}
