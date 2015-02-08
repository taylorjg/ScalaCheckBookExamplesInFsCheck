using FsCheck;
using FsCheck.Fluent;
using Microsoft.FSharp.Core;
using NUnit.Framework;
using ScalaCheckBookExamplesInFsCheck.Utils;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6.RecursiveGenerators
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Sample()
        {
            GenIntTree.DumpSamples();
        }

        private static Gen<Tree<int>> GenIntTree
        {
            get { return GenTree(Arb.generate<int>()); }
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
