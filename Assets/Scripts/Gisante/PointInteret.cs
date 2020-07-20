using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class PointInteret : MonoBehaviour
{
    public int ID;
    public bool canBeMirrored = false;

    [Space(20)]

    [SerializeField] private PointInteretButton linkedButton;

    private CanvasGroup linkedButtonAlpha;
    public float fadeSpeed = 2f;

    public Camera reconstructionCam;
    public bool isVisible = true;
    public bool modeBrut = true;

    Transform t;
    bool onStart = true;


    // Start is called before the first frame update
    void Awake()
    {
        t = transform;
        linkedButtonAlpha = linkedButton.GetComponent<CanvasGroup>();

    }

    private void OnEnable()
    {

        StartCoroutine(ShowButtonUI());


    }

    private void OnDisable()
    {
        StopCoroutine(ShowButtonUI());
    }

    private IEnumerator ShowButtonUI()
    {

        float t = linkedButtonAlpha.alpha;

        while (true)
        {
            t += Time.deltaTime * (isVisible ? 1f : -1f) * fadeSpeed;
            t = Mathf.Clamp(t, 0f, 1f);
            linkedButtonAlpha.alpha = t;
            yield return null;
        }
        
    }

    private void FixedUpdate()
{
        RaycastHit hit;
        Ray ray = reconstructionCam.ScreenPointToRay(reconstructionCam.WorldToScreenPoint(t.position));



        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            isVisible = objectHit == t && !CameraGisante.instance.isFocused && modeBrut == PanelGisanteManager.instance.modeBrut && PanelGisanteManager.instance.transitionIsFinished;

        }

        linkedButtonAlpha.interactable = isVisible;

    }


#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (!t)
            t = transform;


            Gizmos.DrawSphere(t.position, .1f);
            Handles.Label(t.position, gameObject.name);
    }



#endif
}
