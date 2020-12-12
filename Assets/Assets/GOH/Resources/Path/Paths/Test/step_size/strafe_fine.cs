using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class strafe_fine : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i <= 16; i++)
            path.Add(new Pip(new Vector2(3.01f - (float)i / 8, -1), Mathf.PI / 2, (float)i / 4));
        return path;
    }
}
