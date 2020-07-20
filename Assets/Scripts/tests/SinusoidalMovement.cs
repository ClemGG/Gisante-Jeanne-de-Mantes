using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinusoidalMovement : MonoBehaviour
{
    public bool useX, useY, useZ;
    public float speed;

    Vector3 _startPosition;
    Transform t;
    void Start()
    {
        t = transform;
        _startPosition = t.position;
    }

    void Update()
    {
        t.position = _startPosition + new Vector3(useX ? Mathf.Sin(Time.time) : 0f, useY ? Mathf.Sin(Time.time) : 0f, useZ ? Mathf.Sin(Time.time) : 0f) * speed;
    }
}
