namespace LockdownNet.Commands
{
    using LockdownNet.Build;
    using McMaster.Extensions.CommandLineUtils;

    public class BuildCommand
    {
        private readonly IConsole console;
        private readonly ISiteBuilder siteBuilder;

        public BuildCommand(IConsole console, ISiteBuilder siteBuilder)
        {
            this.console = console;
            this.siteBuilder = siteBuilder;
        }

        [Option("-r | --root")]
        public string InputPath {get; set;} = "./";

        [Option("-o | --output")]
        public string OutputPath { get; set; } = "./_site";

        public int OnExecute()
        {
            this.console.WriteLine($"Inputpath { this.InputPath }");
            this.console.WriteLine($"Output { this.OutputPath }");

            this.siteBuilder.Build(this.InputPath, this.OutputPath);

            return 0;
        }
    }
}
