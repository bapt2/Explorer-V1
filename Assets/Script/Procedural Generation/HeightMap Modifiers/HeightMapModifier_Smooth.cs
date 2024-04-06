using UnityEngine;



public class HeightMapModifier_Smooth : BaseHeightMapModifier
{
    [SerializeField] int smoothingKernelSize;
    [SerializeField] [Range(0f, 1f)] float maxHeightTreshold = 0.5f;

    [SerializeField] bool useAdaptiveKernel = false;
    [SerializeField] int minKernelSize = 2;
    [SerializeField] int maxKernelSize = 7;

    public override void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        if (biome != null)
        {
            Debug.LogError("HeightMapModifier_Smooth is not supported as a per biome modifier [" + gameObject.name + "]");
            return;
        }

        float[,] smoothedHeights = new float[generationData.mapResolution, generationData.mapResolution];

        for (int y = 0; y < generationData.mapResolution; ++y)
        {
            for (int x = 0; x < generationData.mapResolution; ++x)
            {
                float heightSum = 0f;
                int numValues = 0;

                //set the kernell size
                int kernelSize = smoothingKernelSize;
                if (useAdaptiveKernel)
                {
                    kernelSize = Mathf.RoundToInt(Mathf.Lerp(maxKernelSize, minKernelSize, generationData.heightMap[x, y] / maxHeightTreshold));
                }

                //sum the neighbouring values
                for (int yDelta = -kernelSize; yDelta <= kernelSize; ++yDelta)
                {
                    int workingY = y + yDelta;
                    if (workingY < 0 || workingY >= generationData.mapResolution)
                        continue;

                    for (int xDelta = -kernelSize; xDelta <= kernelSize; ++xDelta)
                    {
                        int workingX = x + xDelta;
                        if (workingX < 0 || workingX >= generationData.mapResolution)
                            continue;

                        heightSum += generationData.heightMap[workingX, workingY];
                        ++numValues;
                    }
                }

                //store the smoothed height
                smoothedHeights[x, y] = heightSum / numValues;
            }
        }

        for (int y = 0; y < generationData.mapResolution; ++y)
        {
            for (int x = 0; x < generationData.mapResolution; ++x)
            {
                generationData.heightMap[x, y] = Mathf.Lerp(generationData.heightMap[x, y], smoothedHeights[x, y], strength);
            }
        }
    }
}
