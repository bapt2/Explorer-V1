using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomPainterConfig
{
    public TextureConfig textureToPaint;
    [Range(0f, 1f)] public float intensityModifier = 1f;

    public float noiseScale;
    [Range(0f, 1f)] public float noiseTreshold;
}

public class TexturePainter_Random : BaseTexturePainter
{
    [SerializeField] TextureConfig baseTexture;
    [SerializeField] List<RandomPainterConfig> paintingConfigs;

    public override void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        int baseTextureLayer = generationData.manager.GetLayerForTexture(baseTexture);
        for (int y = 0; y < generationData.alphaMapResolution; y++)
        {
            int heightMapY = Mathf.FloorToInt((float)y * (float)generationData.mapResolution / (float)generationData.alphaMapResolution);
            for (int x = 0; x < generationData.alphaMapResolution; x++)
            {
                int heightMapX = Mathf.FloorToInt((float)x * (float)generationData.mapResolution / (float)generationData.alphaMapResolution);

                // skip if we have a biome and this is not our biome
                if (biomeIndex >= 0 && generationData.biomeMap[heightMapX, heightMapY] != biomeIndex)
                    continue;

                // perform the painting
                foreach (var config in paintingConfigs)
                {
                    float noiseValue = Mathf.PerlinNoise(x * config.noiseScale, y * config.noiseScale);
                    if (generationData.Random(0f, 1f) >= noiseValue)
                    {
                        int layers = generationData.manager.GetLayerForTexture(config.textureToPaint);
                        generationData.alphaMap[x, y, layers] = strength * config.intensityModifier;
                    }
                }

                generationData.alphaMap[x, y, baseTextureLayer] = strength;
            }
        }
    }

    [System.NonSerialized] List<TextureConfig> cachedTextures = null;

    public override List<TextureConfig> RetrieveTextures()
    {
        if (cachedTextures == null)
        {
            cachedTextures = new List<TextureConfig>();
            cachedTextures.Add(baseTexture);
            foreach (var config in paintingConfigs)
            {
                cachedTextures.Add(config.textureToPaint);
            }
        }

        return cachedTextures;
    }
}
