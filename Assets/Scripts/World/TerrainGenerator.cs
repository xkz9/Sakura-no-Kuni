using UnityEngine;

/// <summary>
/// Generates stepped terrain with small flat-topped tiles.
/// </summary>
public class TerrainGenerator
{
    private const string TerrainObjectName = "Terrain";
    private const float LowlandBumpThreshold = 0.72f;

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

        if (settings.terrainBlockSize < 2)
        {
            Debug.LogError("TerrainGenerator: terrainBlockSize must be at least 2.");
            return;
        }

        float[,] heights = BuildHeightmap(context.Seed, settings, resolution);

        TerrainData terrainData = new TerrainData
        {
            heightmapResolution = resolution,
            size = new Vector3(settings.mapSize, settings.maxTerrainHeight, settings.mapSize)
        };

        terrainData.SetHeights(0, 0, heights);

        GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
        terrainObject.name = TerrainObjectName;
        terrainObject.transform.SetParent(context.WorldRoot, false);

        float halfMapSize = settings.mapSize * 0.5f;
        terrainObject.transform.localPosition = new Vector3(-halfMapSize, 0f, -halfMapSize);
        terrainObject.transform.localRotation = Quaternion.identity;
        terrainObject.transform.localScale = Vector3.one;

        Terrain terrain = terrainObject.GetComponent<Terrain>();
        ApplyTerrainLodSettings(terrain, settings);

        context.Terrain = terrain;
        context.Heightmap = heights;

