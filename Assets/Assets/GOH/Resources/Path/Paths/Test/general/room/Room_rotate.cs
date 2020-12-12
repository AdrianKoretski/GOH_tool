﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_rotate : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (float i = 0; i <= 5; i += 1f)
            path.Add(new Pip(new Vector2(3, 3), Mathf.PI / 16 * i - 5 * Mathf.PI / 128, i));
        return path;
    }
}
