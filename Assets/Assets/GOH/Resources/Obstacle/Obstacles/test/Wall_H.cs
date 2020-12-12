using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_H : MonoBehaviour, Obstacle
{
    public List<Vector2> getVertices()
    {
        List<Vector2> vertices = new List<Vector2>();
        vertices.Add(new Vector2(5.0f, 7.0f));
        vertices.Add(new Vector2(5.0f, - 7.0f));
        vertices.Add(new Vector2(4.0f, - 7.0f));
        vertices.Add(new Vector2(4.0f, 7.0f));
        return vertices;
    }
}
