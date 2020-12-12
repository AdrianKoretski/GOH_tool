using System.Collections.Generic;
using UnityEngine;

namespace GOH
{
    public class SimplifiedGraphVisualiser : MonoBehaviour
    {
        private List<GameObject> m_polygons = new List<GameObject>();
        private List<float> m_timestamps = new List<float>();

        public static SimplifiedGraphVisualiser instance;

        private void Start()
        {
            if (instance != null)
            {
                UnityEngine.Debug.LogError("SimplifiedGraphVisualiser already instantiated.");
                return;
            }
            instance = this;
        }

        public GameObject AddLayer(float timestamp)
        {
            GameObject go = new GameObject("Display Layer TS: " + timestamp);
            go.transform.parent = transform;

            m_polygons.Add(go);
            m_timestamps.Add(timestamp);

            return go;
        }

        public void AddPoint(Node node, float timestamp)
        {
            int index = m_timestamps.IndexOf(timestamp);
            if (index == -1)
            {
                UnityEngine.Debug.LogError("ERROR");        // TODO: add proper error message. 
                return;
            }

        }

        public void AddEdge(Edge edge, float timestamp)
        {

        }

        public void AddGeom(GameObject go, float timestamp)
        {
            int index = m_timestamps.IndexOf(timestamp);
            if (index == -1)
            {
                UnityEngine.Debug.LogError("ERROR");        // TODO: add proper error message. 
                return;
            }
            go.transform.parent = m_polygons[index].transform;
        }
    }
}