using UnityEngine;


[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public Color[] baseColors;
    [Range(0, 1)]
    public float[] baseStartHeights;

    float savedMinHeight;
    float savedMaxHeight;

    public static class ShaderProps
    {
        public const string minHeight = "_minHeight";
        public const string maxHeight = "_maxHeight";

        public const string baseColorCount = "_baseColorCount";
        public const string baseColors = "_baseColors";
        public const string baseStartHeights = "_baseStartHeights";
    }

    public void ApplyToMaterial(Material material)
    {
        material.SetInt(ShaderProps.baseColorCount, baseColors.Length);
        material.SetColorArray(ShaderProps.baseColors, baseColors);
        material.SetFloatArray(ShaderProps.baseStartHeights, baseStartHeights);

        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        savedMinHeight = minHeight;
        savedMaxHeight = maxHeight;

        material.SetFloat(ShaderProps.minHeight, minHeight);
        material.SetFloat(ShaderProps.maxHeight, maxHeight);
    }
}
