using System.Collections;
using HollowStyleMVP.Core;
using UnityEngine;
using UnityEngine.UI;

namespace HollowStyleMVP.Dialogue
{
    public class DialogueController : MonoBehaviour
    {
        public static DialogueController Instance { get; private set; }
        [SerializeField] private GameObject panel;
        [SerializeField] private Text speakerText;
        [SerializeField] private Text bodyText;
        [SerializeField] private float typeSpeed = 0.025f;
        private string speaker;
        private string[] activeLines;
        private int index;
        private Coroutine typing;
        private bool modalRegistered;

        private void Awake()
        {
            Instance = this;
            if (panel != null) panel.SetActive(false);
        }

        private void Update()
        {
            if (panel == null || !panel.activeSelf) return;
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)) Next();
            if (Input.GetKeyDown(KeyCode.Escape)) Close();
        }

        public void Open(DialogueAsset asset)
        {
            if (asset == null) return;
            OpenLines(asset.speakerName, asset.lines);
        }

        public void OpenLines(string speakerName, string[] lines)
        {
            if (lines == null || lines.Length == 0 || panel == null) return;
            speaker = speakerName;
            activeLines = lines;
            index = 0;
            RegisterModal();
            panel.SetActive(true);
            Time.timeScale = 1f;
            if (speakerText != null) speakerText.text = speaker;
            ShowLine();
        }

        private void Next()
        {
            if (activeLines == null) return;
            if (typing != null)
            {
                StopCoroutine(typing);
                typing = null;
                if (bodyText != null) bodyText.text = activeLines[index];
                return;
            }
            index++;
            if (index >= activeLines.Length) Close(); else ShowLine();
        }

        private void ShowLine()
        {
            if (typing != null) StopCoroutine(typing);
            typing = StartCoroutine(TypeLine(activeLines[index]));
        }

        private IEnumerator TypeLine(string line)
        {
            if (bodyText == null) yield break;
            bodyText.text = string.Empty;
            foreach (char c in line)
            {
                bodyText.text += c;
                yield return new WaitForSecondsRealtime(typeSpeed);
            }
            typing = null;
        }

        private void Close()
        {
            if (typing != null) StopCoroutine(typing);
            typing = null;
            activeLines = null;
            if (panel != null) panel.SetActive(false);
            UnregisterModal();
            Time.timeScale = 1f;
        }

        private void RegisterModal()
        {
            if (modalRegistered) return;
            modalRegistered = true;
            UiModalState.Open();
        }

        private void UnregisterModal()
        {
            if (!modalRegistered) return;
            modalRegistered = false;
            UiModalState.Close();
        }
    }
}
