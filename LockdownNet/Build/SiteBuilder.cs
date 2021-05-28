namespace LockdownNet.Build
{
    using System.IO.Abstractions;

    public class SiteBuilder : ISiteBuilder
    {
        private readonly IFileSystem fileSystem;

        public SiteBuilder(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public virtual void CleanFolder(string folder)
        {
            if (this.fileSystem.Directory.Exists(folder))
            {
                this.fileSystem.Directory.Delete(folder, recursive: true);
            }

            this.fileSystem.Directory.CreateDirectory(folder);
        }

        public void Build(string inputPath, string outputPath)
        {
            this.CleanFolder(outputPath);
        }
    }
}
