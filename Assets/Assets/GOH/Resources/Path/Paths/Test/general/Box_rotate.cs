using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_rotate : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (float i = 0; i <= 15; i += 1f)
            path.Add(new Pip(new Vector2(0, -2), Mathf.PI / 16 * i - 5 * Mathf.PI / 128, i));
        return path;
    }
}