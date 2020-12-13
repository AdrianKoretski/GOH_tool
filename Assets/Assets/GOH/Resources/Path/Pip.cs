using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pip
{
    public Vector2 position { get; private set; }
    public float orientation { get; private set; }
    public float timestamp { get; private set; }

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

    public void Wiggle(float wiggle_delta)
    {
        Vector2 position = this.position;
        position.x += UnityEngine.Random.Range(-wiggle_delta, wiggle_delta);
        position.y += UnityEngine.Random.Range(-wiggle_delta, wiggle_delta);
        this.position = position;
    }
}
