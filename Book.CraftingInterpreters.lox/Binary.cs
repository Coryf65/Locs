namespace Book.CraftingInterpreters.lox;

public class Binary : Expression
{
    private Expression _left;
    private Token _operator;
    private Expression _right;
    
    public Binary(Expression left, Token theOperator, Expression right)
    {
        _left = left;
        _operator = theOperator;
        _right = right;
    }
}