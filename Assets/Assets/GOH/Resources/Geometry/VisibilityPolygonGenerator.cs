using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOH
{
    public class VisibilityPolygonGenerator
    {
        public readonly float m_depth_of_field;
        public readonly float m_field_of_view;
        public readonly float m_wiggle_delta;
        public readonly float m_threshold;
        public readonly int m_max_reattempts;
        public readonly List<Node> m_terrain_nodes;

        public VisibilityPolygonGenerator(List<Node> terrain_nodes, Settings settings)
        {
            m_terrain_nodes = terrain_nodes;
            m_depth_of_field = settings.MaxViewDistance;
            m_field_of_view = settings.FieldOfView * Mathf.PI / 360;
            m_wiggle_delta = settings.WiggleDelta;
            m_threshold = settings.NonSimilarityThreshold;
            m_max_reattempts = settings.MaxReAttempts;
        }

        //------------------------------3.2 start

        public VisibilityPolygon GetVisibilityPolygon(Pip pip)
        {
            VisibilityPolygon vis_polygon = null;
            int safety_count = 0;
            try
            {
                vis_polygon = GenerateVisibilityPolygon(pip);
            }
            catch
            {
                safety_count = 0;
                while (safety_count < m_max_reattempts)
                {
                    try
                    {
                        vis_polygon = GenerateVisibilityPolygonWiggle(pip);
                        break;
                    }
                    catch
                    {
                        safety_count++;
                    }
                }
            }
            if (safety_count == m_max_reattempts)
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

        //------------------------------3.2 end
        //------------------------------3.2.1 start

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

        //------------------------------3.2.1 end
        //------------------------------3.2.2-nodes start

        private List<Node> GenerateObstacleVertices(Vector2[] visibility_triangle, float timestamp)
        {
            List<Node> obstacle_nodes = new List<Node>();
            foreach (Node node in m_terrain_nodes.Where(n => TriangleCondition(visibility_triangle, n)))
                obstacle_nodes.Add(node.CopyToPinned(timestamp));
            return obstacle_nodes;
        }

        private bool TriangleCondition(Vector2[] visibility_triangle, Node node)
        {
            return Helpers.IsContained(visibility_triangle, node) || HasEdgeCrossingTriangle(visibility_triangle, node);
        }

        private bool HasEdgeCrossingTriangle(Vector2[] visibility_triangle, Node node)
        {
            foreach (Node node_neighbour in node.neighbours)
                for (int j = 0; j < 3; j++)
                    if (Helpers.HasIntersect(node.position, node_neighbour.position, visibility_triangle[j], visibility_triangle[(j + 1) % 3]))
                        return true;
            return false;
        }

        //------------------------------3.2.2-nodes end
        //------------------------------3.2.2-edges start

        private List<Edge> GenerateObstacleEdges(List<Node> node_list)
        {
            List<Edge> edge_list = new List<Edge>();
            foreach (Node node_0 in node_list)
                foreach (Node node_1 in node_list)
                    if (!(node_0).IsNeighbor(node_1) && m_terrain_nodes[node_0.ID_0].IsNeighbor(m_terrain_nodes[node_1.ID_0]))
                        edge_list.Add(new Edge(node_0, node_1));
            return edge_list;
        }

        //------------------------------3.2.2-edges end
        //------------------------------3.3.2 start

        public List<VisibilityPolygon> GenerteIntermediatePolygons(VisibilityPolygon poly_0, VisibilityPolygon poly_1)
        {
            List<VisibilityPolygon> polys = new List<VisibilityPolygon>();
            if (poly_1.timestamp - poly_0.timestamp <= m_threshold)
                return polys;
            VisibilityPolygon top_poly = poly_0;
            do
            {
                polys.AddRange(GenerateVisibilityPair(top_poly, poly_1));
                top_poly = polys[polys.Count - 1];
            } while (!VisibilityPolygon.Compare(top_poly, poly_1) && poly_1.timestamp - top_poly.timestamp > m_threshold);
            return polys;
        }

        public List<VisibilityPolygon> GenerateVisibilityPair(VisibilityPolygon poly_0, VisibilityPolygon poly_1)
        {
            List<VisibilityPolygon> polys = new List<VisibilityPolygon>();

            VisibilityPolygon B = poly_0;
            VisibilityPolygon A = poly_1;

            while (A.timestamp - B.timestamp > m_threshold)
            {
                Pip average = Helpers.Average(A.pip, B.pip);
                VisibilityPolygon N = GetVisibilityPolygon(average);

                _ = VisibilityPolygon.Compare(N, B) ? B = N : A = N;
            }
            if (B != poly_0)
                polys.Add(B);
            if (A != poly_1)
                polys.Add(A);
            return polys;
        }
        //------------------------------3.3.2 end
    }
}