using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "App Version", menuName = "App Version/Create App Version")]
public class AppVersionSO : ScriptableObject
{
    [Header("Version Information")]
    [SerializeField] private string appVersion = "0.0.0";

    [Header("Auto Increment Settings")]
    [SerializeField] private VersionIncrementType incrementType = VersionIncrementType.Build;

    public string AppVersionString => appVersion;

    public enum VersionIncrementType
    {
        Build,    // 1.0.0 -> 1.0.1
        Minor,    // 1.0.0 -> 1.1.0
        Major     // 1.0.0 -> 2.0.0
    }

    public void IncrementVersion()
    {
        var parts = appVersion.Split('.');
        if (parts.Length != 3)
        {
            Debug.LogWarning("Version format should be X.Y.Z (e.g., 1.0.0)");
            return;
        }

        if (!int.TryParse(parts[0], out int major) ||
            !int.TryParse(parts[1], out int minor) ||
            !int.TryParse(parts[2], out int build))
        {
            Debug.LogError("Invalid version format. Unable to parse version numbers.");
            return;
        }

        switch (incrementType)
        {
            case VersionIncrementType.Major:
                major++;
                minor = 0;
                build = 0;
                break;
            case VersionIncrementType.Minor:
                minor++;
                build = 0;
                break;
            case VersionIncrementType.Build:
            default:
                build++;
                break;
        }

        appVersion = $"{major}.{minor}.{build}";

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
        Debug.Log($"Version updated to: {appVersion}");
    }

    public void SyncWithPlayerSettings() // Help function to sync version from Inspector
    {
#if UNITY_EDITOR
        appVersion = PlayerSettings.bundleVersion;
        EditorUtility.SetDirty(this);
        Debug.Log($"Synced AppVersionSO with PlayerSettings: {appVersion}");
#endif
    }

#if UNITY_EDITOR
    public void ExecuteBuildProcess(bool shouldIncrement) // Help function to build from Inspector
    {
        if (shouldIncrement)
        {
            // Increase the SO version and update the player setting version to match SO version
            IncrementVersion();
            PlayerSettings.bundleVersion = AppVersionString;
            AssetDatabase.SaveAssets();
        }
        else
        {
            // Update the player setting version to match SO version
            PlayerSettings.bundleVersion = AppVersionString;
        }

        // Open Unity build windows
        BuildPlayerWindow.ShowBuildPlayerWindow();
    }
#endif
}

#if UNITY_EDITOR
// --- Custom Editor: Create buttons on Inspector ---
[CustomEditor(typeof(AppVersionSO))]
public class AppVersionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AppVersionSO script = (AppVersionSO)target;

        GUILayout.Space(20);
        GUILayout.Label("Build Actions", EditorStyles.boldLabel);

        // Build next version
        GUI.backgroundColor = new Color(0.7f, 1f, 0.7f); // Green
        if (GUILayout.Button("Build Next Version", GUILayout.Height(35)))
        {
            script.ExecuteBuildProcess(true);
        }

        // Build current version
        GUI.backgroundColor = new Color(0.7f, 0.8f, 1f); // Blue
        if (GUILayout.Button("Build Current Version", GUILayout.Height(35)))
        {
            script.ExecuteBuildProcess(false);
        }

        GUILayout.Space(20);
        GUILayout.Label("Sync Actions", EditorStyles.boldLabel);

        // Set current version from Player Settings to SO
        GUI.backgroundColor = new Color(1f, 0.8f, 0.4f); // Orange
        if (GUILayout.Button("Sync Version From Player Settings", GUILayout.Height(25)))
        {
            script.SyncWithPlayerSettings();
        }

        GUI.backgroundColor = Color.white;
    }
}
#endif