using System.Text;
using FsCheckUtils;
using NUnit.Framework;
using ScalaCheckBookExamplesInFsCheck.Utils;

namespace ScalaCheckBookExamplesInFsCheck.Chapter6
{
    [TestFixture]
    public class SomeOf
    {
        [Test]
        public void NumbersSample()
        {
            var numbers = GenExtensions.SomeOfValues(1, 2, 3, 4);
            numbers.DumpSamples(Formatters.FormatCollection);
        }

        [Test]
        public void NumberListsSample()
        {
            var numbers = GenExtensions.SomeOfValues(1, 2, 3, 4);
            var numberLists = GenExtensions.SomeOfGenerators(numbers, numbers, numbers);
            numberLists.DumpSamples(xss =>
            {
                var sb = new StringBuilder();
                sb.AppendLine();
                foreach (var xs in xss)
                {
                    sb.AppendLine(Formatters.FormatCollection(xs));
                }
                return sb.ToString();
            });
        }
    }
}
