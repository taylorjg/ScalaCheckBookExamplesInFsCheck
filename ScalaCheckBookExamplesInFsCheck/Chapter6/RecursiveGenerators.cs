using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Fluent;
using Microsoft.FSharp.Core;
using NUnit.Framework;
using ScalaCheckBookExamplesInFsCheck.Utils;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6
{
    // ReSharper disable once UnusedTypeParameter
    internal abstract class Tree<T>
    {
        public abstract int Size { get; }
    }

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

        private readonly T _item;
    }

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
            get { return _children.Select(child => child.Size).Sum(); }
        }

        private readonly List<Tree<T>> _children;
    }

    [TestFixture]
    public class RecursiveGenerators
    {
        [Test]
        public void Test()
        {
            GenIntTree.DumpSamples();
        }

        private static Gen<Tree<int>> GenIntTree
        {
            get { return GenTree(Any.OfType<int>()); }
        }

        private static Gen<Tree<T>> GenTree<T>(Gen<T> genT)
        {
            return Gen.oneof(new[] {GenLeaf(genT), GenNode(genT)});
        }

        private static Gen<Tree<T>> GenLeaf<T>(Gen<T> genT)
        {
            return genT.Select(t => new Leaf<T>(t) as Tree<T>);
        }

        private static Gen<Tree<T>> GenNode<T>(Gen<T> genT)
        {
            return Gen.sized(FSharpFunc<int, Gen<Tree<T>>>.FromConverter(size =>
                from s in Gen.choose(0, size)
                let g = Gen.resize(size/(s + 1), GenTree(genT))
                from children in g.MakeListOfLength(s)
                select new Node<T>(children) as Tree<T>));
        }
    }
}
