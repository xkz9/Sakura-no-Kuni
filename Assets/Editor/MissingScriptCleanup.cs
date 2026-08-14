#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Finds and removes missing script components from the open scene.
/// </summary>
public static class MissingScriptCleanup
{
    private const string SampleScenePath = "Assets/Scenes/SampleScene.unity";

    [MenuItem("Japanese City Builder/Fix/Remove Missing Scripts In Scene")]
    public static void RemoveMissingScriptsInOpenScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.isLoaded)
        {
            Debug.LogError("MissingScriptCleanup: No scene is loaded.");
            return;
        }

        int removed = RemoveFromScene(scene, out string report);
        Debug.Log(report);

        if (removed > 0)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveOpenScenes();
        }
    }

    public static void RemoveMissingScriptsFromSampleSceneBatch()
    {
        Scene scene = EditorSceneManager.OpenScene(SampleScenePath);
        int removed = RemoveFromScene(scene, out string report);
        Debug.Log(report);

        if (removed > 0)
        {
            EditorSceneManager.SaveScene(scene);
        }

        EditorApplication.Exit(0);
    }

    private static int RemoveFromScene(Scene scene, out string report)
    {
        int removed = 0;
        var log = new StringBuilder();
        log.AppendLine($"MissingScriptCleanup: scanning scene '{scene.name}'.");

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            removed += RemoveFromGameObject(root, root.name, log);
        }

        log.AppendLine($"MissingScriptCleanup: removed {removed} missing script component(s).");
        report = log.ToString();
        return removed;
    }

    private static int RemoveFromGameObject(GameObject gameObject, string path, StringBuilder log)
    {
        int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);
        if (missingCount > 0)
        {
            log.AppendLine($"  - {path}: {missingCount} missing");
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
        }

        int removed = missingCount;
        foreach (Transform child in gameObject.transform)
        {
            removed += RemoveFromGameObject(child.gameObject, $"{path}/{child.name}", log);
        }

        return removed;
    }
}

public static class MissingScriptCleanupBatchRunner
{
    public static void Run()
    {
        MissingScriptCleanup.RemoveMissingScriptsFromSampleSceneBatch();
    }
}
#endif
