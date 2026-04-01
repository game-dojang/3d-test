using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class LightmapPrefabBaker
{
#if UNITY_EDITOR
    [MenuItem("Tools/Bake Prefab Lightmap")]
    static void Bake()
    {
        var root = Selection.activeGameObject;

        if (root == null)
        {
            Debug.LogError("No root selected");
            return;
        }

        var component = root.GetComponent<PrefabLightmapData>();

        if (component == null)
        {
            component = root.AddComponent<PrefabLightmapData>();
        }

        var renderers = root.GetComponentsInChildren<Renderer>();

        component.rendererLightmapInfo = renderers.Select(r => new RendererLightmapInfo
        {
            renderer = r,
            lightmapIndex = r.lightmapIndex,
            scaleOffset = r.lightmapScaleOffset
        }).ToArray();

        var lightmaps = LightmapSettings.lightmaps;

        var asset = ScriptableObject.CreateInstance<LightmapAssets>();

        asset.lightmapColor = lightmaps.Select(l => l.lightmapColor).ToArray();
        asset.lightmapDir = lightmaps.Select(l => l.lightmapDir).ToArray();
        asset.showdowMask = lightmaps.Select(lightmap => lightmap.shadowMask).ToArray();

        string path = "Assets/Lightmaps/" + root.name + "_Lightmap.asset";

        AssetDatabase.CreateAsset(asset, path);
        component.lightmapAsset = asset;
        EditorUtility.SetDirty(root);

        Debug.Log("Baked");
    }
#endif
}
