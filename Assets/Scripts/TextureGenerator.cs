using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }

    public static Texture2D HeightBasedTexture(Color[] colorMap, float[] baseStartHeights, float maxHeight)
    {
        int width = 1;
        int height = Mathf.RoundToInt(maxHeight);

        if (height <= 0)
        {
            return new Texture2D(width, height);
        }

        if (colorMap.Length != baseStartHeights.Length)
        {
            return new Texture2D(width, height);
        }

        Color[] scaledColorMap = new Color[Mathf.RoundToInt(height)];
        Texture2D texture = new Texture2D(width, height);

        int numSegments = baseStartHeights.Length;
        int[] startIndices = new int[numSegments];

        for (int i = 0; i < numSegments; i++)
        {
            startIndices[i] = Mathf.RoundToInt(baseStartHeights[i] * height);
        }



        for (int i = 0; i < numSegments; i++)
        {
            int nextStep = (i < numSegments - 1) ? startIndices[i + 1] : height;
            int start = startIndices[i];

            if (start > nextStep)
                continue;


            Array.Fill(scaledColorMap, colorMap[i], Mathf.RoundToInt(baseStartHeights[i] * height), nextStep - Mathf.RoundToInt(baseStartHeights[i] * height));
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(scaledColorMap);
        texture.Apply();

        return texture;
    }

    public static Texture2DArray Texture2DArrayFromColorMap(Color[] colorMap) //Unlikely to be used
    {
        int slices = colorMap.Length;

        Texture2DArray textureArray = new Texture2DArray(1, 1, slices, TextureFormat.RGBA32, false);

        for(int i = 0; i < slices; i++)
        {
            Color currentColor = colorMap[i];

            Color[] colors = new Color[1];
            colors[0] = currentColor;

            textureArray.SetPixels(colors, i);
        }

        textureArray.Apply();

        return textureArray;
    }

    public static Texture2D CombineTextures(Texture2D[] textures, float[] baseStartHeights, int width, int height)
    {
        Texture2D finalTex = new Texture2D(width, height);

        int y = 0;
        for(int i = 0; i < textures.Length; i++)
        {
            Texture2D temp = textures[i];

            int sliceWidth = textures[i].width;
            int sliceHeight = Mathf.RoundToInt(height * baseStartHeights[i]) - y;

            finalTex.SetPixels(0, y, sliceWidth, sliceHeight, textures[i].GetPixels(), 0);
            y = Mathf.RoundToInt(height * baseStartHeights[i]);
        }

        finalTex.Apply();

        return finalTex;
    }
}
