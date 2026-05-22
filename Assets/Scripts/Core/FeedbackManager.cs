using System.Collections;
using HollowStyleMVP.Config;
using UnityEngine;

namespace HollowStyleMVP.Core
{
    public enum FeedbackSound { Jump, Dash, Attack, DownSlash, Hit, Crit, Death, Pickup, Buy, Open, UiConfirm, UiCancel }

    public class FeedbackManager : MonoBehaviour
    {
        public static FeedbackManager Instance { get; private set; }
        [SerializeField] private AudioFeedbackConfig audioConfig;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private float hitStopSeconds = 0.04f;
        [SerializeField] private bool playProceduralBgm = true;
        private Coroutine hitStopRoutine;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.volume = 0.22f;
            }
        }

        private void Start()
        {
            PlayBgm();
        }

        public void Play(FeedbackSound sound)
        {
            AudioClip clip = GetConfiguredClip(sound);
            if (clip == null) clip = CreateTone(sound);
            if (clip != null && sfxSource != null) sfxSource.PlayOneShot(clip);
        }

        public void PlayBgm(AudioClip overrideClip = null)
        {
            if (bgmSource == null) return;
            AudioClip clip = overrideClip != null ? overrideClip : audioConfig != null ? audioConfig.bgm : null;
            if (clip == null && playProceduralBgm) clip = CreateBgmLoop();
            if (clip == null) return;
            if (bgmSource.clip == clip && bgmSource.isPlaying) return;
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        public void HitStop()
        {
            if (hitStopRoutine != null)
            {
                StopCoroutine(hitStopRoutine);
                hitStopRoutine = null;
            }

            Time.timeScale = 1f;
            hitStopRoutine = StartCoroutine(HitStopRoutine());
        }

        private AudioClip GetConfiguredClip(FeedbackSound sound)
        {
            if (audioConfig == null) return null;
            return sound switch
            {
                FeedbackSound.Jump => audioConfig.jump,
                FeedbackSound.Dash => audioConfig.dash,
                FeedbackSound.Attack => audioConfig.attack,
                FeedbackSound.DownSlash => audioConfig.downSlash,
                FeedbackSound.Hit => audioConfig.hit,
                FeedbackSound.Crit => audioConfig.crit,
                FeedbackSound.Death => audioConfig.death,
                FeedbackSound.Pickup => audioConfig.pickup,
                FeedbackSound.Buy => audioConfig.buy,
                FeedbackSound.Open => audioConfig.open,
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
                FeedbackSound.DownSlash => 520f,
                FeedbackSound.Hit => 180f,
                FeedbackSound.Crit => 960f,
                FeedbackSound.Death => 90f,
                FeedbackSound.Pickup => 680f,
                FeedbackSound.Buy => 480f,
                FeedbackSound.Open => 420f,
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

        private static AudioClip CreateBgmLoop()
        {
            int sampleRate = 22050;
            float duration = 8f;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];
            float[] notes = { 146.83f, 174.61f, 196f, 220f, 196f, 174.61f, 164.81f, 146.83f };
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                int noteIndex = Mathf.FloorToInt(t) % notes.Length;
                float note = notes[noteIndex];
                float pulse = Mathf.Sin(2f * Mathf.PI * note * t) * 0.035f;
                float harmony = Mathf.Sin(2f * Mathf.PI * note * 0.5f * t) * 0.025f;
                data[i] = pulse + harmony;
            }
            var clip = AudioClip.Create("Procedural_Test_BGM", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private IEnumerator HitStopRoutine()
        {
            float originalScale = Time.timeScale > 0f ? Time.timeScale : 1f;
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(hitStopSeconds);
            Time.timeScale = originalScale;
            hitStopRoutine = null;
        }
    }
}
