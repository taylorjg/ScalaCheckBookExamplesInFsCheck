using System;
using FsCheck.Fluent;
using FsCheckUtils;
using NUnit.Framework;
using ScalaCheckBookExamplesInFsCheck.Utils;

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
            stringsGen.DumpSamples();
        }

        private static string Take4(string s)
        {
            return s.Substring(0, Math.Min(4, s.Length));
        }
    }
}
