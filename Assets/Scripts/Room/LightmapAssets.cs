using UnityEngine;

[CreateAssetMenu(menuName = "Lighting/Lightmap Data")]
public class LightmapAssets : ScriptableObject
{
    public Texture2D[] lightmapColor;
    public Texture2D[] lightmapDir;
    public Texture2D[] showdowMask;
}
