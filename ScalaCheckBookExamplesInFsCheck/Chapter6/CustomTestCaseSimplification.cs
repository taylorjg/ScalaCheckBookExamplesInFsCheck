using System;
using FsCheck;
using FsCheck.Fluent;
using Microsoft.FSharp.Core;
using NUnit.Framework;
using ScalaCheckBookExamplesInFsCheck.Utils;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6
{
    public abstract class Expression
    {
        public override string ToString()
        {
            return Show();
        }

        public Expression Rewrite()
        {
            return Match(
                eConst => eConst as Expression,
                eAdd =>
                {
                    // ReSharper disable ConvertIfStatementToReturnStatement
                    if (eAdd.Expression1.Equals(eAdd.Expression2)) return new Mul(new Const(2), eAdd.Expression1);
                    if (eAdd.Expression1.Equals(new Const(0))) return eAdd.Expression2;
                    if (eAdd.Expression2.Equals(new Const(0))) return eAdd.Expression1;
                    return eAdd;
                    // ReSharper restore ConvertIfStatementToReturnStatement
                },
                eMul =>
                {
                    // ReSharper disable ConvertIfStatementToReturnStatement
                    if (eMul.Expression1.Equals(new Const(0))) return new Const(0);
                    if (eMul.Expression2.Equals(new Const(0))) return new Const(0);
                    if (eMul.Expression1.Equals(new Const(1))) return eMul.Expression2;
                    if (eMul.Expression2.Equals(new Const(1))) return eMul.Expression1;
                    return eMul;
                    // ReSharper restore ConvertIfStatementToReturnStatement
                });
        }

        public abstract int Eval();
        public abstract string Show();

        private T Match<T>(Func<Const, T> constFunc, Func<Add, T> addFunc, Func<Mul, T> mulFunc)
        {
            if (this is Const) return constFunc(this as Const);
            if (this is Add) return addFunc(this as Add);
            if (this is Mul) return mulFunc(this as Mul);
            throw new InvalidOperationException("Unknown expression type");
        }
    }

    public class Const : Expression
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

        public override bool Equals(object rhs)
        {
            if (rhs == null) return false;
            if (ReferenceEquals(this, rhs)) return true;
            var other = rhs as Const;
            if (other == null) return false;
            return (Value == other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        private readonly int _value;
    }

    public class Add : Expression
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

        public override bool Equals(object rhs)
        {
            if (rhs == null) return false;
            if (ReferenceEquals(this, rhs)) return true;
            var other = rhs as Add;
            if (other == null) return false;
            return (Expression1 == other.Expression1 && Expression2 == other.Expression2);
        }

        public override int GetHashCode()
        {
            return Expression1.GetHashCode() + Expression2.GetHashCode();
        }

        private readonly Expression _expression1;
        private readonly Expression _expression2;
    }

    public class Mul : Expression
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

        public override bool Equals(object rhs)
        {
            if (rhs == null) return false;
            if (ReferenceEquals(this, rhs)) return true;
            var other = rhs as Mul;
            if (other == null) return false;
            return (Expression1 == other.Expression1 && Expression2 == other.Expression2);
        }

        public override int GetHashCode()
        {
            return Expression1.GetHashCode() * Expression2.GetHashCode();
        }

        private readonly Expression _expression1;
        private readonly Expression _expression2;
    }

    [TestFixture]
    public class CustomTestCaseSimplification
    {
        private static readonly Config Config = Config.VerboseThrowOnFailure;

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
