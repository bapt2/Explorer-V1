using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeConfig
{
    public BiomeConfigSO biome;

    [Range(0f, 1f)] public float wheighting = 1f;
}

[CreateAssetMenu(fileName = "ProcGen Config", menuName = "Procedural Generation/ProcGen Config", order = -1)]
public class ProcGenConfigSO : ScriptableObject
{
    public List<BiomeConfig> biomes;


    public GameObject biomeGenerators;
    public GameObject initialHeightModifier;
    public GameObject HeightPostProcessingModifier;

    public GameObject paintingPostProcessingModifier;
    public GameObject detailPaintingPostProcessingModifier;

    public float waterHeight = 8.5f;

    public int numBiomes => biomes.Count;

    public float totalWheighting
    {
        get
        {
            float sum = 0f;

            foreach (var config in biomes)
            {
                sum += config.wheighting;
            }

            return sum;
        }
    }

    public byte GetIndexForBiome(BiomeConfigSO biome)
    {
        for (int index = 0; index < biomes.Count; index++)
        {
            if (biomes[index].biome = biome)
                return (byte)index;
        }

        return byte.MaxValue;
    }
}
