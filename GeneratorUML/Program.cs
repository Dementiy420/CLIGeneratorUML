using DotMake.CommandLine;
using GeneratorUML.CliCommands;
public class Program
{
    static void Main(string[] args)
    {
        Cli.Run<GenerateDiagram>();
    }
}