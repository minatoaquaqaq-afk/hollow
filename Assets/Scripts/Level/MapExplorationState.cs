using System.Collections.Generic;
using UnityEngine;

namespace HollowStyleMVP.Level
{
    public class MapExplorationState : MonoBehaviour
    {
        private readonly HashSet<string> visitedRooms = new HashSet<string>();
        public void MarkVisited(string roomId)
        {
            if (!string.IsNullOrWhiteSpace(roomId)) visitedRooms.Add(roomId);
        }
        public bool IsVisited(string roomId) => visitedRooms.Contains(roomId);
        public List<string> ToSaveList() => new List<string>(visitedRooms);
    }
}
