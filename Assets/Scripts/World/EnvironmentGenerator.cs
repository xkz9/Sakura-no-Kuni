using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Places trees in organic forest zones plus smaller groves and occasional singles.
/// </summary>
public class EnvironmentGenerator
{
    private const string EnvironmentObjectName = "Environment";
    private const string TreesObjectName = "Trees";
    private const float TrunkCollisionFactor = 0.19f;
    private const float FoliageCollisionFactor = 1.05f;
    private const float HexRowFactor = 0.8660254f;

    public void Generate(WorldGenerationContext context)
    {
        if (context.WorldRoot == null)
        {
            Debug.LogError("EnvironmentGenerator: WorldRoot is missing.");
            return;
        }

        if (context.Terrain == null)
        {
            Debug.LogError("EnvironmentGenerator: Terrain is missing.");
            return;
        }

        WorldSettings settings = context.Settings;
        if (settings.treeCount <= 0)
        {
            return;
        }

        Transform environmentRoot = CreateEnvironmentRoot(context.WorldRoot);
        Transform treesRoot = CreateTreesRoot(environmentRoot);

        StylizedTreeBuilder.Palette treePalette = null;
        if (!HasTreePrefabs(settings))
        {
            treePalette = StylizedTreeBuilder.CreatePalette();
        }

        List<PlacedTree> placedTrees = new List<PlacedTree>(settings.treeCount);
        List<ForestZone> forestZones = new List<ForestZone>();

        int majorBudget = Mathf.RoundToInt(settings.treeCount * settings.majorForestTreeRatio);
        int minorBudget = Mathf.RoundToInt(settings.treeCount * settings.minorForestTreeRatio);
        majorBudget = Mathf.Clamp(majorBudget, 0, settings.treeCount);
        minorBudget = Mathf.Clamp(minorBudget, 0, settings.treeCount - majorBudget);
        int scatteredBudget = settings.treeCount - majorBudget - minorBudget;
        int maxScattered = Mathf.RoundToInt(settings.treeCount * settings.maxScatteredTreeRatio);
        scatteredBudget = Mathf.Clamp(scatteredBudget, 0, maxScattered);

        int majorCount = PlaceMajorForestZones(
            context,
            treesRoot,
            placedTrees,
            forestZones,
            treePalette,
            majorBudget,
            settings.majorForestZoneCount,
            settings.treesPerMajorForestMin,
            settings.treesPerMajorForestMax,
            settings.majorForestRadius,
            settings.majorForestPackSpacing,
            settings.majorForestTreeGap,
            settings.forestShapeNoiseScale,
            settings.majorForestInteriorSkipChance);

        int minorCount = PlaceMinorForestZones(
            context,
            treesRoot,
            placedTrees,
            forestZones,
            treePalette,
            minorBudget,
            settings.minorForestZoneCount,
            settings.treesPerMinorForestMin,
            settings.treesPerMinorForestMax,
            settings.minorForestRadius,
            settings.minorForestPackSpacing,
            settings.minorForestTreeGap,
            settings.forestShapeNoiseScale);

        int scatteredCount = PlaceScatteredAccents(
            context,
            treesRoot,
            placedTrees,
            treePalette,
            scatteredBudget,
            settings.scatteredDuoGroupCount,
            settings.scatteredDuoSpacing);

        Debug.Log(
            $"EnvironmentGenerator: Placed {majorCount + minorCount + scatteredCount}/{settings.treeCount} trees " +
            $"(majorForests={majorCount}, minorGroves={minorCount}, accents={scatteredCount}, seed={context.Seed}).",
            treesRoot.gameObject);
    }

    private static Transform CreateEnvironmentRoot(Transform worldRoot)
    {
        Transform existing = worldRoot.Find(EnvironmentObjectName);
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
        }

