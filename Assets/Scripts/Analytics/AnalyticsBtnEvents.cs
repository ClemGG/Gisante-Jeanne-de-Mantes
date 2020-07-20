using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;


public static class AnalyticsBtnEvents
{

    #region Accueil

    public static void OnModeLibreButtonPressed()
    {
        GameAnalytics.NewDesignEvent("ui:btnPressed:libre");
    }

    public static void OnModeHistoireButtonPressed()
    {
        GameAnalytics.NewDesignEvent("ui:btnPressed:histoire");
    }

    public static void OnTranslateButtonPressed(string fileLanguage)
    {
        GameAnalytics.NewDesignEvent($"ui:btnPressed:langue:{fileLanguage}");
    }

    public static void OnHomeButtonPressed()
    {
        GameAnalytics.NewDesignEvent("ui:btnPressed:home");
    }

    public static void OnCreditsButtonPressed()
    {
        GameAnalytics.NewDesignEvent("ui:btnPressed:credits");
    }

    public static void OnModeVeilleActivated()
    {
        GameAnalytics.NewDesignEvent("events:activation:veille");
    }
    public static void TimeSpentOnModeAccueil(float timeSpent)
    {
        GameAnalytics.NewDesignEvent("time:timeSpent:accueil", timeSpent);
    }

    public static void TotalTimeSpentOnApp(float timeSpent)
    {
        GameAnalytics.NewDesignEvent("time:timeSpent:total", timeSpent);
    }

    #endregion



    #region Mode Histoire

    public static void OnPlayVideoButtonPressed()
    {
        GameAnalytics.NewDesignEvent("ui:btnPressed:play");
    }

    public static void OnReplayVideoButtonPressed()
    {
        GameAnalytics.NewDesignEvent("ui:btnPressed:replay");
    }

    public static void OnMuteButtonPressed()
    {
        GameAnalytics.NewDesignEvent("ui:btnPressed:mute");
    }

    public static void TimeSpentOnModeHistoire(float timeSpent)
    {
        GameAnalytics.NewDesignEvent("time:timeSpent:histoire", timeSpent);
    }

    public static void TimeSpentOnVideo(float timeSpent)
    {
        GameAnalytics.NewDesignEvent("time:timeSpent:video", timeSpent);
    }

    public static void OnVideoCompleted()
    {
        GameAnalytics.NewDesignEvent("events:completed:video");
    }

    #endregion



    #region Mode Libre

    public static void OnPointInteretButtonPressed()
    {
        GameAnalytics.NewDesignEvent("ui:btnPressed:pointInteret");
    }

    public static void TimeSpentOnModeLibre(float timeSpent)
    {
        GameAnalytics.NewDesignEvent("time:timeSpent:libre", timeSpent);
    }

    #endregion
}
