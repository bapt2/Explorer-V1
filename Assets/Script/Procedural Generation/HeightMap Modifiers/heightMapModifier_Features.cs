using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FeatureConfig
{
    public Texture2D heightMap;
    public float height;
    public int radius;
    public int numToSpawn = 1;
}

public class heightMapModifier_Features : BaseHeightMapModifier
{
    [SerializeField] List<FeatureConfig> features;

    protected void SpawnFeature(FeatureConfig feature, int spawnX, int spawnY, int mapResolution, float[,]  heightMap, Vector3 heightmapScale)
    {
        float averageHeight = 0f;
        int numHeightSamples = 0;

        // sum the height value under the featur
        for (int y = -feature.radius; y <= feature.radius; ++y)
        {
            for (int x = -feature.radius; x <= feature.radius; ++x)
            {
                //sum the neighbouring values

                averageHeight += heightMap[x + spawnX, y + spawnY];
                ++numHeightSamples;
            }
        }

        // calculate the average map
        averageHeight /= numHeightSamples;

        float targetHeight = averageHeight + (feature.height / heightmapScale.y);

        //apply the feature
        for (int y = -feature.radius; y <= feature.radius; ++y)
        {
            int workingY = y + spawnY;
            float textureY = Mathf.Clamp01((float)(y + feature.radius) / (feature.radius * 2f));

            for (int x = -feature.radius; x <= feature.radius; ++x)
            {
                int workingX = x + spawnX;
                float textureX = Mathf.Clamp01((float)(x + feature.radius) / (feature.radius * 2f));

                // sample the heightMap
                var pixelColor = feature.heightMap.GetPixelBilinear(textureX, textureY);
                float strength = pixelColor.r;

                heightMap[workingX, workingY] = Mathf.Lerp(heightMap[workingX, workingY], targetHeight, strength);
            }
        }
    }


public override void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        // travers the features
        foreach (var feature in features)
        {
            for (int featureIndex = 0; featureIndex < feature.numToSpawn; ++featureIndex)
            {
                int spawnX = generationData.Random(feature.radius, generationData.mapResolution - feature.radius);
                int spawnY = generationData.Random(feature.radius, generationData.mapResolution - feature.radius);

                SpawnFeature(feature, spawnX, spawnY, generationData.mapResolution, generationData.heightMap, generationData.heightmapScale);

            }
        }
    }
}