using UnityEngine.EventSystems;

public abstract class UIParamBase { }

public abstract class UIBase : UIBehaviour
{
    public abstract EUIType EuiTypeValue { get; }
    public virtual void OpenUI(UIParamBase param)
    {
        gameObject.SetActive(true);
    }

    public virtual void CloseUI()
    {
        gameObject.SetActive(false);
    }
}
