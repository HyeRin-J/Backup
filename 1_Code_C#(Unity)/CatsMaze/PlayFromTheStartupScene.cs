using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class PlayFromTheStartupScene
{
    static PlayFromTheStartupScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            if (EditorPrefs.HasKey("lastLoadedScenePath"))
            {
                EditorSceneManager.OpenScene(EditorPrefs.GetString("lastLoadedScenePath"));
                EditorPrefs.DeleteKey("lastLoadedScenePath");
            }
        }
    }

    [MenuItem("Edit/PlayFromStartupScene %0")]
    public static void PlayFromStartupScene()
    {
        if (EditorApplication.isPlaying == true)
        {
            EditorApplication.isPlaying = false;
            return;
        }

        EditorPrefs.SetString("lastLoadedScenePath", EditorSceneManager.GetActiveScene().path);
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/DataScene.unity");
        EditorApplication.isPlaying = true;
    }
}
