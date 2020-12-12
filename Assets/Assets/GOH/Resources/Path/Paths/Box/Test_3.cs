using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Test_3 : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i < 21; i++)
            path.Add(new Pip(new Vector2(-(float)(Math.Cos((i * Math.PI) / 10) * 3.5f), -(float)(Math.Sin((i * Math.PI) / 10) * 3.5f)), (float)((i * Math.PI) / 10), i * 3));
        return path;
    }
}
