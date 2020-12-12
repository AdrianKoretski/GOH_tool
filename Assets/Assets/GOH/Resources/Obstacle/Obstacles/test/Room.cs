using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour, Obstacle
{
    public List<Vector2> getVertices()
    {
        List<Vector2> vertices = new List<Vector2>();
        vertices.Add(new Vector2(7.0f, 7.0f));
        vertices.Add(new Vector2(-7.0f, 7.0f));
        vertices.Add(new Vector2(-7.0f, -7.0f));
        vertices.Add(new Vector2(7.0f, -7.0f));
        return vertices;
    }
}
