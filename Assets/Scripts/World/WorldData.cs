using UnityEngine;

/// <summary>
/// Represents a completed generated world.
/// Created when WorldGenerator finishes GenerateWorld and kept for other systems (grid, save/load, etc.).
/// </summary>
public class WorldData
{
    public int Seed { get; }
    public Terrain Terrain { get; }
    public Transform WorldRoot { get; }
    public Vector3 StartPosition { get; }
    public float StartRotation { get; }
    public Transform CastleOrigin { get; }
    public float CastleClearRadius { get; }
    public float ExpansionRadius { get; }

    public WorldData(
        int seed,
        Terrain terrain,
        Transform worldRoot,
        Vector3 startPosition,
        float startRotation,
        Transform castleOrigin,
        float castleClearRadius,
        float expansionRadius)
    {
        Seed = seed;
        Terrain = terrain;
        WorldRoot = worldRoot;
        StartPosition = startPosition;
        StartRotation = startRotation;
        CastleOrigin = castleOrigin;
        CastleClearRadius = castleClearRadius;
        ExpansionRadius = expansionRadius;
    }
}
