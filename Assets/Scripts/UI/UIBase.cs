using UnityEngine.EventSystems;

public abstract class UIBase : UIBehaviour
{
    public abstract UIEnum UIEnumValue { get; }
    public virtual void OpenUI()
    {
        gameObject.SetActive(true);
    }

    public virtual void CloseUI()
    {
        gameObject.SetActive(false);
    }
}
