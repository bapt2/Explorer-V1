using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using TMPro;

#if UNITY_EDITOR
#endif //UNITY_EDITOR

public enum EGenerationStage
{
    Beginning = 1,

    BuildTextureMap,
    BuildDetailMap,
    BuildBiomeMap,
    HeightMapGeneration,
    TerrainPainting,
    ObjectPlacement,
    DetailPainting,
    PlacingPlayer,

    Complete,
    NumStage = Complete

}

public class ProcGenManager : MonoBehaviour
{
    public class GenerationData
    {
        System.Random RNG;


        public ProcGenManager manager;
        public ProcGenConfigSO globalConfig;
        public Transform objectRoot;

        public int mapResolution;
        public float[,] heightMap;
        public Vector3 heightmapScale;
        public float[,,] alphaMap;
        public int alphaMapResolution;

        public byte[,] biomeMap;
        public float[,] biomeStrengths;

        public float[,] slopeMap;

        public Dictionary<TextureConfig, int> biomeTextureToTerrainLayerIndex = new Dictionary<TextureConfig, int>();
        public Dictionary<TerrainDetailsConfig, int> biomeTerrainDetailToDetailLayerIndex = new Dictionary<TerrainDetailsConfig, int>();

        public List<int[,]> detailLayerMaps;
        public int detailMapResolution;
        public int maxDetailPerPatch;

        public GenerationData(int inSeed)
        {
            RNG = new System.Random(inSeed);
        }

        public int Random(int minInclusive, int maxExclusive)
        {
            return RNG.Next(minInclusive, maxExclusive);
        }

        public float Random(float minInclusive, float maxInclusive)
        {
            return Mathf.Lerp(minInclusive, maxInclusive, (float)RNG.NextDouble());
        }
    }

    [SerializeField] protected int maxInvalidLocationSkips = 10;

    [SerializeField] ProcGenConfigSO config;
    [SerializeField] Terrain targetTerrain;
    [SerializeField] int seed;
    [SerializeField] bool randomiseSeedEveryTime = true;
    bool canSpawnInWater = false;
    bool isValid = true;

    [Header("Debuging")]
    [SerializeField] bool Debug_EnableObjectPlacers = false;

    [Header("UI")]
    [SerializeField] GameObject GenerationCanvas;
    [SerializeField] TextMeshProUGUI stageText;
    [SerializeField] Animator animator;

    GenerationData data;

    private void Awake()
    {
        StartCoroutine(AsyncRegenerateWorld());
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Perform_SpecifiqueSpawn();
        }
    }

    public IEnumerator AsyncRegenerateWorld(System.Action<EGenerationStage, string> reportStatus = null)
    {
        animator.SetBool("LoadingFinish", false);

        int workingSeed = seed;

        if (randomiseSeedEveryTime)
        {
            workingSeed = Random.Range(int.MinValue, int.MaxValue);
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Undo.RecordObject(this, "Randomise seed");
                seed = workingSeed;
            }
#endif
        }

        data = new GenerationData(workingSeed);

        // cache the core info
        data.manager = this;
        data.globalConfig = config;
        data.objectRoot = targetTerrain.transform;

        //cache the map resolution
        data.mapResolution = targetTerrain.terrainData.heightmapResolution;
        data.alphaMapResolution = targetTerrain.terrainData.alphamapResolution;
        data.detailMapResolution = targetTerrain.terrainData.detailResolution;
        data.maxDetailPerPatch = targetTerrain.terrainData.detailResolutionPerPatch;
        data.heightmapScale = targetTerrain.terrainData.heightmapScale;
       
        //clear out any previously spawned objects
        for (int childIndex = data.objectRoot.childCount - 1; childIndex >= 0; childIndex--)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                Destroy(data.objectRoot.GetChild(childIndex).gameObject);
            else
                Undo.DestroyObjectImmediate(data.objectRoot.GetChild(childIndex).gameObject);

#else
            Destroy(data.objectRoot.GetChild(childIndex).gameObject);
