using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacers_Random : BaseObjectPlacers
{

    public override void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        base.Execute(generationData, biomeIndex, biome);
        
            // get potential spawn location
        List<Vector3> candidateLocations = GetAllLocationsForBiome(generationData, biomeIndex);

        ExecuteSimpleSpawning(generationData, candidateLocations, generationData.objectRoot);
    }
}