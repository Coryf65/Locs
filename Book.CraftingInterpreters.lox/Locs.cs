namespace Book.CraftingInterpreters.lox;

public class Locs
{
    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: locs [script]");
            return; // exit the application
        } 
        
        if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    /// <summary>
    /// Runs locs from the interpreter, write code and run right away
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private static void RunPrompt()
    {
        StreamReader inputStream = new StreamReader(Console.OpenStandardInput());

        while(true)
        {
            Console.WriteLine("> ");
            string? line = inputStream.ReadLine();
            if (line == null)
                break;
            Run(line);
            
        }
    }

    /// <summary>
    /// Run locs from a file, runs and executes the locs file
    /// </summary>
    /// <param name="path">path to the source locs file</param>
    private static void RunFile(string path)
    {
        byte[] file = File.ReadAllBytes(Path.GetFullPath(path));
        Run(file.ToString());
    }

    /// <summary>
    /// Runs the given prompt right away printing each token found for now.
    /// </summary>
    /// <param name="source"></param>
    private static void Run(string? source)
    {
        if (source == null)
            return;
        
        string[] tokens = source.Split(' ');
        
        foreach (string token in tokens)
            Console.WriteLine(token);
    }
}