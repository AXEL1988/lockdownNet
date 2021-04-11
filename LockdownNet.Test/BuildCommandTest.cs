using LockdownNet.Commands;
using LockdownNet.Test.Utils;
using System;
using Xunit;

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
            
            Assert.Equal("You execute the build command\r\n", writtenText);
        }
    }
}
