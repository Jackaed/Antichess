namespace Antichess.AI
{
    public enum NodeType : byte
    {
        NotEvaluated,
        Exact,
        UpperBound,
        LowerBound
    }
}