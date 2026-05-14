using HollowStyleMVP.Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HollowStyleMVP.Level
{
    public class SceneDoor : MonoBehaviour, IInteractable
    {
        [SerializeField] private string sceneName;
        [SerializeField] private string spawnPointId;
        public string Prompt => "按 E 进入";

        public void Interact()
        {
            if (!string.IsNullOrWhiteSpace(sceneName)) SceneManager.LoadScene(sceneName);
        }
    }
}
