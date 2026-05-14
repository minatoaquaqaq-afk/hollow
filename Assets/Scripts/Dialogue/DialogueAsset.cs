using UnityEngine;

namespace HollowStyleMVP.Dialogue
{
    [CreateAssetMenu(menuName = "Hollow Style MVP/Dialogue")]
    public class DialogueAsset : ScriptableObject
    {
        public string speakerName;
        [TextArea(2, 5)] public string[] lines;
    }
}
