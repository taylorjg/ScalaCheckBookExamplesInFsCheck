using FsCheckUtils;
using NUnit.Framework;
using ScalaCheckBookExamplesInFsCheck.Utils;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6
{
    [TestFixture]
    public class Pick
    {
        [Test]
        public void Sample()
        {
            var twoStrings = GenExtensions.PickValues(2, "red", "blue", "green", "pink");
            twoStrings.DumpSamples(Formatters.FormatCollection);
        }
    }
}
