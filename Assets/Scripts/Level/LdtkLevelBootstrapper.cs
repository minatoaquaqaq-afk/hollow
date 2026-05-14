using System.Collections.Generic;
using UnityEngine;

namespace HollowStyleMVP.Level
{
    public class LdtkLevelBootstrapper : MonoBehaviour
    {
        [Header("Entity Replacement Prefabs")]
        [SerializeField] private GameObject playerSpawnPrefab;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject bossPrefab;
        [SerializeField] private GameObject npcPrefab;
        [SerializeField] private GameObject shopPrefab;
        [SerializeField] private GameObject savePointPrefab;

        [Header("Import Behavior")]
        [SerializeField] private bool replaceImportedMarkers = true;
        [SerializeField] private bool runOnStart;

        private static readonly Dictionary<string, string> Aliases = new Dictionary<string, string>
        {
            { "playerspawn", "player" },
            { "player", "player" },
            { "enemyspawn", "enemy" },
            { "enemy", "enemy" },
            { "bossspawn", "boss" },
            { "boss", "boss" },
            { "npc", "npc" },
            { "dialoguenpc", "npc" },
            { "shop", "shop" },
            { "shopnpc", "shop" },
            { "savepoint", "save" },
            { "save", "save" }
        };

        private void Start()
        {
            if (runOnStart) BuildGameplayObjects();
        }

        [ContextMenu("Build Gameplay Objects From LDtk Root")]
        public void BuildGameplayObjects()
        {
            var transforms = GetComponentsInChildren<Transform>(true);
            foreach (var child in transforms)
            {
                if (child == transform) continue;
                string identifier = GetIdentifier(child.gameObject);
                if (string.IsNullOrWhiteSpace(identifier)) continue;
                if (!Aliases.TryGetValue(Normalize(identifier), out string role)) continue;

                GameObject prefab = GetPrefab(role);
                if (prefab == null) continue;

                var instance = Instantiate(prefab, child.position, child.rotation, transform);
                instance.name = $"{prefab.name}_{identifier}";
                var marker = instance.GetComponent<LdtkEntityMarker>();
                if (marker == null) marker = instance.AddComponent<LdtkEntityMarker>();
                marker.Configure(identifier, child.gameObject.name);

                if (replaceImportedMarkers) child.gameObject.SetActive(false);
            }
        }

        private GameObject GetPrefab(string role)
        {
            return role switch
            {
                "player" => playerSpawnPrefab,
                "enemy" => enemyPrefab,
                "boss" => bossPrefab,
                "npc" => npcPrefab,
                "shop" => shopPrefab,
                "save" => savePointPrefab,
                _ => null
            };
        }

        private static string GetIdentifier(GameObject obj)
        {
            var marker = obj.GetComponent<LdtkEntityMarker>();
            if (marker != null && !string.IsNullOrWhiteSpace(marker.EntityIdentifier)) return marker.EntityIdentifier;

            // LDtkToUnity entity GameObjects usually preserve useful names; this fallback keeps the bridge package-agnostic.
            string name = obj.name;
            int cloneIndex = name.IndexOf("(", System.StringComparison.Ordinal);
            return cloneIndex > 0 ? name.Substring(0, cloneIndex).Trim() : name.Trim();
        }

        private static string Normalize(string value)
        {
            return value.Replace("_", string.Empty).Replace("-", string.Empty).Replace(" ", string.Empty).ToLowerInvariant();
        }
    }
}
