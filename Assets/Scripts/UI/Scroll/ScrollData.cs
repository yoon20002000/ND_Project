interface IScrollItem<in T> where T : ScrollData
{
    public void OnUpdateScrollData(T data);
}

public abstract class ScrollData
{
    public int Index { get; private set; }
}