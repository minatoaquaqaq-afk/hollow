using System.Collections.Generic;
using System.Linq;
using HollowStyleMVP.Combat;
using HollowStyleMVP.Core;
using HollowStyleMVP.Player;
using HollowStyleMVP.Roguelike;
using UnityEngine;

namespace HollowStyleMVP.Inventory
{
    public enum SkillSlot { U, I, O }

    public class EquipmentSystem : MonoBehaviour
    {
        public static EquipmentSystem Instance { get; private set; }

        [SerializeField] private int charmSlots = 3;
        [SerializeField] private InventoryItem weapon;
        [SerializeField] private InventoryItem armor;
        [SerializeField] private InventoryItem accessory;
        [SerializeField] private List<InventoryItem> charms = new List<InventoryItem>();
        [SerializeField] private InventoryItem skillU;
        [SerializeField] private InventoryItem skillI;
        [SerializeField] private InventoryItem skillO;

        private const string PushSkillId = "shop_skill_push_runtime";
        private const string HealSkillId = "shop_skill_heal_runtime";
        private const string ChargedShotSkillId = "shop_skill_charged_shot_runtime";
        private const float PushRadius = 4f;
        private const float PushKnockback = 13f;
        private const float ChargedShotSeconds = 2f;
        private static string persistedSkillUId;
        private static string persistedSkillIId;
        private static string persistedSkillOId;

        private CombatStats stats;
        private PlayerStatsController playerStats;
        private Health health;
        private SkillSlot? chargingSlot;
        private float chargeStartedAt;
        private bool chargeReadyAnnounced;
        private GameObject chargeVisual;

        public IReadOnlyList<InventoryItem> Charms => charms;
        public InventoryItem Weapon => weapon;
        public InventoryItem Armor => armor;
        public InventoryItem Accessory => accessory;
        public InventoryItem SkillU => skillU;
        public InventoryItem SkillI => skillI;
        public InventoryItem SkillO => skillO;

        public event System.Action EquipmentChanged;

        private void Awake()
        {
            Instance = this;
            stats = GetComponent<CombatStats>();
            playerStats = GetComponent<PlayerStatsController>();
            health = GetComponent<Health>();
            RestorePersistedSkillSlots();
            ApplyBonuses();
        }

        private void Start()
        {
            RestorePersistedSkillSlots();
        }

        private void Update()
        {
            if (UiModalState.HasOpenModal)
            {
                if (HasVisibleBlockingUi()) return;
                UiModalState.Reset();
            }

            UpdateSkillKey(SkillSlot.U, KeyCode.U);
            UpdateSkillKey(SkillSlot.I, KeyCode.I);
            UpdateSkillKey(SkillSlot.O, KeyCode.O);
            UpdateChargeVisual();
        }

        public bool TryEquip(InventoryItem item)
        {
            if (item == null || item.type != ItemType.Equipment) return false;
            switch (item.equipmentSlot)
            {
                case EquipmentSlot.Weapon:
                    weapon = item;
                    break;
                case EquipmentSlot.Armor:
                    armor = item;
                    break;
                case EquipmentSlot.Accessory:
                    accessory = item;
                    break;
                default:
                    return false;
            }

            ApplyBonuses();
            EquipmentChanged?.Invoke();
            return true;
        }

        public void Unequip(EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    weapon = null;
                    break;
                case EquipmentSlot.Armor:
                    armor = null;
                    break;
                case EquipmentSlot.Accessory:
                    accessory = null;
                    break;
                default:
                    return;
            }

            ApplyBonuses();
            EquipmentChanged?.Invoke();
        }

        public bool TryEquipCharm(InventoryItem item)
        {
            if (item == null || item.type != ItemType.Charm || charms.Contains(item)) return false;
            int used = charms.Sum(charm => Mathf.Max(1, charm.charmCost));
            int cost = Mathf.Max(1, item.charmCost);
            if (used + cost > charmSlots) return false;
            charms.Add(item);
            ApplyBonuses();
            EquipmentChanged?.Invoke();
            return true;
        }

