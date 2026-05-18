using UnityEngine;

public class LightmapSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class LightmapSet
    {
        public Texture2D[] lightmapsColor;
        public Texture2D[] lightmapsDir;
        public Texture2D[] shadowMasks;
    }

    [SerializeField] private LightmapSet[] lightmapSets;
    [SerializeField] private bool loadSet0OnStart = true;

    private void Start()
    {
        if (loadSet0OnStart && lightmapSets.Length > 0)
        {
            LoadLightmapSet(0);
        }
    }

    public void LoadLightmapSet(int index)
    {
        if (index < 0 || index >= lightmapSets.Length)
        {
            Debug.LogError($"Индекс {index} вне диапазона. Доступно {lightmapSets.Length} наборов.");
            return;
        }

        LightmapSet set = lightmapSets[index];

        if (set.lightmapsColor == null || set.lightmapsColor.Length == 0)
        {
            Debug.LogError($"Набор {index} не содержит карт освещения.");
            return;
        }

        UnityEngine.LightmapData[] newLightmapData = new UnityEngine.LightmapData[set.lightmapsColor.Length];

        for (int i = 0; i < set.lightmapsColor.Length; i++)
        {
            newLightmapData[i] = new UnityEngine.LightmapData();
            newLightmapData[i].lightmapColor = set.lightmapsColor[i];

            if (set.lightmapsDir != null && i < set.lightmapsDir.Length && set.lightmapsDir[i] != null)
                newLightmapData[i].lightmapDir = set.lightmapsDir[i];

            if (set.shadowMasks != null && i < set.shadowMasks.Length && set.shadowMasks[i] != null)
                newLightmapData[i].shadowMask = set.shadowMasks[i];
        }

        LightmapSettings.lightmaps = newLightmapData;
        Debug.Log($"Загружен набор карт освещения #{index} ({set.lightmapsColor.Length} карт)");
    }
}