using FsCheck.Fluent;
using FsCheckUtils;
using NUnit.Framework;
using ScalaCheckBookExamplesInFsCheck.Utils;

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
            genString.DumpSamples();
        }
    }
}
