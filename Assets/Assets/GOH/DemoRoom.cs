using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoRoom : MonoBehaviour, Obstacle
{
    public List<Vector2> getVertices()
    {
        List<Vector2> vertices = new List<Vector2>();
        vertices.Add(new Vector2(7.0f, -7.0f));
        vertices.Add(new Vector2(7.0f, 7.0f));
        vertices.Add(new Vector2(3.0f, 7.0f));
        vertices.Add(new Vector2(3.0f, -3.0f));
        vertices.Add(new Vector2(2.0f, -3.0f));
        vertices.Add(new Vector2(2.0f, 7.0f));
        vertices.Add(new Vector2(-7.0f, 7.0f));
        vertices.Add(new Vector2(-7.0f, -7.0f));
        vertices.Add(new Vector2(-3.0f, -7.0f));
        vertices.Add(new Vector2(-3.0f, 3.0f));
        vertices.Add(new Vector2(-2.0f, 3.0f));
        vertices.Add(new Vector2(-2.0f, -7.0f));
        return vertices;
    }
}