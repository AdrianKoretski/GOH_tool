using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class strafe_coarse : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i <= 4; i++)
            path.Add(new Pip(new Vector2(3.01f - (float)i / 2, -1), Mathf.PI / 2, i));
        return path;
    }
}
