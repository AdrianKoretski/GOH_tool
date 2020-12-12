using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plus : MonoBehaviour, Obstacle
{
    public List<Vector2> getVertices()
    {
        List<Vector2> vertices = new List<Vector2>();
        vertices.Add(new Vector2(2.5f, 2.5f));
        vertices.Add(new Vector2(2.5f, 7.5f));
        vertices.Add(new Vector2(-2.5f, 7.5f));
        vertices.Add(new Vector2(-2.5f, 2.5f));
        vertices.Add(new Vector2(-7.5f, 2.5f));
        vertices.Add(new Vector2(-7.5f, -2.5f));
        vertices.Add(new Vector2(-2.5f, -2.5f));
        vertices.Add(new Vector2(-2.5f, -7.5f));
        vertices.Add(new Vector2(2.5f, -7.5f));
        vertices.Add(new Vector2(2.5f, -2.5f));
        vertices.Add(new Vector2(7.5f, -2.5f));
        vertices.Add(new Vector2(7.5f, 2.5f));
        return vertices;
    }
}
