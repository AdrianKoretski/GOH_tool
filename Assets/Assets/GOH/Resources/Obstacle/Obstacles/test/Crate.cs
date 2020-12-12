using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour, Obstacle
{
    public List<Vector2> getVertices()
    {
        List<Vector2> vertices = new List<Vector2>();
        vertices.Add(new Vector2(1.001f, 1.001f));
        vertices.Add(new Vector2(-1.001f, 1.001f));
        vertices.Add(new Vector2(-1.001f, -1.001f));
        vertices.Add(new Vector2(1.001f, -1.001f));
        return vertices;
    }
}
