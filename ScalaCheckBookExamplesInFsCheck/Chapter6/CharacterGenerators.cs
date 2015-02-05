using System;
using FsCheck;
using FsCheck.Fluent;
using FsCheckUtils;
using NUnit.Framework;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6
{
    [TestFixture]
    public class CharacterGenerators
    {
        [Test]
        public void Test()
        {
            var genString =
                from c1 in GenExtensions.NumChar
                from c2 in GenExtensions.AlphaUpperChar
                from c3 in GenExtensions.AlphaLowerChar
                from c4 in GenExtensions.AlphaChar
                from c5 in GenExtensions.AlphaNumChar
                select new string(new[] {c1, c2, c3, c4, c5});
            var sample = Gen.sample(10, 10, genString);
            foreach (var value in sample) Console.WriteLine(value);
        }
    }
}
