using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    int grid_size = 50;

    void Start()
    {
        for (int i = -grid_size; i <= grid_size; i++)
        {
            Vector3[] points = new Vector3[2];
            points[0] = new Vector3(-grid_size, i, 1);
            points[1] = new Vector3(grid_size, i, 1);

            GameObject horizontal_empty_object = new GameObject("line");
            LineRenderer horizontal_line_renderer = horizontal_empty_object.AddComponent<LineRenderer>();
            horizontal_line_renderer.material = new Material(Shader.Find("Sprites/Default"));
            horizontal_line_renderer.startWidth = 0.1f;
            horizontal_line_renderer.endWidth = 0.1f;
            horizontal_line_renderer.SetPositions(points);
            horizontal_line_renderer.startColor = new Color(0.85f, 0.85f, 0.85f);
            horizontal_line_renderer.endColor = new Color(0.85f, 0.85f, 0.85f);
            horizontal_empty_object.transform.parent = this.transform;


            points[0] = new Vector3(i, -grid_size, 1);
            points[1] = new Vector3(i, grid_size, 1);

            GameObject vertical_empty_object = new GameObject("line");
            LineRenderer vertical_line_renderer = vertical_empty_object.AddComponent<LineRenderer>();
            vertical_line_renderer.material = new Material(Shader.Find("Sprites/Default"));
            vertical_line_renderer.startWidth = 0.1f;
            vertical_line_renderer.endWidth = 0.1f;
            vertical_line_renderer.SetPositions(points);
            vertical_line_renderer.startColor = new Color(0.85f, 0.85f, 0.85f);
            vertical_line_renderer.endColor = new Color(0.85f, 0.85f, 0.85f);
            vertical_line_renderer.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
