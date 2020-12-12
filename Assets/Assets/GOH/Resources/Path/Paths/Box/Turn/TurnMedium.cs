using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnMedium : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i <= 20; i++)
            path.Add(new Pip(new Vector2(0, -3.5f), (float)(i * Math.PI / 2) / 10, (float)(i) ));
        return path;
    }
}