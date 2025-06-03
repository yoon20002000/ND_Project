using UnityEngine;

public class UIManager : MonoSingletonPersistent<UIManager>
{
    [SerializeField]
    private Canvas mainCanvas;
    [SerializeField]
    private UIList uiList;

    

    private GameObject mainUI;
    public void OpenUI(UIEnum uiEnum, Camera mainCamera, Vector3 worldPos)
    {
        if (mainUI != null)
        {
            Destroy(mainUI);    
        }
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPos);

        GameObject prefab = uiList.GetUIPrefab(uiEnum);

        if (prefab == null)
        {
            Debug.LogWarning($"{uiEnum} not found");
            return;
        }

        mainUI = Instantiate(prefab, mainCanvas.transform, true);
        
        RectTransform rectTransform = mainUI.transform as RectTransform;
        if (rectTransform != null)
        {
            rectTransform.position = screenPos;
        }

        UIBase uiBase = mainUI.GetComponent<UIBase>();
        if (uiBase)
        {
            uiBase.OpenUI();
        }
    }

    public void CloseUI(string uiName)
    {
        if (mainUI != null)
        {
            Destroy(mainUI);    
        }
    }
}
