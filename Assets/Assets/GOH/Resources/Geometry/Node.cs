using System.Collections.Generic;
using UnityEngine;

namespace GOH
{
    public class Node
    {
        public enum NodeType
        {
            obstacle,
            pinned,
            shadow,
            intercept,
            guard,
            leg
        };
        private static int m_pinned_id_count = 0;

        public readonly NodeType type;
        public readonly int ID_0;
        public readonly int ID_1;
        public readonly Vector2 position;
        public readonly float timestamp;

        public List<Node> neighbours = new List<Node>();

        public Node(Vector2 position, NodeType type, float timestamp = -1, int ID_0 = -1, int ID_1 = -1)
        {
            if (ID_0 < ID_1)
            {
                int t = ID_0;
                ID_0 = ID_1;
                ID_1 = t;
            }
            if (type == NodeType.pinned)
                ID_0 = m_pinned_id_count++;
            this.type = type;
            this.ID_0 = ID_0;
            this.ID_1 = ID_1;
            this.position = position;
            this.timestamp = timestamp;
        }

        public static bool Compare(Node node_0, Node node_1)
        {
            return HaveSameIdentifiers(node_0, node_1) && HaveSameType(node_0, node_1);
        }

        private static bool HaveSameIdentifiers(Node node_0, Node node_1)
        {
            return node_0.ID_0 == node_1.ID_0 && node_0.ID_1 == node_1.ID_1;
        }

        private static bool HaveSameType(Node node_0, Node node_1)
        {
            return
                   node_0.type == node_1.type
                || node_0.type == NodeType.obstacle && node_1.type == NodeType.pinned
                || node_0.type == NodeType.pinned && node_1.type == NodeType.obstacle;
        }

        public void RemoveConnection(Node node)
        {
            neighbours.Remove(node);
        }

        public void AddConnection(Node node)
        {
            neighbours.Add(node);
        }

        public bool IsNeighbor(Node node)
        {
            return neighbours.Contains(node);
        }

        public int GetNeighborCount()
        {
            return neighbours.Count;
        }

        public Node GetNeighborNode(int index)
        {
            return neighbours[index];
        }

        public Node CopyToPinned(float timestamp)
        {
            if (type != NodeType.pinned)
            {
                UnityEngine.Debug.LogError("[ TODO: ERROR MESSAGE ]");  // TODO: Write a proper error message. 
                return null;
            }
            return new Node(position, NodeType.obstacle, timestamp, ID_0);
        }

        public bool HasNeighborFromTime(float time)
        {
            for (int i = 0; i < GetNeighborCount(); i++)
                if (GetNeighborNode(i).timestamp == time)
                    return true;
            return false;
        }
    }
}