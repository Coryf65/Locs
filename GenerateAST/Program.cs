using System.Text;

Console.WriteLine("Generate AST code templates!\nby: Cory");

string outputDirectory = string.Empty;
string ast = string.Empty;
List<string> types = new();

if (args.Length <= 0)
    Console.WriteLine("usage: ./generate_ast.exe -o <output_directory>");

for (int i = 0; i < args.Length; i++)
{
    string currentArg = args[i];
    
    Console.WriteLine($"Argument={currentArg}");
    
    // default as of now, until I see more about how this will be used
    types.Add("Binary   : Expr left, Token operator, Expr right");
    types.Add("Grouping : Expr expression");
    types.Add("Literal  : Object value");
    types.Add("Unary    : Token operator, Expr right");
    
    ast = DefineASTText(currentArg, null);

    switch (currentArg)
    {
        case "-help":
        case "?":
        case "-h":
            Console.WriteLine("helper text for this CLI tool");
            break;
        case "-c":
            // print to console
            WriteToConsole(ast);
            break;
        case "-o":
            // write to file, next on is the value
            outputDirectory = args[i+1];
            WriteToFilePath(outputDirectory, ast);
            break;
        default:
            Console.WriteLine("usage: ./generate_ast.exe -o <output_directory> -c 'writes to current console'");
            break;
    }
}

return 0;

string DefineASTText(string baseName, List<string> types)
{
    StringBuilder ast = new();

    // temp test to see how this will work...
    ast.Append("namespace Book.CraftingInterpreters.lox;");
    ast.Append(string.Empty);
    ast.Append($"public class {baseName} : Expression");
    ast.Append("{");
    ast.AppendLine("    private Expression _left;");
    ast.AppendLine("    private Token _operator;");
    ast.AppendLine("    private Expression _right;");
    ast.AppendLine("");
    ast.AppendLine($"   public {baseName}(Expression left, Token theOperator, Expression right)");
    ast.Append("    {");
    ast.AppendLine("    _left = left;");
    ast.AppendLine("    _operator = theOperator;");
    ast.AppendLine("    _right = right;");
    ast.AppendLine("    }");
    ast.AppendLine("}");
    
    // foreach (var type in types)
    // {
    //     ast.Append($"public {type}() {baseName}{type}");
    // }
    
    return ast.ToString();
}

// outputs the AST into the console for a copy-paste
void WriteToConsole(string astText)
{
    Console.WriteLine(astText);
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