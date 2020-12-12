using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOH
{
    public class VisibilityPolygon
    {
        public readonly Pip pip;
        public readonly Node guard_node;
        [SerializeField] private Node leg_node_0;
        [SerializeField] private Node leg_node_1;
        [SerializeField] private Node dummy_node_0;
        [SerializeField] private Node dummy_node_1;
        [SerializeField] private List<Node> m_p_nodes = new List<Node>();
        [SerializeField] private List<Edge> m_w_edges = new List<Edge>();
        [SerializeField] private List<Node> vis_graph = new List<Node>();
        public Boolean is_valid = true;

        //------------------------------Setup start

        public VisibilityPolygon(Vector2[] triangle_corners, Pip pip, List<Node> nodes, List<Edge> edges)
        {
            guard_node = new Node(triangle_corners[0], Node.NodeType.guard, pip.timestamp, 0);
            dummy_node_0 = new Node(triangle_corners[1], Node.NodeType.leg, pip.timestamp, 0);
            dummy_node_1 = new Node(triangle_corners[2], Node.NodeType.leg, pip.timestamp, 1);
            this.pip = pip;
            m_w_edges = edges;
            m_p_nodes = nodes;
            generatePolygon();
        }

        //------------------------------Setup end

        private void generatePolygon()
        {
            List<Node> i_nodes = generateInterceptNodes();      // 2.3
            if (!is_valid)
                return;
            List<Edge> back = generateTriangleBase(i_nodes);    // 2.4
            castCornerShadowNodes();                            // 2.6
            if (!is_valid)
                return;
            m_w_edges.AddRange(back);
            cleaup();
            castObjectShadows();                                // 2.5
            if (!is_valid)
                return;
            generateVisibilityArea();                           // 2.7
        }

        //------------------------------2.3 start
        private List<Node> generateInterceptNodes()
        {
            List<Node> i_nodes = new List<Node>();
            for (int i = m_w_edges.Count - 1; i >= 0; i--)
            {
                if (Helpers.HasIntersect(m_w_edges[i], dummy_node_0, dummy_node_1))
                {
                    Vector2 inter_point = Helpers.InterceptPoint(m_w_edges[i], dummy_node_0, dummy_node_1);
                    if (float.IsNaN(inter_point.x))
                    {
                        is_valid = false;
                        return null;
                    }
                    Node i_node = new Node(inter_point, Node.NodeType.intercept, pip.timestamp, m_w_edges[i].GetNodeIDs()[0], m_w_edges[i].GetNodeIDs()[1]);
                    i_nodes.Add(i_node);
                    Edge[] splits = m_w_edges[i].Split(i_node);
                    m_w_edges.AddRange(splits);
                    m_w_edges.RemoveAt(i);
                }
            }
            return i_nodes;
        }
        //------------------------------2.3 end
        //------------------------------2.4 start
        private List<Edge> generateTriangleBase(List<Node> i_nodes)
        {
            List<Edge> back = new List<Edge>();
            Node prev = dummy_node_0;
            while (i_nodes.Count != 0)
            {
                Node next = getClosestInterceptNode(prev.position, i_nodes);
                Edge edge = new Edge(prev, next);
                back.Add(edge);
                prev = next;
                i_nodes.Remove(next);
            }
            Edge e = new Edge(prev, dummy_node_1);
            back.Add(e);
            return back;
        }

        private Node getClosestInterceptNode(Vector2 position, List<Node> i_nodes)
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

        private void castObjectShadows()
        {
            for (int j = 0; j < m_p_nodes.Count; j++)
            {
                if (!Helpers.IsContained(guard_node, dummy_node_0, dummy_node_1, m_p_nodes[j]))
                    continue;
                Vector2 guard_pos = guard_node.position;
                Vector2 o_node_pos = m_p_nodes[j].position;
                float distance = float.PositiveInfinity;
                float o_node_distance = (o_node_pos - guard_pos).magnitude;
                Edge closest_edge = null;
                Vector2 closest_point = new Vector2();
                for (int i = 0; i < m_w_edges.Count; i++)
                {
                    if (m_p_nodes[j].IsNeighbor(m_w_edges[i]))
                        continue;
                    Vector2 point = Helpers.InterceptPoint(m_w_edges[i], guard_node, m_p_nodes[j]);
                    if (float.IsNaN(point.x))
                    {
                        is_valid = false;
                        return;
                    }
                    if (distance > (point - guard_pos).magnitude)
                    {
                        closest_edge = m_w_edges[i];
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
                    Node node = new Node(closest_point, Node.NodeType.shadow, pip.timestamp, m_p_nodes[j].ID_0);
                    new Edge(m_p_nodes[j], node);

                    m_w_edges.Remove(closest_edge);
                    Edge[] splits = closest_edge.Split(node);
                    m_w_edges.AddRange(splits);
                }
            }
        }
        //------------------------------2.5 end
        //------------------------------2.6 start
        private void castCornerShadowNodes()
        {
            float left_distance = Helpers.Distance(dummy_node_0, guard_node);
            Vector2 guard_position = guard_node.position;
            Node closest_left = dummy_node_0;
            List<Edge> new_edges = new List<Edge>();
            for (int i = 0; i < m_w_edges.Count; i++)
            {
                if (Helpers.HasIntersect(m_w_edges[i], dummy_node_0, guard_node))
                {
                    Vector2 inter_point = Helpers.InterceptPoint(m_w_edges[i], dummy_node_0, guard_node);
                    if (float.IsNaN(inter_point.x))
                    {
                        is_valid = false;
                        return;
                    }
                    Node dummy = new Node(inter_point, Node.NodeType.leg, pip.timestamp, 0);
                    new_edges.AddRange(m_w_edges[i].Split(dummy));
                    m_w_edges.RemoveAt(i);
                    i--;
                    if (left_distance > (inter_point - guard_position).magnitude)
                    {
                        closest_left = dummy;
                        left_distance = (inter_point - guard_position).magnitude;
                    }
                }
            }
            m_w_edges.AddRange(new_edges);
            new_edges.Clear();
            leg_node_0 = closest_left;
            new Edge(leg_node_0, guard_node);

            float right_distance = Helpers.Distance(dummy_node_1, guard_node);
            Node closest_right = dummy_node_1;
            for (int i = 0; i < m_w_edges.Count; i++)
            {
                if (Helpers.HasIntersect(m_w_edges[i], dummy_node_1, guard_node))
                {
                    Vector2 inter_point = Helpers.InterceptPoint(m_w_edges[i], dummy_node_1, guard_node);
                    if (float.IsNaN(inter_point.x))
                    {
                        is_valid = false;
                        return;
                    }
                    Node dummy = new Node(inter_point, Node.NodeType.leg, pip.timestamp, 1);
                    new_edges.AddRange(m_w_edges[i].Split(dummy));
                    m_w_edges.RemoveAt(i);
                    i--;
                    if (right_distance > (inter_point - guard_position).magnitude)
                    {
                        closest_right = dummy;
                        right_distance = (inter_point - guard_position).magnitude;
                    }
                }
            }
            m_w_edges.AddRange(new_edges);
            new_edges.Clear();
            leg_node_1 = closest_right;
            new Edge(leg_node_1, guard_node);
        }
        //------------------------------2.6 end
        //------------------------------2.7 start
        private void generateVisibilityArea()
        {
            Edge previous_edge = guard_node.GetNeighborEdge(0);
            Node current = guard_node.GetNeighborNode(0);
            Node next;
            vis_graph.Add(guard_node);
            vis_graph.Add(current);
            while (current != guard_node)
            {
                next = null;
                float min = float.PositiveInfinity;
                for (int i = 0; i < current.GetNeighborCount(); i++)
                {
                    if (current.GetNeighborEdge(i) == previous_edge)
                        continue;
                    if (min > Helpers.Angle(previous_edge.GetOtherNode(current), current, current.GetNeighborNode(i)))
                    {
                        min = Helpers.Angle(previous_edge.GetOtherNode(current), current, current.GetNeighborNode(i));
                        next = current.GetNeighborNode(i);
                    }
                }
                if (next == null)
                {
                    is_valid = false;
                    break;
                }
                if (next != guard_node)
                {
                    vis_graph.Add(next);
                }
                previous_edge = next.GetNeighborEdge(current);
                current = next;
            }
        }
        //------------------------------2.7 end
        //------------------------------Cleanup start

        private void cleaup()
        {
            for (int i = m_p_nodes.Count - 1; i >= 0; i--)
            {
                if (Helpers.IsContained(guard_node, dummy_node_0, dummy_node_1, m_p_nodes[i]))
                    continue;
                for (int j = m_p_nodes[i].GetNeighborCount() - 1; j >= 0; j--)
                {
                    Edge neighbor = m_p_nodes[i].GetNeighborEdge(j);
                    m_w_edges.Remove(neighbor);
                    neighbor.Destroy();
                }
                //Destroy(m_p_nodes[i].gameObject);
                m_p_nodes.RemoveAt(i);
            }
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

        public int count()
        {
            return vis_graph.Count;
        }

        public Node last()
        {
            return vis_graph[vis_graph.Count - 1];
        }

        public Node first()
        {
            return vis_graph[0];
        }

        public Node at(int index)
        {
            return vis_graph[index];
        }

        public int IndexOf(Node n)
        {
            return vis_graph.IndexOf(n);
        }

        public bool compare(VisibilityPolygon vp)
        {
            if (vis_graph.Count != vp.vis_graph.Count)
                return false;
            for (int i = 0; i < vis_graph.Count; i++)
                if (!Node.Compare(vp.vis_graph[i], vis_graph[i]))
                    return false;
            return true;
        }

        public Pip getPathPip()
        {
            return pip;
        }

        public float getTimestamp()
        {
            return pip.timestamp;
        }

        public List<Node> getVisibilityArea()
        {
            return vis_graph;
        }
    }
}