using UnityEngine;

public abstract class ScrollItemBase : MonoBehaviour
{
    protected int scrollItemIndex;
    public int GetIndex() => scrollItemIndex;
    public void SetIndex(int index) => scrollItemIndex = index;
    
    protected Transform scrollItemTransform;
    public Transform GetTransform() => scrollItemTransform;

    public virtual void Initialize(int index, Transform inScrollItemTransform)
    {
        SetIndex(index);
        scrollItemTransform = inScrollItemTransform;
    }
    
    public abstract void SetData(ScrollData data);
}