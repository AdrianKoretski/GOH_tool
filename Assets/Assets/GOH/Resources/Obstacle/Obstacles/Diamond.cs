using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour, Obstacle
{
    public List<Vector2> getVertices()
    {
        List<Vector2> vertices = new List<Vector2>();
        vertices.Add(new Vector2(4.5f, 1.5f));
        vertices.Add(new Vector2(5.5f, -1.5f));
        vertices.Add(new Vector2(6.5f, -1.5f));
        vertices.Add(new Vector2(6, 1.5f));
        return vertices;
    }
}
