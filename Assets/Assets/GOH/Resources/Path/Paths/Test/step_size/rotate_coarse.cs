using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_coarse : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i <= 5; i++)
            path.Add(new Pip(new Vector2(0, -2), Mathf.PI / 16 * i, i));
        return path;
    }
}
