using System.Text;

// a very simple class generator so I do not have to manually do this everytime.
// meant to be run from cli as a dotnet tool, but can be run from the debugger or whatever...
Console.WriteLine("Generate AST code templates!\nby: Cory\n");

string outputDirectory = string.Empty;
string ast = string.Empty;
List<string> types = new();

while (true)
{
    // simple arg checks
    if (args.Length == 0)
        Console.WriteLine("usage: ./generate_ast.exe -name <nameOfClass> -o <output_directory> or -c 'outputs into console'");
    
    if (args.Length <= 0 || args[0] == string.Empty)
    {
        args = Console.ReadLine()?.Split(' ')!;
        continue;
    }
    
    for (int i = 0; i < args.Length; i++)
    {
        // debug fun
        string currentArg = args[i];
        string value = string.Empty;
        
        if (args.Length >=  i + 1)
            value = args[+1];
        
        Console.WriteLine($"Argument={currentArg}");
        
        // default as of now, until I see more about how this will be used
        types.Add("Binary   : Expr left, Token operator, Expr right");
        types.Add("Grouping : Expr expression");
        types.Add("Literal  : Object value");
        types.Add("Unary    : Token operator, Expr right");

        if (!args.Contains("-name"))
        {
            Console.WriteLine("please add -name <className>");
            break;
        }

        // check flags
        switch (currentArg)
        {
            case "-quit":
            case "-q":
                return;
            case "-help":
            case "?":
            case "-h":
                Console.WriteLine("helper text for this CLI tool");
                break;
            case "-name":
                
            case "-c":
                // print to console
                ast = DefineASTText(value, types);
                WriteToConsole(ast);
                break;
            case "-o":
                // write to file, next on is the value
                outputDirectory = value;
                WriteToFilePath(outputDirectory, ast);
                break;
            default:
                Console.WriteLine("usage: ./generate_ast.exe -o <output_directory> -c 'writes to current console'");
                break;
        }

        // reset for another run
        args = [];
    }
}

// creates the Abstract Syntax Tree (AST) class definition text
string DefineASTText(string baseName, List<string> types)
{
    StringBuilder ast = new();

    // temp test to see how this will work...
    ast.AppendLine("namespace Book.CraftingInterpreters.lox;");
    ast.AppendLine("");
    ast.AppendLine($"public class {baseName} : Expression");
    ast.AppendLine("{");
    ast.AppendLine("    private Expression _left;");
    ast.AppendLine("    private Token _operator;");
    ast.AppendLine("    private Expression _right;");
    ast.AppendLine("");
    ast.AppendLine($"   public {baseName}(Expression left, Token theOperator, Expression right)");
    ast.AppendLine("    {");
    ast.AppendLine("        _left = left;");
    ast.AppendLine("        _operator = theOperator;");
    ast.AppendLine("        _right = right;");
    ast.AppendLine("    }");
    ast.AppendLine("}");
    
    // maybe later?
    // foreach (var type in types)
    // {
    //     ast.Append($"public {type}() {baseName}{type}");
    // }
    return ast.ToString();
}

// outputs the AST into the console for a copy-paste
void WriteToConsole(string astText)
{
    Console.WriteLine("");
    Console.WriteLine(astText);
    Console.WriteLine("");
}

// outputs the AST into a file by the path we are giving it
void WriteToFilePath(string outputPath, string astText)
{
    if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);

    if (!File.Exists(outputPath))
    {
        if (Path.HasExtension(outputPath))
            File.WriteAllText(Path.GetFullPath(outputPath), astText);
        else
        {
            outputPath = Path.GetFullPath(outputPath + ".cs");
            File.WriteAllText(Path.GetFullPath(outputPath), astText);
                        
        }
    }
}