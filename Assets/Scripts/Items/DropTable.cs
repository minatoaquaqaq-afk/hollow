using UnityEngine;

namespace HollowStyleMVP.Items
{
    [System.Serializable]
    public class DropEntry
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float chance = 1f;
        public int minAmount = 1;
        public int maxAmount = 1;
    }

    [CreateAssetMenu(menuName = "Hollow Style MVP/Config/Drop Table")]
    public class DropTable : ScriptableObject
    {
        public DropEntry[] drops;
    }
}
