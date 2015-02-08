using System;
using FsCheck;
using FsCheck.Fluent;
using Microsoft.FSharp.Core;
using NUnit.Framework;
using ScalaCheckBookExamplesInFsCheck.Utils;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6
{
    internal abstract class Expression
    {
        public override string ToString()
        {
            return Show();
        }

        public abstract int Eval();
        public abstract string Show();
    }

    internal class Const : Expression
    {
        public Const(int value)
        {
            _value = value;
        }

        public int Value
        {
            get { return _value; }
        }

        public override int Eval()
        {
            return Value;
        }

        public override string Show()
        {
            return Convert.ToString(Value);
        }

        private readonly int _value;
    }

    internal class Add : Expression
    {
        public Add(Expression expression1, Expression expression2)
        {
            _expression1 = expression1;
            _expression2 = expression2;
        }

        public Expression Expression1
        {
            get { return _expression1; }
        }

        public Expression Expression2
        {
            get { return _expression2; }
        }

        public override int Eval()
        {
            return Expression1.Eval() + Expression2.Eval();
        }

        public override string Show()
        {
            return string.Format("({0} + {1})", Expression1.Show(), Expression2.Show());
        }

        private readonly Expression _expression1;
        private readonly Expression _expression2;
    }

    internal class Mul : Expression
    {
        public Mul(Expression expression1, Expression expression2)
        {
            _expression1 = expression1;
            _expression2 = expression2;
        }

        public Expression Expression1
        {
            get { return _expression1; }
        }

        public Expression Expression2
        {
            get { return _expression2; }
        }

        public override int Eval()
        {
            return Expression1.Eval() * Expression2.Eval();
        }

        public override string Show()
        {
            return string.Format("({0} * {1})", Expression1.Show(), Expression2.Show());
        }

        private readonly Expression _expression1;
        private readonly Expression _expression2;
    }

    [TestFixture]
    public class CustomTestCaseSimplification
    {
        [Test]
        public void Test()
        {
            GenExpr.DumpSamples();
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
