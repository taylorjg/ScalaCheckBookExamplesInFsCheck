using System;
using FsCheck;
using Microsoft.FSharp.Core;
using NUnit.Framework;

namespace ScalaCheckBookExamplesInFsCheck.Chapter5
{
    using Property = Gen<Rose<Result>>;

    [TestFixture]
    public class Throws
    {
        [Test]
        public void Test()
        {
            Check.QuickThrowOnFailure(Prop.forAll(Arb.from<int>(), FSharpFunc<int, Property>.FromConverter(n =>
            {
                return Prop.throws<DivideByZeroException, int>(new Lazy<int>(() => n/(n - n)));
            })));
        }

        [FsCheck.NUnit.Property]
        public Property Property(int n)
        {
            return Prop.throws<DivideByZeroException, int>(new Lazy<int>(() => n/(n - n)));
        }
    }
}
