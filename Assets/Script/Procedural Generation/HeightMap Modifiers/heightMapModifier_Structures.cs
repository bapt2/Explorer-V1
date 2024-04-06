using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class StructuresConfig
{
    public Texture2D heightMap;
    public GameObject prefab;
    public int radius;
    public int numToSpawn = 1;

    public Quaternion quaternion = new ();

    public bool hasHeightLimite = false;
    public float minHeightToSpawn = 0f;
    public float MaxHeightToSpawn = 0f;

    public bool canGoInWater = false;
    public bool canGoAboveWater = true;
}

public class heightMapModifier_Structures : BaseHeightMapModifier
{
    [SerializeField] List<StructuresConfig> structures;

    protected void SpawnStructures(ProcGenConfigSO globalConfig, StructuresConfig structure, int spawnX, int spawnY, int mapResolution, float[,] heightMap, Vector3 heightmapScale, Transform structureRoot)
    {
        float averageHeight = 0f;
        int numHeightSamples = 0;

        // sum the height value under the structure
        for (int y = -structure.radius; y <= structure.radius; ++y)
        {
            for (int x = -structure.radius; x <= structure.radius; ++x)
            {
                //sum the heightmap values

                averageHeight += heightMap[x + spawnX, y + spawnY];
                ++numHeightSamples;
            }
        }

        // calculate the average height
        averageHeight /= numHeightSamples;

        float targetHeight = averageHeight;

        if (!structure.canGoInWater)
            targetHeight = Mathf.Max(targetHeight, globalConfig.waterHeight / heightmapScale.y);

        if (structure.hasHeightLimite)
            targetHeight = Mathf.Clamp(targetHeight, structure.minHeightToSpawn / heightmapScale.y, structure.MaxHeightToSpawn / heightmapScale.y);

        //apply the structure heightMap
        for (int y = -structure.radius; y <= structure.radius; ++y)
        {
            int workingY = y + spawnY;
            float textureY = Mathf.Clamp01((float)(y + structure.radius) / (structure.radius * 2f));

            for (int x = -structure.radius; x <= structure.radius; ++x)
            {
                int workingX = x + spawnX;
                float textureX = Mathf.Clamp01((float)(x + structure.radius) / (structure.radius * 2f));

                // sample the heightMap
                var pixelColor = structure.heightMap.GetPixelBilinear(textureX, textureY);
                float strength = pixelColor.r;

                heightMap[workingX, workingY] = Mathf.Lerp(heightMap[workingX, workingY], targetHeight, strength);
            }
        }
        //spawn the structure
        Vector3 structureLocation = new Vector3(spawnY * heightmapScale.z, heightMap[spawnX, spawnY] * heightmapScale.y, spawnX * heightmapScale.x);

#if UNITY_EDITOR
        if(Application.isPlaying)
            Instantiate(structure.prefab, structureLocation, structure.quaternion, structureRoot);

        else
        {
            var spawnedGO = PrefabUtility.InstantiatePrefab(structure.prefab, structureRoot) as GameObject;
            spawnedGO.transform.position = structureLocation;
            Undo.RegisterCreatedObjectUndo(spawnedGO, "Add structures");

        }
#else
            Instantiate(structure.prefab, structureLocation, structure.quaternion, structureRoot);
#endif
    }

    protected List<Vector2Int> GetSpawnLocationForStructures(ProcGenConfigSO globalConfig, int mapResolution, float[,] heightMap, Vector3 heightmapScale, StructuresConfig structuresConfig)
    {
        List<Vector2Int> locations = new List<Vector2Int>(mapResolution * mapResolution / 10);

        for (int y = structuresConfig.radius; (y < mapResolution - structuresConfig.radius); y += structuresConfig.radius * 2)
        {
            for (int x = structuresConfig.radius; x < (mapResolution - structuresConfig.radius); x += structuresConfig.radius * 2)
            {

                // invalide Height
                float height = heightMap[x, y] * heightmapScale.y;

                if (height < globalConfig.waterHeight && !structuresConfig.canGoInWater)
                    continue;

                if (height >= globalConfig.waterHeight && !structuresConfig.canGoAboveWater)
                    continue;

                if (structuresConfig.hasHeightLimite && (height < structuresConfig.minHeightToSpawn || height >= structuresConfig.MaxHeightToSpawn))
                    continue;

                locations.Add(new Vector2Int(x, y));
            }
        }
        return locations;
    }

    public override void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        var buildingRoot = FindObjectOfType<ProcGenManager>().transform;

        // travers the structures
        foreach (var structure in structures)
        {
            var spawnLocation = GetSpawnLocationForStructures(generationData.globalConfig, generationData.mapResolution, generationData.heightMap, generationData.heightmapScale, structure);

            for (int structureIndex = 0; structureIndex < structure.numToSpawn && spawnLocation.Count > 0; ++structureIndex)
            {
                int spawnIndex = generationData.Random(0, spawnLocation.Count);
                var spawnpos = spawnLocation[spawnIndex];
                spawnLocation.RemoveAt(spawnIndex);

                SpawnStructures(generationData.globalConfig, structure, spawnpos.x, spawnpos.y, generationData.mapResolution, generationData.heightMap, generationData.heightmapScale, generationData.objectRoot);
            }

        }

    }
}