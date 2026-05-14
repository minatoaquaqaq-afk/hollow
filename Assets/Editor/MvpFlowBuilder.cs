using UnityEditor;
using UnityEditor.SceneManagement;

namespace HollowStyleMVP.EditorTools
{
    public static class MvpFlowBuilder
    {
        [MenuItem("Hollow Style MVP/Rebuild Complete Test Flow")]
        [MenuItem("Tools/Hollow Style MVP/Rebuild Complete Test Flow")]
        [MenuItem("Assets/Hollow Style MVP/Rebuild Complete Test Flow")]
        [MenuItem("GameObject/Hollow Style MVP/Rebuild Complete Test Flow", false, 48)]
        public static void RebuildCompleteTestFlow()
        {
            DefaultConfigBuilder.CreateDefaultConfigAssets();
            MvpUiBuilder.BuildUiPrefabs();
            MvpSceneBuilder.BuildTestRoom();
            MvpSceneBuilder.BuildTestRoom2();
            MvpUiBuilder.BuildMainMenuScene();
            BuildSettingsHelper.AddMvpScenes();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            EditorUtility.DisplayDialog("Hollow Style MVP", "完整测试流程已生成并已打开 MainMenu。现在点击 Play：MainMenu -> TestRoom -> 传送门到 TestRoom2。", "OK");
        }
    }
}