        public void UnequipCharm(InventoryItem item)
        {
            if (item == null || !charms.Remove(item)) return;
            ClearSkillSlotsProvidedBy(item);
            ApplyBonuses();
            EquipmentChanged?.Invoke();
        }

        public bool TryEquipSkill(InventoryItem item, SkillSlot slot)
        {
            if (!CanEquipAsSkill(item)) return false;
            switch (slot)
            {
                case SkillSlot.U:
                    skillU = item;
                    persistedSkillUId = item.id;
                    break;
                case SkillSlot.I:
                    skillI = item;
                    persistedSkillIId = item.id;
                    break;
                case SkillSlot.O:
                    skillO = item;
                    persistedSkillOId = item.id;
                    break;
                default:
                    return false;
            }

            EquipmentChanged?.Invoke();
            return true;
        }

        public bool TryEquipSkillToFirstEmpty(InventoryItem item)
        {
            if (!CanEquipAsSkill(item)) return false;
            if (skillU == null) return TryEquipSkill(item, SkillSlot.U);
            if (skillI == null) return TryEquipSkill(item, SkillSlot.I);
            if (skillO == null) return TryEquipSkill(item, SkillSlot.O);
            return TryEquipSkill(item, SkillSlot.U);
        }

        public void UnequipSkill(SkillSlot slot)
        {
            switch (slot)
            {
                case SkillSlot.U:
                    skillU = null;
                    persistedSkillUId = null;
                    break;
                case SkillSlot.I:
                    skillI = null;
                    persistedSkillIId = null;
                    break;
                case SkillSlot.O:
                    skillO = null;
                    persistedSkillOId = null;
                    break;
            }

            EquipmentChanged?.Invoke();
        }

        public InventoryItem GetSkill(SkillSlot slot)
        {
            return slot switch
            {
                SkillSlot.U => skillU,
                SkillSlot.I => skillI,
                SkillSlot.O => skillO,
                _ => null
            };
        }

        public bool TryUseSkill(SkillSlot slot)
        {
            var skill = GetSkill(slot);
            if (skill == null)
            {
                GameEvents.RaiseSceneMessage($"{slot} 技能槽为空");
                return false;
            }

            if (IsChargedShot(skill))
            {
                StartCharging(slot, skill);
                return true;
            }

            return UseInstantSkill(slot, skill);
        }

        public string BuildSummary()
        {
            string weaponName = NameOf(weapon, "无武器");
            string armorName = NameOf(armor, "无护甲");
            string accessoryName = NameOf(accessory, "无饰品");
            string charmText = charms.Count == 0 ? "无护符" : string.Join("、", charms.Select(charm => charm.displayName));
            int used = charms.Sum(charm => Mathf.Max(1, charm.charmCost));
            return $"装备：{weaponName} / {armorName} / {accessoryName}\n护符：{charmText} ({used}/{charmSlots})\n技能：U[{NameOf(skillU)}] I[{NameOf(skillI)}] O[{NameOf(skillO)}]";
        }

        private void ApplyBonuses()
        {
            if (stats == null) stats = GetComponent<CombatStats>();
            if (stats == null) return;
            var equipment = Add(Add(weapon != null ? weapon.statModifier : default, armor != null ? armor.statModifier : default), accessory != null ? accessory.statModifier : default);
            var charmBonus = default(StatModifier);
            foreach (var charm in charms)
            {
                if (charm != null) charmBonus = Add(charmBonus, charm.statModifier);
            }
            stats.SetEquipmentBonus(equipment);
            stats.SetCharmBonus(charmBonus);
        }

        private void RestorePersistedSkillSlots()
        {
            skillU ??= ResolvePersistedSkill(persistedSkillUId);
            skillI ??= ResolvePersistedSkill(persistedSkillIId);
            skillO ??= ResolvePersistedSkill(persistedSkillOId);
        }