#endif
        }
        //DisablePlayerOnGeneration();

        if (reportStatus != null) reportStatus.Invoke(EGenerationStage.BuildTextureMap, "Building texture map");
        stageText.text = $"{((int)EGenerationStage.BuildTextureMap)} - {((int)EGenerationStage.Complete)} : {EGenerationStage.BuildTextureMap}";
        yield return new WaitForSeconds(1f);
        // step 1 - 9 

        // Generate the texture mapping
        Perform_GenerateTextureMapping();

        if (reportStatus != null) reportStatus.Invoke(EGenerationStage.BuildDetailMap, "Building Detail map");
        stageText.text = $"{((int)EGenerationStage.BuildDetailMap)} - {((int)EGenerationStage.Complete)} : {EGenerationStage.BuildDetailMap}";

        yield return new WaitForSeconds(1f);

        // Generate the detail mapping
        Perform_GenerateDetailMapping();

        if (reportStatus != null) reportStatus.Invoke(EGenerationStage.BuildBiomeMap, "Generate Low res biome map");
        stageText.text = $"{((int)EGenerationStage.BuildBiomeMap)} - {((int)EGenerationStage.Complete)} : {EGenerationStage.BuildBiomeMap}";

        yield return new WaitForSeconds(1f);

        //generate the biome map
        Perform_BiomeGeneration();

        if (reportStatus != null) reportStatus.Invoke(EGenerationStage.HeightMapGeneration, "Modifiying height");
        stageText.text = $"{((int)EGenerationStage.HeightMapGeneration)} - {((int)EGenerationStage.Complete)} : {EGenerationStage.HeightMapGeneration}";

        yield return new WaitForSeconds(1f);

        //update the terrain height
        Perform_HeightMapModification();

        if (reportStatus != null) reportStatus.Invoke(EGenerationStage.TerrainPainting, "Painting the terrain");
        stageText.text = $"{((int)EGenerationStage.TerrainPainting)} - {((int)EGenerationStage.Complete)} : {EGenerationStage.TerrainPainting}";

        yield return new WaitForSeconds(1f);

        // paint the terrain
        Perform_TerrainPainting();

        if (reportStatus != null) reportStatus.Invoke(EGenerationStage.ObjectPlacement, "Placing object");
        stageText.text = $"{((int)EGenerationStage.ObjectPlacement)} - {((int)EGenerationStage.Complete)} : {EGenerationStage.ObjectPlacement}";

        yield return new WaitForSeconds(1f);

        // place the object
        Perform_ObjectPlacement();

        // paint the details
        if (reportStatus != null) reportStatus.Invoke(EGenerationStage.DetailPainting, "Painting details");
        stageText.text = $"{((int)EGenerationStage.DetailPainting)} - {((int)EGenerationStage.Complete)} : {EGenerationStage.DetailPainting}";

        yield return new WaitForSeconds(1f);

        Perform_DetailPainting();

        if (reportStatus != null) reportStatus.Invoke(EGenerationStage.PlacingPlayer, "PlacingPlayer");
        stageText.text = $"{((int)EGenerationStage.PlacingPlayer)} - {((int)EGenerationStage.Complete)} : {EGenerationStage.PlacingPlayer}";

        yield return new WaitForSeconds(1f);

       // EnablePlayerAfterGeneration();
       // Perform_SpecifiqueSpawn();

        if (reportStatus != null) reportStatus.Invoke(EGenerationStage.Complete, "Generation Complete");
        stageText.text = $"{((int)EGenerationStage.Complete)} - {((int)EGenerationStage.Complete)} : {EGenerationStage.Complete}";

        animator.SetBool("LoadingFinish", true);
        GenerationCanvas.SetActive(false);
        Debug.Log("Generation completed, you can be happy or not if you have an issue");
    }


    public void Perform_GenerateTextureMapping()
    {
        data.biomeTextureToTerrainLayerIndex.Clear();

        //build up a list of all textures 
        List<TextureConfig> allTextures = new List<TextureConfig>();
        foreach (var biomeMetaData in config.biomes)
        {
            List<TextureConfig> biomeTextures = biomeMetaData.biome.RetrieveTextures();

            if (biomeTextures == null || biomeTextures.Count == 0)
                continue;

            allTextures.AddRange(biomeTextures);
        }

        if (config.paintingPostProcessingModifier != null)
        {
            // extract all textures from every painter
            BaseTexturePainter[] allPainters = config.paintingPostProcessingModifier.GetComponents<BaseTexturePainter>();
            foreach (var painter in allPainters)
            {
                var painterTextures = painter.RetrieveTextures();
                if (painterTextures == null || painterTextures.Count == 0)
                    continue;

                allTextures.AddRange(painterTextures);
            }
        }

        allTextures = allTextures.Distinct().ToList();

        // iterate over the textures Configs
        int layerIndex = 0;
        foreach (var textureConfig in allTextures)
        {
            data.biomeTextureToTerrainLayerIndex[textureConfig] = layerIndex;
            layerIndex++;
        }

    }

    public void Perform_GenerateDetailMapping()
    {
        data.biomeTerrainDetailToDetailLayerIndex.Clear();

        //build up a list of all terrain details 
        List<TerrainDetailsConfig> allTerrainDetails = new List<TerrainDetailsConfig>();
        foreach (var biomeMetaData in config.biomes)
        {
            List<TerrainDetailsConfig> biomeTextures = biomeMetaData.biome.RetrieveTerrainDetails();
            if (biomeTextures == null || biomeTextures.Count == 0)
                continue;

            allTerrainDetails.AddRange(biomeTextures);
        }

        if (config.detailPaintingPostProcessingModifier != null)
        {
            // extract all terrain details from every painter
            BaseDetailPainter[] allPainters = config.detailPaintingPostProcessingModifier.GetComponents<BaseDetailPainter>();
            foreach (var painter in allPainters)
            {
                var terrainDetails = painter.RetrieveTerrainDetails();
                if (terrainDetails == null || terrainDetails.Count == 0)
                    continue;

                allTerrainDetails.AddRange(terrainDetails);
            }
        }

        allTerrainDetails = allTerrainDetails.Distinct().ToList();

        // iterate over the terrain details Configs
        int layerIndex = 0;
        foreach (var terrainDetail in allTerrainDetails)
        {
            data.biomeTerrainDetailToDetailLayerIndex[terrainDetail] = layerIndex;
            layerIndex++;
        }
    }

