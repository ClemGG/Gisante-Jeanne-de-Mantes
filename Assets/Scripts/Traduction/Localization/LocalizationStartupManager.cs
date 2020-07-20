using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationStartupManager : MonoBehaviour {


    [HideInInspector] public LocalizedText[] textsToModify;

    public static LocalizationStartupManager instance;


    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    // Use this for initialization
    public IEnumerator Start () {

        textsToModify = (LocalizedText[])Resources.FindObjectsOfTypeAll(typeof(LocalizedText));

        while (!LocalizationManager.instance.CheckIfReady())
        {
            yield return null;
        }

        //ChangeAllTextsInScene();
    }

    public void ChangeAllTextsInScene()
    {
        for (int i = 0; i < textsToModify.Length; i++)
        {
            textsToModify[i].ChangeText();
        }
    }
}
