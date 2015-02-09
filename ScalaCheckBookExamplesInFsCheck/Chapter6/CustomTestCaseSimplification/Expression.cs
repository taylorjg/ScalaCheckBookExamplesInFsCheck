using System;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6.CustomTestCaseSimplification
{
    public abstract class Expression
    {
        public override string ToString()
        {
            return Show();
        }

        public Expression Rewrite(bool causeDeliberateError = false)
        {
            return Match(
                eConst => eConst as Expression,
                eAdd =>
                {
                    // ReSharper disable ConvertIfStatementToReturnStatement
                    if (eAdd.Expression1.Equals(eAdd.Expression2)) return new Mul(new Const(2), eAdd.Expression1);
                    if (eAdd.Expression1.Equals(new Const(causeDeliberateError ? 1 : 0))) return eAdd.Expression2;
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

        public T Match<T>(Func<Const, T> constFunc, Func<Add, T> addFunc, Func<Mul, T> mulFunc)
        {
            if (this is Const) return constFunc(this as Const);
            if (this is Add) return addFunc(this as Add);
            if (this is Mul) return mulFunc(this as Mul);
            throw new InvalidOperationException("Unknown expression type");
        }
    }
}
