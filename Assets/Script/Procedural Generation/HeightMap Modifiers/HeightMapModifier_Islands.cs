using UnityEngine;

public class HeightMapModifier_Islands : BaseHeightMapModifier
{
    [SerializeField] [Range(1, 100)] int numIslands = 100;
    [SerializeField] float minIslandsSize = 20f;
    [SerializeField] float maxIslandsSize = 80f;
    [SerializeField] float minIslandsHeight = 10f;
    [SerializeField] float maxIslandsHeight = 40f;
    [SerializeField] float angleNoiseScale = 1f;
    [SerializeField] float distanceNoiseScale = 1f;
    [SerializeField] float NoiseHeightDelta = 5f;
    [SerializeField] AnimationCurve islandsShapeCurve;



    public override void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        for (int island = 0; island < numIslands; island++)
        {
            PlaceIsland(generationData, generationData.mapResolution, generationData.heightMap, generationData.heightmapScale, generationData.biomeMap, biomeIndex, biome);
        }
    }

    void PlaceIsland(ProcGenManager.GenerationData generationData, int mapResolution, float[,] heightMap, Vector3 heightmapScale, byte[,] biomeMap = null, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        int workingIslandSize = Mathf.RoundToInt(generationData.Random(minIslandsSize, maxIslandsSize) / heightmapScale.x);
        float workingIslandHeight = (generationData.Random(minIslandsHeight, maxIslandsHeight) + generationData.globalConfig.waterHeight) / heightmapScale.y;

        int centreX = generationData.Random(workingIslandSize, mapResolution - workingIslandSize);
        int centreY = generationData.Random(workingIslandSize, mapResolution - workingIslandSize);

        for (int islandY = -workingIslandSize; islandY <= workingIslandSize; islandY++)
        {
            int y = centreY + islandY;

            if (y < 0 || y >= mapResolution)
                continue;

            for (int islandX = -workingIslandSize; islandX <= workingIslandSize; islandX++)
            {
                int x = centreX + islandX;

                if (x < 0 || x >= mapResolution)
                    continue;

                float normalisedDistance = Mathf.Sqrt(islandX * islandX + islandY * islandY) / workingIslandSize;

                if (normalisedDistance > 1)
                    continue;

                float normalisedAngle = Mathf.Clamp01((Mathf.Atan2(islandY, islandX) + Mathf.PI) / (2 * Mathf.PI));
                float noise = Mathf.PerlinNoise(normalisedAngle * angleNoiseScale, normalisedDistance * distanceNoiseScale);

                float noiseHeightDelta = ((noise - 0.5f) * 2) * NoiseHeightDelta / heightmapScale.y;

                float height = workingIslandHeight * islandsShapeCurve.Evaluate(normalisedDistance) + noiseHeightDelta;

                heightMap[x, y] = Mathf.Max(heightMap[x, y], height);
            }
        }
    }
}
