using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOH
{
    public class VisibilityPolygon
    {
        public readonly Pip pip;
        private Node[] m_visibility_triangle = new Node[3];
        private List<Node> m_pinned_nodes = new List<Node>();
        private List<Edge> m_wall_edges = new List<Edge>();
        private List<Node> vis_graph = new List<Node>();

        //------------------------------Setup start

        public VisibilityPolygon(Vector2[] triangle_corners, Pip pip, List<Node> nodes, List<Edge> edges)
        {
            for (int i = 0; i < 3; i++)
                m_visibility_triangle[i] = new Node(triangle_corners[i], i == 0 ? Node.NodeType.guard : Node.NodeType.leg, pip.timestamp, i);
            this.pip = pip;
            m_wall_edges = edges;
            m_pinned_nodes = nodes;

            GeneratePolygon();
        }

        //------------------------------Setup end

        private void GeneratePolygon()
        {
            List<Node> i_nodes = GenerateInterceptNodes();      // 2.3
            List<Edge> back = GenerateTriangleBase(i_nodes);    // 2.4
            CastCornerShadowNodes();                            // 2.6
            m_wall_edges.AddRange(back);
            Cleaup();
            CastObjectShadows();                                // 2.5
            GenerateVisibilityArea();                           // 2.7
        }

        //------------------------------2.3 start
        private List<Node> GenerateInterceptNodes()
        {
            List<Node> i_nodes = new List<Node>();
            for (int i = m_wall_edges.Count - 1; i >= 0; i--)
            {
                if (Helpers.HasIntersect(m_wall_edges[i], m_visibility_triangle[1], m_visibility_triangle[2]))
                {
                    Vector2 inter_point = Helpers.InterceptPoint(m_wall_edges[i], m_visibility_triangle[1], m_visibility_triangle[2]);
                    Node i_node = new Node(inter_point, Node.NodeType.intercept, pip.timestamp, m_wall_edges[i].node_0.ID_0, m_wall_edges[i].node_1.ID_0);
                    i_nodes.Add(i_node);
                    Edge[] splits = m_wall_edges[i].Split(i_node);
                    m_wall_edges.AddRange(splits);
                    m_wall_edges.RemoveAt(i);
                }
            }
            return i_nodes;
        }
        //------------------------------2.3 end
        //------------------------------2.4 start
        private List<Edge> GenerateTriangleBase(List<Node> i_nodes)
        {
            List<Edge> back = new List<Edge>();
            Node prev = m_visibility_triangle[1];
            while (i_nodes.Count != 0)
            {
                Node next = GetClosestInterceptNode(prev.position, i_nodes);
                Edge edge = new Edge(prev, next);
                back.Add(edge);
                prev = next;
                i_nodes.Remove(next);
            }
            Edge e = new Edge(prev, m_visibility_triangle[2]);
            back.Add(e);
            return back;
        }

        private Node GetClosestInterceptNode(Vector2 position, List<Node> i_nodes)
        {
            Node closest_node = null;
            float distance = float.PositiveInfinity;
            for (int i = 0; i < i_nodes.Count; i++)
            {
                Vector2 current_position = i_nodes[i].position;
                if (distance > Vector2.Distance(current_position, position))
                {
                    closest_node = i_nodes[i];
                    distance = Vector2.Distance(current_position, position);
                }
            }
            return closest_node;
        }
        //------------------------------2.4 end
        //------------------------------2.5 start

        private void CastObjectShadows()
        {
            for (int j = 0; j < m_pinned_nodes.Count; j++)
            {
                if (!Helpers.IsContained(m_visibility_triangle, m_pinned_nodes[j]))
                    continue;
                Vector2 guard_pos = m_visibility_triangle[0].position;
                Vector2 o_node_pos = m_pinned_nodes[j].position;
                float distance = float.PositiveInfinity;
                float o_node_distance = (o_node_pos - guard_pos).magnitude;
                Edge closest_edge = null;
                Vector2 closest_point = new Vector2();
                for (int i = 0; i < m_wall_edges.Count; i++)
                {
                    if (m_wall_edges[i].Connects(m_pinned_nodes[j]))
                        continue;
                    Vector2 point = Helpers.InterceptPoint(m_wall_edges[i], m_visibility_triangle[0], m_pinned_nodes[j]);
                    if (distance > (point - guard_pos).magnitude)
                    {
                        closest_edge = m_wall_edges[i];
                        distance = (point - guard_pos).magnitude;
                        closest_point = point;
                        if (distance < o_node_distance)
                        {
                            distance = float.PositiveInfinity;
                            break;
                        }
                    }
                }
                if (distance != float.PositiveInfinity)
                {
                    Node node = new Node(closest_point, Node.NodeType.shadow, pip.timestamp, m_pinned_nodes[j].ID_0);
                    Node.Connect(m_pinned_nodes[j], node);

                    m_wall_edges.Remove(closest_edge);
                    Edge[] splits = closest_edge.Split(node);
                    m_wall_edges.AddRange(splits);
                }
            }
        }
        //------------------------------2.5 end
        //------------------------------2.6 start
        private void CastCornerShadowNodes()
        {
            float left_distance = Helpers.Distance(m_visibility_triangle[1], m_visibility_triangle[0]);
            Vector2 guard_position = m_visibility_triangle[0].position;
            Node closest_left = m_visibility_triangle[1];
            List<Edge> new_edges = new List<Edge>();
            for (int i = 0; i < m_wall_edges.Count; i++)
            {
                if (Helpers.HasIntersect(m_wall_edges[i], m_visibility_triangle[1], m_visibility_triangle[0]))
                {
                    Vector2 inter_point = Helpers.InterceptPoint(m_wall_edges[i], m_visibility_triangle[1], m_visibility_triangle[0]);
                    Node dummy = new Node(inter_point, Node.NodeType.leg, pip.timestamp, 0);
                    new_edges.AddRange(m_wall_edges[i].Split(dummy));
                    m_wall_edges.RemoveAt(i);
                    i--;
                    if (left_distance > (inter_point - guard_position).magnitude)
                    {
                        closest_left = dummy;
                        left_distance = (inter_point - guard_position).magnitude;
                    }
                }
            }
            m_wall_edges.AddRange(new_edges);
            new_edges.Clear();
            Node.Connect(closest_left, m_visibility_triangle[0]);

            float right_distance = Helpers.Distance(m_visibility_triangle[2], m_visibility_triangle[0]);
            Node closest_right = m_visibility_triangle[2];
            for (int i = 0; i < m_wall_edges.Count; i++)
            {
                if (Helpers.HasIntersect(m_wall_edges[i], m_visibility_triangle[2], m_visibility_triangle[0]))
                {
                    Vector2 inter_point = Helpers.InterceptPoint(m_wall_edges[i], m_visibility_triangle[2], m_visibility_triangle[0]);
                    Node dummy = new Node(inter_point, Node.NodeType.leg, pip.timestamp, 1);
                    new_edges.AddRange(m_wall_edges[i].Split(dummy));
                    m_wall_edges.RemoveAt(i);
                    i--;
                    if (right_distance > (inter_point - guard_position).magnitude)
                    {
                        closest_right = dummy;
                        right_distance = (inter_point - guard_position).magnitude;
                    }
                }
            }
            m_wall_edges.AddRange(new_edges);
            new_edges.Clear();
            Node.Connect(closest_right, m_visibility_triangle[0]);
        }
        //------------------------------2.6 end
        //------------------------------2.7 start
        private void GenerateVisibilityArea()
        {
            Node previous = m_visibility_triangle[0];
            Node current = m_visibility_triangle[0].neighbours[0];
            Node next;
            vis_graph.Add(m_visibility_triangle[0]);
            vis_graph.Add(current);
            while (current != m_visibility_triangle[0])
            {
                next = null;
                float min = float.PositiveInfinity;
                foreach (Node node_neighbour in current.neighbours)
                {
                    if (node_neighbour == previous)
                        continue;
                    if (min > Helpers.Angle(previous, current, node_neighbour))
                    {
                        min = Helpers.Angle(previous, current, node_neighbour);
                        next = node_neighbour;
                    }
                }
                if (next == null)
                    throw new Exception("Invalid polygon. ");
                if (next != m_visibility_triangle[0])
                {
                    vis_graph.Add(next);
                }
                previous = current;
                current = next;
            }
        }
        //------------------------------2.7 end
        //------------------------------Cleanup start

        private void Cleaup()
        {
            foreach (Node pinned_node in m_pinned_nodes)
            {
                if (!Helpers.IsContained(m_visibility_triangle, pinned_node))
                {
                    m_wall_edges.RemoveAll((Edge e) => { return e.Connects(pinned_node); });
                    Node.DisconnectAll(pinned_node);
                }
            }
            m_pinned_nodes.RemoveAll((Node n) => { return !Helpers.IsContained(m_visibility_triangle, n); });
        }

        //------------------------------Cleanup end

        public void GetVisibilityArea(out Vector3[] vertices, out int[] indices)
        {
            List<Vector3> triangle_corners = new List<Vector3>();
            List<int> triangle_indices = new List<int>();

            for (int i = 0; i < vis_graph.Count; i++)
            {
                Vector3 temp = vis_graph[i].position;
                temp.z = 0.25f;
                triangle_corners.Add(temp);
            }

            for (int i = 1; i < vis_graph.Count - 1; i++)
            {
                if (vis_graph[i] == vis_graph[i + 1])
                    continue;
                triangle_indices.Add(0);
                triangle_indices.Add(i);
                triangle_indices.Add(i + 1);
            }
            vertices = triangle_corners.ToArray();
            indices = triangle_indices.ToArray();
        }

        public int Count()
        {
            return vis_graph.Count;
        }

        public Node Last()
        {
            return vis_graph[vis_graph.Count - 1];
        }

        public Node First()
        {
            return vis_graph[0];
        }

        public Node At(int index)
        {
            return vis_graph[index];
        }

        public int IndexOf(Node n)
        {
            return vis_graph.IndexOf(n);
        }

        public bool Compare(VisibilityPolygon vp)
        {
            if (vis_graph.Count != vp.vis_graph.Count)
                return false;
            for (int i = 0; i < vis_graph.Count; i++)
                if (!Node.Compare(vp.vis_graph[i], vis_graph[i]))
                    return false;
            return true;
        }

        public Pip GetPathPip()
        {
            return pip;
        }

        public float GetTimestamp()
        {
            return pip.timestamp;
        }

        public List<Node> GetVisibilityArea()
        {
            return vis_graph;
        }
    }
}