﻿using System.Collections.Generic;
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

        void Start()
        {
            List<Node> nodes = obstacle_manager.getObstacles();
            m_path = path_manager.GeneratePath();
            if (m_path == null)
                Destroy(this);
            Settings settings = this.GetComponent<Settings>();
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
            VisibilityPolygon vis = vis_poly_gen.generateVisibilityPolygon(m_path[m_path_index]);
            addPolygon(vis);
        }

        private void addPolygon(VisibilityPolygon vis)
        {
            if (m_path_index != 0 && !vis.compare(vis_polys[vis_polys.Count - 1]))
            {
                List<VisibilityPolygon> polys = vis_poly_gen.generteIntermediatePolygons(vis_polys[vis_polys.Count - 1], vis);
                insertIntermediatePolygons(polys);
            }
            vis_polys.Add(vis);
            PolygonVisualiser.instance.CreatePolygon(vis.getTimestamp());
            vis.GetVisibilityArea(out Vector3[] v, out int[] tni);
            PolygonVisualiser.instance.AddMesh(v, tni, vis.getTimestamp());

            visibility_manifold.addArea(vis);
            if (m_path_index == m_path.Count - 1)
                visibility_manifold.cap();
        }

        private void insertIntermediatePolygons(List<VisibilityPolygon> polys)
        {
            for (int i = 0; i < polys.Count; i++)
            {
                PolygonVisualiser.instance.CreatePolygon(polys[i].getTimestamp());
                polys[i].GetVisibilityArea(out Vector3[] v, out int[] tni);
                PolygonVisualiser.instance.AddMesh(v, tni, polys[i].getTimestamp());
                m_path.Insert(m_path_index, polys[i].getPathPip());
                vis_polys.Add(polys[i]);
                visibility_manifold.addArea(polys[i]);
                m_path_index++;
            }
        }
    }
}