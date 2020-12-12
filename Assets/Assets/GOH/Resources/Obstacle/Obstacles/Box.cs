using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, Obstacle
{
    public List<Vector2> getVertices()
    {
        List<Vector2> vertices = new List<Vector2>();
        vertices.Add(new Vector2(1.00f, 1.00f));
        vertices.Add(new Vector2(-1.00f, 1.00f));
        vertices.Add(new Vector2(-1.00f, -1.00f));
        vertices.Add(new Vector2(1.00f, -1.00f));
        return vertices;
    }
}
