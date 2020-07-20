using UnityEngine;

[System.Serializable]
public class LocalizationData {

    public LocalizationItem[] items;

}



[System.Serializable]
public class LocalizationItem
{
    public string key;
    [TextArea(3,10)] 
    public string value;

}
