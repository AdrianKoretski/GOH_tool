using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_V : MonoBehaviour, Obstacle
{
    public List<Vector2> getVertices()
    {
        List<Vector2> vertices = new List<Vector2>();
        vertices.Add(new Vector2(7.0f, 5.0f));
        vertices.Add(new Vector2(-7.0f, 5.0f));
        vertices.Add(new Vector2(-7.0f, 4.0f));
        vertices.Add(new Vector2(7.0f, 4.0f));
        return vertices;
    }
}
