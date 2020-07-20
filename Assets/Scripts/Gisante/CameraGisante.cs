using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraGisante : MonoBehaviour
{

    #region Variables


    [Space(10)]
    [Header("Components : ")]
    [Space(10)]

    public bool exportTablette = true;
    public bool isFocused = false, isResetting = false;

    public Transform gisante, startTransform;


    [Space(20)]

    public PointInteret[] gisantesTriggers; //La liste de tous les triggers sur laquelle la caméra doit se centrer



    [Space(10)]
    [Header("Zoom : ")]
    [Space(10)]

    public Vector2 distancesMinEtMax = new Vector2(2f, 5f);
    public float zoomSpeedSouris = 2f, zoomSpeedTablette = .5f, zoomSmoothTime = .05f;
    float zoomSpeed;
    public AnimationCurve zoomCurve;

    float zoomDst;
    float zoomDrag = 0f;
    float zoomSmoothVelocity;







    [Space(10)]
    [Header("Camera rotation : ")]
    [Space(10)]

    public bool invertXAxis;
    public bool invertYAxis;

    public Vector2 clampAnglesPitch = new Vector2(0f, 45f); //Angle minimal de rotaion de la caméra sur l'axe X
    public float cameraSensitivitySouris = 5f, cameraSensitivityTablette = 100f;
    float cameraSensitivity;

    public float rotationSmoothTime = .05f;
    public AnimationCurve rotCurve;


    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    float yaw = 0f, pitch = 0f; //Yaw : Axe Y de rotation. Pitch : Axe X de rotation




    [Space(10)]
    [Header("Camera movement : ")]
    [Space(10)]

    public float moveSpeed = 1.5f;
    public AnimationCurve moveCurve;
    float moveTimer = 0f;




    [Space(10)]
    [Header("Camera focus : ")]
    [Space(10)]


    public AnimationCurve focusCurve;
    public float focusMoveSpeed = .5f, focusRotSpeed = .1f;
    PosRot unFocusedPos, startPos;
    float focusTimer = 0f;


    [Space(10)]
    [Header("Input touch : ")]
    [Space(10)]

    public bool checkDistance = true;
    public float maxTimeWait = .5f;
    public float maxDoubleTapPosition = 1f;

    float doubleTapTimer = 0f;
    int doubleTapCount = 0;
    Touch lastTouch;




    [System.Serializable]
    public struct PosRot
    {
        public Vector3 pos, euler;
        public Quaternion rot;
        public float zoom;
    }





    public static CameraGisante instance;
    Transform t, cible;




    #endregion



#if UNITY_EDITOR

    private void OnValidate()
    {
        zoomSpeed = exportTablette ? zoomSpeedTablette : zoomSpeedSouris;
        cameraSensitivity = exportTablette ? cameraSensitivityTablette : cameraSensitivitySouris;
    }

#endif




    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }

        instance = this;


    }







    private void OnEnable()
    {

        t = transform;
        cible = gisante;

        startPos.pos = t.position = startTransform.position;
        startPos.euler = t.eulerAngles = startTransform.eulerAngles;
        startPos.zoom = distancesMinEtMax.y;

        zoomDrag = zoomDst = distancesMinEtMax.y;


        yaw = t.eulerAngles.y;
        pitch = t.eulerAngles.x;
        Vector3 targetRot = new Vector3(pitch, yaw);
        currentRotation = targetRot;
        t.eulerAngles = currentRotation;

        Application.targetFrameRate = 60;

        unFocusedPos = startPos;
        cible = gisante;
        moveTimer = 0f;
        focusTimer = 0f;
        //ChangerCible();


        zoomSpeed = exportTablette ? zoomSpeedTablette : zoomSpeedSouris;
        cameraSensitivity = exportTablette ? cameraSensitivityTablette : cameraSensitivitySouris;

    }


    //private void OnGUI()
    //{
    //    GUI.Label(new Rect(0, 0, 500, 50), zoomSpeed.ToString());
    //    GUI.Label(new Rect(0, 50, 500, 50), cameraSensitivity.ToString());
    //    GUI.Label(new Rect(0, 100, 500, 50), focusMoveSpeed.ToString());
    //    GUI.Label(new Rect(0, 150, 500, 50), focusRotSpeed.ToString());
    //    GUI.Label(new Rect(0, 200, 500, 50), Time.timeScale.ToString());
    //}








    private void Update()
    {

        //if (!PanelManager.instance.PanelReconstructionActif)
        //    return;



        //if (isFocused)
        //{
        //    if (!isResetting)
        //    {
        //        FocusCamera();
        //    }
        //    else
        //    {
        //        ResetPositionAndRotation();
        //    }

        //}
        if(!isFocused && !isResetting)//else
        {
            Rotate();



#if UNITY_STANDALONE_WIN
            if (IsDoubleTap()) //Ramène la caméra au point de départ fixé
            {
                unFocusedPos = startPos;
                ChangerCible();
            }
#endif


#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(1) && Input.touches.Length == 0) //Ramène la caméra au point de départ fixé
            {
                unFocusedPos = startPos;
                ChangerCible();
            }
#endif



        }
            Zoom();

    }




    private bool ShouldMirror()
    {
        return t.position.x < gisante.position.x;
    }


