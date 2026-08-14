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

    [Header("Stepped Terrain")]
    [Tooltip("How many heightmap pixels make up one tile. Lower = smaller tiles.")]
    public int terrainBlockSize = 7;

    [Tooltip("Number of discrete height steps across the whole map.")]
    public int heightTierCount = 16;

    [Tooltip("Normal lowland height.")]
    public int lowlandMinTier = 1;

    [Tooltip("Highest lowland bump (only used for rare small rises, not pits).")]
    public int lowlandMaxTier = 2;

    [Tooltip("Lowest tier in rolling hill areas.")]
    public int hillMinTier = 3;

    [Tooltip("Highest tier in rolling hill areas.")]
    public int hillMaxTier = 9;

    [Tooltip("Lowest tier in mountain areas.")]
    public int mountainMinTier = 10;

    [Tooltip("Stops tiles dropping more than this many tiers below neighbors.")]
    public int maxDepressionTiers = 1;

    [Tooltip("Maximum tier change between neighboring block tiles. Keep at 1 for blocky steps.")]
    public int maxTierStep = 1;

    [Tooltip("Max passes when enforcing block-tier stairs (runs until stable or this limit).")]
    public int tierSmoothingPasses = 80;

    [Tooltip("Lower keeps block tile edges sharper when zooming the camera out.")]
    public float heightmapPixelError = 1f;

    [Header("Terrain Regions")]
    [Tooltip("Large-scale noise that picks lowland, hill, and mountain zones.")]
    public float regionNoiseScale = 0.0009f;

    [Tooltip("Region noise below this is low buildable ground.")]
    [Range(0f, 1f)]
    public float flatRegionMax = 0.48f;

    [Tooltip("Region noise above this becomes mountainous.")]
    [Range(0f, 1f)]
    public float mountainRegionMin = 0.86f;

    [Tooltip("Noise scale for hill height. Lower = broader, more gradual hills.")]
    public float elevationNoiseScale = 0.003f;

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

    [Header("Environment — Trees")]
    [Tooltip("Target number of trees to place across all forest types.")]
    public int treeCount = 175;

    [Tooltip("Overall tree size multiplier.")]
    public float treeScale = 4f;

    [Tooltip("Fraction of trees placed in large forest areas.")]
    [Range(0f, 1f)]
    public float majorForestTreeRatio = 0.66f;

    [Tooltip("Fraction of trees placed in smaller groves.")]
    [Range(0f, 1f)]
    public float minorForestTreeRatio = 0.28f;

    [Tooltip("Maximum fraction of trees placed as lone singles and pairs in open ground.")]
    [Range(0f, 0.2f)]
    public float maxScatteredTreeRatio = 0.11f;

    [Tooltip("How many open-ground spots get exactly two trees.")]
    public int scatteredDuoGroupCount = 7;

    [Tooltip("Distance between the two trees in a scattered pair.")]
    public float scatteredDuoSpacing = 3.6f;

    [Tooltip("Number of large forest areas spread across the map.")]
    public int majorForestZoneCount = 4;

    [Tooltip("Minimum trees per large forest.")]
    public int treesPerMajorForestMin = 24;

    [Tooltip("Maximum trees per large forest.")]
    public int treesPerMajorForestMax = 30;

    [Tooltip("Approximate size of a large forest area.")]
    public float majorForestRadius = 24f;

    [Tooltip("Distance between tree centers inside a large forest.")]
    public float majorForestPackSpacing = 2.2f;

    [Tooltip("Extra trunk gap inside a large forest.")]
    public float majorForestTreeGap = 0.18f;

    [Tooltip("Random gaps inside large forests. Middle-ground target is ~0.07.")]
    [Range(0f, 1f)]
    public float majorForestInteriorSkipChance = 0.07f;

    [Tooltip("Noise scale used to break up circular forest edges.")]
    public float forestShapeNoiseScale = 0.07f;

    [Tooltip("Number of smaller grove areas.")]
    public int minorForestZoneCount = 5;

    [Tooltip("Minimum trees per small grove.")]
    public int treesPerMinorForestMin = 6;

    [Tooltip("Maximum trees per small grove.")]
    public int treesPerMinorForestMax = 10;

    [Tooltip("Approximate size of a small grove.")]
    public float minorForestRadius = 15f;

    [Tooltip("Distance between tree centers inside a small grove.")]
    public float minorForestPackSpacing = 4.2f;

    [Tooltip("Extra trunk gap inside a small grove.")]
    public float minorForestTreeGap = 0.65f;

    [Tooltip("Minimum distance between separate forest zone centers.")]
    public float forestZoneSeparation = 16f;

    [Tooltip("Extra gap around occasional single trees.")]
    public float scatteredTreeGap = 7f;

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
