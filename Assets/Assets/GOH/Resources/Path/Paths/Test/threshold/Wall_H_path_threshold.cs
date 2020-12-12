using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_H_path_threshold : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i < 2; i++)
            path.Add(new Pip(new Vector2(i - 1.375f, 0), Mathf.PI / 2, i));
        return path;
    }
}