#if UNITY_STANDALONE_WIN
    private bool IsDoubleTap()
    {
        bool result = false;


        if (Input.touches.Length == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            float deltaPositionLength = Input.GetTouch(0).position.magnitude - lastTouch.position.magnitude;
            lastTouch = Input.GetTouch(0);
            //print(deltaPositionLength);


            if ((!checkDistance || deltaPositionLength < maxDoubleTapPosition) && doubleTapTimer < maxTimeWait)
            {
                doubleTapCount++;
            }

            if (doubleTapCount == 2)
            {
                result = true;
            }
        }

        if (doubleTapTimer > maxTimeWait || doubleTapCount == 2)
        {
            doubleTapTimer = 0f;
            doubleTapCount = 0;
        }
        else
        {
            if (doubleTapCount == 1)
            {
                doubleTapTimer += Time.unscaledDeltaTime;
            }
        }

        //print(result);
        return result;
    }
#endif





    public IEnumerator FocusCameraOnPointInteret()
    { 
        
        while (focusTimer < 1f)
        {
            focusTimer += Time.unscaledDeltaTime * focusMoveSpeed;


            t.eulerAngles = new Vector3(
            Mathf.LerpAngle(t.eulerAngles.x, cible.eulerAngles.x, rotCurve.Evaluate(focusTimer * focusRotSpeed)),
            Mathf.LerpAngle(t.eulerAngles.y, cible.eulerAngles.y, rotCurve.Evaluate(focusTimer * focusRotSpeed)),
            Mathf.LerpAngle(t.eulerAngles.z, cible.eulerAngles.z, rotCurve.Evaluate(focusTimer * focusRotSpeed)));

            yield return null;
        }


    }


    public IEnumerator ResetCamPos()
    {
        isFocused = false;


        while (focusTimer < 1f)
        {
            focusTimer += Time.unscaledDeltaTime * focusMoveSpeed;

            Vector3 v = t.eulerAngles;

            v = new Vector3(
            Mathf.LerpAngle(v.x, unFocusedPos.euler.x, rotCurve.Evaluate(focusTimer * focusRotSpeed)),
            Mathf.LerpAngle(v.y, unFocusedPos.euler.y, rotCurve.Evaluate(focusTimer * focusRotSpeed)),
            Mathf.LerpAngle(v.z, unFocusedPos.euler.z, rotCurve.Evaluate(focusTimer * focusRotSpeed)));

            //t.eulerAngles = v;


            yaw = v.y;
            pitch = v.x;
            Vector3 targetRot = new Vector3(pitch, yaw);
            currentRotation = targetRot;
            t.eulerAngles = currentRotation;




            yield return null;
        }



        isResetting = false;


    }










    //private void FocusCamera()
    //{


    //    if (focusTimer < 1f)
    //    {
    //        focusTimer += Time.unscaledDeltaTime * focusMoveSpeed;
    //    }

    //    t.eulerAngles = new Vector3(
    //    Mathf.LerpAngle(t.eulerAngles.x, cible.eulerAngles.x, rotCurve.Evaluate(focusTimer * focusRotSpeed)),
    //    Mathf.LerpAngle(t.eulerAngles.y, cible.eulerAngles.y, rotCurve.Evaluate(focusTimer * focusRotSpeed)),
    //    Mathf.LerpAngle(t.eulerAngles.z, cible.eulerAngles.z, rotCurve.Evaluate(focusTimer * focusRotSpeed)));

    //    zoomDrag = 1f;

    //}







    //private void ResetPositionAndRotation()
    //{



    //    if (focusTimer < 1f)
    //    {
    //        focusTimer += Time.unscaledDeltaTime * focusMoveSpeed;
    //    }
    //    else
    //    {

    //        yaw = t.eulerAngles.y;
    //        pitch = t.eulerAngles.x;
    //        Vector3 targetRot = new Vector3(pitch, yaw);
    //        currentRotation = targetRot;
    //        t.eulerAngles = currentRotation;

    //        isResetting = false;
    //        isFocused = false;
    //    }

    //    Vector3 v = t.eulerAngles;

    //    v = new Vector3(
    //    Mathf.LerpAngle(v.x, unFocusedPos.euler.x, rotCurve.Evaluate(focusTimer * focusRotSpeed)),
    //    Mathf.LerpAngle(v.y, unFocusedPos.euler.y, rotCurve.Evaluate(focusTimer * focusRotSpeed)),
    //    Mathf.LerpAngle(v.z, unFocusedPos.euler.z, rotCurve.Evaluate(focusTimer * focusRotSpeed)));

    //    t.eulerAngles = v;



    //    zoomDrag = unFocusedPos.zoom;

    //}









    private void LateUpdate()
    {
        UpdateCameraPosition();
    }



    private void UpdateCameraPosition()
    {
        moveTimer += Time.unscaledDeltaTime * moveSpeed;
        Vector3 nouvellePos = cible.position - t.forward * zoomDst;


        t.position = Vector3.Lerp(t.position, nouvellePos, isFocused ? focusCurve.Evaluate(focusTimer) : moveCurve.Evaluate(moveTimer));
    }







    private void Rotate()
    {


#if UNITY_STANDALONE_WIN
        if (!isResetting && Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 drag = Vector3.zero;
                drag.x = touch.deltaPosition.x / Screen.width;
                drag.y = touch.deltaPosition.y / Screen.height;
                drag.x = Mathf.Clamp(drag.x, -1f, 1f);
                drag.y = Mathf.Clamp(drag.y, -1f, 1f);


                yaw += drag.x * (invertXAxis ? -1 : 1) * cameraSensitivity;
                pitch += drag.y * cameraSensitivity;
                pitch = Mathf.Clamp(pitch, clampAnglesPitch.x, clampAnglesPitch.y);

            }

        }
