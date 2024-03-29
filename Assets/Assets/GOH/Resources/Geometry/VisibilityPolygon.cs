﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOH
{
    public class VisibilityPolygon : List<Node>
    {
        public readonly Pip pip;

        public float timestamp { get { return pip.timestamp; } private set { } }
        public Node first { get { return this[0]; } private set { } }
        public Node last { get { return this[Count - 1]; } private set { } }

        private Node[] m_triangle = new Node[3];
        private List<Node> m_pinned_nodes = new List<Node>();
        private List<Edge> m_wall_edges = new List<Edge>();

        //------------------------------Setup start

        public VisibilityPolygon(Vector2[] triangle_corners, Pip pip, List<Node> nodes, List<Edge> edges)
        {
            for (int i = 0; i < 3; i++)
                m_triangle[i] = new Node(triangle_corners[i], i == 0 ? Node.NodeType.guard : Node.NodeType.leg, pip.timestamp, i);
            this.pip = pip;
            m_wall_edges = edges;
            m_pinned_nodes = nodes;

            GeneratePolygon();
        }

        //------------------------------Setup end

        private void GeneratePolygon()
        {
            List<Node> i_nodes = GenerateInterceptNodes();                      // 3.2.3
            SplitEdges(out List<Node> left_nodes, out List<Node> right_nodes);  // 3.2.6
            ConnectClosestNode(left_nodes, 1);
            ConnectClosestNode(right_nodes, 2);
            GenerateTriangleBase(i_nodes);                                      // 3.2.4
            CastObjectShadows();                                                // 3.2.5
            try
            {
                GenerateVisibilityArea();                                       // 3.2.7
            }
            catch
            {
                throw new Exception("Invalid polygon. ");
            }
        }

        //------------------------------3.2.3 start
        private List<Node> GenerateInterceptNodes()
        {
            List<Node> intercept_nodes = new List<Node>();
            List<Edge> edges_to_remove = new List<Edge>();
            List<Edge> edges_to_add = new List<Edge>();
            Vector2 t1 = m_triangle[1].position;
            Vector2 t2 = m_triangle[2].position;
            foreach (Edge edge in m_wall_edges.Where(e => Helpers.HasIntersect(e, t1, t2)))
            {
                Vector2 inter_point = Helpers.InterceptPoint(edge, t1, t2);
                Node i_node = new Node(inter_point, Node.NodeType.intercept, edge, pip.timestamp);
                intercept_nodes.Add(i_node);
                edges_to_add.AddRange(edge.Split(i_node));
                edges_to_remove.Add(edge);
            }

            m_wall_edges = m_wall_edges.Except(edges_to_remove).ToList();
            m_wall_edges.AddRange(edges_to_add);

            return intercept_nodes;
        }

        //------------------------------3.2.3 end
        //------------------------------3.2.4 start

        private void GenerateTriangleBase(List<Node> i_nodes)
        {
            Node prev = m_triangle[1];
            while (i_nodes.Count != 0)
            {
                Node next = GetClosestInterceptNode(prev.position, i_nodes);
                Edge edge = new Edge(prev, next);
                m_wall_edges.Add(edge);
                prev = next;
                i_nodes.Remove(next);
            }
            m_wall_edges.Add(new Edge(prev, m_triangle[2]));
        }

        private Node GetClosestInterceptNode(Vector2 position, List<Node> intercept_nodes)
        {
            Node closest_node = null;
            float distance = float.PositiveInfinity;
            foreach (Node node in intercept_nodes.Where( n => distance > Vector2.Distance(n.position, position)))
            {
                closest_node = node;
                distance = Vector2.Distance(node.position, position);
            }
            return closest_node;
        }

        //------------------------------3.2.4 end
        //------------------------------3.2.5 start

        private void CastObjectShadows()
        {
            Vector2 guard_pos = m_triangle[0].position;
            float distance;
            Edge closest_edge;
            Vector2 closest_point;
            foreach (Node pinned_node in m_pinned_nodes)
            {
                distance = float.PositiveInfinity;
                closest_edge = null;
                closest_point = new Vector2();
                foreach (Edge edge in m_wall_edges.Where(edge => ShadowCriteria(edge, pinned_node, distance, out distance)))
                {
                    closest_point = Helpers.InterceptPoint(edge, m_triangle[0], pinned_node); ;
                    closest_edge = edge;
                }
                if (distance != float.PositiveInfinity && distance != float.NegativeInfinity)
                {
                    Node node = new Node(closest_point, Node.NodeType.shadow, pip.timestamp, pinned_node.ID_0);
                    Node.Connect(pinned_node, node);
                    m_wall_edges.Remove(closest_edge);
                    m_wall_edges.AddRange(closest_edge.Split(node));
                }
            }
        }

        private bool ShadowCriteria(Edge edge, Node node, float distance_in, out float distance_out)
        {
            float dist = (Helpers.InterceptPoint(edge, m_triangle[0], node) - m_triangle[0].position).magnitude;
            bool result = !edge.Connects(node) && distance_in > dist;

            // Handles cases where there is an obstruction between the node and the guard. 
            if (dist < (node.position - m_triangle[0].position).magnitude)
            {
                dist = float.NegativeInfinity;
                result = true;
            }

            if (result)
                distance_out = dist;
            else
                distance_out = distance_in;
            return result;
        }

        //------------------------------3.2.5 end
        //------------------------------3.2.6 start

        private void SplitEdges(out List<Node> left_nodes, out List<Node> right_nodes)
        {
            bool left = false;
            bool right = false;
            List<Edge> new_edges = new List<Edge>();
            List<Edge> to_remove = new List<Edge>();
            left_nodes = new List<Node>();
            right_nodes = new List<Node>();
            foreach (Edge edge in m_wall_edges.Where(
                e =>
                {
                    left = Helpers.HasIntersect(e, m_triangle[1], m_triangle[0]);
                    right = Helpers.HasIntersect(e, m_triangle[2], m_triangle[0]);
                    return left || right;
                }))
            {
                if (left && right)
                {
                    Node dummy_1 = new Node(Helpers.InterceptPoint(edge, m_triangle[1], m_triangle[0]), Node.NodeType.leg, pip.timestamp, 0);
                    Node dummy_2 = new Node(Helpers.InterceptPoint(edge, m_triangle[2], m_triangle[0]), Node.NodeType.leg, pip.timestamp, 1);
                    left_nodes.Add(dummy_1);
                    right_nodes.Add(dummy_2);
                    Edge[] edges = edge.Split(dummy_1, dummy_2);
                    Node.Disconnect(dummy_1, edge.node_0);
                    Node.Disconnect(dummy_1, edge.node_1);
                    Node.Disconnect(dummy_2, edge.node_0);
                    Node.Disconnect(dummy_2, edge.node_1);
                    new_edges.Add(edges[1]);
                }
                else
                {
                    Node dummy = new Node(Helpers.InterceptPoint(edge, m_triangle[left ? 1 : 2], m_triangle[0]), Node.NodeType.leg, pip.timestamp, left ? 0 : 1);
                    if (left) left_nodes.Add(dummy);
                    else right_nodes.Add(dummy);
                    Edge[] edges = edge.Split(dummy);
                    if (!Helpers.IsContained(m_triangle, edge.node_0))
                    {
                        Node.Disconnect(dummy, edge.node_0);
                        new_edges.Add(edges[1]);
                    }
                    else
                    {
                        Node.Disconnect(dummy, edge.node_1);
                        new_edges.Add(edges[0]);
                    }
                    new_edges.AddRange(edge.Split(dummy));
                }
                to_remove.Add(edge);
            }
            m_wall_edges.RemoveAll((Edge e) => { return to_remove.Contains(e); });
            m_wall_edges.AddRange(new_edges);
            m_pinned_nodes.RemoveAll((Node n) => { return !Helpers.IsContained(m_triangle, n); });
        }

        private void ConnectClosestNode(List<Node> nodex, int index)
        {
            float distance = Helpers.Distance(m_triangle[index], m_triangle[0]);
            Node closest_node = m_triangle[index];
            foreach (Node node in nodex.Where(n => distance > (n.position - m_triangle[0].position).magnitude))
            {
                closest_node = node;
                distance = (node.position - m_triangle[0].position).magnitude;
            }
            Node.Connect(closest_node, m_triangle[0]);
        }

        //------------------------------3.2.6 end
        //------------------------------3.2.7 start

        private void GenerateVisibilityArea()
        {
            Node previous = m_triangle[0];
            Node current = m_triangle[0].neighbours[0];
            Node next;
            float min;
            Add(m_triangle[0]);
            Add(current);
            while (current != m_triangle[0])
            {
                next = null;
                min = float.PositiveInfinity;
                foreach (Node node_neighbour in current.neighbours.Where(n => (n != previous) && min > Helpers.Angle(previous, current, n)))
                {
                    min = Helpers.Angle(previous, current, node_neighbour);
                    next = node_neighbour;
                }
                Add(next);
                previous = current;
                current = next;
            }
            RemoveAt(Count - 1);
        }
        //------------------------------3.2.7 end


        // TODO: This should not be in this class. 
        public void GetVisibilityArea(out Vector3[] vertices, out int[] indices)
        {
            List<Vector3> triangle_corners = new List<Vector3>();
            List<int> triangle_indices = new List<int>();

            for (int i = 0; i < Count; i++)
            {
                Vector3 temp = this[i].position;
                temp.z = 0.25f;
                triangle_corners.Add(temp);
            }

            for (int i = 1; i < Count - 1; i++)
            {
                if (this[i] == this[i + 1])
                    continue;
                triangle_indices.Add(0);
                triangle_indices.Add(i);
                triangle_indices.Add(i + 1);
            }
            vertices = triangle_corners.ToArray();
            indices = triangle_indices.ToArray();
        }

        public static bool Compare(VisibilityPolygon vp_0, VisibilityPolygon vp_1)
        {
            if (vp_0 == null || vp_1 == null || vp_0.Count != vp_1.Count)
                return false;
            for (int i = 0; i < vp_0.Count; i++)
                if (!Node.Compare(vp_0[i], vp_1[i]))
                    return false;
            return true;
        }
    }
}