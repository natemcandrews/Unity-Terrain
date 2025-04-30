using UnityEngine;


[CreateAssetMenu()]
public class BiomeData : UpdatableData
{
    [Header("Biome Info")]
    public string biomeName;

    [Header("Texture Settings")]
    const int maxLayerCount = 8;
    public TextureData.Layer[] layers = new TextureData.Layer[maxLayerCount];
    [Range(0, 1)] public float smoothness;
    [Range(0, 1)] public float metallic;

    [Header("Noise Settings")]
    public Noise.NormalizeMode normalizeMode;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)] public float persistence;
    public float lacunarity;
    public int seed;
    public Vector2 offset;

    [Header("Terrain Settings")]
    public bool useFalloff;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    public bool useFlatShading;
    public float uniformScale = 2.5f;

    private TextureData textureData;
    private TerrainData terrainData;
    private NoiseData noiseData;

    private void OnEnable()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        textureData = new TextureData
        {
            layers = layers,
            smoothness = smoothness,
            metallic = metallic
        };

        terrainData = new TerrainData
        {
            useFalloff = useFalloff,
            meshHeightMultiplier = meshHeightMultiplier,
            meshHeightCurve = meshHeightCurve,
            useFlatShading = useFlatShading,
            uniformScale = uniformScale
        };

        noiseData = new NoiseData
        {
            normalizeMode = normalizeMode,
            noiseScale = noiseScale,
            octaves = octaves,
            persistence = persistence,
            lacunarity = lacunarity,
            seed = seed,
            offset = offset
        };
    }

    public TextureData GetTextureData() => textureData;
    public TerrainData GetTerrainData() => terrainData;
    public NoiseData GetNoiseData() => noiseData;

}
