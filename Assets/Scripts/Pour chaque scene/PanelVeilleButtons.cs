using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelVeilleButtons : MonoBehaviour
{
    public GameObject panelCredits, panelVeille;
    public Transform buttonsRight;

    [Space(10)]

    public Color selectedBtnColor, normalBtnColor;




    [Space(10)]
    [Header("Analytics : ")]
    [Space(10)]

    public float delayBeforeReturnToVeille = 300f;
    float timeSpentOnAccueil;
    float returnTimer;
    bool isVeille = true;



    private void Start()
    {
        ChangerLangue(PlayerPrefs.GetString("langue", "fr"));
        panelCredits.SetActive(false);
    }


    private void Update()
    {

        if (!panelCredits.activeSelf)
        {
            timeSpentOnAccueil += Time.deltaTime;

            if(!isVeille)
            returnTimer += Time.deltaTime;
        }


        if(returnTimer > delayBeforeReturnToVeille && !isVeille)
        {
            returnTimer = 0f;
            panelVeille.GetComponent<Animator>().Play("transition vers veille");
            isVeille = true;
        }
    }










    public void ToggleCredits()
    {
        if (SceneFader.isTransitioning)
            return;


        if (!panelCredits.activeSelf)
        {
            panelCredits.SetActive(true);
            Time.timeScale = 0f;
            AnalyticsBtnEvents.OnCreditsButtonPressed();
        }
        else
        {
            Animator a = panelCredits.GetComponent<Animator>();

            if (a)
            {
                if (a.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                    a.Play("hide");
            }
            else
            {
                panelCredits.SetActive(false);
            }

            Time.timeScale = 1f;
        }


    }
    public void ToggleCredits(bool b)
    {

        if (SceneFader.isTransitioning)
            return;


        if (b)
        {
            panelCredits.SetActive(true);
            Time.timeScale = 0f;
            AnalyticsBtnEvents.OnCreditsButtonPressed();
        }
        else
        {
            Animator a = panelCredits.GetComponent<Animator>();

            if (a)
            {
                if (a.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                    a.Play("hide");
            }
            else
            {
                panelCredits.SetActive(false);
            }

            Time.timeScale = 1f;
        }
    }




    public void ToggleAccueil(bool versAccueil)
    {
        Animator a = panelVeille.GetComponent<Animator>();


        if (a)
        {

            if (versAccueil)
            {
                a.Play("transition vers accueil");
                isVeille = false;
            }
            else
            {
                a.Play("transition vers veille");

            }
            //if (a.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            //{

            //    if (versAccueil)
            //    {
            //        a.Play("transition vers accueil");
            //        isVeille = false;
            //    }
            //    else
            //    {
            //        a.Play("transition vers veille");

            //    }
            //}
        }
        else
        {
            Debug.LogError("Erreur : L'animation du panelVeille n'a pas encore été faite");
        }
    }






    public void ChangerLangue(string fileLanguage)
    {
        PlayerPrefs.SetString("langue", fileLanguage);
        ShowLanguageButton(fileLanguage);
        //PlayerPrefs.Save();       //Appelé automatiquement dans OnApplicationQuit, on n'a donc pas besoin de l'appeler nous-même
        LocalizationManager.instance.LoadLocalizedText(fileLanguage);
        print(fileLanguage);

        AnalyticsBtnEvents.OnTranslateButtonPressed(fileLanguage);
    }

    public void ShowLanguageButton(string s)
    {

        foreach (Transform child in buttonsRight.transform)
        {
            if (child.childCount > 0)
            {
                TextMeshProUGUI t = child.GetChild(0).GetComponent<TextMeshProUGUI>();
                if (t)
                    t.color = t.text.ToLower().Equals(s.ToLower()) ? selectedBtnColor : normalBtnColor;
            }
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        Time.timeScale = 1f;

        LocalizationStartupManager.instance.textsToModify = (LocalizedText[])Resources.FindObjectsOfTypeAll(typeof(LocalizedText));
        LocalizationStartupManager.instance.ChangeAllTextsInScene();
        ShowLanguageButton(PlayerPrefs.GetString("langue", "fr"));
    }

}
