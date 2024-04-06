using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator_VoronoiBased : BaseBiomeMapGenerator
{

    [SerializeField] int numCells = 20;
    [SerializeField] int resampleDistance = 10;

    public override void Execute(ProcGenManager.GenerationData generationData)
    {
        int cellSize = Mathf.CeilToInt((float)generationData.mapResolution / numCells);

        PrepareToSpawnBiome(generationData, numCells * numCells);

        // generate our seed points
        Vector3Int[] biomeSeeds = new Vector3Int[numCells * numCells];
        Vector2 normalisedPosition = Vector2.zero;
        for (int cellY = 0; cellY < numCells; cellY++)
        {
            int centreY = Mathf.RoundToInt((cellY + 0.5f) * cellSize);

            for (int cellX = 0; cellX < numCells; cellX++)
            {
                int cellIndex = cellX + cellY * numCells;

                int centreX = Mathf.RoundToInt((cellX + 0.5f) * cellSize);

                biomeSeeds[cellIndex].x = centreX + generationData.Random(-cellSize / 2, cellSize / 2);
                biomeSeeds[cellIndex].y = centreY + generationData.Random(-cellSize / 2, cellSize / 2);

                normalisedPosition.x = ((float)biomeSeeds[cellIndex].x / (float)generationData.mapResolution);
                normalisedPosition.x = (normalisedPosition.x - 0.5f) * 2f;
                
                normalisedPosition.y = ((float)biomeSeeds[cellIndex].y / (float)generationData.mapResolution);
                normalisedPosition.y = (normalisedPosition.y - 0.5f) * 2f;

                biomeSeeds[cellIndex].z = PickBiomeType(generationData, normalisedPosition);
            }
        }
        
        //generate our based biome map
        byte[,] baseBiomeMap = new byte[generationData.mapResolution, generationData.mapResolution];
        for (int y = 0; y < generationData.mapResolution; y++)
        {
            for (int x = 0; x < generationData.mapResolution; x++)
            {
                baseBiomeMap[x, y] = FindClosestBiome(x, y, numCells, cellSize, biomeSeeds);
            }
        }

        // distorted biome map
        for (int y = 0; y < generationData.mapResolution; y++)
        {
            for (int x = 0; x < generationData.mapResolution; x++)
            {
                generationData.biomeMap[x, y] = ResampleBiomeMap(x, y, baseBiomeMap, generationData.mapResolution);
            }
        }

#if UNITY_EDITOR
        Texture2D biomeMapTexture = new Texture2D(generationData.mapResolution, generationData.mapResolution, TextureFormat.RGB24, false);
        for (int y = 0; y < generationData.mapResolution; ++y)
        {
            for (int x = 0; x < generationData.mapResolution; ++x)
            {
                float hue = ((float)baseBiomeMap[x, y] / (float)generationData.globalConfig.numBiomes);

                biomeMapTexture.SetPixel(x, y, Color.HSVToRGB(hue, 0.75f, 0.75f));
            }
        }

        biomeMapTexture.Apply();
        System.IO.File.WriteAllBytes("BiomeMap_Voronoi_Base.png", biomeMapTexture.EncodeToPNG());

        biomeMapTexture = new Texture2D(generationData.mapResolution, generationData.mapResolution, TextureFormat.RGB24, false);
        for (int y = 0; y < generationData.mapResolution; ++y)
        {
            for (int x = 0; x < generationData.mapResolution; ++x)
            {
                float hue = ((float)generationData.biomeMap[x, y] / (float)generationData.globalConfig.numBiomes);

                biomeMapTexture.SetPixel(x, y, Color.HSVToRGB(hue, 0.75f, 0.75f));
            }
        }

        biomeMapTexture.Apply();
        System.IO.File.WriteAllBytes("BiomeMap_Voronoi_Final.png", biomeMapTexture.EncodeToPNG());
#endif // UNITY_EDITOR
    }

    Vector2Int[] neighbourOffsets = new Vector2Int[]{
        new Vector2Int(0,1),
        new Vector2Int(0,-1),
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(1,1),
        new Vector2Int(-1,-1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1),
};

    byte FindClosestBiome(int x, int y, int numCells, int cellSize, Vector3Int[] biomeSeeds)
    {
        int cellX = x / cellSize;
        int cellY = y / cellSize;
        int cellIndex = cellX + cellY * numCells;

        float closestSeedDistanceSq = (biomeSeeds[cellIndex].x - x) * (biomeSeeds[cellIndex].x - x) +
                                      (biomeSeeds[cellIndex].y - y) * (biomeSeeds[cellIndex].y - y);
        byte bestBiome = (byte)biomeSeeds[cellIndex].z;

        foreach (var neighbourOffset in neighbourOffsets)
        {
            int workingCellX = cellX + neighbourOffset.x;
            int workingCellY = cellY + neighbourOffset.y;

            cellIndex = workingCellX + workingCellY * numCells;

            if (workingCellX < 0 || workingCellY < 0 || workingCellX >= numCells || workingCellY >= numCells)
                continue;

            float distanceSq = (biomeSeeds[cellIndex].x - x) * (biomeSeeds[cellIndex].x - x) +
                               (biomeSeeds[cellIndex].y - y) * (biomeSeeds[cellIndex].y - y);
            if (distanceSq < closestSeedDistanceSq)
            {
                closestSeedDistanceSq = distanceSq;
                bestBiome = (byte)biomeSeeds[cellIndex].z;
            }
        }

        return bestBiome;
    }

    byte ResampleBiomeMap(int x, int y, byte[,] biomeMap, int mapResolution)
    {
        float noise = 2f * (Mathf.PerlinNoise((float)x / mapResolution, (float)y / mapResolution) - 0.5f);

        int newX = Mathf.Clamp(Mathf.RoundToInt(x + noise * resampleDistance), 0, mapResolution - 1);
        int newY = Mathf.Clamp(Mathf.RoundToInt(y + noise * resampleDistance), 0, mapResolution - 1);

        return biomeMap[newX, newY];
    }
}


