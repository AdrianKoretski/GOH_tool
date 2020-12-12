using System;
using System.Collections.Generic;
using UnityEngine;

public class StrafeMedium : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i <= 28; i++)
            path.Add(new Pip(new Vector2((float)(i) / 2 - 7, -3.5f), (float)(Math.PI / 2), (float)(i) / 2));
        return path;
    }
}