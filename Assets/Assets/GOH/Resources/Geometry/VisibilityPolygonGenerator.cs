using System.Collections.Generic;
using UnityEngine;

namespace GOH
{
    public class VisibilityPolygonGenerator
    {
        [SerializeField] private float m_depth_of_field;
        [SerializeField] private float m_field_of_view;
        [SerializeField] private float m_wiggle_delta;
        [SerializeField] private float m_threshold;
        [SerializeField] private int m_max_reattempts;

        [SerializeField] private List<Node> m_terrain_nodes;

        public VisibilityPolygonGenerator(List<Node> terrain_nodes, Settings settings)
        {
            m_terrain_nodes = terrain_nodes;
            m_depth_of_field = settings.MaxViewDistance;
            m_field_of_view = settings.FieldOfView * Mathf.PI / 360;
            m_wiggle_delta = settings.WiggleDelta;
            m_threshold = settings.NonSimilarityThreshold;
            m_max_reattempts = settings.MaxReAttempts;
        }

        //------------------------------2.0 start

        public VisibilityPolygon GetVisibilityPolygon(Pip pip)
        {
            VisibilityPolygon vis_polygon = GenerateVisibilityPolygon(pip);
            int safety_count = 0;
            while (!vis_polygon.is_valid && safety_count < m_max_reattempts)
            {
                vis_polygon = GenerateVisibilityPolygonWiggle(pip);
                safety_count++;
            }
            if (!vis_polygon.is_valid)
                MonoBehaviour.print("oh no");                               // TODO: Add proper error handling. 
            return vis_polygon;
        }

        private VisibilityPolygon GenerateVisibilityPolygon(Pip pip)
        {
            Vector2[] visibility_triangle = GenerateVisibilityTriangle(pip);

            List<Node> node_list = GenerateObstacleVertices(visibility_triangle, pip.timestamp);
            List<Edge> edge_list = GenerateObstacleEdges(node_list);

            VisibilityPolygon new_vis_polygon = new VisibilityPolygon(visibility_triangle, pip, node_list, edge_list);
            return new_vis_polygon;
        }

        private VisibilityPolygon GenerateVisibilityPolygonWiggle(Pip pip)
        {
            Pip delta_pip = new Pip(pip);
            delta_pip.Wiggle(m_wiggle_delta);
            return GenerateVisibilityPolygon(delta_pip);
        }

        //------------------------------2.0 end
        //------------------------------2.1 start

        Vector2[] GenerateVisibilityTriangle(Pip path_position)
        {
            Vector2[] visibility_triangle = new Vector2[3];
            visibility_triangle[0] = path_position.position;
            float orientation = path_position.orientation;
            float angle_from_center = m_depth_of_field / Mathf.Cos(m_field_of_view);
            visibility_triangle[1] = visibility_triangle[0] + new Vector2(Mathf.Cos(orientation + m_field_of_view) * angle_from_center, Mathf.Sin(orientation + m_field_of_view) * angle_from_center);
            visibility_triangle[2] = visibility_triangle[0] + new Vector2(Mathf.Cos(orientation - m_field_of_view) * angle_from_center, Mathf.Sin(orientation - m_field_of_view) * angle_from_center);
            return visibility_triangle;
        }

        //------------------------------2.1 end
        //------------------------------2.2-nodes start

        private List<Node> GenerateObstacleVertices(Vector2[] visibility_triangle, float timestamp)
        {
            List<Node> obstacle_nodes = new List<Node>();
            foreach (Node node in m_terrain_nodes)
                if (Helpers.IsContained(visibility_triangle, node.position) || HasEdgeCrossingTriangle(visibility_triangle, node))
                    obstacle_nodes.Add(node.CopyToPinned(timestamp));
            return obstacle_nodes;
        }

        private bool HasEdgeCrossingTriangle(Vector2[] visibility_triangle, Node node)
        {
            for (int i = 0; i < node.GetNeighborCount(); i++)
                for (int j = 0; j < 3; j++)
                    if (Helpers.HasIntersect(node.position, node.GetNeighborNode(i).position, visibility_triangle[j], visibility_triangle[(j + 1) % 3]))
                        return true;
            return false;
        }

        //------------------------------2.2-nodes end
        //------------------------------2.2-edges start

        private List<Edge> GenerateObstacleEdges(List<Node> node_list)
        {
            List<Edge> edge_list = new List<Edge>();
            foreach (Node node in node_list)
            {
                Node p_node = m_terrain_nodes[node.ID_0];
                for (int j = 0; j < p_node.GetNeighborCount(); j++)
                {
                    Node p_node_neighbor = p_node.GetNeighborNode(j);
                    Node neighbor = FindNodeCopy(p_node_neighbor, node_list);
                    if (neighbor != null && !node.IsNeighbor(neighbor))
                        edge_list.Add(new Edge(node, neighbor));
                }
            }
            return edge_list;
        }

        private Node FindNodeCopy(Node node_copy, List<Node> node_list)
        {
            foreach (Node node in node_list)
                if (Node.Compare(node, node_copy))
                    return node;
            return null;
        }

        //------------------------------2.2-edges end
        //------------------------------3.2 start

        public List<VisibilityPolygon> GenerteIntermediatePolygons(VisibilityPolygon poly_0, VisibilityPolygon poly_1)
        {
            List<VisibilityPolygon> polys = new List<VisibilityPolygon>();
            if (poly_1.GetTimestamp() - poly_0.GetTimestamp() <= m_threshold)
                return polys;
            polys.AddRange(GenerateVisibilityPair(poly_0, poly_1));
            while (!polys[polys.Count - 1].Compare(poly_1) && poly_1.GetTimestamp() - polys[polys.Count - 1].GetTimestamp() > m_threshold)
                polys.AddRange(GenerateVisibilityPair(polys[polys.Count - 1], poly_1));
            return polys;
        }

        public List<VisibilityPolygon> GenerateVisibilityPair(VisibilityPolygon poly_0, VisibilityPolygon poly_1)
        {
            List<VisibilityPolygon> polys = new List<VisibilityPolygon>();

            VisibilityPolygon B = poly_0;
            VisibilityPolygon A = poly_1;

            while (A.GetTimestamp() - B.GetTimestamp() > m_threshold)
            {
                Pip average = Helpers.Average(A.GetPathPip(), B.GetPathPip());
                VisibilityPolygon N = GetVisibilityPolygon(average);

                _ = N.Compare(B) ? B = N : A = N;
            }
            if (B != poly_0)
                polys.Add(B);
            if (A != poly_1)
                polys.Add(A);
            return polys;
        }
        //------------------------------3.2 end
    }
}