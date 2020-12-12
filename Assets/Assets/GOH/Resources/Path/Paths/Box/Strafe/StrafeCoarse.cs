using System;
using System.Collections.Generic;
using UnityEngine;

public class StrafeCoarse : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i <= 14; i++)
            path.Add(new Pip(new Vector2(i - 7, -3.5f), (float)(Math.PI / 2), i));
        return path;
    }
}