        int center = resolution / 2;
        Debug.Log(
            $"TerrainGenerator: seed={context.Seed}, centerHeight={heights[center, center]:F4}, style=stepped-tiles",
            terrainObject);
    }

    private static void ApplyTerrainLodSettings(Terrain terrain, WorldSettings settings)
    {
        if (terrain == null)
        {
            return;
        }

        terrain.heightmapPixelError = settings.heightmapPixelError;
        terrain.basemapDistance = 2000f;
        terrain.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    private static float[,] BuildHeightmap(int seed, WorldSettings settings, int resolution)
    {
        int blockSize = settings.terrainBlockSize;
        int blockCount = Mathf.CeilToInt(resolution / (float)blockSize);

        int[,] blockTiers = BuildBlockTierMap(seed, settings, blockCount);
        EnforceAdjacentTierStepsUntilStable(
            blockTiers,
            blockCount,
            settings.maxTierStep,
            settings.tierSmoothingPasses);
        LimitDepressions(blockTiers, blockCount, settings.maxDepressionTiers, settings.lowlandMinTier);
        EnforceAdjacentTierStepsUntilStable(
            blockTiers,
            blockCount,
            settings.maxTierStep,
            settings.tierSmoothingPasses);

        float tierScale = 1f / settings.heightTierCount;
        float[,] heights = new float[resolution, resolution];

        for (int z = 0; z < resolution; z++)
        {
            int blockZ = Mathf.Min(z / blockSize, blockCount - 1);
            for (int x = 0; x < resolution; x++)
            {
                int blockX = Mathf.Min(x / blockSize, blockCount - 1);
                heights[z, x] = blockTiers[blockZ, blockX] * tierScale;
            }
        }

        return heights;
    }

    private static int[,] BuildBlockTierMap(int seed, WorldSettings settings, int blockCount)
    {
        int[,] tiers = new int[blockCount, blockCount];

        float regionOffsetX = GetDeterministicNoiseOffset(seed, 301);
        float regionOffsetZ = GetDeterministicNoiseOffset(seed, 302);
        float elevationOffsetX = GetDeterministicNoiseOffset(seed, 303);
        float elevationOffsetZ = GetDeterministicNoiseOffset(seed, 304);

        for (int blockZ = 0; blockZ < blockCount; blockZ++)
        {
            for (int blockX = 0; blockX < blockCount; blockX++)
            {
                float worldX = GetBlockWorldCoordinate(blockX, blockCount, settings.mapSize);
                float worldZ = GetBlockWorldCoordinate(blockZ, blockCount, settings.mapSize);

                float regionValue = Mathf.PerlinNoise(
                    worldX * settings.regionNoiseScale + regionOffsetX,
                    worldZ * settings.regionNoiseScale + regionOffsetZ);

                float elevationValue = Mathf.PerlinNoise(
                    worldX * settings.elevationNoiseScale + elevationOffsetX,
                    worldZ * settings.elevationNoiseScale + elevationOffsetZ);

                tiers[blockZ, blockX] = GetTierForRegion(regionValue, elevationValue, settings);
            }
        }

        return tiers;
    }

    private static float GetBlockWorldCoordinate(int blockIndex, int blockCount, int mapSize)
    {
        return (blockIndex + 0.5f) / blockCount * mapSize;
    }

    /// <summary>
    /// Hills rise above flat ground. Lowlands stay flat with no pits.
    /// </summary>
    private static int GetTierForRegion(float regionValue, float elevationValue, WorldSettings settings)
    {
        int flatTier = settings.lowlandMinTier;

        if (regionValue < settings.flatRegionMax)
        {
            if (elevationValue > LowlandBumpThreshold &&
                flatTier < settings.lowlandMaxTier)
            {
                return flatTier + 1;
            }

            return flatTier;
        }

        if (regionValue > settings.mountainRegionMin)
        {
            float mountainBlend = Smooth01(
                Mathf.InverseLerp(settings.mountainRegionMin, 1f, regionValue));
            int maxLift = settings.heightTierCount - 1 - flatTier;
            int lift = Mathf.FloorToInt(elevationValue * (maxLift + 1) * mountainBlend);
            return flatTier + lift;
        }

        float hillBlend = Smooth01(Mathf.InverseLerp(
            settings.flatRegionMax,
            settings.mountainRegionMin,
            regionValue));
        int hillLiftRange = settings.hillMaxTier - flatTier;
        int hillLift = Mathf.FloorToInt(elevationValue * (hillLiftRange + 1) * hillBlend);
        return flatTier + hillLift;
    }

    private static float Smooth01(float value)
    {
        value = Mathf.Clamp01(value);
        return value * value * (3f - 2f * value);
    }

    /// <summary>
    /// Stops deep bowl indents while keeping gradual hill slopes.
    /// </summary>
    private static void LimitDepressions(int[,] tiers, int size, int maxDepression, int minTier)
    {
        if (maxDepression < 1)
        {
            return;
        }

        int[,] buffer = new int[size, size];

        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                buffer[z, x] = tiers[z, x];
            }
        }

        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                int neighborAverage = GetNeighborAverage(buffer, x, z, size);
                int minAllowed = Mathf.Max(minTier, neighborAverage - maxDepression);
                tiers[z, x] = Mathf.Max(buffer[z, x], minAllowed);
            }
        }
    }

    /// <summary>
    /// Guarantees adjacent block tiles differ by at most maxStep tiers — blocky stairs, not cliffs.
    /// </summary>
    private static void EnforceAdjacentTierStepsUntilStable(
        int[,] tiers,
        int size,
        int maxStep,
        int maxPasses)
    {
        if (maxStep < 1 || maxPasses < 1)
        {
            return;
        }

        for (int pass = 0; pass < maxPasses; pass++)
        {
            if (!EnforceAdjacentTierStepPass(tiers, size, maxStep))
            {
                break;
            }
        }
    }

    private static bool EnforceAdjacentTierStepPass(int[,] tiers, int size, int maxStep)
    {
        int[,] buffer = CopyTierMap(tiers, size);
        bool changed = false;

        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                int minAllowed = int.MinValue;
                int maxAllowed = int.MaxValue;

                for (int offsetZ = -1; offsetZ <= 1; offsetZ++)
                {
                    for (int offsetX = -1; offsetX <= 1; offsetX++)
                    {
                        if (offsetX == 0 && offsetZ == 0)
                        {
                            continue;
                        }

                        int sampleX = x + offsetX;
                        int sampleZ = z + offsetZ;
                        if (sampleX < 0 || sampleZ < 0 || sampleX >= size || sampleZ >= size)
                        {
                            continue;
                        }

                        int neighborTier = buffer[sampleZ, sampleX];
                        minAllowed = Mathf.Max(minAllowed, neighborTier - maxStep);
                        maxAllowed = Mathf.Min(maxAllowed, neighborTier + maxStep);
                    }
                }

                int clamped = Mathf.Clamp(buffer[z, x], minAllowed, maxAllowed);
                if (clamped != tiers[z, x])
                {
                    changed = true;
                }

                tiers[z, x] = clamped;
            }
        }

        return changed;
    }

    private static int[,] CopyTierMap(int[,] tiers, int size)
    {
        int[,] copy = new int[size, size];

        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                copy[z, x] = tiers[z, x];
            }
        }

        return copy;
    }

    private static int GetNeighborAverage(int[,] tiers, int x, int z, int size)
    {
        int sum = 0;
        int count = 0;

        for (int offsetZ = -1; offsetZ <= 1; offsetZ++)
        {
            for (int offsetX = -1; offsetX <= 1; offsetX++)
            {
                if (offsetX == 0 && offsetZ == 0)
                {
                    continue;
                }

                int sampleX = x + offsetX;
                int sampleZ = z + offsetZ;
                if (sampleX < 0 || sampleZ < 0 || sampleX >= size || sampleZ >= size)
                {
                    continue;
                }

                sum += tiers[sampleZ, sampleX];
                count++;
            }
        }

        if (count == 0)
        {
            return tiers[z, x];
        }

        return Mathf.RoundToInt(sum / (float)count);
    }

    private static float GetDeterministicNoiseOffset(int seed, int salt)
    {
        unchecked
        {
            uint hash = (uint)seed;
            hash ^= (uint)salt * 0x9E3779B9u;
            hash ^= hash >> 16;
            hash *= 0x85EBCA6Bu;
            hash ^= hash >> 13;
            hash *= 0xC2B2AE35u;
            hash ^= hash >> 16;
            return (hash / (float)uint.MaxValue) * 100_000f;
        }
    }
}
