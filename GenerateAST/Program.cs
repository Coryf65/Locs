using System.Text;

// a very simple class generator so I do not have to manually do this everytime.
// meant to be run from cli as a dotnet tool, but can be run from the debugger or whatever...
string filename = string.Empty;
string ast = string.Empty;
string name = string.Empty;
// default as of now, until I see more about how this will be used
List<string> types =
[
    "Binary   : Expr left, Token operator, Expr right",
    "Grouping : Expr expression",
    "Literal  : Object value",
    "Unary    : Token operator, Expr right"
];

while (true)
{
    // simple arg checks, this would be the start or another run.
    if (args.Length == 0)
        ResetFromStart();
    
    if (args.Length <= 0 || args[0] == string.Empty)
    {
        args = Console.ReadLine()?.Split(' ')!;
        continue;
    }
    
    for (int i = 0; i < args.Length; i++)
    {
        // not the flag skip to next flag
        if (!args[i].Contains("-"))
            continue;
        
        if (!args.Contains("-name"))
        {
            Console.WriteLine("please add -name <className>");
            break;
        }
        
        // debug fun
        string currentArg = args[i];
        string value = string.Empty;
        
        // get the value, if exists
        if (args.Length - 1 >=  i + 1)
            value = args[i + 1];
        
        Console.WriteLine($"argument={currentArg}");
        Console.WriteLine($"value={value}");
        
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
                ast = DefineASTText(value, types);
                name = value;
                break;
            case "-c":
                // print to console
                WriteToConsole(ast);
                break;
            case "-o":
                // write to file, next on is the value
                filename = value;
                WriteToFilePath(name, filename, ast);
                break;
            default:
                Console.WriteLine("usage: ./generate_ast.exe -name <nameOfClass> -o <filename/default(nameOfClass.cs)> or -c 'outputs into console'");
                break;
        }
    }
    // reset for another run
    args = [];
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
void WriteToFilePath(string name, string filename, string astText)
{
    if (filename == string.Empty)
        filename = name;

    Console.WriteLine(Environment.CurrentDirectory);
    Console.WriteLine(filename);
    
    string outputPath = Path.Combine(Environment.CurrentDirectory, filename);
    
    if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);

    if (Path.HasExtension(outputPath))
    {
        // TODO: file permission issues within `/BIN` on Linux, look into this...
        File.WriteAllText(outputPath, astText);
    }
    else
    {
        outputPath = outputPath + ".cs";
        File.WriteAllText(outputPath, astText);
                    
    }
}

void ResetFromStart()
{
    string aboutText = "----------------------------\nGenerate AST code templates!\nby: Cory\n----------------------------\n";
    string usageText = "usage: ./generate_ast.exe -name <className> -o <filename default: className.cs> or -c 'outputs into console'";
    
    Console.Clear();
    Console.WriteLine(aboutText);
    Console.WriteLine(usageText);
}