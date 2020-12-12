using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WalkFine : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i <= 175; i++)
            path.Add(new Pip(new Vector2(-3, -7 + (float)(i) / 25), (float)(Math.PI / 2), (float)(i) / 25));
        return path;
    }
}