using System.Collections.Generic;
using UnityEngine;

namespace HollowStyleMVP.Roguelike
{
    public enum Direction2D { North, East, South, West }
    public enum RoomType { Start, Combat, Treasure, Shop, Boss }

    public class DungeonRoomNode
    {
        public Vector2Int GridPosition { get; }
        public RoomType Type { get; set; }
        public bool Visited { get; set; }
        public bool Cleared { get; set; }
        public Dictionary<Direction2D, DungeonRoomNode> Neighbors { get; } = new Dictionary<Direction2D, DungeonRoomNode>();

        public DungeonRoomNode(Vector2Int gridPosition, RoomType type)
        {
            GridPosition = gridPosition;
            Type = type;
        }
    }

    public class DungeonLayout
    {
        private readonly Dictionary<Vector2Int, DungeonRoomNode> rooms = new Dictionary<Vector2Int, DungeonRoomNode>();

        public IReadOnlyDictionary<Vector2Int, DungeonRoomNode> Rooms => rooms;
        public DungeonRoomNode StartRoom { get; private set; }
        public DungeonRoomNode BossRoom { get; set; }

        public DungeonRoomNode GetOrCreate(Vector2Int position, RoomType type)
        {
            if (rooms.TryGetValue(position, out var existing)) return existing;
            var node = new DungeonRoomNode(position, type);
            rooms[position] = node;
            if (type == RoomType.Start) StartRoom = node;
            return node;
        }

        public bool TryGet(Vector2Int position, out DungeonRoomNode node) => rooms.TryGetValue(position, out node);
    }

    public static class Direction2DExtensions
    {
        public static Vector2Int ToGridOffset(this Direction2D direction)
        {
            return direction switch
            {
                Direction2D.North => Vector2Int.up,
                Direction2D.East => Vector2Int.right,
                Direction2D.South => Vector2Int.down,
                Direction2D.West => Vector2Int.left,
                _ => Vector2Int.zero
            };
        }

        public static Direction2D Opposite(this Direction2D direction)
        {
            return direction switch
            {
                Direction2D.North => Direction2D.South,
                Direction2D.East => Direction2D.West,
                Direction2D.South => Direction2D.North,
                Direction2D.West => Direction2D.East,
                _ => Direction2D.South
            };
        }
    }
}
