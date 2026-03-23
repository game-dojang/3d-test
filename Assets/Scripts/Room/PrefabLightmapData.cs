using UnityEngine;

public class PrefabLightmapData : MonoBehaviour
{
    public LightmapAssets lightmapAsset;
    public RendererLightmapInfo[] rendererLightmapInfo;

    public void ApplyLightmap()
    {
        if (lightmapAsset == null)
            return;

        var lightmaps = new LightmapData[lightmapAsset.lightmapColor.Length];

        for (int i = 0; i < lightmaps.Length; i++)
        {
            lightmaps[i] = new LightmapData();
            lightmaps[i].lightmapColor = lightmapAsset.lightmapColor[i];

            if (lightmapAsset.lightmapDir != null &&
                i < lightmapAsset.lightmapDir.Length)
            {
                lightmaps[i].lightmapDir = lightmapAsset.lightmapDir[i];
            }

            if (lightmapAsset.showdowMask != null &&
                i < lightmapAsset.showdowMask.Length)
            {
                lightmaps[i].shadowMask = lightmapAsset.showdowMask[i];
            }
        }
        
        LightmapSettings.lightmaps = lightmaps;

        foreach (var info in rendererLightmapInfo)
        {
            if (info.renderer == null)
            {
                continue;
            }

            info.renderer.lightmapIndex = info.lightmapIndex;
            info.renderer.lightmapScaleOffset = info.scaleOffset;
        }
    }

    private void Start()
    {
        ApplyLightmap();
    }
}
