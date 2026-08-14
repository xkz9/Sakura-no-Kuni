using UnityEngine;

/// <summary>
/// Builds placeholder evergreen trees with a visible trunk, layered foliage, and simple procedural textures.
/// </summary>
public static class StylizedTreeBuilder
{
    public sealed class Palette
    {
        public Material Trunk;
        public Material FoliageDark;
        public Material FoliageMid;
        public Material FoliageLight;
        public Material FoliageTip;
    }

    public static Palette CreatePalette()
    {
        Texture2D barkTexture = CreateBarkTexture();
        Texture2D foliageTexture = CreateFoliageTexture();

        return new Palette
        {
            Trunk = CreateTexturedMaterial(
                barkTexture,
                new Color(0.50f, 0.31f, 0.16f),
                0.12f),
            FoliageDark = CreateTexturedMaterial(
                foliageTexture,
                new Color(0.12f, 0.38f, 0.18f),
                0.08f),
            FoliageMid = CreateTexturedMaterial(
                foliageTexture,
                new Color(0.16f, 0.46f, 0.21f),
                0.10f),
            FoliageLight = CreateTexturedMaterial(
                foliageTexture,
                new Color(0.22f, 0.54f, 0.25f),
                0.12f),
            FoliageTip = CreateTexturedMaterial(
                foliageTexture,
                new Color(0.32f, 0.64f, 0.30f),
                0.14f)
        };
    }

    public static GameObject Build(Palette palette, System.Random random)
    {
        GameObject treeRoot = new GameObject("StylizedEvergreen");
        Transform foliageRoot = new GameObject("Foliage").transform;
        foliageRoot.SetParent(treeRoot.transform, false);

        float widthScale = 0.92f + (float)random.NextDouble() * 0.16f;

        BuildTrunk(treeRoot.transform, palette.Trunk);
        BuildFoliage(foliageRoot, palette, widthScale, random);

        float leanX = Lerp(random, -2f, 2f);
        float leanZ = Lerp(random, -2f, 2f);
        foliageRoot.localRotation = Quaternion.Euler(leanX, Lerp(random, 0f, 360f), leanZ);

        return treeRoot;
    }

    private static void BuildTrunk(Transform parent, Material trunkMaterial)
    {
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        trunk.transform.SetParent(parent, false);
        trunk.transform.localPosition = new Vector3(0f, 0.72f, 0f);
        trunk.transform.localScale = new Vector3(0.34f, 0.72f, 0.34f);
        ApplyMaterial(trunk, trunkMaterial);
    }

    private static void BuildFoliage(
        Transform foliageRoot,
        Palette palette,
        float widthScale,
        System.Random random)
    {
        AddCapsuleTier(foliageRoot, palette.FoliageDark, 1.95f, 0.95f, 1.30f, 0f);
        AddCapsuleTier(foliageRoot, palette.FoliageMid, 1.55f, 0.78f, 1.95f, 0.05f);
        AddCapsuleTier(foliageRoot, palette.FoliageLight, 1.15f, 0.66f, 2.55f, -0.04f);
        AddCapsuleTier(foliageRoot, palette.FoliageLight, 0.82f, 0.52f, 3.05f, 0.03f);
        AddSphereTip(foliageRoot, palette.FoliageTip, 0.48f, 3.45f);

        if (random.NextDouble() > 0.25d)
        {
            AddBranchNub(foliageRoot, palette.FoliageMid, 1.35f, 1.75f, 18f);
        }

        if (random.NextDouble() > 0.35d)
        {
            AddBranchNub(foliageRoot, palette.FoliageLight, 1.05f, 2.35f, -24f);
        }

        foliageRoot.localScale = new Vector3(widthScale, 1f, widthScale);
    }

    private static void AddCapsuleTier(
        Transform parent,
        Material material,
        float width,
        float height,
        float y,
        float xOffset)
    {
        GameObject tier = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        tier.name = "FoliageTier";
        tier.transform.SetParent(parent, false);
        tier.transform.localPosition = new Vector3(xOffset, y, 0f);
        tier.transform.localScale = new Vector3(width, height, width);
        ApplyMaterial(tier, material);
    }

    private static void AddSphereTip(Transform parent, Material material, float size, float y)
    {
        GameObject tip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tip.name = "FoliageTip";
        tip.transform.SetParent(parent, false);
        tip.transform.localPosition = new Vector3(0f, y, 0f);
        tip.transform.localScale = new Vector3(size, size * 0.85f, size);
        ApplyMaterial(tip, material);
    }

    private static void AddBranchNub(Transform parent, Material material, float size, float y, float yaw)
    {
        GameObject branch = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        branch.name = "BranchNub";
        branch.transform.SetParent(parent, false);
        branch.transform.localPosition = new Vector3(0f, y, 0f);
        branch.transform.localRotation = Quaternion.Euler(28f, yaw, 0f);
        branch.transform.localScale = new Vector3(size * 0.45f, size * 0.35f, size * 0.45f);
        ApplyMaterial(branch, material);
    }

    private static void ApplyMaterial(GameObject target, Material material)
    {
        Collider collider = target.GetComponent<Collider>();
        if (collider != null)
        {
            Object.DestroyImmediate(collider);
        }

        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }
    }

    private static Material CreateTexturedMaterial(Texture2D texture, Color tint, float smoothness)
    {
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.SetTexture("_BaseMap", texture);
        material.SetColor("_BaseColor", tint);
        material.SetFloat("_Smoothness", smoothness);
        return material;
    }

    private static Texture2D CreateBarkTexture()
    {
        const int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Repeat,
            filterMode = FilterMode.Bilinear
        };

        Color dark = new Color(0.28f, 0.17f, 0.09f);
        Color mid = new Color(0.42f, 0.26f, 0.14f);
        Color light = new Color(0.52f, 0.33f, 0.18f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float vertical = Mathf.Sin((x / (float)size) * Mathf.PI * 14f) * 0.5f + 0.5f;
                float grain = Mathf.PerlinNoise(x * 0.18f, y * 0.05f);
                float tone = vertical * 0.55f + grain * 0.45f;
                Color color = tone < 0.45f ? Color.Lerp(dark, mid, tone / 0.45f) : Color.Lerp(mid, light, (tone - 0.45f) / 0.55f);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private static Texture2D CreateFoliageTexture()
    {
        const int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Repeat,
            filterMode = FilterMode.Bilinear
        };

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float patch = Mathf.PerlinNoise(x * 0.11f, y * 0.11f);
                float speck = Mathf.PerlinNoise(x * 0.35f + 12.7f, y * 0.35f + 8.3f);
                float tone = patch * 0.7f + speck * 0.3f;
                Color color = Color.Lerp(new Color(0.75f, 0.75f, 0.75f), Color.white, tone);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private static float Lerp(System.Random random, float min, float max)
    {
        return min + (float)random.NextDouble() * (max - min);
    }
}
