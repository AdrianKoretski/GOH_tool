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

        [SerializeField] private List<Node> m_terrain_nodes;
        [SerializeField] private readonly Vector2[] triangle_corners = new Vector2[3];

        public VisibilityPolygonGenerator(List<Node> terrain_nodes, GOH_Settings settings)
        {
            m_terrain_nodes = terrain_nodes;
            m_depth_of_field = settings.MaxViewDistance;
            m_field_of_view = settings.FieldOfView * Mathf.PI / 360;
            m_wiggle_delta = settings.WiggleDelta;
            m_threshold = settings.NonSimilarityThreshold;
        }

        //------------------------------2.0 start

        public VisibilityPolygon generateVisibilityPolygon(Pip pip)
        {
            updateView(pip);

            List<Node> node_list = gatherObstacleVertices(pip.timestamp);
            List<Edge> edge_list = connectObstacleVertices(node_list);

            VisibilityPolygon new_vis_polygon = new VisibilityPolygon(triangle_corners, pip, node_list, edge_list);

            int safety = 0;
            while (safety < 3 && !new_vis_polygon.is_valid)
            {
                Pip delta_pip = pip;
                delta_pip.position.x += UnityEngine.Random.Range(-m_wiggle_delta, m_wiggle_delta);
                delta_pip.position.y += UnityEngine.Random.Range(-m_wiggle_delta, m_wiggle_delta);
                updateView(delta_pip);

                node_list = gatherObstacleVertices(delta_pip.timestamp);
                edge_list = connectObstacleVertices(node_list);

                new_vis_polygon = new VisibilityPolygon(triangle_corners, delta_pip, node_list, edge_list);
                safety++;
            }
            if (safety == 3)
            {
                MonoBehaviour.print("oh no");
            }
            return new_vis_polygon;
        }

        //------------------------------2.0 end
        //------------------------------2.1 start

        void updateView(Pip path_position)
        {
            Vector2 position = path_position.position;
            float orientation = path_position.orientation;
            float angle_from_center = m_depth_of_field / Mathf.Cos(m_field_of_view);
            triangle_corners[0] = position;
            triangle_corners[1] = position + new Vector2(Mathf.Cos(orientation + m_field_of_view) * angle_from_center, Mathf.Sin(orientation + m_field_of_view) * angle_from_center);
            triangle_corners[2] = position + new Vector2(Mathf.Cos(orientation - m_field_of_view) * angle_from_center, Mathf.Sin(orientation - m_field_of_view) * angle_from_center);
        }

        //------------------------------2.1 end
        //------------------------------2.2-nodes start

        private List<Node> gatherObstacleVertices(float timestamp)
        {
            List<Node> vertex_list = new List<Node>();
            for (int i = 0; i < m_terrain_nodes.Count; i++)
                if (Helpers.IsContained(triangle_corners[0], triangle_corners[1], triangle_corners[2], m_terrain_nodes[i].position) || hasEdgeCrossingTriangle(m_terrain_nodes[i]))
                    vertex_list.Add(m_terrain_nodes[i].CopyPinnedObstacleNode(timestamp));
            return vertex_list;
        }

        private bool hasEdgeCrossingTriangle(Node vertex)
        {
            for (int i = 0; i < vertex.GetNeighborCount(); i++)
                for (int j = 0; j < 3; j++)
                    if (vertex.DoesNeighborIntersect(i, triangle_corners[j], triangle_corners[(j + 1) % 3]))
                        return true;
            return false;
        }

        //------------------------------2.2-nodes end
        //------------------------------2.2-edges start

        private List<Edge> connectObstacleVertices(List<Node> vertex_list)
        {
            List<Edge> edge_list = new List<Edge>();
            for (int i = 0; i < vertex_list.Count; i++)
            {
                Node o_node = vertex_list[i];
                Node p_node = m_terrain_nodes[o_node.ID_0];
                for (int j = 0; j < p_node.GetNeighborCount(); j++)
                {
                    Node p_node_neighbor = p_node.GetNeighborNode(j);
                    Node neighbor = findNodeCopy(p_node_neighbor, vertex_list);
                    if (neighbor != null && !o_node.IsNeighbor(neighbor))
                        edge_list.Add(new Edge(o_node, neighbor));
                }
            }
            return edge_list;
        }

        private Node findNodeCopy(Node v, List<Node> vertex_list)
        {
            for (int i = 0; i < vertex_list.Count; i++)
                if (Node.Compare(vertex_list[i], v))
                    return vertex_list[i];
            return null;
        }

        //------------------------------2.2-edges end
        //------------------------------3.2 start

        public List<VisibilityPolygon> generteIntermediatePolygons(VisibilityPolygon poly_0, VisibilityPolygon poly_1)
        {
            List<VisibilityPolygon> polys = new List<VisibilityPolygon>();
            if (poly_1.getTimestamp() - poly_0.getTimestamp() <= m_threshold)
                return polys;
            polys.AddRange(generateVisibilityPair(poly_0, poly_1));
            while (!polys[polys.Count - 1].compare(poly_1) && poly_1.getTimestamp() - polys[polys.Count - 1].getTimestamp() > m_threshold)
                polys.AddRange(generateVisibilityPair(polys[polys.Count - 1], poly_1));
            return polys;
        }

        public List<VisibilityPolygon> generateVisibilityPair(VisibilityPolygon poly_0, VisibilityPolygon poly_1)
        {
            List<VisibilityPolygon> polys = new List<VisibilityPolygon>();

            VisibilityPolygon B = poly_0;
            VisibilityPolygon A = poly_1;

            while (A.getTimestamp() - B.getTimestamp() > m_threshold)
            {
                VisibilityPolygon N = generateVisibilityPolygon(Helpers.Average(A.getPathPip(), B.getPathPip()));
                if (N.compare(B))
                {
                    /*if (B != poly_0)
                        UnityEngine.Object.Destroy(B.gameObject);*/
                    B = N;
                }
                else
                {
                    /*if (A != poly_1)
                        UnityEngine.Object.Destroy(A.gameObject);*/
                    A = N;
                }
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