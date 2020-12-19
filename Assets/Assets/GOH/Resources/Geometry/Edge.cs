using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOH
{
    public class Edge
    {
        public Node node_0;
        public Node node_1;

        public Edge(Node node_0, Node node_1)
        {
            this.node_0 = node_0;
            this.node_1 = node_1;

            this.node_0.neighbours.Add(this.node_1);
            this.node_1.neighbours.Add(this.node_0);
        }

        public Edge[] Split(Node node)
        {
            Edge[] edges = new Edge[2];
            edges[0] = new Edge(node_0, node);
            edges[1] = new Edge(node, node_1);

            node_0.neighbours.Remove(node_1);
            node_1.neighbours.Remove(node_0);

            return edges;
        }

        public void Destroy()
        {
            node_0.neighbours.Remove(node_1);
            node_1.neighbours.Remove(node_0);
        }

        public int[] GetNodeIDs()
        {
            return new int[] { node_0.ID_0, node_1.ID_0 };
        }

        public bool Connects(Node v)
        {
            return v == node_0 || v == node_1;
        }
    }
}