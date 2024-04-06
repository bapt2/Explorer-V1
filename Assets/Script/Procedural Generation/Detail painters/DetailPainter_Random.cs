using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomDetailPainterConfig
{
    public TerrainDetailsConfig detailToPaint;
    [Range(0f, 1f)] public float intensityModifier = 1f;

    public float noiseScale;
    [Range(0f, 1f)] public float noiseTreshold;
}

public class DetailPainter_Random : BaseDetailPainter
{
    [SerializeField] List<RandomDetailPainterConfig> paintingConfigs = new List<RandomDetailPainterConfig>()
    {
        new RandomDetailPainterConfig()
    };


    public override void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        for (int y = 0; y < generationData.detailMapResolution; y++)
        {
            int heightMapY = Mathf.FloorToInt((float)y * (float)generationData.mapResolution / (float)generationData.detailMapResolution);
            for (int x = 0; x < generationData.detailMapResolution; x++)
            {
                int heightMapX = Mathf.FloorToInt((float)x * (float)generationData.mapResolution / (float)generationData.detailMapResolution);

                // skip if we have a biome and this is not our biome
                if (biomeIndex >= 0 && generationData.biomeMap[heightMapX, heightMapY] != biomeIndex)
                    continue;

                // perform the painting
                foreach (var config in paintingConfigs)
                {
                    float noiseValue = Mathf.PerlinNoise(x * config.noiseScale, y * config.noiseScale);
                    if (generationData.Random(0f, 1f) >= noiseValue)
                    {
                        int layers = generationData.manager.GetDetailLayerForTerrainDetail(config.detailToPaint);
                        generationData.detailLayerMaps[layers][x, y] = Mathf.FloorToInt(strength * config.intensityModifier * generationData.maxDetailPerPatch);
                    }
                }
            }
        }
    }

    [System.NonSerialized] List<TerrainDetailsConfig> cachedTerrainDetails = null;

    public override List<TerrainDetailsConfig> RetrieveTerrainDetails()
    {
        if (cachedTerrainDetails == null)
        {
            cachedTerrainDetails = new List<TerrainDetailsConfig>();
            foreach (var config in paintingConfigs)
                cachedTerrainDetails.Add(config.detailToPaint);
        }
        return cachedTerrainDetails;
    }
}
