using HollowStyleMVP.Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HollowStyleMVP.Level
{
    public class Teleporter : MonoBehaviour, IInteractable
    {
        [SerializeField] private string sceneName = "TestRoom";
        [SerializeField] private string spawnPointId;
        public string Prompt => "按 E 传送";

        public void Interact()
        {
            if (!string.IsNullOrWhiteSpace(sceneName)) SceneManager.LoadScene(sceneName);
        }
    }
}
