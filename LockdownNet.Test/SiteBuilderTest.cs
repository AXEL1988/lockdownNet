﻿using LockdownNet.Build;
using Moq;
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
        private void AssertDirectoryIsEmpty(string output)
        {
            fakeFileSystem.Directory.Exists(output).ShouldBeTrue();
            fakeFileSystem.Directory.EnumerateFiles(output).Any().ShouldBeFalse();
            fakeFileSystem.Directory.EnumerateDirectories(output).Any().ShouldBeFalse();
        }
    }
}
