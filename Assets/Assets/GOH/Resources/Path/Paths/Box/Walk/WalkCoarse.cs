using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WalkCoarse : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i <= 7; i++)
            path.Add(new Pip(new Vector2(-3, -7 + i), (float)(Math.PI / 2), i));
        return path;
    }
}