#endif

#if UNITY_EDITOR

        //Ne fonctionne pas si les BuildSettings n'ont pas comme platforme le PC
        if (!isResetting)
        {
            yaw += Input.GetAxis("Mouse X") * (invertXAxis ? -1 : 1) * cameraSensitivity;
            pitch += Input.GetAxis("Mouse Y") * (invertYAxis ? -1 : 1) * cameraSensitivity;
            pitch = Mathf.Clamp(pitch, clampAnglesPitch.x, clampAnglesPitch.y);
        }
#endif


        Vector3 targetRot = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRot, ref rotationSmoothVelocity, rotationSmoothTime);
        t.eulerAngles = currentRotation;


    }






    private void Zoom()
    {


#if UNITY_STANDALONE_WIN
        if (!isFocused)
        {
            if (Input.touchCount == 2) {

                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                if (touchZero.phase == TouchPhase.Moved && touchOne.phase == TouchPhase.Moved)
                {

                    // Find the position in the previous frame of each touch.
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    // Find the difference in the distances between each frame.
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    zoomDrag += deltaMagnitudeDiff * zoomSpeed;
                    zoomDrag = Mathf.Clamp(zoomDrag, distancesMinEtMax.x, distancesMinEtMax.y);

                }

            }
        }
#endif


#if UNITY_EDITOR

        if (!isFocused)
        {
            zoomDrag -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            zoomDrag = Mathf.Clamp(zoomDrag, distancesMinEtMax.x, distancesMinEtMax.y);
        }
#endif
        zoomDst = Mathf.SmoothDamp(zoomDst, zoomDrag, ref zoomSmoothVelocity, zoomSmoothTime);


    }












    public void ChangerCible(int index)
    {
        cible = null;
        while (!cible)
        {
            if (gisantesTriggers[index].ID == index)
            {
                if (gisantesTriggers[index].canBeMirrored)
                {
                    cible = gisantesTriggers[index].transform.GetChild(ShouldMirror() ? 1 : 0);
                }
                else
                {
                    cible = gisantesTriggers[index].transform.GetChild(0);

                }
            }
        }



        unFocusedPos.pos = t.localPosition;
        unFocusedPos.euler = t.eulerAngles;
        unFocusedPos.zoom = zoomDrag;

        isFocused = true;
        moveTimer = 0f;
        focusTimer = 0f;
        zoomDrag = 1f;

        StopAllCoroutines();
        StartCoroutine(FocusCameraOnPointInteret());

    }

    public void ChangerCible()
    {
        cible = gisante;
        isResetting = true;
        //isFocused = true;
        moveTimer = 0f;
        focusTimer = 0f;
        zoomDrag = unFocusedPos.zoom;

        StopAllCoroutines();
        StartCoroutine(ResetCamPos());
    }
}
