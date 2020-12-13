using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOH
{
    public class Edge
    {
        private Node m_node_0;
        private Node m_node_1;

        public Edge(Node node_0, Node node_1)
        {
            m_node_0 = node_0;
            m_node_1 = node_1;
            m_node_0.AddConnection(this);
            m_node_1.AddConnection(this);
        }

        public Edge[] Split(Node node)
        {
            Edge[] edges = new Edge[2];
            edges[0] = new Edge(m_node_0, node);
            edges[1] = new Edge(node, m_node_1);

            m_node_0.RemoveConnection(this);
            m_node_1.RemoveConnection(this);

            return edges;
        }

        public void Destroy()
        {
            m_node_0.RemoveConnection(this);
            m_node_1.RemoveConnection(this);
        }

        public Node GetOtherNode(Node node)
        {
            if (node == m_node_0)
                return m_node_1;
            if (node == m_node_1)
                return m_node_0;
            return null;
        }

        public Node GetNode(int index)
        {
            if (index == 0)
                return m_node_0;
            if (index == 1)
                return m_node_1;
            return null;
        }

        public int[] GetNodeIDs()
        {
            return new int[] { m_node_0.ID_0, m_node_1.ID_0 };
        }

        public bool Connects(Node v)
        {
            return v == m_node_0 || v == m_node_1;
        }
    }
}