#if UNITY_EDITOR
    public void RegenerateTextures()
    {
        Perform_LayerSetup();
    }

    public void RegenerateDetailPrototypes()
    {
        Perform_DetailPrototypeSetup();
    }

    void Perform_LayerSetup()
    {
        //delete any existing layers 
        if (targetTerrain.terrainData.terrainLayers == null || targetTerrain.terrainData.terrainLayers.Length < 0)
        {
            Undo.RecordObject(targetTerrain, "Clear previous layers");

            //build up a list of asset path for each layer
            List<string> layersToDelete = new List<string>();
            foreach (var layer in targetTerrain.terrainData.terrainLayers)
            {
                if (layer == null)
                    continue;

                layersToDelete.Add(AssetDatabase.GetAssetPath(layer.GetInstanceID()));
            }

            // remove all link to layer
            targetTerrain.terrainData.terrainLayers = null;

            foreach (var layerFile in layersToDelete)
            {
                if (string.IsNullOrEmpty(layerFile))
                    continue;

                AssetDatabase.DeleteAsset(layerFile);
            }
        }

        string scenePath = System.IO.Path.GetDirectoryName(SceneManager.GetActiveScene().path);

        Perform_GenerateTextureMapping();

        // generate all of the layers
        int numLayers = data.biomeTextureToTerrainLayerIndex.Count;
        List<TerrainLayer> newLayers = new List<TerrainLayer>(numLayers);

        //preallocate layers
        for (int layerIndex = 0; layerIndex < numLayers; layerIndex++)
        {
            newLayers.Add(new TerrainLayer());
        }


        foreach (var textureMappingEntry in data.biomeTextureToTerrainLayerIndex)
        {
            var textureConfig = textureMappingEntry.Key;
            var textureLayerIndex = textureMappingEntry.Value;
            var textureLayer = newLayers[textureLayerIndex];

            //configure the terrain layer textures
            textureLayer.diffuseTexture = textureConfig.diffuse;
            textureLayer.normalMapTexture = textureConfig.normalMap;

            string layerPath = System.IO.Path.Combine(scenePath, "Layer_" + textureLayerIndex);
            AssetDatabase.CreateAsset(textureLayer, $"{layerPath}.asset");
        }


        Undo.RecordObject(targetTerrain.terrainData, "Updating terrain layers");
        targetTerrain.terrainData.terrainLayers = newLayers.ToArray();
    }

    void Perform_DetailPrototypeSetup()
    {
        Perform_GenerateDetailMapping();

        //build the list of detail prototypes
        var detailPrototypes = new DetailPrototype[data.biomeTerrainDetailToDetailLayerIndex.Count];

        foreach (var kvp in data.biomeTerrainDetailToDetailLayerIndex)
        {
            TerrainDetailsConfig detailData = kvp.Key;
            int layerIndex = kvp.Value;

            DetailPrototype newDetail = new DetailPrototype();

            // is this a mesh
            if (detailData.detailPrefab)
            {
                newDetail.prototype = detailData.detailPrefab;
                newDetail.renderMode = DetailRenderMode.VertexLit;
                newDetail.usePrototypeMesh = true;
                newDetail.useInstancing = true;
            }
            else
            {
                newDetail.prototypeTexture = detailData.billboardTexture;
                newDetail.renderMode = DetailRenderMode.GrassBillboard;
                newDetail.usePrototypeMesh = false;
                newDetail.useInstancing = false;
                newDetail.healthyColor = detailData.healthyColor;
                newDetail.dryColor = detailData.dryColor;
            }

            // transfer the common data
            newDetail.minWidth = detailData.minWidth;
            newDetail.maxWidth = detailData.maxWidth;
            newDetail.minHeight = detailData.minHeight;
            newDetail.maxHeight = detailData.maxHeight;
            newDetail.noiseSeed = detailData.noiseSeed;
            newDetail.noiseSpread = detailData.noiseSpread;
            newDetail.holeEdgePadding = detailData.holeEdgePadding;

            // check the prototype
            string errorMessage;
            if (!newDetail.Validate(out errorMessage))
            {
                throw new System.InvalidCastException(errorMessage);
            }

            detailPrototypes[layerIndex] = newDetail;
        }

        //update the detail prototypes
        Undo.RecordObject(targetTerrain.terrainData, "Updating Detail Prototypes");
        targetTerrain.terrainData.detailPrototypes = detailPrototypes;

        targetTerrain.terrainData.RefreshPrototypes();
    }