        GameObject environmentObject = new GameObject(EnvironmentObjectName);
        environmentObject.transform.SetParent(worldRoot, false);
        environmentObject.transform.localPosition = Vector3.zero;
        environmentObject.transform.localRotation = Quaternion.identity;
        environmentObject.transform.localScale = Vector3.one;
        return environmentObject.transform;
    }

    private static Transform CreateTreesRoot(Transform environmentRoot)
    {
        GameObject treesObject = new GameObject(TreesObjectName);
        treesObject.transform.SetParent(environmentRoot, false);
        treesObject.transform.localPosition = Vector3.zero;
        treesObject.transform.localRotation = Quaternion.identity;
        treesObject.transform.localScale = Vector3.one;
        return treesObject.transform;
    }

    private static int PlaceMajorForestZones(
        WorldGenerationContext context,
        Transform treesRoot,
        List<PlacedTree> placedTrees,
        List<ForestZone> forestZones,
        StylizedTreeBuilder.Palette treePalette,
        int targetCount,
        int zoneCount,
        int treesPerZoneMin,
        int treesPerZoneMax,
        float zoneRadius,
        float packSpacing,
        float treeGap,
        float shapeNoiseScale,
        float interiorSkipChance)
    {
        if (targetCount <= 0 || zoneCount <= 0)
        {
            return 0;
        }

        WorldSettings settings = context.Settings;
        System.Random random = context.SeededRandom;
        SpawnBounds bounds = GetSpawnBounds(settings);
        float castleClearRadiusSquared = settings.castleClearRadius * settings.castleClearRadius;
        Vector2 castleCenter = new Vector2(context.StartPosition.x, context.StartPosition.z);

        int placedCount = 0;
        List<SpawnSlot> slots = BuildSpreadSlots(bounds, zoneCount, random);

        for (int zoneIndex = 0; zoneIndex < slots.Count && placedCount < targetCount; zoneIndex++)
        {
            if (!TryPickForestZoneCenter(
                    random,
                    bounds,
                    castleCenter,
                    castleClearRadiusSquared,
                    placedTrees,
                    forestZones,
                    settings.forestZoneSeparation,
                    zoneRadius,
                    settings,
                    slots[zoneIndex],
                    out Vector2 zoneCenter))
            {
                continue;
            }

            ForestZone zone = CreateForestZone(
                random,
                zoneCenter,
                zoneRadius,
                shapeNoiseScale,
                lobeCount: random.Next(2, 4),
                stretchMin: 0.8f,
                stretchMax: 1.25f);
            forestZones.Add(zone);

            int zoneBudget = random.Next(treesPerZoneMin, treesPerZoneMax + 1);
            zoneBudget = Mathf.Min(zoneBudget, targetCount - placedCount);

            placedCount += FillNaturalForestZone(
                context,
                treesRoot,
                placedTrees,
                treePalette,
                zone,
                packSpacing,
                treeGap,
                zoneBudget,
                random,
                bounds,
                castleCenter,
                castleClearRadiusSquared,
                interiorSkipChance,
                edgeSoftness: 0.72f,
                minStrength: 0.36f);
        }

        return placedCount;
    }

    private static int PlaceMinorForestZones(
        WorldGenerationContext context,
        Transform treesRoot,
        List<PlacedTree> placedTrees,
        List<ForestZone> forestZones,
        StylizedTreeBuilder.Palette treePalette,
        int targetCount,
        int zoneCount,
        int treesPerZoneMin,
        int treesPerZoneMax,
        float zoneRadius,
        float packSpacing,
        float treeGap,
        float shapeNoiseScale)
    {
        if (targetCount <= 0 || zoneCount <= 0)
        {
            return 0;
        }

        WorldSettings settings = context.Settings;
        System.Random random = context.SeededRandom;
        SpawnBounds bounds = GetSpawnBounds(settings);
        float castleClearRadiusSquared = settings.castleClearRadius * settings.castleClearRadius;
        Vector2 castleCenter = new Vector2(context.StartPosition.x, context.StartPosition.z);

        int placedCount = 0;
        List<SpawnSlot> slots = BuildSpreadSlots(bounds, zoneCount, random);

        for (int zoneIndex = 0; zoneIndex < slots.Count && placedCount < targetCount; zoneIndex++)
        {
            if (!TryPickForestZoneCenter(
                    random,
                    bounds,
                    castleCenter,
                    castleClearRadiusSquared,
                    placedTrees,
                    forestZones,
                    settings.forestZoneSeparation * 0.75f,
                    zoneRadius,
                    settings,
                    slots[zoneIndex],
                    out Vector2 zoneCenter))
            {
                continue;
            }

            ForestZone zone = CreateForestZone(
                random,
                zoneCenter,
                zoneRadius,
                shapeNoiseScale,
                lobeCount: random.Next(1, 3),
                stretchMin: 0.65f,
                stretchMax: 1.2f);
            forestZones.Add(zone);

            int zoneBudget = random.Next(treesPerZoneMin, treesPerZoneMax + 1);
            zoneBudget = Mathf.Min(zoneBudget, targetCount - placedCount);

            placedCount += FillNaturalForestZone(
                context,
                treesRoot,
                placedTrees,
                treePalette,
                zone,
                packSpacing,
                treeGap,
                zoneBudget,
                random,
                bounds,
                castleCenter,
                castleClearRadiusSquared,
                interiorSkipChance: 0.28f,
                edgeSoftness: 0.78f,
                minStrength: 0.32f);
        }

        return placedCount;
    }

    private static ForestZone CreateForestZone(
        System.Random random,
        Vector2 center,
        float baseRadius,
        float shapeNoiseScale,
        int lobeCount,
        float stretchMin,
        float stretchMax)
    {
        ForestZone zone = new ForestZone
        {
            Center = center,
            RotationRadians = NextFloat(random, 0f, Mathf.PI * 2f),
            NoiseOffsetX = NextFloat(random, 0f, 1000f),
            NoiseOffsetY = NextFloat(random, 0f, 1000f),
            NoiseScale = shapeNoiseScale
        };

        for (int lobeIndex = 0; lobeIndex < lobeCount; lobeIndex++)
        {
            float angle = NextFloat(random, 0f, Mathf.PI * 2f);
            float lobeDistance = baseRadius * NextFloat(random, 0.08f, 0.42f);
            Vector2 lobeCenter = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * lobeDistance;
            float radiusX = baseRadius * NextFloat(random, stretchMin, stretchMax);
            float radiusZ = baseRadius * NextFloat(random, stretchMin, stretchMax);

            zone.Lobes.Add(new ForestLobe(lobeCenter, radiusX, radiusZ));
        }

        return zone;
    }

    private static int FillNaturalForestZone(
        WorldGenerationContext context,
        Transform treesRoot,
        List<PlacedTree> placedTrees,
        StylizedTreeBuilder.Palette treePalette,
        ForestZone zone,
        float packSpacing,
        float treeGap,
        int maxTrees,
        System.Random random,
        SpawnBounds bounds,
        Vector2 castleCenter,
        float castleClearRadiusSquared,
        float interiorSkipChance,
        float edgeSoftness,
        float minStrength)
    {
        if (maxTrees <= 0 || packSpacing <= 0f)
        {
            return 0;
        }

        float maxRadius = zone.GetMaxRadius();
        float rowHeight = packSpacing * HexRowFactor;
        int extent = Mathf.CeilToInt(maxRadius / packSpacing);
        float jitterRadius = packSpacing * 0.28f;

        int placedCount = 0;
        List<Vector2Int> cellOrder = BuildShuffledCellOrder(random, extent);

        for (int cellIndex = 0; cellIndex < cellOrder.Count && placedCount < maxTrees; cellIndex++)
        {
            Vector2Int cell = cellOrder[cellIndex];
            float rowOffset = (cell.y & 1) != 0 ? packSpacing * 0.5f : 0f;
            Vector2 local = new Vector2(cell.x * packSpacing + rowOffset, cell.y * rowHeight);

            Vector2 jitter = new Vector2(
                NextFloat(random, -jitterRadius, jitterRadius),
                NextFloat(random, -jitterRadius, jitterRadius));
            Vector2 candidate = zone.Center + Rotate(local + jitter, zone.RotationRadians);

            if (!bounds.Contains(candidate))
            {
                continue;
            }

            if ((candidate - castleCenter).sqrMagnitude < castleClearRadiusSquared)
            {
                continue;
            }

            float strength = zone.EvaluateStrength(candidate);
            if (strength < minStrength)
            {
                continue;
            }

            float edgeFactor = Mathf.InverseLerp(minStrength, 1f, strength);
            float placeChance = edgeSoftness + edgeFactor * (1f - edgeSoftness);
            placeChance *= 1f - interiorSkipChance * (1f - edgeFactor);

            if (random.NextDouble() > placeChance)
            {
                continue;
            }

            if (TryPlaceTreeAt(
                    context,
                    treesRoot,
                    placedTrees,
                    treePalette,
                    candidate,
                    random,
                    treeGap,
                    useTrunkCollision: true))
            {
                placedCount++;
            }
        }

        if (placedCount < maxTrees)
        {
            placedCount += FillForestZoneRandomTopUp(
                context,
                treesRoot,
                placedTrees,
                treePalette,
                zone,
                treeGap,
                maxTrees - placedCount,
                random,
                bounds,
                castleCenter,
                castleClearRadiusSquared,
                minStrength);
        }

        return placedCount;
    }

    private static int FillForestZoneRandomTopUp(
        WorldGenerationContext context,
        Transform treesRoot,
        List<PlacedTree> placedTrees,
        StylizedTreeBuilder.Palette treePalette,
        ForestZone zone,
        float treeGap,
        int treesNeeded,
        System.Random random,
        SpawnBounds bounds,
        Vector2 castleCenter,
        float castleClearRadiusSquared,
        float minStrength)
    {
        if (treesNeeded <= 0)
        {
            return 0;
        }

        float maxRadius = zone.GetMaxRadius();
        int placedCount = 0;
        int maxAttempts = treesNeeded * 30;

        for (int attempt = 0; attempt < maxAttempts && placedCount < treesNeeded; attempt++)
        {
            Vector2 offset = RandomPointInRadius(random, maxRadius);
            Vector2 candidate = zone.Center + Rotate(offset, zone.RotationRadians);

            if (!bounds.Contains(candidate))
            {
                continue;
            }

            if ((candidate - castleCenter).sqrMagnitude < castleClearRadiusSquared)
            {
                continue;
            }

            if (zone.EvaluateStrength(candidate) < minStrength)
            {
                continue;
            }

            if (TryPlaceTreeAt(
                    context,
                    treesRoot,
                    placedTrees,
                    treePalette,
                    candidate,
                    random,
                    treeGap,
                    useTrunkCollision: true))
            {
                placedCount++;
            }
        }

        return placedCount;
    }

    private static Vector2 RandomPointInRadius(System.Random random, float radius)
    {
        float angle = NextFloat(random, 0f, Mathf.PI * 2f);
        float distance = Mathf.Sqrt((float)random.NextDouble()) * radius;
        return new Vector2(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
    }

    private static List<SpawnSlot> BuildSpreadSlots(SpawnBounds bounds, int count, System.Random random)
    {
        List<SpawnSlot> slots = new List<SpawnSlot>(count);
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(count));
        float cellWidth = bounds.Size / gridSize;
        float inset = cellWidth * 0.12f;

        List<int> cellIndices = new List<int>(gridSize * gridSize);
        for (int i = 0; i < gridSize * gridSize; i++)
        {
            cellIndices.Add(i);
        }

        Shuffle(cellIndices, random);

        for (int i = 0; i < count && i < cellIndices.Count; i++)
        {
            int cell = cellIndices[i];
            int col = cell % gridSize;
            int row = cell / gridSize;

            float minX = bounds.Min + col * cellWidth + inset;
            float maxX = bounds.Min + (col + 1) * cellWidth - inset;
            float minZ = bounds.Min + row * cellWidth + inset;
            float maxZ = bounds.Min + (row + 1) * cellWidth - inset;

            slots.Add(new SpawnSlot(minX, maxX, minZ, maxZ));
        }

        return slots;
    }

    private static int PlaceScatteredAccents(
        WorldGenerationContext context,
        Transform treesRoot,
        List<PlacedTree> placedTrees,
        StylizedTreeBuilder.Palette treePalette,
        int scatteredBudget,
        int duoGroupCount,
        float duoSpacing)
    {
        if (scatteredBudget <= 0)
        {
            return 0;
        }

        WorldSettings settings = context.Settings;
        System.Random random = context.SeededRandom;
        SpawnBounds bounds = GetSpawnBounds(settings);
        float castleClearRadiusSquared = settings.castleClearRadius * settings.castleClearRadius;
        Vector2 castleCenter = new Vector2(context.StartPosition.x, context.StartPosition.z);

        int duoGroups = Mathf.Min(duoGroupCount, scatteredBudget / 2);
        int singlesBudget = scatteredBudget - duoGroups * 2;
        int placedCount = 0;

        for (int duoIndex = 0; duoIndex < duoGroups; duoIndex++)
        {
            for (int attempt = 0; attempt < 40; attempt++)
            {
                Vector2 anchor = bounds.RandomPoint(random);

                if ((anchor - castleCenter).sqrMagnitude < castleClearRadiusSquared)
                {
                    continue;
                }

                if (!TryPlaceTreeAt(
                        context,
                        treesRoot,
                        placedTrees,
                        treePalette,
                        anchor,
                        random,
                        settings.scatteredTreeGap,
                        useTrunkCollision: false))
                {
                    continue;
                }

                placedCount++;

                float angle = NextFloat(random, 0f, Mathf.PI * 2f);
                float pairDistance = duoSpacing * NextFloat(random, 0.85f, 1.15f);
                Vector2 partner = anchor + new Vector2(
                    Mathf.Cos(angle) * pairDistance,
                    Mathf.Sin(angle) * pairDistance);

                if (bounds.Contains(partner)
                    && (partner - castleCenter).sqrMagnitude >= castleClearRadiusSquared
                    && TryPlaceTreeAt(
                        context,
                        treesRoot,
                        placedTrees,
                        treePalette,
                        partner,
                        random,
                        settings.scatteredTreeGap * 0.55f,
                        useTrunkCollision: true))
                {
                    placedCount++;
                }

                break;
            }
        }

        placedCount += PlaceScatteredSingles(
            context,
            treesRoot,
            placedTrees,
            treePalette,
            singlesBudget,
            bounds,
            castleCenter,
            castleClearRadiusSquared,
            settings.scatteredTreeGap);

        return placedCount;
    }

    private static int PlaceScatteredSingles(
        WorldGenerationContext context,
        Transform treesRoot,
        List<PlacedTree> placedTrees,
        StylizedTreeBuilder.Palette treePalette,
        int targetCount,
        SpawnBounds bounds,
        Vector2 castleCenter,
        float castleClearRadiusSquared,
        float treeGap)
    {
        if (targetCount <= 0)
        {
            return 0;
        }

        System.Random random = context.SeededRandom;
        int placedCount = 0;
        int maxAttempts = targetCount * 50;

        for (int attempt = 0; attempt < maxAttempts && placedCount < targetCount; attempt++)
        {
            Vector2 candidate = bounds.RandomPoint(random);

            if ((candidate - castleCenter).sqrMagnitude < castleClearRadiusSquared)
            {
                continue;
            }

            if (TryPlaceTreeAt(
                    context,
                    treesRoot,
                    placedTrees,
                    treePalette,
                    candidate,
                    random,
                    treeGap,
                    useTrunkCollision: false))
            {
                placedCount++;
            }
        }

        return placedCount;
    }

    private static bool TryPlaceTreeAt(
        WorldGenerationContext context,
        Transform treesRoot,
        List<PlacedTree> placedTrees,
        StylizedTreeBuilder.Palette treePalette,
        Vector2 candidate,
        System.Random random,
        float extraGap,
        bool useTrunkCollision)
    {
        WorldSettings settings = context.Settings;
        Terrain terrain = context.Terrain;

        float sizeVariation = NextFloat(random, 0.88f, 1.12f);
        float treeRadius = GetTreeCollisionRadius(settings, sizeVariation, useTrunkCollision);

        if (OverlapsExistingTree(candidate, placedTrees, treeRadius, extraGap))
        {
            return false;
        }

        Vector3 worldPosition = new Vector3(candidate.x, 0f, candidate.y);
        worldPosition.y = terrain.SampleHeight(worldPosition);

        GameObject treeInstance = CreateTreeInstance(settings, random, treePalette);
        if (treeInstance == null)
        {
            return false;
        }

        treeInstance.transform.SetParent(treesRoot, false);
        treeInstance.transform.position = worldPosition;
        treeInstance.transform.rotation = Quaternion.Euler(0f, NextFloat(random, 0f, 360f), 0f);
        treeInstance.transform.localScale = Vector3.one * settings.treeScale * sizeVariation;

        placedTrees.Add(new PlacedTree(candidate, treeRadius));
        return true;
    }

    private static bool TryPickForestZoneCenter(
        System.Random random,
        SpawnBounds bounds,
        Vector2 castleCenter,
        float castleClearRadiusSquared,
        List<PlacedTree> placedTrees,
        List<ForestZone> forestZones,
        float forestZoneSeparation,
        float zoneRadius,
        WorldSettings settings,
        SpawnSlot slot,
        out Vector2 zoneCenter)
    {
        float maxTreeRadius = GetTreeCollisionRadius(settings, 1.12f, useTrunkCollision: false);
        float minCenterSpacing = forestZoneSeparation + zoneRadius + maxTreeRadius;

        for (int attempt = 0; attempt < 30; attempt++)
        {
            zoneCenter = slot.RandomPoint(random);

            if ((zoneCenter - castleCenter).sqrMagnitude < castleClearRadiusSquared)
            {
                continue;
            }

            if (IsTooCloseToForestZones(zoneCenter, forestZones, minCenterSpacing))
            {
                continue;
            }

            if (OverlapsExistingTree(zoneCenter, placedTrees, maxTreeRadius, zoneRadius * 0.25f))
            {
                continue;
            }

            return true;
        }

        zoneCenter = Vector2.zero;
        return false;
    }

    private static bool IsTooCloseToForestZones(
        Vector2 candidate,
        List<ForestZone> forestZones,
        float minCenterSpacing)
    {
        float minSpacingSquared = minCenterSpacing * minCenterSpacing;

        for (int i = 0; i < forestZones.Count; i++)
        {
            if ((candidate - forestZones[i].Center).sqrMagnitude < minSpacingSquared)
            {
                return true;
            }
        }

        return false;
    }

    private static bool OverlapsExistingTree(
        Vector2 candidate,
        List<PlacedTree> placedTrees,
        float candidateRadius,
        float extraGap)
    {
        for (int i = 0; i < placedTrees.Count; i++)
        {
            PlacedTree placed = placedTrees[i];
            float minDistance = placed.Radius + candidateRadius + extraGap;
            if ((candidate - placed.Position).sqrMagnitude < minDistance * minDistance)
            {
                return true;
            }
        }

        return false;
    }

    private static float GetTreeCollisionRadius(
        WorldSettings settings,
        float sizeVariation,
        bool useTrunkCollision)
    {
        float factor = useTrunkCollision ? TrunkCollisionFactor : FoliageCollisionFactor;
        return settings.treeScale * sizeVariation * factor;
    }

    private static Vector2 Rotate(Vector2 point, float radians)
    {
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(
            point.x * cos - point.y * sin,
            point.x * sin + point.y * cos);
    }

    private static List<Vector2Int> BuildShuffledCellOrder(System.Random random, int extent)
    {
        List<Vector2Int> cells = new List<Vector2Int>((extent * 2 + 1) * (extent * 2 + 1));

        for (int row = -extent; row <= extent; row++)
        {
            for (int col = -extent; col <= extent; col++)
            {
                cells.Add(new Vector2Int(col, row));
            }
        }

        Shuffle(cells, random);
        return cells;
    }

    private static void Shuffle<T>(List<T> list, System.Random random)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int swapIndex = random.Next(i + 1);
            T temp = list[i];
            list[i] = list[swapIndex];
            list[swapIndex] = temp;
        }
    }

    private static SpawnBounds GetSpawnBounds(WorldSettings settings)
    {
        float halfMapSize = settings.mapSize * 0.5f;
        float minCoord = -halfMapSize + settings.environmentSpawnMargin;
        float maxCoord = halfMapSize - settings.environmentSpawnMargin;
        return new SpawnBounds(minCoord, maxCoord);
    }

    private static GameObject CreateTreeInstance(
        WorldSettings settings,
        System.Random random,
        StylizedTreeBuilder.Palette treePalette)
    {
        if (HasTreePrefabs(settings))
        {
            GameObject prefab = settings.treePrefabs[random.Next(settings.treePrefabs.Length)];
            if (prefab != null)
            {
                return Object.Instantiate(prefab);
            }
        }

        return StylizedTreeBuilder.Build(treePalette, random);
    }

    private static bool HasTreePrefabs(WorldSettings settings)
    {
        return settings.treePrefabs != null && settings.treePrefabs.Length > 0;
    }

    private static float NextFloat(System.Random random, float min, float max)
    {
        return min + (float)random.NextDouble() * (max - min);
    }

    private sealed class ForestZone
    {
        public Vector2 Center;
        public float RotationRadians;
        public float NoiseOffsetX;
        public float NoiseOffsetY;
        public float NoiseScale;
        public List<ForestLobe> Lobes = new List<ForestLobe>();

        public float GetMaxRadius()
        {
            float maxRadius = 0f;

            for (int i = 0; i < Lobes.Count; i++)
            {
                ForestLobe lobe = Lobes[i];
                float lobeReach = (lobe.Center - Center).magnitude + Mathf.Max(lobe.RadiusX, lobe.RadiusZ);
                maxRadius = Mathf.Max(maxRadius, lobeReach);
            }

            return maxRadius;
        }

        public float EvaluateStrength(Vector2 worldPosition)
        {
            float maxStrength = 0f;

            for (int i = 0; i < Lobes.Count; i++)
            {
                ForestLobe lobe = Lobes[i];
                Vector2 local = worldPosition - lobe.Center;
                local = Rotate(local, -RotationRadians);

                float normalizedX = local.x / lobe.RadiusX;
                normalizedX *= normalizedX;
                float normalizedZ = local.y / lobe.RadiusZ;
                normalizedZ *= normalizedZ;
                float ellipseDistance = normalizedX + normalizedZ;

                if (ellipseDistance > 1f)
                {
                    continue;
                }

                float edgeFalloff = 1f - ellipseDistance;
                float noise = Mathf.PerlinNoise(
                    worldPosition.x * NoiseScale + NoiseOffsetX,
                    worldPosition.y * NoiseScale + NoiseOffsetY);
                float strength = edgeFalloff * (0.5f + noise * 0.5f);
                maxStrength = Mathf.Max(maxStrength, strength);
            }

            return maxStrength;
        }
    }

    private readonly struct ForestLobe
    {
        public Vector2 Center { get; }
        public float RadiusX { get; }
        public float RadiusZ { get; }

        public ForestLobe(Vector2 center, float radiusX, float radiusZ)
        {
            Center = center;
            RadiusX = radiusX;
            RadiusZ = radiusZ;
        }
    }

    private readonly struct PlacedTree
    {
        public Vector2 Position { get; }
        public float Radius { get; }

        public PlacedTree(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
        }
    }

    private readonly struct SpawnSlot
    {
        private readonly float minX;
        private readonly float maxX;
        private readonly float minZ;
        private readonly float maxZ;

        public SpawnSlot(float minX, float maxX, float minZ, float maxZ)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minZ = minZ;
            this.maxZ = maxZ;
        }

        public Vector2 RandomPoint(System.Random random)
        {
            float x = minX + (float)random.NextDouble() * (maxX - minX);
            float z = minZ + (float)random.NextDouble() * (maxZ - minZ);
            return new Vector2(x, z);
        }
    }

    private readonly struct SpawnBounds
    {
        public float Min { get; }
        public float Max { get; }
        public float Size => Max - Min;

        public SpawnBounds(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public bool Contains(Vector2 point)
        {
            return point.x >= Min && point.x <= Max && point.y >= Min && point.y <= Max;
        }

        public Vector2 RandomPoint(System.Random random)
        {
            float x = Min + (float)random.NextDouble() * Size;
            float z = Min + (float)random.NextDouble() * Size;
            return new Vector2(x, z);
        }
    }
}
