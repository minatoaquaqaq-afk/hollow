using System.Collections.Generic;
using UnityEditor;

namespace HollowStyleMVP.EditorTools
{
    public static class BuildSettingsHelper
    {
        [MenuItem("Hollow Style MVP/Add MVP Scenes To Build Settings")]
        [MenuItem("Tools/Hollow Style MVP/Add MVP Scenes To Build Settings")]
        [MenuItem("Assets/Hollow Style MVP/Add MVP Scenes To Build Settings")]
        [MenuItem("GameObject/Hollow Style MVP/Add MVP Scenes To Build Settings", false, 49)]
        public static void AddMvpScenes()
        {
            var scenes = new List<EditorBuildSettingsScene>
            {
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/TestRoom.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/TestRoom2.unity", true)
            };

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}



