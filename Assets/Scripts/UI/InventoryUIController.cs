using System;
using System.Collections.Generic;
using HollowStyleMVP.Core;
using HollowStyleMVP.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace HollowStyleMVP.UI
{
    public class InventoryUIController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform contentRoot;
        [SerializeField] private Text rowPrefab;
        [SerializeField] private Text coinsText;
        private bool modalRegistered;
        private Transform equipmentRoot;
        private bool layoutPrepared;

        private void Awake()
        {
            if (panel != null) panel.SetActive(false);
        }

        private void OnEnable()
        {
            if (InventorySystem.Instance != null) InventorySystem.Instance.InventoryChanged += Rebuild;
            if (EquipmentSystem.Instance != null) EquipmentSystem.Instance.EquipmentChanged += Rebuild;
        }

        private void OnDisable()
        {
            if (InventorySystem.Instance != null) InventorySystem.Instance.InventoryChanged -= Rebuild;
            if (EquipmentSystem.Instance != null) EquipmentSystem.Instance.EquipmentChanged -= Rebuild;
            UnregisterModal();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Tab)) Toggle();
            if (panel != null && panel.activeSelf && Input.GetKeyDown(KeyCode.Escape)) Close();
        }

        public void Toggle()
        {
            if (panel == null) return;
            if (panel.activeSelf) Close();
            else Open();
        }

        private void Open()
        {
            RegisterModal();
            PrepareLayout();
            panel.SetActive(true);
            Rebuild();
            Time.timeScale = 1f;
            FeedbackManager.Instance?.Play(FeedbackSound.Open);
        }

        public void Close()
        {
            if (panel != null) panel.SetActive(false);
            UnregisterModal();
            Time.timeScale = 1f;
            FeedbackManager.Instance?.Play(FeedbackSound.UiCancel);
        }

        private void Rebuild()
        {
            if (InventorySystem.Instance == null) return;
            if (coinsText != null) coinsText.text = $"金币 {InventorySystem.Instance.Coins}";
            if (contentRoot == null || rowPrefab == null) return;
            PrepareLayout();

            foreach (Transform child in contentRoot)
            {
                if (child == rowPrefab.transform) continue;
                Destroy(child.gameObject);
            }
            if (equipmentRoot != null)
            {
                foreach (Transform child in equipmentRoot)
                {
                    Destroy(child.gameObject);
                }
            }

            var equipment = EquipmentSystem.Instance;
            if (equipment != null)
            {
                AddEquipmentRow("装备槽（左键卸下）", true);
                AddEquipmentRow("武器  " + NameOf(equipment.Weapon, "空"), false, equipment.Weapon != null ? () => equipment.Unequip(EquipmentSlot.Weapon) : null);
                AddEquipmentRow("护甲  " + NameOf(equipment.Armor, "空"), false, equipment.Armor != null ? () => equipment.Unequip(EquipmentSlot.Armor) : null);
                AddEquipmentRow("饰品  " + NameOf(equipment.Accessory, "空"), false, equipment.Accessory != null ? () => equipment.Unequip(EquipmentSlot.Accessory) : null);

                AddEquipmentRow("技能槽（U / I / O）", true);
                AddEquipmentRow("U  " + NameOf(equipment.SkillU, "空"), false, equipment.SkillU != null ? () => equipment.UnequipSkill(SkillSlot.U) : null);
                AddEquipmentRow("I  " + NameOf(equipment.SkillI, "空"), false, equipment.SkillI != null ? () => equipment.UnequipSkill(SkillSlot.I) : null);
                AddEquipmentRow("O  " + NameOf(equipment.SkillO, "空"), false, equipment.SkillO != null ? () => equipment.UnequipSkill(SkillSlot.O) : null);

                if (equipment.Charms.Count > 0)
                {
                    AddEquipmentRow("已装备护符", true);
                    foreach (var charm in equipment.Charms)
                    {
                        var captured = charm;
                        AddEquipmentRow(NameOf(captured) + $"  槽{Mathf.Max(1, captured.charmCost)}", false, () => equipment.UnequipCharm(captured));
                    }
                }
            }
            else
            {
                AddEquipmentRow("装备：未找到玩家装备系统", true);
            }

            AddInventorySections();
        }

        private void AddInventorySections()
        {
            var sections = new Dictionary<ItemType, string>
            {
                { ItemType.Consumable, "消耗品" },
                { ItemType.Material, "材料" },
                { ItemType.Quest, "任务物品" },
                { ItemType.Equipment, "装备：点击装备" },
                { ItemType.Charm, "护符：点击装备/加入技能槽" },
                { ItemType.Ability, "技能：点击加入技能槽" }
            };

            bool hasItem = false;
            foreach (var section in sections)
            {
                AddRow("【" + section.Value + "】", true);
                bool sectionHasItem = false;
                foreach (var entry in InventorySystem.Instance.GetItemsByType(section.Key))
                {
                    sectionHasItem = true;
                    hasItem = true;
                    var captured = entry.item;
                    string bonus = BuildBonusText(captured);
                    Action click = BuildInventoryClick(captured);
                    AddRow($"{captured.displayName} x{entry.amount}{bonus}", false, click);
                }
                if (!sectionHasItem) AddRow("  空", true);
            }

            if (!hasItem) AddRow("提示：去商店买装备/护符，或打开 TestRoom2 宝箱。", true);
        }

        private Action BuildInventoryClick(InventoryItem item)
        {
            if (item == null || EquipmentSystem.Instance == null) return null;
            return item.type switch
            {
                ItemType.Equipment => () => EquipmentSystem.Instance.TryEquip(item),
                ItemType.Charm => () =>
                {
                    EquipmentSystem.Instance.TryEquipCharm(item);
                    EquipmentSystem.Instance.TryEquipSkillToFirstEmpty(item);
                },
                ItemType.Ability => () => EquipmentSystem.Instance.TryEquipSkillToFirstEmpty(item),
                _ => null
            };
        }

        private string BuildBonusText(InventoryItem item)
        {
            if (item == null) return "";
            var stat = item.statModifier;
            List<string> parts = new List<string>();
            if (stat.maxHealth != 0) parts.Add("血+" + stat.maxHealth);
            if (stat.attackPower != 0) parts.Add("攻+" + stat.attackPower);
            if (stat.defense != 0) parts.Add("防+" + stat.defense);
            if (stat.critChance > 0f) parts.Add("暴+" + Mathf.RoundToInt(stat.critChance * 100f) + "%");
            if (stat.critResistance > 0f) parts.Add("抗暴+" + Mathf.RoundToInt(stat.critResistance * 100f) + "%");
            if (item.type == ItemType.Charm) parts.Add("槽" + item.charmCost);
            return parts.Count == 0 ? "" : "  (" + string.Join(" ", parts) + ")";
        }

        private void AddEquipmentRow(string text, bool muted = false, Action onClick = null)
        {
            AddRowTo(equipmentRoot != null ? equipmentRoot : contentRoot, text, muted, onClick);
        }

        private void AddRow(string text, bool muted = false, Action onClick = null)
        {
            AddRowTo(contentRoot, text, muted, onClick);
        }

        private void AddRowTo(Transform parent, string text, bool muted = false, Action onClick = null)
        {
            var row = Instantiate(rowPrefab, parent);
            row.gameObject.SetActive(true);
            row.text = text;
            row.color = muted ? new Color(0.75f, 0.75f, 0.75f) : Color.white;
            row.alignment = TextAnchor.MiddleLeft;
            row.fontSize = muted ? 18 : 17;
            row.horizontalOverflow = HorizontalWrapMode.Wrap;
            row.verticalOverflow = VerticalWrapMode.Truncate;

            var rect = row.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, muted ? 30f : 34f);

            var layout = row.GetComponent<LayoutElement>();
            if (layout == null) layout = row.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = muted ? 30f : 34f;
            layout.preferredHeight = muted ? 30f : 34f;
            layout.flexibleWidth = 1f;

            var button = row.GetComponent<Button>();
            if (onClick != null)
            {
                if (button == null) button = row.gameObject.AddComponent<Button>();
                button.transition = Selectable.Transition.ColorTint;
                button.targetGraphic = row;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    onClick();
                    Rebuild();
                });
            }
            else if (button != null)
            {
                Destroy(button);
            }
        }

        private void PrepareLayout()
        {
            if (layoutPrepared || panel == null || contentRoot == null || rowPrefab == null) return;
            layoutPrepared = true;

            if (panel.TryGetComponent<RectTransform>(out var panelRect))
            {
                panelRect.anchorMin = new Vector2(0.5f, 0.5f);
                panelRect.anchorMax = new Vector2(0.5f, 0.5f);
                panelRect.pivot = new Vector2(0.5f, 0.5f);
                panelRect.anchoredPosition = Vector2.zero;
                panelRect.sizeDelta = new Vector2(900f, 560f);
            }

            if (coinsText != null)
            {
                coinsText.alignment = TextAnchor.MiddleLeft;
                coinsText.fontSize = 20;
                var coinsRect = coinsText.rectTransform;
                coinsRect.anchorMin = new Vector2(0f, 1f);
                coinsRect.anchorMax = new Vector2(0f, 1f);
                coinsRect.pivot = new Vector2(0f, 1f);
                coinsRect.anchoredPosition = new Vector2(28f, -78f);
                coinsRect.sizeDelta = new Vector2(240f, 32f);
            }

            var equipmentPanel = CreatePanel("Equipment Column", panel.transform, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(28f, 28f), new Vector2(360f, -120f));
            equipmentRoot = equipmentPanel.transform;
            ConfigureVerticalList(equipmentRoot, 12, 12, 8);

            var itemsPanel = CreatePanel("Inventory Column", panel.transform, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(-28f, 28f), new Vector2(460f, -120f));
            contentRoot.SetParent(itemsPanel.transform, false);
            ConfigureScrollContent(contentRoot as RectTransform);
            ConfigureVerticalList(contentRoot, 12, 12, 6);
            ConfigureScrollView(itemsPanel, contentRoot as RectTransform);

            rowPrefab.gameObject.SetActive(false);
            rowPrefab.alignment = TextAnchor.MiddleLeft;
            rowPrefab.fontSize = 17;
            rowPrefab.horizontalOverflow = HorizontalWrapMode.Wrap;
        }

        private static RectTransform CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(parent, false);
            var rect = obj.GetComponent<RectTransform>();
            ConfigureRect(rect, anchorMin, anchorMax, anchoredPosition, sizeDelta);
            var image = obj.GetComponent<Image>();
            image.color = new Color(0.04f, 0.05f, 0.06f, 0.82f);
            return rect;
        }

        private static void ConfigureRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            if (rect == null) return;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(anchorMin.x <= 0.5f ? 0f : 1f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
        }

        private static void ConfigureVerticalList(Transform root, int horizontalPadding, int verticalPadding, int spacing)
        {
            if (root == null) return;
            var layout = root.GetComponent<VerticalLayoutGroup>();
            if (layout == null) layout = root.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(horizontalPadding, horizontalPadding, verticalPadding, verticalPadding);
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        private static void ConfigureScrollContent(RectTransform rect)
        {
            if (rect == null) return;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            var fitter = rect.GetComponent<ContentSizeFitter>();
            if (fitter == null) fitter = rect.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        private static void ConfigureScrollView(RectTransform viewport, RectTransform content)
        {
            if (viewport == null || content == null) return;
            var mask = viewport.GetComponent<Mask>();
            if (mask == null) mask = viewport.gameObject.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            var scroll = viewport.GetComponent<ScrollRect>();
            if (scroll == null) scroll = viewport.gameObject.AddComponent<ScrollRect>();
            scroll.content = content;
            scroll.viewport = viewport;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 24f;
        }

        private static string NameOf(InventoryItem item, string fallback = "空")
        {
            return item != null ? item.displayName : fallback;
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
