using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnCoarse : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i <= 5; i++)
            path.Add(new Pip(new Vector2(0, -3.5f), (float)(i * Math.PI / 2) / 5, (float)(i) / 0.5f));
        return path;
    }
}