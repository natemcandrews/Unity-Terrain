using UnityEngine;


[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public Color[] baseColors;
    [Range(0, 1)]
    public float[] baseStartHeights;

    public Texture2D[] textures;

    float savedMinHeight;
    float savedMaxHeight;

    public void ApplyToMaterial(Material material)
    {
        //Texture2D texture = TextureGenerator.HeightBasedTexture(baseColors, baseStartHeights, savedMaxHeight);

        //material.SetTexture("_heightBasedColorTexture", texture);

        Texture2D heightBasedTexture = TextureGenerator.CombineTextures(textures, baseStartHeights, textures[0].width, textures[0].width);

        material.SetTexture("_heightBasedTexture", heightBasedTexture);

        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        savedMinHeight = minHeight;
        savedMaxHeight = maxHeight;

        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
}
