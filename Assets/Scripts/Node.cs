using Unity.Mathematics;

public struct Node
{
    public int2 Position;
    public int FScore;

    public int CompareTo(Node other)
    {
        return FScore.CompareTo(other.FScore);
    }
}