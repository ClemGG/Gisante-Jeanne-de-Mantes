using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

#if UNITY_STANDALONE_WIN
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#endif

    }

}
