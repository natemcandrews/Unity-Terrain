using UnityEngine;
using System.Linq;


[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    const int textureSize = 512;
    const TextureFormat textureFormat = TextureFormat.RGB565;

    public Layer[] layers;

    [Range(0, 1)]
    public float smoothness;
    [Range(0, 1)]
    public float metallic;

    float savedMinHeight;
    float savedMaxHeight;

    public static class ShaderProps
    {
        public const string minHeight = "_minHeight";
        public const string maxHeight = "_maxHeight";

        public const string layerCount = "_layerCount";
        public const string baseColors = "_baseColors";
        public const string baseStartHeights = "_baseStartHeights";
        public const string baseBlends = "_baseBlends";
        public const string baseColorStrength = "_baseColorStrength";

        public const string baseTextureScales = "_baseTextureScales";
        public const string baseTextures = "_baseTextures";

        public const string smoothness = "_smoothness";
        public const string metallic = "_metallic";
    }

    public void ApplyToMaterial(Material material)
    {
        material.SetInt(ShaderProps.layerCount, layers.Length);
        material.SetColorArray(ShaderProps.baseColors, layers.Select(x => x.tint).ToArray());
        material.SetFloatArray(ShaderProps.baseColorStrength, layers.Select(x => x.tintStrength).ToArray());
        material.SetFloatArray(ShaderProps.baseStartHeights, layers.Select(x => x.startHeight).ToArray());
        material.SetFloatArray(ShaderProps.baseBlends, layers.Select(x => x.blendStrength).ToArray());
        material.SetFloatArray(ShaderProps.baseTextureScales, layers.Select(x => x.textureScale).ToArray());

        Texture2DArray texturesArray = GenerateTextureArray(layers.Select(x => x.texture).ToArray());
        material.SetTexture(ShaderProps.baseTextures, texturesArray);

        material.SetFloat(ShaderProps.smoothness, smoothness);
        material.SetFloat(ShaderProps.metallic, metallic);

        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
        Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);

        for (int i = 0; i < textures.Length; i++)
        {
            textureArray.SetPixels(textures[i].GetPixels(), i);
        }
        textureArray.Apply();

        return textureArray;
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        savedMinHeight = minHeight;
        savedMaxHeight = maxHeight;

        material.SetFloat(ShaderProps.minHeight, minHeight);
        material.SetFloat(ShaderProps.maxHeight, maxHeight);
    }

    [System.Serializable]
    public class Layer
    {
        public Texture2D texture;
        public Color tint;
        [Range(0, 1)]
        public float tintStrength;
        [Range(0, 1)]
        public float startHeight;
        [Range(0, 1)]
        public float blendStrength;
        public float textureScale;
    }
}