        private static InventoryItem ResolvePersistedSkill(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId) || InventorySystem.Instance == null) return null;
            if (!InventorySystem.Instance.GetSnapshot().ContainsKey(itemId)) return null;
            if (!InventorySystem.Instance.TryGetItemDefinition(itemId, out var item)) return null;
            return CanEquipAsSkill(item) ? item : null;
        }

        private void UpdateSkillKey(SkillSlot slot, KeyCode key)
        {
            if (Input.GetKeyDown(key)) TryUseSkill(slot);
            if (Input.GetKey(key) && chargingSlot == slot) ShowChargeProgress();
            if (Input.GetKeyUp(key) && chargingSlot == slot) ReleaseChargedSkill(slot);
        }

        private bool UseInstantSkill(SkillSlot slot, InventoryItem skill)
        {
            switch (skill.id)
            {
                case PushSkillId:
                    return TryPushEnemies(skill);
                case HealSkillId:
                    return TryHeal(skill);
                default:
                    GameEvents.RaiseSceneMessage($"释放技能：{skill.displayName}");
                    return true;
            }
        }

        private bool TryPushEnemies(InventoryItem skill)
        {
            if (!TrySpendSkillEnergy(20, skill.displayName)) return false;

            int hitCount = 0;
            var targets = FindObjectsByType<Health>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var target in targets)
            {
                if (target == null || target.IsDead) continue;
                if (target.transform.root == transform.root) continue;
                if (target.CompareTag("Player")) continue;
                if (Vector2.Distance(transform.position, target.transform.position) > PushRadius) continue;

                target.Damage(1, transform.position, PushKnockback);
                PushBody(target.transform);
                hitCount++;
            }

            SkillVfx.SpawnPulse(transform.position, Color.cyan, PushRadius * 2f, 0.35f, 40);
            HitEffect.Spawn(transform.position, Color.cyan);
            FeedbackManager.Instance?.Play(FeedbackSound.Attack);
            GameEvents.RaiseSceneMessage($"{skill.displayName}：击退 {hitCount} 个敌人");
            return true;
        }

        private bool TryHeal(InventoryItem skill)
        {
            if (!TrySpendSkillEnergy(50, skill.displayName)) return false;
            if (health == null) health = GetComponent<Health>();
            health?.Heal(5);
            SkillVfx.SpawnPulse(transform.position, Color.green, 2.2f, 0.45f, 42);
            SkillVfx.SpawnBurst(transform.position, Color.green, 8, 1.7f);
            FeedbackManager.Instance?.Play(FeedbackSound.Open);
            GameEvents.RaiseSceneMessage($"{skill.displayName}：回复 5 HP");
            return true;
        }

        private void StartCharging(SkillSlot slot, InventoryItem skill)
        {
            chargingSlot = slot;
            chargeStartedAt = Time.time;
            chargeReadyAnnounced = false;
            EnsureChargeVisual();
            GameEvents.RaiseSceneMessage($"{skill.displayName}：蓄力中");
        }

        private void ShowChargeProgress()
        {
            float charged = Time.time - chargeStartedAt;
            if (charged < ChargedShotSeconds || chargeReadyAnnounced) return;
            chargeReadyAnnounced = true;
            GameEvents.RaiseSceneMessage("蓄力完成，松开释放");
        }

        private void ReleaseChargedSkill(SkillSlot slot)
        {
            var skill = GetSkill(slot);
            chargingSlot = null;
            chargeReadyAnnounced = false;
            DestroyChargeVisual();
            if (!IsChargedShot(skill)) return;

            float charged = Time.time - chargeStartedAt;
            if (charged < ChargedShotSeconds)
            {
                GameEvents.RaiseSceneMessage($"{skill.displayName}：蓄力不足");
                return;
            }

            if (!TrySpendSkillEnergy(30, skill.displayName)) return;
            FireChargedShot(skill);
        }

        private void FireChargedShot(InventoryItem skill)
        {
            var target = FindNearestTarget(12f);
            Vector2 direction = target != null
                ? ((Vector2)target.position - (Vector2)transform.position).normalized
                : (transform.localScale.x >= 0f ? Vector2.right : Vector2.left);

            var obj = new GameObject("Charged Skill Projectile");
            obj.transform.position = transform.position + (Vector3)(direction * 0.85f);
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = SkillVfx.CircleSprite;
            renderer.color = Color.yellow;
            renderer.sortingOrder = 35;
            var body = obj.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            var collider = obj.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;

            var projectile = obj.AddComponent<Projectile>();
            projectile.Configure(direction, new ProjectileStats
            {
                damage = 10,
                speed = 12f,
                range = 10f,
                fireDelay = 0f,
                size = 0.55f
            }, stats, "", false);
            projectile.ConfigureHoming(target, 10f);

            FeedbackManager.Instance?.Play(FeedbackSound.Attack);
            SkillVfx.SpawnPulse(transform.position, Color.yellow, 2.5f, 0.25f, 35);
            GameEvents.RaiseSceneMessage($"{skill.displayName}：发射 10 伤害子弹");
        }

        private Transform FindNearestTarget(float range)
        {
            Transform best = null;
            float bestSqrDistance = range * range;
            var candidates = FindObjectsByType<Health>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var candidate in candidates)
            {
                if (candidate == null || candidate.IsDead) continue;
                if (candidate.transform.root == transform.root) continue;
                if (candidate.CompareTag("Player")) continue;

                float sqrDistance = ((Vector2)candidate.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (sqrDistance > bestSqrDistance) continue;
                bestSqrDistance = sqrDistance;
                best = candidate.transform;
            }

            return best;
        }

        private bool TrySpendSkillEnergy(int cost, string skillName)
        {
            if (playerStats == null) playerStats = GetComponent<PlayerStatsController>();
            if (playerStats == null || playerStats.TrySpendEnergy(cost)) return true;

            GameEvents.RaiseSceneMessage($"{skillName}：能量不足 {playerStats.CurrentEnergy}/{cost}");
            return false;
        }

        private static bool HasVisibleBlockingUi()
        {
            var canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var canvas in canvases)
            {
                if (canvas == null || !canvas.isActiveAndEnabled) continue;
                if (!HasVisibleNamedPanel(canvas.transform)) continue;
                return true;
            }

            return false;
        }

        private static bool HasVisibleNamedPanel(Transform root)
        {
            foreach (Transform child in root)
            {
                if (!child.gameObject.activeInHierarchy) continue;
                string name = child.name;
                if (name.Contains("Inventory Panel") || name.Contains("Shop Panel") || name.Contains("Dialogue Panel") || name.Contains("Death Panel"))
                    return true;
                if (HasVisibleNamedPanel(child)) return true;
            }

            return false;
        }

        private void PushBody(Transform target)
        {
            if (target == null || !target.TryGetComponent<Rigidbody2D>(out var body)) return;
            Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
            body.AddForce(new Vector2(direction.x, 0.35f).normalized * PushKnockback, ForceMode2D.Impulse);
        }

        private void EnsureChargeVisual()
        {
            DestroyChargeVisual();
            chargeVisual = new GameObject("Charged Skill Aura");
            chargeVisual.transform.SetParent(transform, false);
            chargeVisual.transform.localPosition = Vector3.zero;
            var renderer = chargeVisual.AddComponent<SpriteRenderer>();
            renderer.sprite = SkillVfx.RingSprite;
            renderer.color = new Color(1f, 0.85f, 0.1f, 0.75f);
            renderer.sortingOrder = 34;
            chargeVisual.transform.localScale = Vector3.one * 0.5f;
        }

        private void UpdateChargeVisual()
        {
            if (chargeVisual == null || chargingSlot == null) return;
            float t = Mathf.Clamp01((Time.time - chargeStartedAt) / ChargedShotSeconds);
            chargeVisual.transform.localScale = Vector3.one * Mathf.Lerp(0.5f, 2.2f, t);
            var renderer = chargeVisual.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                float alpha = chargeReadyAnnounced ? 1f : Mathf.Lerp(0.35f, 0.85f, t);
                renderer.color = new Color(1f, Mathf.Lerp(0.45f, 0.9f, t), 0.05f, alpha);
            }
        }

        private void DestroyChargeVisual()
        {
            if (chargeVisual == null) return;
            Destroy(chargeVisual);
            chargeVisual = null;
        }

        private static bool IsChargedShot(InventoryItem skill)
        {
            return skill != null && skill.id == ChargedShotSkillId;
        }

        private static bool CanEquipAsSkill(InventoryItem item)
        {
            return item != null && (item.type == ItemType.Ability || item.type == ItemType.Charm);
        }

        private void ClearSkillSlotsProvidedBy(InventoryItem item)
        {
            if (skillU == item)
            {
                skillU = null;
                persistedSkillUId = null;
            }
            if (skillI == item)
            {
                skillI = null;
                persistedSkillIId = null;
            }
            if (skillO == item)
            {
                skillO = null;
                persistedSkillOId = null;
            }
        }

        private static string NameOf(InventoryItem item, string fallback = "空")
        {
            return item != null ? item.displayName : fallback;
        }

        private static StatModifier Add(StatModifier a, StatModifier b)
        {
            return new StatModifier
            {
                maxHealth = a.maxHealth + b.maxHealth,
                maxEnergy = a.maxEnergy + b.maxEnergy,
                attackPower = a.attackPower + b.attackPower,
                defense = a.defense + b.defense,
                critChance = a.critChance + b.critChance,
                critResistance = a.critResistance + b.critResistance,
                critDamageBonus = a.critDamageBonus + b.critDamageBonus,
                moveSpeedBonus = a.moveSpeedBonus + b.moveSpeedBonus,
                jumpForceBonus = a.jumpForceBonus + b.jumpForceBonus
            };
        }
    }

    internal class SkillVfx : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private float elapsed;
        private float duration = 0.35f;
        private Vector3 startScale = Vector3.one;
        private Vector3 endScale = Vector3.one * 2f;
        private Color color = Color.white;
        private Vector2 velocity;

        private static Sprite ringSprite;
        private static Sprite circleSprite;

        public static Sprite RingSprite => ringSprite != null ? ringSprite : ringSprite = CreateDiscSprite(true);
        public static Sprite CircleSprite => circleSprite != null ? circleSprite : circleSprite = CreateDiscSprite(false);

        public static void SpawnPulse(Vector3 position, Color color, float diameter, float duration, int sortingOrder)
        {
            var obj = new GameObject("Skill Pulse VFX");
            obj.transform.position = position;
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = RingSprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;

            var effect = obj.AddComponent<SkillVfx>();
            effect.spriteRenderer = renderer;
            effect.color = color;
            effect.duration = Mathf.Max(0.05f, duration);
            effect.startScale = Vector3.one * 0.2f;
            effect.endScale = Vector3.one * Mathf.Max(0.2f, diameter);
        }

        public static void SpawnBurst(Vector3 position, Color color, int count, float radius)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = (Mathf.PI * 2f / Mathf.Max(1, count)) * i;
                var obj = new GameObject("Skill Spark VFX");
                obj.transform.position = position;
                var renderer = obj.AddComponent<SpriteRenderer>();
                renderer.sprite = CircleSprite;
                renderer.color = color;
                renderer.sortingOrder = 41;

                var effect = obj.AddComponent<SkillVfx>();
                effect.spriteRenderer = renderer;
                effect.color = color;
                effect.duration = 0.45f;
                effect.startScale = Vector3.one * 0.18f;
                effect.endScale = Vector3.one * 0.05f;
                effect.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            }
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localScale = Vector3.Lerp(startScale, endScale, EaseOut(t));
            transform.position += (Vector3)(velocity * Time.deltaTime);
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(color.r, color.g, color.b, color.a * (1f - t));
            if (elapsed >= duration) Destroy(gameObject);
        }

        private static float EaseOut(float t)
        {
            return 1f - (1f - t) * (1f - t);
        }

        private static Sprite CreateDiscSprite(bool ring)
        {
            const int size = 128;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float outer = size * 0.46f;
            float inner = ring ? size * 0.36f : 0f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float outerAlpha = Mathf.Clamp01(outer - distance);
                    float innerAlpha = ring ? Mathf.Clamp01(distance - inner) : 1f;
                    float alpha = Mathf.Clamp01(outerAlpha * innerAlpha);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }
    }
}
