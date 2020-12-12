using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walk_coarse : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i < 6; i++)
            path.Add(new Pip(new Vector2(0, -6 + i), Mathf.PI / 2, i));
        return path;
    }
}
