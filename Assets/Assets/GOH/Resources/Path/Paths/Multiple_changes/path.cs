using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class path : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        path.Add(new Pip(new Vector2(-1, 0), 0 * Mathf.PI / 2, 1));
        path.Add(new Pip(new Vector2(3, 0), 0 * Mathf.PI / 2, 2));
        return path;
    }
}
