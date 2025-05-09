using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

public static class Program
{
    static string PATH = Environment.GetEnvironmentVariable("PATH");

    public static int Main(string[] args)
    {
        while (true)
        {
            // Wait for user input
            Console.Write("$ ");
            var command = Console.ReadLine();
            
            if (string.IsNullOrEmpty(command))
                continue;
                
            if (command == "exit 0")
            {
                return 0;
            }
            else if (command.StartsWith("echo "))
            {
                Console.WriteLine(command.Substring("echo ".Length));
            }
            else if (command.StartsWith("type"))
            {
                CheckType(command.Substring("type ".Length).Trim());
            }
            else
            {
                // Just defer to the shell to execute the command
                // This will solve the issue with argv[0]
                RunProgramUsingShell(command);
            }
        }
    }
    public static void printCommandNotFound(string command)
    {
        Console.WriteLine($"{command}: command not found");
    }

    private static void RunProgramUsingShell(string command)
    {
        var commandParts = command.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        var commandPath = FindExecutable(commandParts[0]);

        if (commandPath == null){
            printCommandNotFound(commandParts[0]);
            return;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = commandParts[0],
            Arguments = command.Substring(commandParts[0].Length).Trim(),
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        
        var process = new Process { StartInfo = startInfo };
        process.Start();
        
        // // Read output
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        
        Console.Write(output);
        if (!string.IsNullOrEmpty(error))
            printCommandNotFound(command);
            Console.Error.Write(error);
            
        process.WaitForExit();
    }

    public static void CheckType(string command)
    {
        string[] builtInCommands = { "echo", "exit", "type", "pwd"};
        if (builtInCommands.Contains(command))
        {
            Console.WriteLine($"{command} is a shell builtin");
            return;
        }
        string result = FindExecutable(command);
        if (result != null)
        {
            System.Console.WriteLine($"{command} is {result}");
        }
        else
        {
            Console.WriteLine($"{command}: not found");
        }
    }

    private static string FindExecutable(string command)
    {
        string[] paths = PATH.Split(":", StringSplitOptions.RemoveEmptyEntries);
        foreach (var path in paths)
        {
            string fullPath = Path.Combine(path, command);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }
        return null;
    }
}