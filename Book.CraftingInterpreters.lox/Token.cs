namespace Book.CraftingInterpreters.lox;

public class Token
{
    private TokenType _type;
    private string _lexeme;
    private Object _literal;
    private int _line;

    public Token(TokenType type, string lexeme, Object literal, int line)
    {
        _type = type;
        _lexeme = lexeme;
        _literal = literal;
        _line = line;
    }

    public override string ToString()
    {
        return $"{_type} {_lexeme} : {_literal}";
    }
}