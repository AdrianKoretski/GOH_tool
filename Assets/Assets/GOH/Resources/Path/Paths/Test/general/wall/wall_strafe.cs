﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wall_strafe : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (float i = 0; i <= 5; i += 1f)
            path.Add(new Pip(new Vector2(0, -3.375f + i), 0, i));
        return path;
    }
}
