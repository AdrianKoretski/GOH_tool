using System;
using System.Collections.Generic;
using UnityEngine;

public class StrafeFine : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i <= 56; i++)
            path.Add(new Pip(new Vector2((float)(i) / 4 - 7, -3.5f), (float)(Math.PI / 2), (float)(i) / 4));
        return path;
    }
}