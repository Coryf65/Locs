using System.Text.RegularExpressions;

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
            case ' ': break;
            case '\r': break;
            case '\t': break;
            case '\n' :
                _line++;
                break;
            case '"':
                // strings
                String();
                break;
            default:
                Locs.Error(_line, $"unexpected character '{c}'");
                break;
        }
    }

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
    /// determine if the current char is '>' or '>='
    /// </summary>
    /// <param name="expected"></param>
    /// <returns></returns>
    private bool Match(char expected)
    {
        bool isExpected = false;

        if (IsAtEnd())
            return isExpected;

        if (_source[_current] != expected)
            return isExpected;
        
        _current++;
        isExpected = true;
        
        return isExpected;
    }

    /// <summary>
    /// Peek at the next token in the source.
    /// </summary>
    /// <returns>next char found, if at the end '\0' is returned.</returns>
    private char Peek()
    {
        if (IsAtEnd())
            return '\0';
        
        return _source[_current];
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