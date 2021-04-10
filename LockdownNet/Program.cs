namespace LockdownNet
{
    using System;
    using System.IO;

    public class Program
    {
        public static void Main(string[] args)
        {
            var arguments = string.Join("; ", args);
            var directory = Directory.GetCurrentDirectory();
            Console.WriteLine($"Hola me estoy ejecutando en: {directory}");
            Console.WriteLine(arguments);
        }
    }
}
