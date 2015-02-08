using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Fluent;
using FsCheckUtils;
using Microsoft.FSharp.Core;
using NUnit.Framework;
using ScalaCheckBookExamplesInFsCheck.Utils;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6.CustomTestCaseSimplifications
{
    [TestFixture]
    public class Tests
    {
        private static readonly Config Config = Config
            .QuickThrowOnFailure
            .WithStartSize(100)
            .WithMaxTest(500);

        private static readonly Config Config2 = Config
            .Quick
            .WithStartSize(Config.StartSize)
            .WithMaxTest(Config.MaxTest)
            .WithEveryShrink(Config.Verbose.EveryShrink);

        [Test]
        public void Sample()
        {
            GenExpr.DumpSamples();
        }

        [Test]
        public void PassingTestWhereItWorksProperly()
        {
            var shrinker = FSharpFunc<Expression, IEnumerable<Expression>>.FromConverter(ShrinkExpr);
            var arb = Arb.fromGenShrink(GenExpr, shrinker);
            var body = FSharpFunc<Expression, bool>.FromConverter(e => e.Rewrite().Eval() == e.Eval());
            var property = Prop.forAll(arb, body);
            Check.One(Config, property);
        }

        [Test]
        public void FalsifiableTestToDemonstrateShrinking()
        {
            var shrinker = FSharpFunc<Expression, IEnumerable<Expression>>.FromConverter(ShrinkExpr);
            var arb = Arb.fromGenShrink(GenExpr, shrinker);
            var body = FSharpFunc<Expression, bool>.FromConverter(e => e.Rewrite(true).Eval() == e.Eval());
            var property = Prop.forAll(arb, body);
            Check.One(Config2, property);
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

        private static IEnumerable<Expression> ShrinkExpr(Expression e)
        {
            return e.Match(ConstShrinks, AddShrinks, MulShrinks);
        }

        private static IEnumerable<Expression> ConstShrinks(Const e)
        {
            return Arb.shrink(e.Value).Select(value => new Const(value) as Expression);
        }

        private static IEnumerable<Expression> AddShrinks(Add e)
        {
            yield return e.Expression1;
            yield return e.Expression2;
            foreach (var x in ShrinkExpr(e.Expression1).Select(shrink => new Add(shrink, e.Expression2) as Expression)) yield return x;
            foreach (var x in ShrinkExpr(e.Expression2).Select(shrink => new Add(e.Expression1, shrink) as Expression)) yield return x;
        }

        private static IEnumerable<Expression> MulShrinks(Mul e)
        {
            yield return e.Expression1;
            yield return e.Expression2;
            foreach (var x in ShrinkExpr(e.Expression1).Select(shrink => new Mul(shrink, e.Expression2) as Expression)) yield return x;
            foreach (var x in ShrinkExpr(e.Expression2).Select(shrink => new Mul(e.Expression1, shrink) as Expression)) yield return x;
        }
    }
}
