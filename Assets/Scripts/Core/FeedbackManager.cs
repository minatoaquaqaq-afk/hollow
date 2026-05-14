using System.Collections;
using HollowStyleMVP.Config;
using UnityEngine;

namespace HollowStyleMVP.Core
{
    public enum FeedbackSound { Jump, Dash, Attack, Hit, Death, UiConfirm, UiCancel }

    public class FeedbackManager : MonoBehaviour
    {
        public static FeedbackManager Instance { get; private set; }
        [SerializeField] private AudioFeedbackConfig audioConfig;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float hitStopSeconds = 0.04f;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        }

        public void Play(FeedbackSound sound)
        {
            AudioClip clip = GetConfiguredClip(sound);
            if (clip == null) clip = CreateTone(sound);
            if (clip != null && audioSource != null) audioSource.PlayOneShot(clip);
        }

        public void HitStop() => StartCoroutine(HitStopRoutine());

        private AudioClip GetConfiguredClip(FeedbackSound sound)
        {
            if (audioConfig == null) return null;
            return sound switch
            {
                FeedbackSound.Jump => audioConfig.jump,
                FeedbackSound.Dash => audioConfig.dash,
                FeedbackSound.Attack => audioConfig.attack,
                FeedbackSound.Hit => audioConfig.hit,
                FeedbackSound.Death => audioConfig.death,
                FeedbackSound.UiConfirm => audioConfig.uiConfirm,
                FeedbackSound.UiCancel => audioConfig.uiCancel,
                _ => null
            };
        }

        private static AudioClip CreateTone(FeedbackSound sound)
        {
            int sampleRate = 22050;
            float duration = sound == FeedbackSound.Death ? 0.22f : 0.08f;
            float frequency = sound switch
            {
                FeedbackSound.Jump => 620f,
                FeedbackSound.Dash => 880f,
                FeedbackSound.Attack => 740f,
                FeedbackSound.Hit => 180f,
                FeedbackSound.Death => 90f,
                FeedbackSound.UiConfirm => 520f,
                FeedbackSound.UiCancel => 260f,
                _ => 440f
            };
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float fade = 1f - i / (float)samples;
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * 0.12f * fade;
            }
            var clip = AudioClip.Create("Tone_" + sound, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private IEnumerator HitStopRoutine()
        {
            float originalScale = Time.timeScale;
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(hitStopSeconds);
            Time.timeScale = originalScale;
        }
    }
}
