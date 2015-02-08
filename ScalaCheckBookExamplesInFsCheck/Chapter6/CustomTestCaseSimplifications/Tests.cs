using System;
using FsCheck;
using FsCheck.Fluent;
using Microsoft.FSharp.Core;
using NUnit.Framework;
using ScalaCheckBookExamplesInFsCheck.Utils;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6.CustomTestCaseSimplifications
{
    [TestFixture]
    public class Tests
    {
        private static readonly Config Config = Config.QuickThrowOnFailure;

        [Test]
        public void Sample()
        {
            GenExpr.DumpSamples();
        }

        [Test]
        public void Test()
        {
            var arb = Arb.fromGen(GenExpr);
            var body = FSharpFunc<Expression, bool>.FromConverter(e => e.Rewrite().Eval() == e.Eval());
            var property = Prop.forAll(arb, body);
            Check.One(Config, property);
        }

        private static Gen<Expression> GenExpr
        {
            get
            {
                return Gen.sized(FSharpFunc<int, Gen<Expression>>.FromConverter(sz =>
                    Gen.frequency(new[]
                    {
                        Tuple.Create(Math.Max(sz, 1), GenConst),
                        Tuple.Create(sz - (int) Math.Sqrt(sz), Gen.resize(sz/2, GenAdd)),
                        Tuple.Create(sz - (int) Math.Sqrt(sz), Gen.resize(sz/2, GenMul))
                    })));
            }
        }

        private static Gen<Expression> GenConst
        {
            get { return Gen.choose(0, 10).Select(value => new Const(value) as Expression); }
        }

        private static Gen<Expression> GenAdd
        {
            get
            {
                return
                    from e1 in GenExpr
                    from e2 in GenExpr
                    select new Add(e1, e2) as Expression;
            }
        }

        private static Gen<Expression> GenMul
        {
            get
            {
                return
                    from e1 in GenExpr
                    from e2 in GenExpr
                    select new Mul(e1, e2) as Expression;
            }
        }
    }
}
