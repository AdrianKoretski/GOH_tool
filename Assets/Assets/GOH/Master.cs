using System.Collections.Generic;
using UnityEngine;

namespace GOH
{
    public class Master : MonoBehaviour
    {
        ObstacleManager obstacle_manager = new ObstacleManager();
        PathManager path_manager = new PathManager();
        VisibilityPolygonGenerator vis_poly_gen;

        [SerializeField] private List<Pip> m_path;
        [SerializeField] private int m_path_index;

        private List<VisibilityPolygon> vis_polys = new List<VisibilityPolygon>();
        private VisibilityManifold visibility_manifold;

        public Settings settings;

        void Start()
        {
            List<Node> nodes = obstacle_manager.getObstacles();
            m_path = path_manager.GeneratePath();
            if (m_path == null)
                Destroy(this);
            vis_poly_gen = new VisibilityPolygonGenerator(nodes, settings);
            visibility_manifold = new VisibilityManifold();
        }

        void Update()
        {
            generateVisibilityPolygon();
            m_path_index++;
        }

        private void generateVisibilityPolygon()
        {
            if (m_path_index >= m_path.Count)
                return;
            VisibilityPolygon vis = vis_poly_gen.GetVisibilityPolygon(m_path[m_path_index]);
            AddPolygon(vis);
        }

        private void AddPolygon(VisibilityPolygon vis)
        {
            if (m_path_index != 0 && !vis.Compare(vis_polys[vis_polys.Count - 1]))
            {
                List<VisibilityPolygon> polys = vis_poly_gen.GenerteIntermediatePolygons(vis_polys[vis_polys.Count - 1], vis);
                InsertIntermediatePolygons(polys);
            }
            vis_polys.Add(vis);
            PolygonVisualiser.instance.CreatePolygon(vis.GetTimestamp());
            vis.GetVisibilityArea(out Vector3[] v, out int[] tni);
            PolygonVisualiser.instance.AddMesh(v, tni, vis.GetTimestamp());

            visibility_manifold.addArea(vis);
            if (m_path_index == m_path.Count - 1)
                visibility_manifold.cap();
        }

        private void InsertIntermediatePolygons(List<VisibilityPolygon> polys)
        {
            for (int i = 0; i < polys.Count; i++)
            {
                PolygonVisualiser.instance.CreatePolygon(polys[i].GetTimestamp());
                polys[i].GetVisibilityArea(out Vector3[] v, out int[] tni);
                PolygonVisualiser.instance.AddMesh(v, tni, polys[i].GetTimestamp());
                m_path.Insert(m_path_index, polys[i].GetPathPip());
                vis_polys.Add(polys[i]);
                visibility_manifold.addArea(polys[i]);
                m_path_index++;
            }
        }
    }
}