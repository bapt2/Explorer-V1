using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class PlaceableObjectConfig
{
    public bool hasHeightLimite = false;
    public float minHeightToSpawn = 0f;
    public float MaxHeightToSpawn = 0f;

    public bool canGoInWater = false;
    public bool canGoAboveWater = true;
    [Range(0f, 1f)]public float Wheighting = 1f;
    public List<GameObject> prefabs;


    public float normaliseWheighting { get; set; } = 0f;
}

public class BaseObjectPlacers : MonoBehaviour
{
    [SerializeField] protected List<PlaceableObjectConfig> objects;
    [SerializeField] protected float targetDensity = 0.1f;
    [SerializeField] protected int maxSpawnCount = 1000;
    [SerializeField] protected int maxInvalidLocationSkips = 10;
    [SerializeField] protected float maxPositionJitter = 0.15f;

    protected List<Vector3> GetAllLocationsForBiome(ProcGenManager.GenerationData generationData, int biomeIndex)
    {
        List<Vector3> locations = new List<Vector3>(generationData.mapResolution * generationData.mapResolution / 10);

        for (int y = 0; y < generationData.mapResolution; y++)
        {
            for (int x = 0; x < generationData.mapResolution; x++)
            {
                if (generationData.biomeMap[x, y] != biomeIndex)
                    continue;

                float height = generationData.heightMap[x, y] * generationData.heightmapScale.y;

                locations.Add(new Vector3(y * generationData.heightmapScale.z, height, x * generationData.heightmapScale.x));
            }
        }

        return locations;
    }

    public virtual void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        // validate the config
        foreach (var config in objects)
        {
            if (!config.canGoInWater && !config.canGoAboveWater)
                throw new System.InvalidOperationException($"Object placer forbids both in and out water. Won't run");
        }

        // normalise the wheighting
        float wheightSum = 0f;
        foreach (var config in objects)
            wheightSum += config.Wheighting;
        foreach (var config in objects)
            config.normaliseWheighting = config.Wheighting / wheightSum;

    }

    protected virtual void ExecuteSimpleSpawning(ProcGenManager.GenerationData generationData, List<Vector3> candidateLocations, Transform objectRoot)
    {

        foreach (var spawnConfig in objects)
        {
            // pick a random prefab
            var prefab = spawnConfig.prefabs[generationData.Random(0, spawnConfig.prefabs.Count)];

            float baseSpawnCount = Mathf.Min(maxSpawnCount, candidateLocations.Count * targetDensity);
            int numToSpawn = Mathf.FloorToInt(spawnConfig.normaliseWheighting * baseSpawnCount);

            int skipCount = 0;
            int numPlaced = 0;

            for (int index = 0; index < numToSpawn; index++)
            {

                int randomLocationIndex = generationData.Random(0, candidateLocations.Count);
                Vector3 spawnLocation = candidateLocations[randomLocationIndex];

                // invalide Height
                bool isValid = true;
                if (spawnLocation.y < generationData.globalConfig.waterHeight && !spawnConfig.canGoInWater)
                    isValid = false;

                if (spawnLocation.y >= generationData.globalConfig.waterHeight && !spawnConfig.canGoAboveWater)
                    isValid = false;

                if (spawnConfig.hasHeightLimite && (spawnLocation.y < spawnConfig.minHeightToSpawn || spawnLocation.y >= spawnConfig.MaxHeightToSpawn))
                    isValid = false;

                if (!isValid)
                {
                    skipCount++;
                    index--;

                    if (skipCount >= maxInvalidLocationSkips)
                        break;

                    continue;
                }
                skipCount = 0;
                numPlaced++;
                candidateLocations.RemoveAt(randomLocationIndex);

                SpawnObject(generationData, prefab, spawnLocation);
            }
            Debug.Log($"Placed {numPlaced} objects out of {numToSpawn} for {prefab.name}");
        }
    }

    protected virtual void SpawnObject(ProcGenManager.GenerationData generationData, GameObject prefab, Vector3 spawnLocation)
    {
        Quaternion spawnRotation = Quaternion.Euler(-90, Random.Range(0f, 360f), 0);
        Vector3 possitionOffset = new Vector3(generationData.Random(-maxPositionJitter, maxPositionJitter), 0,
                                              generationData.Random(-maxPositionJitter, maxPositionJitter));
#if UNITY_EDITOR
        if (Application.isPlaying)
            Instantiate(prefab, spawnLocation + possitionOffset, spawnRotation, generationData.objectRoot);
        else
        {
            var spawnedGO = PrefabUtility.InstantiatePrefab(prefab, generationData.objectRoot) as GameObject;
            spawnedGO.transform.position = spawnLocation + possitionOffset;
            spawnedGO.transform.rotation = spawnRotation;
            Undo.RegisterCreatedObjectUndo(spawnedGO, "Placed Object");
        }
#else
            Instantiate(prefab, spawnLocation + possitionOffset, spawnRotation, generationData.objectRoot);
#endif
    }
}
