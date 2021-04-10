namespace LockdownNet.Commands
{
    using System;
    using McMaster.Extensions.CommandLineUtils;

    public class BuildCommand
    {

        public int OnExecute()
        {
            Console.WriteLine("You executed build command");
            return 0;
        }
    }
}
