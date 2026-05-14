using HollowStyleMVP.Interaction;
using UnityEngine;

namespace HollowStyleMVP.Dialogue
{
    public class DialogueNpc : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueAsset dialogue;
        [SerializeField] private string fallbackSpeaker = "测试 NPC";
        [TextArea(2, 5)] [SerializeField] private string[] fallbackLines =
        {
            "你好，这是对话功能测试。",
            "按 E 或空格进入下一句，按 Esc 关闭对话。"
        };

        public string Prompt => "按 E 对话";

        public void Interact()
        {
            if (dialogue != null) DialogueController.Instance?.Open(dialogue);
            else DialogueController.Instance?.OpenLines(fallbackSpeaker, fallbackLines);
        }
    }
}
