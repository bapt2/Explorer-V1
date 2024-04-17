using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseBiomeMapGenerator : MonoBehaviour
{
    public enum EBiomeSelectionMode
    {
        PureRandom,
        WeightedRandom,
        ZoneBasedWeightedRandom
    }

    protected Vector2Int[] neighbourOffsets = new Vector2Int[]
    {
        new Vector2Int(0,1),
        new Vector2Int(0,-1),
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(1,1),
        new Vector2Int(-1,-1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1),
    };

    [System.Serializable]
    public class ZoneConfig
    {
        public BiomeConfigSO biome;

        [Range(0f, 1f)] public float minDistance = 0f;
        [Range(0f, 1f)] public float maxDistance = 1f;
        public AnimationCurve biomeWeightingVsDistance;

        [Range(0f, 360f)] public float startAngle = 0f;
        [Range(0f, 360f)] public float endAngle = 360f;
        public bool insideOfAngle = true;

        public float GetWeightForLocation(Vector2 normalisedLocation)
        {
            float distance = Mathf.Clamp01(normalisedLocation.magnitude);

            if (distance < minDistance || distance > maxDistance)
                return -1f;

            float angle = Mathf.Atan2(normalisedLocation.x, normalisedLocation.y) * Mathf.Rad2Deg;
            if (angle < 0f)
                angle += 360f;

            if (insideOfAngle)
            {
                if (angle < startAngle || angle > endAngle)
                    return -1f;
            }
            else
            {
                if (angle >= startAngle && angle <= endAngle)
                    return -1f;
            }

            return biomeWeightingVsDistance.Evaluate(Mathf.InverseLerp(minDistance, maxDistance, distance));
        }
    }

    [SerializeField] EBiomeSelectionMode mode = EBiomeSelectionMode.PureRandom;
    [SerializeField] List<ZoneConfig> zones = new();
    List<byte> biomesToSpawn = null;


    public virtual void Execute(ProcGenManager.GenerationData generationData)
    {
        Debug.LogError("No implementation of Execute function for " + gameObject.name);
    }

    protected void PrepareToSpawnBiome(ProcGenManager.GenerationData generationData, int numSeedPoint)
    {
        if (mode != EBiomeSelectionMode.WeightedRandom)
        {
            biomesToSpawn = null;
            return;
        }
        biomesToSpawn = new List<byte>(numSeedPoint);

        //populate the biomes to spawn based on wheighting
        float totalBiomeWeighting = generationData.globalConfig.totalWheighting;
        for (int biomeIndex = 0; biomeIndex < generationData.globalConfig.numBiomes; biomeIndex++)
        {
            int numEntries = Mathf.RoundToInt(numSeedPoint * generationData.globalConfig.biomes[biomeIndex].wheighting / totalBiomeWeighting);
            Debug.Log("Will spawn" + numEntries + " seedpoints for " + generationData.globalConfig.biomes[biomeIndex].biome.name);

            for (int entryIndex = 0; entryIndex < numEntries; entryIndex++)
            {
                biomesToSpawn.Add((byte)biomeIndex);
            }
        }
    }
    protected byte PickBiomeType(ProcGenManager.GenerationData generationData, Vector2 normalisedLocation)
    {
        if (mode == EBiomeSelectionMode.WeightedRandom && biomesToSpawn != null && biomesToSpawn.Count > 0)
            return PickBiomeType_WeightedRandom(generationData, normalisedLocation);
        else if (mode == EBiomeSelectionMode.ZoneBasedWeightedRandom && zones != null && zones.Count > 0)
            return PickBiomeType_ZoneBasedWeightedRandom(generationData, normalisedLocation);

        return (byte)generationData.Random(0, generationData.globalConfig.numBiomes);
    }

    byte PickBiomeType_WeightedRandom(ProcGenManager.GenerationData generationData, Vector2 normalisedLocation)
    {
        int seedPointIndex = generationData.Random(0, biomesToSpawn.Count);

        //extract the biome index
        byte biomeIndex = biomesToSpawn[seedPointIndex];

        //remove seed point
        biomesToSpawn.RemoveAt(seedPointIndex);

        return biomeIndex;
    }

    /*byte PickBiomeType_ZoneBasedWeightedRandom(ProcGenManager.GenerationData generationData, Vector2 normalisedLocation)
    {
        List<Tuple<byte, float>> weightedBiomeOptions = new();
        float totalWeight = 0f;

        foreach (var zone in zones)
        {
            float weight = zone.GetWeightForLocation(normalisedLocation);
            if (weight <= 0f)
                continue;

            byte biomeIndex = generationData.globalConfig.GetIndexForBiome(zone.biome);
            if (biomeIndex == byte.MaxValue)
                continue;

            weightedBiomeOptions.Add(new Tuple<byte, float>(biomeIndex, weight + totalWeight));

            totalWeight += weight;
        }

        if (totalWeight > 0f)
        {
            float roll = generationData.Random(0f, totalWeight);
            foreach (var weightedBiome in weightedBiomeOptions)
            {
                if (roll <= weightedBiome.Item2)
                    return weightedBiome.Item1;
            }
        }
        // Debug.Log((byte)generationData.Random(0, generationData.globalConfig.numBiomes));

        return (byte)generationData.Random(0, generationData.globalConfig.numBiomes);
    }*/

    byte PickBiomeType_ZoneBasedWeightedRandom(ProcGenManager.GenerationData generationData, Vector2 normalisedLocation)
    {
        List<Tuple<byte, float>> weightedBiomeOptions = new();
        float totalWeight = 0f;

        foreach (var zone in zones)
        {
            float weight = zone.GetWeightForLocation(normalisedLocation);
            if (weight <= 0f)
                continue;

            byte biomeIndex = generationData.globalConfig.GetIndexForBiome(zone.biome);
            if (biomeIndex == byte.MaxValue)
                continue;

            weightedBiomeOptions.Add(new Tuple<byte, float>(biomeIndex, weight + totalWeight));

            totalWeight += weight;
        }

        if (totalWeight > 0f)
        {
            float roll = generationData.Random(0f, totalWeight);
            foreach (var weightedBiome in weightedBiomeOptions)
            {
                if (roll <= weightedBiome.Item2)
                    return weightedBiome.Item1;
            }
        }

        return (byte)generationData.Random(0, generationData.globalConfig.numBiomes);
    }
}