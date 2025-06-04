using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UIManager : MonoSingletonPersistent<UIManager>
{
    [SerializeField]
    private Canvas mainCanvas;

    [SerializeField] 
    private UILayerGroup layerGroup;
    [SerializeField]
    private UIList uiList;

    private Dictionary<EUIType, UIBase> dicActivedUI = new Dictionary<EUIType, UIBase>();
    private Dictionary<EUILayer, Canvas> dicLayers = new Dictionary<EUILayer, Canvas>();

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Assert.IsNotNull(layerGroup);

        layerGroup.InitLayerGroup();

        for (int layerIndex = 0; layerIndex < layerGroup.Count; ++layerIndex)
        {
            UILayer uiLayer = layerGroup[layerIndex];
            if (dicLayers.ContainsKey(uiLayer.UILayerType))
            {
                Debug.LogWarning($"UI Layer {uiLayer.UILayerType} Already Exists");
                continue;
            }
            GameObject newCanvasObj = Instantiate(uiLayer.LayerPrefab, mainCanvas.transform);
            Canvas newCanvas = newCanvasObj.GetComponent<Canvas>();
            dicLayers.Add(uiLayer.UILayerType, newCanvas);
        }
    }

    public void OpenUI(EUIType eUIType, Camera mainCamera, Vector3 worldPos)
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (!dicActivedUI.TryGetValue(eUIType, out UIBase uiBase) || uiBase == null)
        {
            GameObject prefab = uiList.GetUIPrefab(eUIType);
            if (prefab == null)
            {
                Debug.LogWarning($"UI prefab for {eUIType.ToString()} not found.");
                return;
            }
            
            GameObject instance = Instantiate(prefab, mainCanvas.transform, true);
            if (!instance.TryGetComponent<UIBase>(out uiBase))
            {
                Debug.LogWarning($"{eUIType.ToString()} prefab does not have UIBase component.");
                Destroy(instance);
                return;
            }
            dicActivedUI.Add(eUIType, uiBase);
        }
        
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPos);

        if (uiBase.transform is RectTransform rectTransform)
        {
            rectTransform.position = screenPos;
        }
        
        uiBase.OpenUI();
    }

    public void CloseUI(EUIType eUIType, bool bDestroy = true)
    {
        if (!dicActivedUI.TryGetValue(eUIType, out UIBase ub))
        {
            return;
        }
        
        ub.CloseUI();
        if (bDestroy)
        {
            Destroy(ub.gameObject);
            dicActivedUI.Remove(eUIType);
        }
    }
}
