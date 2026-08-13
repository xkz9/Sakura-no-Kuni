using UnityEngine;

/// <summary>
/// Tunable values for procedural world generation.
/// Create one asset and assign it to WorldGenerator in the Inspector.
/// </summary>
[CreateAssetMenu(
    fileName = "WorldSettings",
    menuName = "Japanese City Builder/World/World Settings")]
public class WorldSettings : ScriptableObject
{
    [Header("Map")]
    [Tooltip("Width and depth of the generated terrain in world units.")]
    public int mapSize = 256;

    [Tooltip("Maximum terrain height in world units.")]
    public float maxTerrainHeight = 60f;

    [Tooltip("Heightmap resolution (e.g. 513 for a 512x512 heightmap).")]
    public int terrainResolution = 513;

    [Header("Seed")]
    [Tooltip("Seed used when generating a world with the default seed.")]
    public int defaultSeed = 42;

    [Header("Terrain Noise")]
    [Tooltip("Overall scale of the terrain noise. Lower = smoother hills.")]
    public float noiseScale = 0.01f;

    [Tooltip("Number of noise layers combined together.")]
    public int octaves = 4;

    [Tooltip("How much each octave contributes to the final height.")]
    public float persistence = 0.5f;

    [Tooltip("Frequency multiplier for each octave.")]
    public float lacunarity = 2f;

    [Header("Start Location")]
    [Tooltip("Radius around the castle where trees and rocks should not spawn.")]
    public float castleClearRadius = 30f;

    [Tooltip("Radius checked for future settlement buildable space.")]
    public float expansionRadius = 80f;

    [Tooltip("Minimum buildable coverage (0-1) required in the expansion zone.")]
    [Range(0f, 1f)]
    public float minBuildableCoverage = 0.6f;

    [Tooltip("How many random points StartLocationFinder samples on the map.")]
    public int searchSampleCount = 150;

    [Tooltip("Maximum slope angle (degrees) that counts as buildable ground.")]
    public float maxBuildableSlope = 15f;

    [Header("Environment")]
    [Tooltip("Target number of trees to place. Used in a later phase.")]
    public int treeCount = 80;

    [Tooltip("Target number of rocks to place. Used in a later phase.")]
    public int rockCount = 30;

    [Tooltip("Minimum distance between scattered environment objects.")]
    public float minEnvironmentSpacing = 4f;

    [Tooltip("Distance from the map edge where environment objects cannot spawn.")]
    public float environmentSpawnMargin = 10f;

    [Header("Prefabs")]
    [Tooltip("Placeholder clan manor prefab. Used in a later phase.")]
    public GameObject castlePlaceholderPrefab;

    [Tooltip("Tree prefabs to scatter. Used in a later phase.")]
    public GameObject[] treePrefabs;

    [Tooltip("Rock prefabs to scatter. Used in a later phase.")]
    public GameObject[] rockPrefabs;
}
