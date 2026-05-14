using UnityEngine;

namespace HollowStyleMVP.Progression
{
    public enum FeatureStatus { Prototype, Partial, Planned }

    [System.Serializable]
    public class FeatureEntry
    {
        public string module;
        public string feature;
        public FeatureStatus status;
        [TextArea] public string note;
    }

    [CreateAssetMenu(menuName = "Hollow Style MVP/Progression/Feature Roadmap")]
    public class FeatureRoadmap : ScriptableObject
    {
        public FeatureEntry[] entries;
    }
}
