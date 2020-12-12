using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WalkMedium : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i <= 35; i++)
            path.Add(new Pip(new Vector2(-3, -7 + (float)(i) / 5), (float)(Math.PI / 2), (float)(i) / 5));
        return path;
    }
}