using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_walk : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (float i = 0; i <= 3; i += 1f)
            path.Add(new Pip(new Vector2(2.375f + i, 2.375f + i), Mathf.PI / 4, i));
        return path;
    }
    /*public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (float i = 0; i <= 5; i += 1f)
            path.Add(new Pip(new Vector2(1.375f + i, 4), 0 * Mathf.PI / 4, i));
        return path;
    }*/
}