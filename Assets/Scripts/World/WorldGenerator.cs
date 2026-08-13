using UnityEngine;

/// <summary>
/// Orchestrates procedural world generation.
/// Call GenerateWorld(seed) from a Game Manager, Main Menu, or debug tools — not from Start().
/// </summary>
public class WorldGenerator : MonoBehaviour
{
    private const string GeneratedWorldObjectName = "GeneratedWorld";

    [Header("References")]
    [Tooltip("Drag Assets/ScriptableObjects/World/WorldSettings here.")]
    [SerializeField] private WorldSettings settings;

    /// <summary>The most recently generated world, or null if none exists yet.</summary>
    public WorldData CurrentWorld { get; private set; }

    /// <summary>
    /// Generates a world from the given seed.
    /// Later phases will add terrain, castle placement, and environment scattering.
    /// </summary>
    public WorldData GenerateWorld(int seed)
    {
        if (settings == null)
        {
            Debug.LogError("WorldGenerator: Assign WorldSettings in the Inspector.", this);
            return null;
        }

        ClearCurrentWorld();

        WorldGenerationContext context = new WorldGenerationContext(seed, settings);
        context.WorldRoot = CreateGeneratedWorldRoot();

        // Phase 2+: TerrainGenerator, StartLocationFinder, CastlePlacer, EnvironmentGenerator.

        WorldData worldData = CreateWorldDataFromContext(context);
        CurrentWorld = worldData;

        Debug.Log($"WorldGenerator: Generated world with seed {seed}.", this);
        return worldData;
    }

    /// <summary>Picks a random seed and generates a new world.</summary>
    public WorldData GenerateRandomWorld()
    {
        int seed = UnityEngine.Random.Range(1, int.MaxValue);
        return GenerateWorld(seed);
    }

    private void ClearCurrentWorld()
    {
        if (CurrentWorld?.WorldRoot != null)
        {
            Destroy(CurrentWorld.WorldRoot.gameObject);
        }

        CurrentWorld = null;

        GameObject leftoverRoot = GameObject.Find(GeneratedWorldObjectName);
        if (leftoverRoot != null)
        {
            Destroy(leftoverRoot);
        }
    }

    private Transform CreateGeneratedWorldRoot()
    {
        GameObject worldRootObject = new GameObject(GeneratedWorldObjectName);
        return worldRootObject.transform;
    }

    private static WorldData CreateWorldDataFromContext(WorldGenerationContext context)
    {
        return new WorldData(
            context.Seed,
            context.Terrain,
            context.WorldRoot,
            context.StartPosition,
            context.StartRotation,
            context.CastleOrigin,
            context.Settings.castleClearRadius,
            context.Settings.expansionRadius);
    }

#if UNITY_EDITOR
    [ContextMenu("Debug: Generate World (Default Seed)")]
    private void DebugGenerateDefaultWorld()
    {
        if (settings == null)
        {
            Debug.LogError("WorldGenerator: Assign WorldSettings in the Inspector.", this);
            return;
        }

        GenerateWorld(settings.defaultSeed);
    }

    [ContextMenu("Debug: Generate Random World")]
    private void DebugGenerateRandomWorld()
    {
        GenerateRandomWorld();
    }
#endif
}
