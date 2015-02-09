namespace ScalaCheckBookExamplesInFsCheck.Chapter6.CustomTestCaseSimplification
{
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
}
