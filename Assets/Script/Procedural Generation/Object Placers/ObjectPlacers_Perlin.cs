// temporary inavailible du to conflict between 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObjectPlacers_Perlin : BaseObjectPlacers
{

    [SerializeField] Vector2 noiseScale = new Vector2(1f / 128f, 1f / 128f);
    [SerializeField] float noiseThreshold = 0.5f;

    List<Vector3> GetFilteredLocationsForBiome(ProcGenManager.GenerationData generationData, int biomeIndex)
    {


        List<Vector3> locations = new List<Vector3>(generationData.mapResolution * generationData.mapResolution / 10);

        for (int y = 0; y < generationData.mapResolution; y++)
        {
            for (int x = 0; x < generationData.mapResolution; x++)
            {
                if (generationData.biomeMap[x, y] != biomeIndex)
                    continue;

                float noiseValue = Mathf.PerlinNoise(x * noiseScale.x, y * noiseScale.y);

                // noise must be above the threshold to be considered a candidate point
                if (noiseValue < noiseThreshold)
                {
                    continue;
                }

                float height = generationData.heightMap[x, y] * generationData.heightmapScale.y;



                locations.Add(new Vector3(y * generationData.heightmapScale.z, height, x * generationData.heightmapScale.x));
            }
        }

        return locations;
    }

    public override void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        if (true)
        {
            Debug.LogWarning("temporary unavailible du to conflict between objectPlacers_Perlin and ZoneBasedGeneration");
            return;
        }

        base.Execute(generationData, biomeIndex, biome);

        // get potential spawn location
        List<Vector3> candidateLocations = GetFilteredLocationsForBiome(generationData, biomeIndex);

        ExecuteSimpleSpawning(generationData, candidateLocations, generationData.objectRoot);

    }
}
