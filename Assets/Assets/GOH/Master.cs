using System.Collections.Generic;
using UnityEngine;

namespace GOH
{
    enum PlayStatus
    {
        pause,
        play,
        frame_forward,
        frame_back
    }

    public class Master : MonoBehaviour
    {
        ObstacleManager obstacle_manager = new ObstacleManager();
        PathManager path_manager = new PathManager();
        VisibilityPolygonGenerator vis_poly_gen;

        [SerializeField] private List<Pip> m_path;
        [SerializeField] private int m_path_index;
        [SerializeField] private PlayStatus play_status = PlayStatus.pause;

        private List<VisibilityPolygon> vis_polys = new List<VisibilityPolygon>();
        private VisibilityManifold visibility_manifold;

        private static Master m_instance;

        public static Master GetGOHMaster()
        {
            return m_instance;
        }

        public static void SetGOHMaster(Master instance)
        {
            m_instance = instance;
        }

        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 50, 30), ">"))
                play_status = PlayStatus.play;
            if (GUI.Button(new Rect(60, 10, 50, 30), "||"))
                play_status = PlayStatus.pause;
            if (GUI.Button(new Rect(10, 40, 50, 30), "|<"))
                play_status = PlayStatus.frame_back;
            if (GUI.Button(new Rect(60, 40, 50, 30), ">|"))
                play_status = PlayStatus.frame_forward;
        }

        private void step(int direction)
        {
            if ((m_path_index + 1 >= m_path.Count && direction == 1) || (m_path_index == 0 && direction == -1))
                return;
            m_path_index += direction;
        }

        void Start()
        {
            SetGOHMaster(this);
            List<Node> nodes = obstacle_manager.getObstacles();
            m_path = path_manager.GeneratePath();
            if (m_path == null)
                Destroy(this);
            GOH_Settings settings = this.GetComponent<GOH_Settings>();
            vis_poly_gen = new VisibilityPolygonGenerator(nodes, settings);
            visibility_manifold = new VisibilityManifold();
        }

        void Update()
        {

            if (play_status == PlayStatus.frame_back)
                step(-1);
            if (play_status == PlayStatus.frame_forward || play_status == PlayStatus.play)
                step(1);
            if (play_status == PlayStatus.frame_forward || play_status == PlayStatus.frame_back)
                play_status = PlayStatus.pause;
            generateVisibilityPolygon();
        }

        private void generateVisibilityPolygon()
        {
            if (m_path_index != vis_polys.Count)
            {
                return;
            }
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