using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggerDetecteur : MonoBehaviour
{

    //public float radius;
    public Transform target;

    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("TriggerDialogue"))
        {
            //DialogueSystem.instance.RepliqueSuivante();
            PanelHistoireButtons.instance.RepliqueSuivante();
        }
    }

    //private void Update()
    //{
    //    transform.LookAt(target);
    //}


    //#if UNITY_EDITOR

    //    private void OnDrawGizmos()
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawWireSphere(transform.parent.position, radius);
    //    }

    //#endif
}
