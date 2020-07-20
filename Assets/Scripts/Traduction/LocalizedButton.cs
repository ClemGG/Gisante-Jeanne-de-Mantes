using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedButton : MonoBehaviour
{
    public Image imgComponent;
    public Sprite greyImg, normalImg;
    public string key;


    // Start is called before the first frame update
    void Start()
    {
        imgComponent.sprite = PlayerPrefs.GetString("langue") == key ? normalImg : greyImg;
    }

}
