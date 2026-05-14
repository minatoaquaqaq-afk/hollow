using UnityEngine;

namespace HollowStyleMVP.Level
{
    public class LdtkEntityMarker : MonoBehaviour
    {
        [SerializeField] private string entityIdentifier;
        [SerializeField] private string iid;

        public string EntityIdentifier => entityIdentifier;
        public string Iid => iid;

        public void Configure(string identifier, string entityIid)
        {
            entityIdentifier = identifier;
            iid = entityIid;
        }
    }
}
