using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOH
{
    public class PolygonVisualiser : MonoBehaviour
    {
        public bool previous = false;
        public bool next = false;

        public int polygon_index = 0;
        public float polygon_timestamp = 0;
        public int max_index = 0;

        private List<GameObject> m_polygons = new List<GameObject>();
        private List<float> m_timestamps = new List<float>();

        private int m_previous_index = 0;

        public static PolygonVisualiser instance;

        private void Start()
        {
            if (instance != null)
            {
                UnityEngine.Debug.LogError("PolygonVisualiser already instantiated.");
                return;
            }
            instance = this;
        }

        void Update()
        {
            if (m_polygons.Count == 0)
            {
                polygon_index = 0;
                polygon_timestamp = 0;
                max_index = 0;
                next = false;
                previous = false;
                return;
            }
            if (next)
            {
                next = false;
                polygon_index++;
            }
            if (previous)
            {
                previous = false;
                polygon_index--;
            }
            polygon_index = (int)Mathf.Min(Mathf.Max(0, polygon_index), m_polygons.Count - 1);

            if (m_previous_index != polygon_index)
            {
                m_polygons[m_previous_index].SetActive(false);
                m_polygons[polygon_index].SetActive(true);
                m_previous_index = polygon_index;
            }
            polygon_timestamp = m_timestamps[polygon_index];
            max_index = m_polygons.Count - 1;
        }

        public void CreatePolygon(float timestamp)
        {
            polygon_index = m_polygons.Count;
            GameObject go = new GameObject();
            go.transform.parent = this.transform;
            go.name = "Visibility Polygon " + m_polygons.Count + ", TS: " + timestamp;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default")) { color = new Color(1, 1, 0.4f) };
            m_polygons.Add(go);
            m_timestamps.Add(timestamp);
            m_polygons[m_previous_index].SetActive(false);
            m_polygons[polygon_index].SetActive(true);
            m_previous_index = polygon_index;
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

        public void AddMesh(Vector3[] vertices, int[] indices, float timestamp)
        {
            int index = m_timestamps.IndexOf(timestamp);
            if (index == -1)
            {
                UnityEngine.Debug.LogError("ERROR");        // TODO: add proper error message. 
                return;
            }
            m_polygons[index].GetComponent<MeshFilter>().mesh.vertices = vertices;
            m_polygons[index].GetComponent<MeshFilter>().mesh.triangles = indices;
        }
    }
}