namespace Book.CraftingInterpreters.lox;

public class TokenMap
{
    private static readonly TokenMap _instance = new();
    private Dictionary<string, TokenType> _keywords;
    private static TokenMap Instance => _instance;
    
    private TokenMap()
    {
        SetupKeywords();
    }
    
    /// <summary>
    /// insert the needed keywords into out dictionary of keywords
    /// </summary>
    private void SetupKeywords()
    {
        if (_keywords == null)
        {
            _keywords = new Dictionary<string, TokenType>();
        }
        
        _keywords.Add("and", TokenType.AND);
        _keywords.Add("class", TokenType.CLASS);
        _keywords.Add("else", TokenType.ELSE);
        _keywords.Add("false", TokenType.FALSE);
        _keywords.Add("for", TokenType.FOR);
        _keywords.Add("fun", TokenType.FUN);
        _keywords.Add("if", TokenType.IF);
        _keywords.Add("nil", TokenType.NIL);
        _keywords.Add("or", TokenType.OR);
        _keywords.Add("print", TokenType.PRINT);
        _keywords.Add("return", TokenType.RETURN);
        _keywords.Add("super", TokenType.SUPER);
        _keywords.Add("this", TokenType.THIS);
        _keywords.Add("true", TokenType.TRUE);
        _keywords.Add("var", TokenType.VAR);
        _keywords.Add("while", TokenType.WHILE);
    }

    /// <summary>
    /// Try to get the value from our keyword dictionary
    /// </summary>
    /// <param name="key"></param>
    /// <returns>null = not found, else it will return the matching keyword as a string like 'else'</returns>
    public static TokenType? Get(string key)
    {
        return Instance._keywords.TryGetValue(key, out var value) ? value : null;
    }
}