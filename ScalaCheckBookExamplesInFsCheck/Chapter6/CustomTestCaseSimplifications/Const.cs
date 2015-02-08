using System;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6.CustomTestCaseSimplifications
{
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
}
