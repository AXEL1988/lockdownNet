using LockdownNet.Build;
using Shouldly;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace LockdownNet.Test
{
    public class SiteBuilderTest
    {
        private readonly IFileSystem fakeFileSystem;
        const string inputPath = "./input";
        const string output = "./output";
        public SiteBuilderTest()
        {
            fakeFileSystem = new MockFileSystem();
        }

        [Fact]
        public void TestOutputFolderExist()
        {
            // Setup
            var fakeFilePath = fakeFileSystem.Path.Combine(output, "archivo.txt");
            fakeFileSystem.Directory.CreateDirectory(fakeFilePath);
            fakeFileSystem.File.WriteAllText(fakeFilePath, "Hola mundo");

            var siteBuilder = new SiteBuilder(fakeFileSystem);
            // Act
            siteBuilder.CleanFolder(output);

            // Assert
            fakeFileSystem.Directory.Exists(output).ShouldBeTrue();
            fakeFileSystem.Directory.EnumerateFiles(output).Any().ShouldBeFalse();
            fakeFileSystem.Directory.EnumerateDirectories(output).Any().ShouldBeTrue();
        }

        [Fact]
        public void TestOutputFolderDoesNotExist()
        {
            var siteBuilder = new SiteBuilder(fakeFileSystem);
            // Act
            siteBuilder.CleanFolder(output);

            // Assert
            fakeFileSystem.Directory.Exists(output).ShouldBeTrue();
            fakeFileSystem.Directory.EnumerateFiles(output).Any().ShouldBeFalse();
            fakeFileSystem.Directory.EnumerateDirectories(output).Any().ShouldBeTrue();
        }
    }
}
