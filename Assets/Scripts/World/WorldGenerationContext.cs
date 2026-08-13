using System;
using UnityEngine;

/// <summary>
/// Temporary data passed through the world generation pipeline.
/// Created at the start of GenerateWorld and discarded when generation finishes.
/// </summary>
public class WorldGenerationContext
{
    public int Seed { get; }
    public System.Random SeededRandom { get; }
    public WorldSettings Settings { get; }

    public Transform WorldRoot { get; set; }
    public Terrain Terrain { get; set; }
    public float[,] Heightmap { get; set; }
    public Vector3 StartPosition { get; set; }
    public float StartRotation { get; set; }
    public Transform CastleOrigin { get; set; }

    public WorldGenerationContext(int seed, WorldSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        Seed = seed;
        Settings = settings;
        SeededRandom = new System.Random(seed);
    }
}
