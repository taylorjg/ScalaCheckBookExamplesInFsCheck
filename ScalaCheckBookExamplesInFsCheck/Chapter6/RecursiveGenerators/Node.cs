using System.Collections.Generic;
using System.Linq;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6.RecursiveGenerators
{
    internal class Node<T> : Tree<T>
    {
        public Node(List<Tree<T>> children)
        {
            _children = children;
        }

        public List<Tree<T>> Children
        {
            get { return _children; }
        }

        public override int Size
        {
            get { return Children.Select(child => child.Size).Sum(); }
        }

        public override string ToString()
        {
            var children = string.Join(", ", Children.Select(child => child.ToString()));
            return string.Format("Node(List({0}))", children);
        }

        private readonly List<Tree<T>> _children;
    }
}
