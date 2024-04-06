using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome Config", menuName = "Procedural Generation/Biome Config", order = -1)]
public class BiomeConfigSO : ScriptableObject
{
    public string Name;

    [Range(0f, 1f)] public float minIntensity = 0.5f;
    [Range(0f, 1f)] public float maxIntensity = 1f;

    [Range(0f, 1f)] public float minDecayRate = 0.1f;
    [Range(0f, 1f)] public float maxDecayRate = 0.2f;

    public GameObject heightModifier;
    public GameObject terrainPainter;
    public GameObject objectPlacer;
    public GameObject detailPainter;

    public List<TextureConfig> RetrieveTextures()
    {
        if (terrainPainter == null)
            return null;

        // extract all textures from every painter
        List<TextureConfig> allTextures = new List<TextureConfig>();
        BaseTexturePainter[] allPainters = terrainPainter.GetComponents<BaseTexturePainter>();
        foreach (var painter in allPainters)
        {
            var painterTextures = painter.RetrieveTextures();
            if (painterTextures == null || painterTextures.Count == 0)
                continue;

            allTextures.AddRange(painterTextures);
        }

        return allTextures;
    }
    
    public List<TerrainDetailsConfig> RetrieveTerrainDetails()
    {
        if (detailPainter == null)
            return null;

        // extract all terrain details from every painter
        List<TerrainDetailsConfig> allTerrainDetails = new List<TerrainDetailsConfig>();
        BaseDetailPainter[] allPainters = detailPainter.GetComponents<BaseDetailPainter>();
        foreach (var painter in allPainters)
        {
            var terrainDetails = painter.RetrieveTerrainDetails();
            if (terrainDetails == null || terrainDetails.Count == 0)
                continue;

            allTerrainDetails.AddRange(terrainDetails);
        }

        return allTerrainDetails;
    }
}
