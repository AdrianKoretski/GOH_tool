using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plus_Circumference : MonoBehaviour, Path
{
    public List<Pip> getPath()
    {
        List<Pip> path = new List<Pip>();
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(1, 6.0f - i * 0.1f), 3 * Mathf.PI / 2, i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(1, 1), 3 * Mathf.PI / 2 + i * Mathf.PI / 100, 5 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(1 + i * 0.1f, 1), 2 * Mathf.PI, 10 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(6, 1), 2 * Mathf.PI - i * Mathf.PI / 100, 15 + i * 0.1f));
        for (int i = 0; i < 20; i++)
            path.Add(new Pip(new Vector2(6, 1 - i * 0.1f), 3 * Mathf.PI / 2, 20 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(6, -1), 3 * Mathf.PI / 2 - i * Mathf.PI / 100, 22 + i * 0.1f));

        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(6.0f - i * 0.1f, -1), Mathf.PI, 27 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(1, -1), Mathf.PI + i * Mathf.PI / 100, 32 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(1, -1 - i * 0.1f), 3 * Mathf.PI / 2, 37 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(1, -6), 3 * Mathf.PI / 2 - i * Mathf.PI / 100, 42 + i * 0.1f));
        for (int i = 0; i < 20; i++)
            path.Add(new Pip(new Vector2(1 - i * 0.1f, -6), Mathf.PI, 47 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(-1, -6), Mathf.PI - i * Mathf.PI / 100, 49 + i * 0.1f));

        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(-1, -6.0f + i * 0.1f), Mathf.PI / 2, 54 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(-1, -1), Mathf.PI / 2 + i * Mathf.PI / 100, 59 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(-1 - i * 0.1f, -1), Mathf.PI, 64 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(-6, -1), Mathf.PI - i * Mathf.PI / 100, 69 + i * 0.1f));
        for (int i = 0; i < 20; i++)
            path.Add(new Pip(new Vector2(-6, -1 + i * 0.1f), Mathf.PI / 2, 74 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(-6, 1), Mathf.PI / 2 - i * Mathf.PI / 100, 76 + i * 0.1f));

        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(-6.0f + i * 0.1f, 1), 0, 81 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(-1, 1), 0 + i * Mathf.PI / 100, 86 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(-1, 1 + i * 0.1f), Mathf.PI / 2, 91 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(-1, 6), Mathf.PI / 2 - i * Mathf.PI / 100, 96 + i * 0.1f));
        for (int i = 0; i < 20; i++)
            path.Add(new Pip(new Vector2(-1 + i * 0.1f, 6), 0, 101 + i * 0.1f));
        for (int i = 0; i < 50; i++)
            path.Add(new Pip(new Vector2(1, 6), 0 - i * Mathf.PI / 100, 103 + i * 0.1f));

        return path;
    }
}
