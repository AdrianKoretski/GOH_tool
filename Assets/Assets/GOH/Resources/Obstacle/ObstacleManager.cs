using System.Collections.Generic;
using UnityEngine;

namespace GOH
{
    public class ObstacleManager
    {
        private GameObject obstacle_holder;
        private List<Node> p_nodes = new List<Node>();

        public List<Node> getObstacles()
        {
            obstacle_holder = new GameObject("Obstacles");
            GenerateObstacles();
            return p_nodes;
        }

        private void GenerateObstacles()
        {
            GameObject[] obstacle_list = GameObject.FindGameObjectsWithTag("Obstacle");
            for (int i = 0; i < obstacle_list.Length; i++)
            {
                List<Vector2> vertices = obstacle_list[i].GetComponent<Obstacle>().getVertices();
                GenerateObstacleData(vertices);
            }
        }

        public void GenerateObstacleData(List<Vector2> vertices)
        {
            List<Node> nodes = GenerateNodes(vertices);
            GenerateEdges(nodes);
        }

        private List<Node> GenerateNodes(List<Vector2> vertices)
        {
            List<Node> nodes = new List<Node>();
            for (int j = 0; j < vertices.Count; j++)
            {
                Node node = new Node(vertices[j], Node.NodeType.pinned);
                nodes.Add(node);
                p_nodes.Add(node);
            }
            return nodes;
        }

        private void GenerateEdges(List<Node> nodes)
        {
            for (int j = 0; j < nodes.Count - 1; j++)
                new Edge(nodes[j], nodes[j + 1]);
            new Edge(nodes[nodes.Count - 1], nodes[0]);
        }

    }
}