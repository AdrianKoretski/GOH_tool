using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walk_fine : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i <= 20; i++)
            path.Add(new Pip(new Vector2(0, -6 + (float)i / 4), Mathf.PI / 2, (float)i / 4));
        return path;
    }
}
