using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturePainter_Height : BaseTexturePainter
{
    [SerializeField] TextureConfig texture;
    [SerializeField] float startHeight;
    [SerializeField] float endHeight;
    [SerializeField] AnimationCurve intensity;
    [SerializeField] bool suppressOtherTexture = false;
    [SerializeField] AnimationCurve suppressionIntensity;
    public override void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        int textureLayer = generationData.manager.GetLayerForTexture(texture);

        float heightMapStart = startHeight / generationData.heightmapScale.y;
        float heightMapEnd = endHeight / generationData.heightmapScale.y;
        float heightMapRangeInv = 1f / (heightMapEnd - heightMapStart);

        int numAlphaMap = generationData.alphaMap.GetLength(2);

        for (int y = 0; y < generationData.alphaMapResolution; y++)
        {
            int heightMapY = Mathf.FloorToInt((float)y * (float)generationData.mapResolution / (float)generationData.alphaMapResolution);
            for (int x = 0; x < generationData.alphaMapResolution; x++)
            {
                int heightMapX = Mathf.FloorToInt((float)x * (float)generationData.mapResolution / (float)generationData.alphaMapResolution);

                // skip if we have a biome and this is not our biome
                if (biomeIndex >= 0 && generationData.biomeMap[heightMapX, heightMapY] != biomeIndex)
                    continue;

                float height = generationData.heightMap[heightMapX, heightMapY];
                if (height < heightMapStart || height > heightMapEnd)
                    continue;

                float heightPercenage = (height - heightMapStart) * heightMapRangeInv;
                generationData.alphaMap[x, y, textureLayer] = strength * intensity.Evaluate(heightPercenage);

                
                if (suppressOtherTexture)
                {
                    float suppression = suppressionIntensity.Evaluate(heightPercenage);

                    for (int layerIndex = 0; layerIndex < numAlphaMap; layerIndex++)
                    {
                        if (layerIndex == textureLayer)
                            continue;

                        generationData.alphaMap[x, y, layerIndex] *= suppression;
                    }
                }
            }
        }
    }

    public override List<TextureConfig> RetrieveTextures()
    {
        List<TextureConfig> allTextures = new List<TextureConfig>();
        allTextures.Add(texture);
        return allTextures;
    }
}
