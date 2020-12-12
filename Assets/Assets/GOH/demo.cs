using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demo : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (float i = 0; i < 40; i += 1f)
            path.Add(new Pip(new Vector2(5.001f, 5.02f-i/4), 3 * Mathf.PI / 2, i/10));
        for (float i = 0; i < 10; i += 1f)
            path.Add(new Pip(new Vector2(5.001f, -4.98f), 3 * Mathf.PI / 2 - i * Mathf.PI / 20, i/10 + 4));
        for (float i = 0; i < 20; i += 1f)
            path.Add(new Pip(new Vector2(5.001f - i / 4, -4.98f), Mathf.PI, i/10 + 5));
        for (float i = 0; i < 10; i += 1f)
            path.Add(new Pip(new Vector2(0.001f, -4.98f), Mathf.PI - i * Mathf.PI / 20, i/10 + 7));
        for (float i = 0; i < 40; i += 1f)
            path.Add(new Pip(new Vector2(0.001f, -4.98f + i / 4), Mathf.PI / 2, i/10 + 8));
        for (float i = 0; i < 10; i += 1f)
            path.Add(new Pip(new Vector2(0.001f, 5.02f), Mathf.PI / 2 + i * Mathf.PI / 20, i/10 + 12));
        for (float i = 0; i < 20; i += 1f)
            path.Add(new Pip(new Vector2(0.001f - i / 4, 5.02f), Mathf.PI, i/10 + 13));
        for (float i = 0; i < 10; i += 1f)
            path.Add(new Pip(new Vector2(-4.999f, 5.02f), Mathf.PI + i * Mathf.PI / 20, i/10 + 15));
        for (float i = 0; i < 40; i += 1f)
            path.Add(new Pip(new Vector2(-4.999f, 5.02f - i / 4), 3 * Mathf.PI / 2, i/10 + 16));
        return path;
    }
}
