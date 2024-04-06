using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapModifier_SetValue : BaseHeightMapModifier
{
    [SerializeField] float targetHeight;

    public override void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        for (int y = 0; y < generationData.mapResolution; ++y)
        {
            for (int x = 0; x < generationData.mapResolution; ++x)
            {
                // skip if we have a biome and this is not our biome
                if(biomeIndex >= 0 && generationData.biomeMap[x,y] != biomeIndex)
                    continue;

                //calculate the new height
                float newHeight = targetHeight / generationData.heightmapScale.y;

                generationData.heightMap[x, y] = Mathf.Lerp(generationData.heightMap[x, y], newHeight, strength);
            }
        }
    }
}
