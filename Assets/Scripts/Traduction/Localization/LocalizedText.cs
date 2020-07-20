using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour {

    [SerializeField] public string key;


    // Use this for initialization
    public void ChangeText () {

        Text text = GetComponent<Text>();
        TextMeshProUGUI textmesh = GetComponent<TextMeshProUGUI>();

        if(text)
            text.text = LocalizationManager.instance.GetLocalizedData(key);
        else if(textmesh)
            textmesh.text = LocalizationManager.instance.GetLocalizedData(key);

    }

}
