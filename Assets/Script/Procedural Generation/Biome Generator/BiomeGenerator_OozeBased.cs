using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator_OozeBased : BaseBiomeMapGenerator
{
    public enum EBiomeMapResolution
    {
        Size_64x64 = 64,
        Size_128x128 = 128,
        Size_256x256 = 256,
        Size_512x512 = 512
    }

    [Range(0f, 1f)] public float biomeSeedPointDensity = 0.1f;
    public EBiomeMapResolution biomeMapResolution = EBiomeMapResolution.Size_64x64;

    byte[,] biomeMap_LowRes;
    float[,] biomeStrengths_LowRes;

    public override void Execute(ProcGenManager.GenerationData generationData)
    {

        Perform_BiomeGeneration_LowRes(generationData, (int)biomeMapResolution);

        Perform_BiomeGeneration_HighRes(generationData.globalConfig, (int)biomeMapResolution, generationData.mapResolution, generationData.biomeMap, generationData.biomeStrengths);
    }

    void Perform_BiomeGeneration_LowRes(ProcGenManager.GenerationData generationData, int mapResolution)
    {
        //allocate the biome map and strength map
        biomeMap_LowRes = new byte[mapResolution, mapResolution];
        biomeStrengths_LowRes = new float[mapResolution, mapResolution];

        //setup space for seed point
        int numSeedPoints = Mathf.FloorToInt(mapResolution * mapResolution * biomeSeedPointDensity);
        PrepareToSpawnBiome(generationData, numSeedPoints);

        // spawn the individual biomes
        Vector2 normalisedPosition = Vector2.zero;
        for (int biomeCount = 0; biomeCount < numSeedPoints; biomeCount++)
        {
            // pick spawn locations
            Vector2Int spawnLocation = new Vector2Int(generationData.Random(0, mapResolution), generationData.Random(0, mapResolution));

            normalisedPosition.x = (((float)spawnLocation.x / (float)mapResolution) - 0.5f) * 2f;
            normalisedPosition.y = (((float)spawnLocation.y / (float)mapResolution) - 0.5f) * 2f;

            //extract the biome index
            byte biomeIndex = PickBiomeType(generationData, normalisedPosition);

            Perform_SpawnIndividualBiome(generationData, biomeIndex, spawnLocation, mapResolution);
        }
#if UNITY_EDITOR
        Texture2D biomeMapTexture = new Texture2D(mapResolution, mapResolution, TextureFormat.RGB24, false);
        for (int y = 0; y < mapResolution; y++)
        {
            for (int x = 0; x < mapResolution; x++)
            {
                float hue = ((float)biomeMap_LowRes[x, y] / (float)generationData.globalConfig.numBiomes);

                biomeMapTexture.SetPixel(x, y, Color.HSVToRGB(hue, 0.75f, 0.75f));
            }
        }

        biomeMapTexture.Apply();
        System.IO.File.WriteAllBytes("BiomeMap_LowRes.png", biomeMapTexture.EncodeToPNG());
#endif // UNITY_EDITOR
    }



    void Perform_SpawnIndividualBiome(ProcGenManager.GenerationData generationData, byte biomeIndex, Vector2Int spawnLocation, int mapResolution)
    {
        //cache biome config
        BiomeConfigSO biomeConfig = generationData.globalConfig.biomes[biomeIndex].biome;


        //pick the starting intensity
        float startIntensity = generationData.Random(biomeConfig.minIntensity, biomeConfig.maxIntensity);

        //setup working list
        Queue<Vector2Int> workingList = new Queue<Vector2Int>();
        workingList.Enqueue(spawnLocation);

        //setup the visited map and target intensity map
        bool[,] visited = new bool[mapResolution, mapResolution];
        float[,] targetIntensity = new float[mapResolution, mapResolution];

        //set the target intensity
        targetIntensity[spawnLocation.x, spawnLocation.y] = startIntensity;

        while (workingList.Count > 0)
        {
            Vector2Int workingLocation = workingList.Dequeue();

            //set the biome
            biomeMap_LowRes[workingLocation.x, workingLocation.y] = biomeIndex;
            visited[workingLocation.x, workingLocation.y] = true;
            biomeStrengths_LowRes[workingLocation.x, workingLocation.y] = targetIntensity[spawnLocation.x, spawnLocation.y];

            //travers the neighbours
            for (int neighbourIndex = 0; neighbourIndex < neighbourOffsets.Length; neighbourIndex++)
            {
                Vector2Int neighbourLocation = workingLocation + neighbourOffsets[neighbourIndex];

                if (neighbourLocation.x < 0 || neighbourLocation.y < 0 || neighbourLocation.x >= mapResolution || neighbourLocation.y >= mapResolution)
                    continue;

                if (visited[neighbourLocation.x, neighbourLocation.y])
                    continue;

                // flag as visited
                visited[neighbourLocation.x, neighbourLocation.y] = true;

                //work out neighbour strength
                float decayAmount = generationData.Random(biomeConfig.minDecayRate, biomeConfig.maxDecayRate) * neighbourOffsets[neighbourIndex].magnitude;
                float neighbourStrength = targetIntensity[workingLocation.x, workingLocation.y] - decayAmount;
                targetIntensity[neighbourLocation.x, neighbourLocation.y] = neighbourStrength;
                // if the strength is too low - stop
                if (neighbourStrength <= 0)
                    continue;

                workingList.Enqueue(neighbourLocation);
            }
        }
    }

    byte CalculateHighResBiomeIndex(int lowResMapSize, int lowResX, int lowResY, float fractionX, float fractionY)
    {
        float a = biomeMap_LowRes[lowResX, lowResY];
        float b = (lowResX + 1) < lowResMapSize ? biomeMap_LowRes[lowResX + 1, lowResY] : a;
        float c = (lowResY + 1) < lowResMapSize ? biomeMap_LowRes[lowResX, lowResY + 1] : a;
        float d = 0;

        if ((lowResX + 1) >= lowResMapSize)
            d = c;
        else if ((lowResY + 1) >= lowResMapSize)
            d = b;
        else
            d = biomeMap_LowRes[lowResX + 1, lowResY + 1];

        // perform bilinear filtering (linear interpolation in 2 dimension)
        float filteredindex = a * (1 - fractionX) * (1 - fractionY) + b * fractionX * (1 - fractionY) *
                  c * fractionY * (1 - fractionX) + d * fractionX * fractionY;

        // build an array of the possible biomes based on the values used to interpolate
        float[] candidateBiomes = new float[] { a, b, c, d };

        //find the neighbouring biome closest to the interpolated biome
        float bestBiome = -1f;
        float bestDelta = float.MaxValue;

        for (int biomeIndex = 0; biomeIndex < candidateBiomes.Length; biomeIndex++)
        {
            float delta = Mathf.Abs(filteredindex - candidateBiomes[biomeIndex]);

            if (delta < bestDelta)
            {
                bestDelta = delta;
                bestBiome = candidateBiomes[biomeIndex];
            }
        }

        return (byte)Mathf.RoundToInt(bestBiome);
    }

    void Perform_BiomeGeneration_HighRes(ProcGenConfigSO config, int lowResMapSIze, int highResMapSize, byte[,] biomeMap, float[,] biomeStrength)
    {

        // calculate map scale
        float mapScale = (float)lowResMapSIze / (float)highResMapSize;

        // calculate the hight res map
        for (int y = 0; y < highResMapSize; y++)
        {
            int lowResY = Mathf.FloorToInt(y * mapScale);
            float yFraction = y * mapScale - lowResY;

            for (int x = 0; x < highResMapSize; ++x)
            {
                int lowResX = Mathf.FloorToInt(x * mapScale);
                float xFraction = x * mapScale - lowResX;

                biomeMap[x, y] = CalculateHighResBiomeIndex(lowResMapSIze, lowResX, lowResY, xFraction, yFraction);

                // this would do no interpolation - point based
                //biomeMap[x, y] = biomeMap_LowRes[lowResX, lowResY];

            }
        }

#if UNITY_EDITOR
        Texture2D biomeMapTexture = new Texture2D(highResMapSize, highResMapSize, TextureFormat.RGB24, false);
        for (int y = 0; y < highResMapSize; y++)
        {
            for (int x = 0; x < highResMapSize; x++)
            {
                float hue = ((float)biomeMap[x, y] / (float)config.numBiomes);

                biomeMapTexture.SetPixel(x, y, Color.HSVToRGB(hue, 0.75f, 0.75f));
            }
        }

        biomeMapTexture.Apply();
        System.IO.File.WriteAllBytes("BiomeMap_HightRes.png", biomeMapTexture.EncodeToPNG());
#endif // UNITY_EDITOR
    }
}
