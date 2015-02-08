namespace ScalaCheckBookExamplesInFsCheck.Chapter6.RecursiveGenerators
{
    internal class Leaf<T> : Tree<T>
    {
        public Leaf(T item)
        {
            _item = item;
        }

        public T Item
        {
            get { return _item; }
        }

        public override int Size
        {
            get { return 1; }
        }

        public override string ToString()
        {
            return string.Format("Leaf({0})", Item);
        }

        private readonly T _item;
    }
}
