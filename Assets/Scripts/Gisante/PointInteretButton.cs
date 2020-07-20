using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointInteretButton : MonoBehaviour
{
    Transform t;
    public Transform camGisante;
    Camera cam;

    [HideInInspector] public float depth;

    // Start is called before the first frame update
    void Start()
    {
        t = transform;
        if (camGisante)
        {
            cam = camGisante.GetComponent<Camera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (camGisante)
        {
            transform.LookAt(t.position + cam.transform.rotation * Vector3.back, cam.transform.rotation * Vector3.up);

            float distance = (t.position - camGisante.transform.position).magnitude;
            depth = -distance;
        }
    }



}

