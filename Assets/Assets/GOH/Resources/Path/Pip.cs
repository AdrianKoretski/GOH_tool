using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pip
{
    public Vector2 position;
    public float orientation;
    public float timestamp;

    public Pip(Vector2 position, float orientation, float timestamp)
    {
        this.position = position;
        this.orientation = orientation;
        this.timestamp = timestamp;
    }

    public Pip(Pip pip)
    {
        this.position = pip.position;
        this.orientation = pip.orientation;
        this.timestamp = pip.timestamp;
    }
}
