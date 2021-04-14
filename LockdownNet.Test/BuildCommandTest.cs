using LockdownNet.Commands;
using LockdownNet.Test.Utils;
using System;
using Xunit;
using Shouldly;

namespace LockdownNet.Test
{
    public class BuildCommandTest
    {
        [Fact]
        public void TestWriteToConsole()
        {
            var testConsole = new TestConsole();
            var buildCommand = new BuildCommand(testConsole);

            buildCommand.OnExecute();

            string writtenText = testConsole.GetWrittenContent();
            writtenText.ShouldBe("Pleas do not use, alpha" + Environment.NewLine);
            //Assert.Equal("You execute the build command\r\n", writtenText);
        }
    }
}
