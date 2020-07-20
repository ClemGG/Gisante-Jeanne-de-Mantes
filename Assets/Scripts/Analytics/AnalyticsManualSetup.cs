using GameAnalyticsSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManualSetup : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GameAnalytics.Initialize();
        GameAnalytics.SetCustomId("tataragne");

        DontDestroyOnLoad(gameObject);
    }


}
