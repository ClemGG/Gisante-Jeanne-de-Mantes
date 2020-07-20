using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedTextPointInteret : LocalizedText
{

    public string tagPointInteret;

    public void ChangeDescrptionTitle(int index)
    {
        key = string.Format(tagPointInteret, (index + 1).ToString());
        ChangeText();
    }
}
