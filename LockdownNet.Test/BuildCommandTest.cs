using LockdownNet.Commands;
using LockdownNet.Test.Utils;
using System;
using Xunit;
using Shouldly;
using Moq;
using LockdownNet.Build;

namespace LockdownNet.Test
{
    public class BuildCommandTest
    {
        [Fact]
        public void TestWriteToConsole()
        {
            // Setup
            var testConsole = new TestConsole();
            var mockSiteBuilder = new Mock<ISiteBuilder>();
            var inputPath = "./";
            var outputhPath = "./_site";
            
            var buildCommand = new BuildCommand(testConsole, siteBuilder: mockSiteBuilder.Object);
            buildCommand.InputPath = inputPath;
            buildCommand.OutputPath = outputhPath;
            
            // Act
            buildCommand.OnExecute();

            // Assert
            mockSiteBuilder.Verify(sb => sb.Build(inputPath, outputhPath), Times.Once);
        }
    }
}
