using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextureConfig
{
    public Texture2D diffuse;
    public Texture2D normalMap;

    public override bool Equals(object obj)
    {
        return obj is TextureConfig config &&
               EqualityComparer<Texture2D>.Default.Equals(diffuse, config.diffuse) &&
               EqualityComparer<Texture2D>.Default.Equals(normalMap, config.normalMap);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(diffuse, normalMap);
    }
}

public class BaseTexturePainter : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] protected float strength = 1f;

    public virtual void Execute(ProcGenManager.GenerationData generationData, int biomeIndex = -1, BiomeConfigSO biome = null)
    {
        Debug.LogError("No implementation of Execute function for " + gameObject.name);
    }

    public virtual List<TextureConfig> RetrieveTextures()
    {
        return null;
    }
}
