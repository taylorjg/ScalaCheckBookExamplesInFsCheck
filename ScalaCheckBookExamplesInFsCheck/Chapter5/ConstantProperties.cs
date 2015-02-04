using System;
using FsCheck;
using Microsoft.FSharp.Collections;
using NUnit.Framework;

namespace ScalaCheckBookExamplesInFsCheck.Chapter5
{
    using Property = Gen<Rose<Result>>;
    
    [TestFixture]
    public class ConstantProperties
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private class ConstantProps
        {
            private static readonly FSharpList<string> EmptyStamp = FSharpList<string>.Empty;
            private static readonly FSharpSet<string> EmptyLabels = SetModule.Empty<string>();
            private static readonly FSharpList<object> EmptyArguments = FSharpList<object>.Empty;

            // ReSharper disable UnusedMember.Local
            public static Property P1()
            {
                return Prop.ofTestable(new Result(Outcome.False, EmptyStamp, EmptyLabels, EmptyArguments));
            }

            public static Property P2()
            {
                return Prop.ofTestable(new Result(Outcome.True, EmptyStamp, EmptyLabels, EmptyArguments));
            }

            public static Property P3()
            {
                return Prop.ofTestable(new Result(Outcome.Rejected, EmptyStamp, EmptyLabels, EmptyArguments));
            }

            public static Property P4()
            {
                return Prop.ofTestable(new Result(Outcome.NewException(new Exception("Wrong banana!")), EmptyStamp, EmptyLabels, EmptyArguments));
            }

            public static Property P5()
            {
                return Prop.ofTestable(new Result(Outcome.NewTimeout(1000), EmptyStamp, EmptyLabels, EmptyArguments));
            }
            // ReSharper restore UnusedMember.Local
        }

        [Test]
        public void Test()
        {
            Check.QuickAll<ConstantProps>();
        }
    }
}
