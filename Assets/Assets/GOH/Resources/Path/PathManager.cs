using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager
{
    public List<Pip> GeneratePath()
    {
        GameObject[] paths = GameObject.FindGameObjectsWithTag("Path");
        if (paths.Length == 0)
        {
            MonoBehaviour.print("ERROR: no paths found.");
            return null;
        }
        if (paths.Length > 1)
            MonoBehaviour.print("Warning: multiple paths found.");
        return paths[0].GetComponent<Path>().getPath();
    }
}