using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 1)]
public class Dialogue : ScriptableObject
{
    public Replique[] repliques;
    [HideInInspector] public bool fini = false;


}



[System.Serializable]
public class Replique
{
    public string characterName;
    [TextArea(3, 10)]
    public string text;
    public AudioClip voiceClip, bruitageClip;
    public bool endBruitage = false;

}
