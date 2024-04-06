using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturePainter_Smooth : BaseTexturePainter
{
    [SerializeField] int smoothingKernelSize;

    public override void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        if (biome != null)
        {
            Debug.LogError("TexturePainter_Smooth is not supported as a perbiome modifier [" + gameObject.name + "]");
            return;
        }

        for (int layer = 0; layer < generationData.alphaMap.GetLength(2); layer++)
        {
            float[,] smoothAlphaMapResolution = new float[generationData.mapResolution, generationData.mapResolution];

            for (int y = 0; y < generationData.mapResolution; y++)
            {
                for (int x = 0; x < generationData.mapResolution; x++)
                {
                    float alphaSum = 0f;
                    int numValues = 0;

                    //sum the neighbouring values
                    for (int yDelta = -smoothingKernelSize; yDelta <= smoothingKernelSize; yDelta++)
                    {
                        int workingY = y + yDelta;
                        if (workingY < 0 || workingY >= generationData.alphaMapResolution)
                        {
                            continue;
                        }


                        for (int xDelta = -smoothingKernelSize; xDelta <= smoothingKernelSize; xDelta++)
                        {
                            int workingX = x + xDelta;
                            if (workingX < 0 || workingX >= generationData.alphaMapResolution)
                            {
                                continue;
                            }

                            alphaSum += generationData.alphaMap[workingX, workingY, layer];
                            numValues++;
                        }
                    }

                    //store the smoothed alpha
                    smoothAlphaMapResolution[x, y] = alphaSum / numValues;
                }
            }

            for (int y = 0; y < generationData.alphaMapResolution; y++)
            {
                for (int x = 0; x < generationData.alphaMapResolution; x++)
                {

                    // blend based on strength
                    generationData.alphaMap[x, y, layer] = Mathf.Lerp(generationData.alphaMap[x, y, layer], smoothAlphaMapResolution[x, y], strength);
                }
            }
        }
    }
        
}
