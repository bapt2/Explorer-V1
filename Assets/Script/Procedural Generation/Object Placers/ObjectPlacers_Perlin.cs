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

    List<Vector3> GetFilteredLocationsForBiome(ProcGenConfigSO globalConfig, int mapResolution, float[,] heightMap, Vector3 heightmapScale, byte[,] biomeMap, int biomeIndex)
    {
        List<Vector3> locations = new List<Vector3>(mapResolution * mapResolution / 10);

        for (int y = 0; y < mapResolution; y++)
        {
            for (int x = 0; x < mapResolution; x++)
            {
                if (biomeMap[x, y] != biomeIndex)
                    continue;

                float noiseValue = Mathf.PerlinNoise(x * noiseScale.x, y * noiseScale.y);

                // noise must be above the threshold to be considered a candidate point
                if (noiseValue < noiseThreshold)
                    continue;

                float height = heightMap[x, y] * heightmapScale.y;



                locations.Add(new Vector3(y * heightmapScale.z, height, x * heightmapScale.x));
            }
        }

        return locations;
    }

    public override void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {

        base.Execute(generationData, biomeIndex, biome);

        // get potential spawn location
        List<Vector3> candidateLocations = GetFilteredLocationsForBiome(generationData.globalConfig, generationData.mapResolution, generationData.heightMap,
                                                                        generationData.heightmapScale, generationData.biomeMap, biomeIndex);

        ExecuteSimpleSpawning(generationData, candidateLocations, generationData.objectRoot);

    }
}
