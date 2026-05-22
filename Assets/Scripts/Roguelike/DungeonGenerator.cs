using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HollowStyleMVP.Roguelike
{
    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField] private int minRooms = 6;
        [SerializeField] private int maxRooms = 12;

        public DungeonLayout Generate(int seed, int targetRoomCount)
        {
            var rng = new System.Random(seed);
            int roomCount = Mathf.Clamp(targetRoomCount, minRooms, maxRooms);
            var layout = new DungeonLayout();
            var start = layout.GetOrCreate(Vector2Int.zero, RoomType.Start);
            var frontier = new List<DungeonRoomNode> { start };

            while (layout.Rooms.Count < roomCount && frontier.Count > 0)
            {
                var from = frontier[rng.Next(frontier.Count)];
                var directions = Enum.GetValues(typeof(Direction2D)).Cast<Direction2D>().OrderBy(_ => rng.Next()).ToArray();
                bool expanded = false;

                foreach (var direction in directions)
                {
                    Vector2Int nextPosition = from.GridPosition + direction.ToGridOffset();
                    if (layout.TryGet(nextPosition, out _)) continue;

                    var next = layout.GetOrCreate(nextPosition, RoomType.Combat);
                    Link(from, next, direction);
                    frontier.Add(next);
                    expanded = true;
                    break;
                }

                if (!expanded) frontier.Remove(from);
            }

            AssignSpecialRooms(layout);
            return layout;
        }

        private static void Link(DungeonRoomNode a, DungeonRoomNode b, Direction2D directionFromA)
        {
            a.Neighbors[directionFromA] = b;
            b.Neighbors[directionFromA.Opposite()] = a;
        }

        private static void AssignSpecialRooms(DungeonLayout layout)
        {
            var roomsByDistance = layout.Rooms.Values
                .Where(room => room.Type != RoomType.Start)
                .OrderByDescending(room => Mathf.Abs(room.GridPosition.x) + Mathf.Abs(room.GridPosition.y))
                .ToList();

            if (roomsByDistance.Count == 0) return;
            layout.BossRoom = roomsByDistance[0];
            layout.BossRoom.Type = RoomType.Boss;

            var deadEnds = roomsByDistance.Where(room => room.Neighbors.Count == 1 && room.Type == RoomType.Combat).ToList();
            if (deadEnds.Count > 0) deadEnds[0].Type = RoomType.Treasure;
            if (deadEnds.Count > 1) deadEnds[1].Type = RoomType.Shop;

            if (!layout.Rooms.Values.Any(room => room.Type == RoomType.Treasure) && roomsByDistance.Count > 1)
                roomsByDistance[1].Type = RoomType.Treasure;
            if (!layout.Rooms.Values.Any(room => room.Type == RoomType.Shop) && roomsByDistance.Count > 2)
                roomsByDistance[2].Type = RoomType.Shop;
        }
    }
}
