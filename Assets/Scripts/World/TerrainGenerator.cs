using UnityEngine;

/// <summary>
/// Generates a Unity Terrain with a seeded procedural heightmap.
/// Phase 2 only — no biomes, water, resources, or decoration.
/// </summary>
public class TerrainGenerator
{
    private const string TerrainObjectName = "Terrain";

    public void Generate(WorldGenerationContext context)
    {
        if (context.WorldRoot == null)
        {
            Debug.LogError("TerrainGenerator: WorldRoot is missing.");
            return;
        }

        WorldSettings settings = context.Settings;
        int resolution = settings.terrainResolution;

        if (resolution < 33)
        {
            Debug.LogError("TerrainGenerator: terrainResolution must be at least 33.");
            return;
        }

        float noiseOffsetX = (float)(context.SeededRandom.NextDouble() * 100_000d);
        float noiseOffsetZ = (float)(context.SeededRandom.NextDouble() * 100_000d);

        float[,] heights = BuildHeightmap(settings, resolution, noiseOffsetX, noiseOffsetZ);

        TerrainData terrainData = new TerrainData
        {
            heightmapResolution = resolution,
            size = new Vector3(settings.mapSize, settings.maxTerrainHeight, settings.mapSize)
        };

        terrainData.SetHeights(0, 0, heights);

        GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
        terrainObject.name = TerrainObjectName;
        terrainObject.transform.SetParent(context.WorldRoot, false);

        // Unity terrain grows from its pivot toward +X and +Z. Offset so the map is centered on WorldGenerator.
        float halfMapSize = settings.mapSize * 0.5f;
        terrainObject.transform.localPosition = new Vector3(-halfMapSize, 0f, -halfMapSize);
        terrainObject.transform.localRotation = Quaternion.identity;
        terrainObject.transform.localScale = Vector3.one;

        context.Terrain = terrainObject.GetComponent<Terrain>();
        context.Heightmap = heights;

        Debug.Log($"TerrainGenerator: Created '{TerrainObjectName}' under '{context.WorldRoot.name}'.", terrainObject);
    }

    private static float[,] BuildHeightmap(
        WorldSettings settings,
        int resolution,
        float noiseOffsetX,
        float noiseOffsetZ)
    {
        float[,] heights = new float[resolution, resolution];

        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                heights[z, x] = SampleNormalizedHeight(settings, x, z, resolution, noiseOffsetX, noiseOffsetZ);
            }
        }

        return heights;
    }

    private static float SampleNormalizedHeight(
        WorldSettings settings,
        int x,
        int z,
        int resolution,
        float noiseOffsetX,
        float noiseOffsetZ)
    {
        float amplitude = 1f;
        float frequency = 1f;
        float noiseSum = 0f;
        float amplitudeSum = 0f;

        for (int octave = 0; octave < settings.octaves; octave++)
        {
            float sampleX = (x + noiseOffsetX) * settings.noiseScale * frequency;
            float sampleZ = (z + noiseOffsetZ) * settings.noiseScale * frequency;

            float perlin = Mathf.PerlinNoise(sampleX, sampleZ);
            noiseSum += perlin * amplitude;
            amplitudeSum += amplitude;

            amplitude *= settings.persistence;
            frequency *= settings.lacunarity;
        }

        float normalized = amplitudeSum > 0f ? noiseSum / amplitudeSum : 0f;
        return Mathf.Clamp01(normalized * settings.heightMultiplier);
    }
}
