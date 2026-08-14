using UnityEngine;
using UnityEngine.SceneManagement;

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
    /// </summary>
    public WorldData GenerateWorld(int seed)
    {
        if (settings == null)
        {
            Debug.LogError("WorldGenerator: Assign WorldSettings in the Inspector.", this);
            return null;
        }

        if (settings.terrainResolution < 33)
        {
            Debug.LogError("WorldGenerator: terrainResolution must be at least 33 on WorldSettings.", this);
            return null;
        }

        ClearCurrentWorld();

        WorldGenerationContext context = new WorldGenerationContext(seed, settings);
        context.WorldRoot = CreateGeneratedWorldRoot();

        if (context.WorldRoot == null)
        {
            Debug.LogError("WorldGenerator: Failed to create GeneratedWorld root.", this);
            return null;
        }

        new TerrainGenerator().Generate(context);

        if (context.Terrain == null)
        {
            Debug.LogError("WorldGenerator: Terrain was not created.", this);
            ClearCurrentWorld();
            return null;
        }

        new EnvironmentGenerator().Generate(context);

        WorldData worldData = CreateWorldDataFromContext(context);
        CurrentWorld = worldData;

        Debug.Log($"WorldGenerator: Generated world with seed {seed}.", this);
        return worldData;
    }

    /// <summary>Picks a random seed and generates a new world.</summary>
    public WorldData GenerateRandomWorld()
    {
        int seed = new System.Random().Next(1, int.MaxValue);
        Debug.Log($"WorldGenerator: Random seed chosen = {seed}.", this);
        return GenerateWorld(seed);
    }

    private void ClearCurrentWorld()
    {
        if (CurrentWorld?.WorldRoot != null)
        {
            DestroyGeneratedObject(CurrentWorld.WorldRoot.gameObject);
        }

        CurrentWorld = null;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name == GeneratedWorldObjectName)
            {
                DestroyGeneratedObject(child.gameObject);
            }
        }

        foreach (GameObject rootObject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (rootObject.name == GeneratedWorldObjectName && rootObject.transform.parent == null)
            {
                DestroyGeneratedObject(rootObject);
            }
        }
    }

    private Transform CreateGeneratedWorldRoot()
    {
        GameObject worldRootObject = new GameObject(GeneratedWorldObjectName);
        worldRootObject.transform.SetParent(transform, false);
        worldRootObject.transform.localPosition = Vector3.zero;
        worldRootObject.transform.localRotation = Quaternion.identity;
        worldRootObject.transform.localScale = Vector3.one;
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

    private void DestroyGeneratedObject(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        // Use DestroyImmediate so the old terrain is gone before we build the new one.
        // Destroy() in Play mode waits until end of frame, which caused mismatched / overlapping terrain.
        DestroyImmediate(target);
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

        Debug.Log($"WorldGenerator: Using default seed {settings.defaultSeed}.", this);
        GenerateWorld(settings.defaultSeed);
    }

    [ContextMenu("Debug: Generate Random World")]
    private void DebugGenerateRandomWorld()
    {
        GenerateRandomWorld();
    }
#endif
}
