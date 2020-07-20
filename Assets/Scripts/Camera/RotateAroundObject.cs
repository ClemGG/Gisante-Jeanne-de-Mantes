using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundObject : MonoBehaviour
{
    public GameObject cible;
    public float speed = 50f;

    Transform t;


    // Start is called before the first frame update
    void Start()
    {
        t = transform;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (cible)
    //    {
    //        t.RotateAround(cible.transform.position, Vector3.up, speed * Time.deltaTime);
    //    }
    //}


    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("TriggerDialogue"))
        {
            DialogueSystem.instance.RepliqueSuivante();
        }
    }
}
