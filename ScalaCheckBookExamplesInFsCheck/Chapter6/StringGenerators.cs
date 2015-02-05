using System;
using FsCheck;
using FsCheck.Fluent;
using FsCheckUtils;
using NUnit.Framework;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6
{
    [TestFixture]
    public class StringGenerators
    {
        [Test]
        public void Test()
        {
            var stringsGen =
                from alpha in GenExtensions.AlphaStr
                from num in GenExtensions.NumStr
                from id in GenExtensions.Identifier
                select Tuple.Create(Take4(alpha), Take4(num), Take4(id));
            var sample = Gen.sample(20, 10, stringsGen);
            foreach (var value in sample) Console.WriteLine(value);
        }

        private static string Take4(string s)
        {
            return s.Substring(0, Math.Min(4, s.Length));
        }
    }
}
