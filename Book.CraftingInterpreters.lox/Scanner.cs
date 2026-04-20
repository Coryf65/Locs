namespace Book.CraftingInterpreters.lox;

public class Scanner
{
    private string _source;
    private List<Token> _tokens;
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;

    private bool IsAtEnd() => _current >= _source.Length;
    
    public Scanner(string source)
    {
        _source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            // we are at the beginning of the next lexeme
            _start = _current;
            ScanTokens();
        }
        
        _tokens.Add(new Token(TokenType.EOF, "", null, _line));
        return _tokens;
    }

    private void ScanToken()
    {
        char c = Advance();
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
            default:
                Locs.Error(_line, $"unexpected character '{c}'");
                break;
        }
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
        string text = _source.Substring(_start, _current);
        _tokens.Add(new Token(type, text, literal, _line));
    }
}