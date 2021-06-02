﻿using AngleSharp;
using AngleSharp.Dom;
using LockdownNet.Build;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
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
        //[Fact]
        //public void TestBuildCallsClean()
        //{
        //    var mockSiteBuilder = new Mock<SiteBuilder>(MockBehavior.Strict, this.fakeFileSystem);
        //    mockSiteBuilder.Setup(sb => sb.CleanFolder(output));
        //    SiteBuilder siteBuilder = mockSiteBuilder.Object;

        //    siteBuilder.Build(inputPath, output);

        //    mockSiteBuilder.Verify(sb => sb.CleanFolder(output));
        //}

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

        /*
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
        */

        private void AssertDirectoryIsEmpty(string output)
        {
            fakeFileSystem.Directory.Exists(output).ShouldBeTrue();
            fakeFileSystem.Directory.EnumerateFiles(output).Any().ShouldBeFalse();
            fakeFileSystem.Directory.EnumerateDirectories(output).Any().ShouldBeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public void TestGetPostsWithSinglePost(int files)
        {
            var postsPath = this.fakeFileSystem.Path.Combine(inputPath, "posts");
            this.fakeFileSystem.Directory.CreateDirectory(postsPath);
            var fileContents = new List<string>();
            for (var i = 0; i < files; i++)
            {
                var postPath = this.fakeFileSystem.Path.Combine(postsPath, $"file_{i}.txt");
                var content = "# Hola Mundo!\n\n**Prueba {i}**";
                this.fakeFileSystem.File.WriteAllText(postPath, content);
                fileContents.Add(content);
            }
            var siteBuilder = new SiteBuilder(this.fakeFileSystem);

            var posts = siteBuilder.GetPosts(inputPath);

            posts.OrderBy(content => content).ShouldBe(fileContents);
        }

        private async Task<IDocument> ParseHtml(string document)
        {
            var context = BrowsingContext.New(Configuration.Default);
            return await context.OpenAsync(req => req.Content(document));
        }

        [Fact]
        public async Task TestRenderContent()
        {
            // Setup: Prepare our test by copying our `demo` folder into our "fake" file system.
            var workspace = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../"));
            var templatePath = Path.Combine(workspace, "LockdownNet", "demo", "templates");
            var dictionary = new Dictionary<string, MockFileData>();
            foreach (var path in Directory.EnumerateFiles(templatePath))
            {
                var fakePath = path.Replace(templatePath, Path.Combine(inputPath, "templates"));
                dictionary.Add(fakePath, new MockFileData(File.ReadAllBytes(path)));
            }
            var fakeFileSystem = new MockFileSystem(dictionary);

            var metadata = new RawPostMetadata { Title = "Test post", Date = new DateTime(2000, 1, 1) };
            var postContent = "Hola Mundo!" + Environment.NewLine + "Hola";
            var siteBuilder = new SiteBuilder(fakeFileSystem);

            // Act
            var convertedPost = siteBuilder.RenderContent(metadata, postContent, inputPath);


            // Assert
            var html = await this.ParseHtml(convertedPost);

            var heading1 = html.All.First(node => node.LocalName == "h1");
            heading1.TextContent.ShouldBe("Test post");
        }

    }
}
