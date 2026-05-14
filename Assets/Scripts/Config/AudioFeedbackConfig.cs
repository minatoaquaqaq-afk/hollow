using UnityEngine;

namespace HollowStyleMVP.Config
{
    [CreateAssetMenu(menuName = "Hollow Style MVP/Config/Audio Feedback Config")]
    public class AudioFeedbackConfig : ScriptableObject
    {
        public AudioClip bgm;
        public AudioClip jump;
        public AudioClip dash;
        public AudioClip attack;
        public AudioClip hit;
        public AudioClip death;
        public AudioClip uiConfirm;
        public AudioClip uiCancel;
    }
}
