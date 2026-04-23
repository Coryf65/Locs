namespace Book.CraftingInterpreters.lox;

public class Scanner
{
    private string _source;
    private List<Token> _tokens;
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;
    private const string EOF = "EOF";
    private const char ENDCHAR = '\0'; // It marks the end of a character sequence in memory, from C

    private bool IsAtEnd() => _current >= _source.Length;
    
    public Scanner(string source)
    {
        _source = source;
        _tokens = new();
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            // we are at the beginning of the next lexeme
            _start = _current;
            ScanToken();
        }
        
        _tokens.Add(new Token(TokenType.EOF, "", null, _line));
        return _tokens;
    }

    private void ScanToken()
    {
        char c = Advance();

        Console.WriteLine($"current token      : {c}");
        
        switch (c)
        {
            case '(':
                AddToken(TokenType.LEFT_PAREN);
                break;
            case ')':
                AddToken(TokenType.RIGHT_PAREN);
                break;
            case '{':
                AddToken(TokenType.LEFT_BRACE);
                break;
            case '}':
                AddToken(TokenType.RIGHT_BRACE);
                break;
            case ',':
                AddToken(TokenType.COMMA);
                break;
            case '.':
                AddToken(TokenType.DOT);
                break;
            case '-':
                AddToken(TokenType.MINUS);
                break;
            case '+':
                AddToken(TokenType.PLUS);
                break;
            case '*':
                AddToken(TokenType.STAR);
                break;
            case ';':
                AddToken(TokenType.SEMICOLON);
                break;
            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.EQUAL);
                break;
            case '/':
                // is a bit special as our comments start with '//'
                if (Match('/')) {
                    // a comment goes until the end of the line
                    while(Peek() != '\n' && !IsAtEnd())
                            Advance();
                } else {
                    AddToken(TokenType.SLASH);
                }
                break;
            // skip meaningless chars to our parser
            case ' ':
            case '\r':
            case '\t': break;
            case '\n' :
                _line++;
                break;
            case '"':
                // starts with a double quote then is must be a string
                String();
                break;
            default:
                if (IsDigit(c))
                {
                    // if this is a digit it must be a number
                    Number();
                }
                else if (IsAlpha(c))
                {
                    // when the lexeme starts with a letter or underscore must be an identifier
                    Identifier();
                }
                else
                {
                    Locs.Error(_line, $"unexpected character '{c}'");
                }
                break;
        }
    }

    /// <summary>
    /// Check for keywords
    /// </summary>
    private void Identifier()
    {
        while (IsAlphaNumeric(Peek()))
        {
            Advance();
        }
        
        string text = _source.Substring(_start, _current);
        TokenType? type = TokenMap.Get(text);
        if (type == null)
            type = TokenType.IDENTIFIER;
        AddToken((TokenType)type);
    }

    /// <summary>
    /// Checks for the opening and closing of a string
    /// </summary>
    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
                _line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Locs.Error(_line, "unterminated string.");
            return;
        }
        
        // the closing "
        Advance();
        
        // trim the surrounding quotes
        string value = _source.Substring(_start + 1, _current - 1);
        AddToken(TokenType.STRING, value);
        // if we would handle escape sequences we'd do that here too.
    }

    /// <summary>
    /// Checks if the char is a valid digit. using a simple digit match no fancy digits.
    /// </summary>
    /// <param name="c">char to look at</param>
    /// <returns>true = is a valid digit, false = NOT a valid digit for this</returns>
    private bool IsDigit(char c)
    {
        // could have used the built-in IsDigit() but anted to restrict the acceptable chars more.
        return c is >= '0' and <= '9';
    }

    /// <summary>
    /// When we know this is a number then we are advancing to incrementer and adding to the tokenizer
    /// </summary>
    private void Number()
    {
        while (IsDigit(Peek()))
        {
            Advance();
        }
        
        // looking for a fractional part
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            // consume the '.'
            Advance();
            
            while (IsDigit(Peek()))
            {
                Advance();
            }
        }
        
        // Note: set to 1 char
        AddToken(TokenType.NUMBER, _source.Substring(_start, 1));
    }
    
    /// <summary>
    /// Checks if the char is a valid Alpha character
    /// </summary>
    /// <param name="c">char to check</param>
    /// <returns>true = valid alpha, false = NOT valid</returns>
    private bool IsAlpha(char c)
    {
        // pattern match version from the example
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
    }

    /// <summary>
    /// Checks if the char is a valid Alpha Numeric character
    /// </summary>
    /// <param name="c">char to check</param>
    /// <returns>true = valid alpha-numeric, false = NOT valid</returns>
    private bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

    /// <summary>
    /// determine if the current char is '>' or '>='
    /// </summary>
    /// <param name="expected"></param>
    /// <returns></returns>
    private bool Match(char expected)
    {
        if (IsAtEnd())
            return false;

        if (_source[_current] != expected)
            return false;
        
        _current++;
        return true;
    }

    /// <summary>
    /// Peek at the next token in the source, and return the result.
    /// </summary>
    /// <returns>next char found, if at the end '\0' is returned.</returns>
    private char Peek()
    {
        if (IsAtEnd())
            return ENDCHAR;
        
        return _source[_current];
    }

    /// <summary>
    /// Peek ahead 2 tokens (the 'next next' token) and return the result.
    /// </summary>
    /// <returns>second char found, if at the end '\0' is returned.</returns>
    private char PeekNext()
    {
        if (_current + 1 >= _source.Length)
            return ENDCHAR;
        
        return _source[_current + 1];
    }

    /// <summary>
    /// Consumes the next character in the source file and returns it.
    /// </summary>
    /// <returns></returns>
    private char Advance()
    {
        return _source[_current++];
    }

    /// <summary>
    /// Grabs the text of the current lexeme and creates a new token for it.
    /// </summary>
    /// <param name="type"></param>
    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object literal)
    {
        // Note: set to 1 char
        string text = _source.Substring(_start, 1);
        _tokens.Add(new Token(type, text, literal, _line));
    }
}