#endif // UNITY_EDITOR

    void Perform_BiomeGeneration()
    {
        // allocate the biome map and strength map
        data.biomeMap = new byte[data.mapResolution, data.mapResolution];
        data.biomeStrengths = new float[data.mapResolution, data.mapResolution];

        //execute initial height modifiers
        if (config.biomeGenerators != null)
        {
            BaseBiomeMapGenerator[] generators = config.biomeGenerators.GetComponents<BaseBiomeMapGenerator>();

            foreach (var generator in generators)
            {
                generator.Execute(data);
            }
        }
    }
    void Perform_HeightMapModification()
    {
        data.heightMap = targetTerrain.terrainData.GetHeights(0, 0, data.mapResolution, data.mapResolution);

        //execute initial height modifiers
        if (config.initialHeightModifier != null)
        {
            BaseHeightMapModifier[] modifiers = config.initialHeightModifier.GetComponents<BaseHeightMapModifier>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(data);
            }
        }

        // run heightmap generation for each biome
        for (int biomeIndex = 0; biomeIndex < config.numBiomes; biomeIndex++)
        {
            var biome = config.biomes[biomeIndex].biome;

            if (biome.heightModifier == null)
            {
                continue;
            }
            BaseHeightMapModifier[] modifiers = biome.heightModifier.GetComponents<BaseHeightMapModifier>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(data, biomeIndex, biome);
            }
        }

        //execute any post processing height modifiers
        if (config.HeightPostProcessingModifier != null)
        {
            BaseHeightMapModifier[] modifiers = config.HeightPostProcessingModifier.GetComponents<BaseHeightMapModifier>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(data);
            }
        }

        targetTerrain.terrainData.SetHeights(0, 0, data.heightMap);

        data.slopeMap = new float[data.alphaMapResolution, data.alphaMapResolution];

        // generate the slope map
        for (int y = 0; y < data.alphaMapResolution; y++)
        {
            for (int x = 0; x < data.alphaMapResolution; x++)
            {
                for (int layerIndex = 0; layerIndex < targetTerrain.terrainData.alphamapLayers; layerIndex++)
                {
                    data.slopeMap[x, y] = targetTerrain.terrainData.GetSteepness((float)x / data.alphaMapResolution, (float)y / data.alphaMapResolution) / 90f;

                }
            }
        }
    }

    public int GetLayerForTexture(TextureConfig TextureConfig)
    {
        return data.biomeTextureToTerrainLayerIndex[TextureConfig];
    }
    public int GetDetailLayerForTerrainDetail(TerrainDetailsConfig terrainDetailsConfig)
    {
        return data.biomeTerrainDetailToDetailLayerIndex[terrainDetailsConfig];
    }

    void Perform_TerrainPainting()
    {
        data.alphaMap = targetTerrain.terrainData.GetAlphamaps(0, 0, data.alphaMapResolution, data.alphaMapResolution);

        // zero out all layers if existe
        if (targetTerrain.terrainData.alphamapLayers > 0)
        {
            for (int y = 0; y < data.alphaMapResolution; y++)
            {
                for (int x = 0; x < data.alphaMapResolution; x++)
                {
                    for (int layerIndex = 0; layerIndex < targetTerrain.terrainData.alphamapLayers; layerIndex++)
                    {
                        data.alphaMap[x, y, layerIndex] = 0;
                    }
                }
            }
        }

        // run terrain paiting generation for each biome
        for (int biomeIndex = 0; biomeIndex < config.numBiomes; biomeIndex++)
        {
            var biome = config.biomes[biomeIndex].biome;

            if (biome.terrainPainter == null)
            {
                continue;
            }
            BaseTexturePainter[] modifiers = biome.terrainPainter.GetComponents<BaseTexturePainter>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(data, biomeIndex, biome);
            }
        }

        //run texture post processing
        if (config.paintingPostProcessingModifier != null)
        {
            BaseTexturePainter[] modifiers = config.paintingPostProcessingModifier.GetComponents<BaseTexturePainter>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(data);
            }
        }

        targetTerrain.terrainData.SetAlphamaps(0, 0, data.alphaMap);
    }

    void Perform_ObjectPlacement()
    {
        if (Debug_EnableObjectPlacers)
            return;

        // run object placement generation for each biome
        for (int biomeIndex = 0; biomeIndex < config.numBiomes; biomeIndex++)
        {
            var biome = config.biomes[biomeIndex].biome;

            if (biome.objectPlacer == null)
                continue;

            BaseObjectPlacers[] modifiers = biome.objectPlacer.GetComponents<BaseObjectPlacers>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(data, biomeIndex, biome);
            }
        }

    }

    void Perform_SpecifiqueSpawn()
    {
        var spawnLocations = GetPositionForPlayerSpawn(data.mapResolution, data.heightMap, data.heightmapScale);

        ChangingPlayerPosition(data, spawnLocations);
    }

    void Perform_DetailPainting()
    {
        //create new empty set of layers
        int numDetailLayers = targetTerrain.terrainData.detailPrototypes.Length;
        data.detailLayerMaps = new List<int[,]>(numDetailLayers);
        for (int layerIndex = 0; layerIndex < numDetailLayers; layerIndex++)
        {
            data.detailLayerMaps.Add(new int[data.detailMapResolution, data.detailMapResolution]);
        }

        // run terrain detail painting for each biome
        for (int biomeIndex = 0; biomeIndex < config.numBiomes; biomeIndex++)
        {
            var biome = config.biomes[biomeIndex].biome;

            if (biome.detailPainter == null)
                continue;

            BaseDetailPainter[] modifiers = biome.detailPainter.GetComponents<BaseDetailPainter>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(data, biomeIndex, biome);
            }
        }

        //run detail painting post processing
        if (config.detailPaintingPostProcessingModifier != null)
        {
            BaseDetailPainter[] modifiers = config.detailPaintingPostProcessingModifier.GetComponents<BaseDetailPainter>();

            foreach (var modifier in modifiers)
            {
                modifier.Execute(data);
            }
        }

        //apply the detail layers
        for (int layerIndex = 0; layerIndex < numDetailLayers; layerIndex++)
        {
            targetTerrain.terrainData.SetDetailLayer(0, 0, layerIndex, data.detailLayerMaps[layerIndex]);
        }
    }

    List<Vector3> GetPositionForPlayerSpawn(int mapResolution, float[,] heightMap, Vector3 heightmapScale)
    {
        List<Vector3> locations = new List<Vector3>(mapResolution * mapResolution / 10);

        for (int y = 0; y < mapResolution; y++)
        {
            for (int x = 0; x < mapResolution; x++)
            {
                float height = heightMap[x, y] * heightmapScale.y;

                locations.Add(new Vector3(y * heightmapScale.z, height + 1, x * heightmapScale.x));
            }
        }

        return locations;
    }

    void ChangingPlayerPosition(GenerationData generationData, List<Vector3> randomLocation)
    {
        int skipCount = 0;
        int numPlaced = 0;

        for (int index = 0; index < randomLocation.Count; index++)
        {
            int randomLocationIndex = generationData.Random(0, randomLocation.Count);
            Vector3 spawnLocation = randomLocation[randomLocationIndex];

            // invalide spawn
            if (spawnLocation.y < generationData.globalConfig.waterHeight && !canSpawnInWater)
                continue;

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

            randomLocation.RemoveAt(randomLocationIndex);
            if (!PlayerController.instance)
            {
                return;
            }
            PlayerController.instance.transform.position = spawnLocation;
        }
    }

    public void DisablePlayerOnGeneration()
    {
        PlayerController.instance.gameObject.SetActive(false);
        InventoryManager.instance.gameObject.SetActive(false);
        CameraController.instance.gameObject.SetActive(false);
    }
    public void EnablePlayerAfterGeneration()
    {
        PlayerController.instance.gameObject.SetActive(true);
        InventoryManager.instance.gameObject.SetActive(true);
        CameraController.instance.gameObject.SetActive(true);
    }
}