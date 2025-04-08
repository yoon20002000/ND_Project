using Unity.Assertions;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateBefore(typeof(GridSpawnerSystem))]
public class StageLoaderAuthoring : MonoBehaviour
{
    public string fileName;
    public const int STASGE_LOAD_TEXT_OFFSET = 1; 
    private class Baker : Baker<StageLoaderAuthoring>
    {
        public override void Bake(StageLoaderAuthoring authoring)
        {
            TextAsset stageData = Resources.Load<TextAsset>(authoring.fileName);
            if (stageData == null)
            {
                return;
            }

            string[] lines = stageData.text.Trim().Split('\n');
            string[] sizeTokens = lines[0].Trim().Split(',');
            
            int height ;
            
#if UNITY_EDITOR
            bool isWidthLoaded = 
#endif
            int.TryParse(sizeTokens[0], out height);
            int width ;
#if UNITY_EDITOR
            bool isHeightLoaded =
#endif
            int.TryParse(sizeTokens[1], out width);
#if UNITY_EDITOR
            Assert.IsTrue(isWidthLoaded && isHeightLoaded, "Width and Height are not loaded");
#endif
            
            Entity entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new StageLoader
            {
                Width = width,
                Height = height,
            });

            DynamicBuffer<StageMapBuffer> buffer = AddBuffer<StageMapBuffer>(entity);

            for (int y = 0; y < height; y++)
            {
                string[] tokens = lines[y + STASGE_LOAD_TEXT_OFFSET].Trim().Split(',');
                for (int x = 0; x < width; x++)
                {
                    int value ;
                    if (int.TryParse(tokens[x], out value))
                    {
                        buffer.Add(new StageMapBuffer
                        {
                            StageType = value,
                        });    
                    }
                    else
                    {
                        Assert.IsTrue(false, "Stage data is invalid!! line : " + (y + 1) + " column :  " + (x + 1));
                        break;
                    }
                }
            }
        }
    }
}

public struct StageLoader : IComponentData
{
    public int Width;
    public int Height;
}

public struct StageMapBuffer : IBufferElementData
{
    public int StageType; // 0 moveable, 1 blocked, 2: placeable
}