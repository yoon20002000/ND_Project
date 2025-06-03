using UnityEngine;

public class UIManager : MonoSingletonPersistent<UIManager>
{
    [SerializeField]
    private Canvas mainCanvas;
    [SerializeField]
    private UIList uiList;

    // to do create layer

    private GameObject mainUI;
    public void OpenUI(string uiName, Camera mainCamera, Vector3 worldPos)
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

        GameObject prefab = uiList.GetUIPrefab(uiName);

        if (prefab == null)
        {
            Debug.LogWarning($"{uiName} not found");
            return;
        }

        mainUI = Instantiate(prefab, mainCanvas.transform, true);
        
        RectTransform rectTransform = mainUI.transform as RectTransform;
        if (rectTransform != null)
        {
            rectTransform.position = screenPos;
        }
        mainUI.gameObject.SetActive(true);
    }

    public void CloseUI(string uiName)
    {
        if (mainUI != null)
        {
            Destroy(mainUI);    
        }
    }
}
