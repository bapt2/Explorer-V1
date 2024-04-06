using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeightNoisePass
{
    public float HeightDelta = 1f;
    public float noiseScale = 1f;
}

public class HeightMapModifier_Noise : BaseHeightMapModifier
{
    [SerializeField] List<HeightNoisePass> passes;

    public override void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        foreach (var pass in passes)
        {
            for (int y = 0; y < generationData.mapResolution; y++)
            {
                for (int x = 0; x < generationData.mapResolution; x++)
                {
                    // skip if we have a biome and this is not our biome
                    if (biomeIndex >= 0 && generationData.biomeMap[x, y] != biomeIndex)
                        continue;

                    float noiseValue = (Mathf.PerlinNoise(x * pass.noiseScale, y * pass.noiseScale) * 2f) - 1f;

                    //calculate the new height
                    float newHeight = generationData.heightMap[x, y] + (noiseValue * pass.HeightDelta / generationData.heightmapScale.y);

                    generationData.heightMap[x, y] = Mathf.Lerp(generationData.heightMap[x, y], newHeight, strength);
                }
            }
        }
    }
}
