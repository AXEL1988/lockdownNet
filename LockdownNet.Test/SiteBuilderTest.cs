using LockdownNet.Build;
using Moq;
using Shouldly;
using System.Collections.Generic;
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
        const string output = ".\\output\\";
        public SiteBuilderTest()
        {
            fakeFileSystem = new MockFileSystem();
        }

        [Fact]
        public void TestOutputFolderExist()
        {
            // Setup
            var fakeFilePath = fakeFileSystem.Path.Combine(output, "archivo.txt");
            fakeFileSystem.Directory.CreateDirectory(output);
            fakeFileSystem.File.WriteAllText(fakeFilePath, "Hola mundo");

            var siteBuilder = new SiteBuilder(fakeFileSystem);
            // Act
            siteBuilder.CleanFolder(output);

            // Assert
            this.AssertDirectoryIsEmpty(output);
        }

        [Fact]
        public void TestOutputFolderDoesNotExist()
        {
            var siteBuilder = new SiteBuilder(fakeFileSystem);
            // Act
            siteBuilder.CleanFolder(output);

            // Assert
            this.AssertDirectoryIsEmpty(output);
        }
        [Fact]
        public void TestBuildCallsClean()
        {
            var mockSiteBuilder = new Mock<SiteBuilder>(MockBehavior.Strict, this.fakeFileSystem);
            mockSiteBuilder.Setup(sb => sb.CleanFolder(output));
            SiteBuilder siteBuilder = mockSiteBuilder.Object;

            siteBuilder.Build(inputPath, output);

            mockSiteBuilder.Verify(sb => sb.CleanFolder(output));
        }

        [Fact]
        public void TestCopyFile()
        {
            // Setup
            var stylesFile = this.fakeFileSystem.Path.Combine(inputPath, "style.css");
            var someOtherFile = this.fakeFileSystem.Path.Combine(inputPath, "subfolder", "style.css");

            var contents = new Dictionary<string, MockFileData>
            {
                {stylesFile, new MockFileData("body { color: #fff; }") },
                {someOtherFile, new MockFileData("more data") }
            };

            var fakeFileSyste = new MockFileSystem(contents);
            fakeFileSyste.Directory.CreateDirectory(output);
            var siteBuilder = new SiteBuilder(fakeFileSyste);

            // Act
            siteBuilder.CopyFiles(inputPath, output);

            // Assert
            fakeFileSyste.Directory.EnumerateFiles(output, "*.*", System.IO.SearchOption.AllDirectories).Count().ShouldBe(2);

        }

        [Fact]
        public void TestBuildCallsOtherMethods()
        {
            var mockSiteBuilder = new Mock<SiteBuilder>(MockBehavior.Strict, this.fakeFileSystem);
            mockSiteBuilder.Setup(sb => sb.CleanFolder(output));
            mockSiteBuilder.Setup(sb => sb.CopyFiles(inputPath, output));
            SiteBuilder siteBuilder = mockSiteBuilder.Object;

            siteBuilder.Build(inputPath, output);

            mockSiteBuilder.Verify(sb => sb.CleanFolder(output));
            mockSiteBuilder.Verify(sb => sb.CopyFiles(inputPath, output));
            
        }
        private void AssertDirectoryIsEmpty(string output)
        {
            fakeFileSystem.Directory.Exists(output).ShouldBeTrue();
            fakeFileSystem.Directory.EnumerateFiles(output).Any().ShouldBeFalse();
            fakeFileSystem.Directory.EnumerateDirectories(output).Any().ShouldBeFalse();
        }
    }
}
