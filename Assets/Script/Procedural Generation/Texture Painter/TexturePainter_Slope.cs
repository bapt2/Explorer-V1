using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturePainter_Slope : BaseTexturePainter
{
    [SerializeField] TextureConfig texture;
    [SerializeField] AnimationCurve intensityVsSlope;
    public override void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        int textureLayer = generationData.manager.GetLayerForTexture(texture);
        for (int y = 0; y < generationData.alphaMapResolution; y++)
        {
            int heightMapY = Mathf.FloorToInt((float)y * (float)generationData.mapResolution / (float)generationData.alphaMapResolution);
            for (int x = 0; x < generationData.alphaMapResolution; x++)
            {
                int heightMapX = Mathf.FloorToInt((float)x * (float)generationData.mapResolution / (float)generationData.alphaMapResolution);

                // skip if we have a biome and this is not our biome
                if (biomeIndex >= 0 && generationData.biomeMap[heightMapX, heightMapY] != biomeIndex)
                    continue;

                generationData.alphaMap[x, y, textureLayer] = strength * intensityVsSlope.Evaluate(1f - generationData.slopeMap[x,y]);
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

