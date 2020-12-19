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

            Node.Connect(node_0, node_1);
        }

        public Edge[] Split(Node node)
        {
            Edge[] edges = new Edge[2];
            edges[0] = new Edge(node_0, node);
            edges[1] = new Edge(node, node_1);

            Node.Disconnect(node_0, node_1);

            return edges;
        }

        public bool Connects(Node v)
        {
            return v == node_0 || v == node_1;
        }
    }
}