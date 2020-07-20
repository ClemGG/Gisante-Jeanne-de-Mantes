using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class PanelManager : MonoBehaviour
{
    [Space(10)]

    public GameObject panelAccueil, panelReconstruction, panelBrut, panelHistoire, panelCredits, buttonsLeft, buttonsRight;
    [Space(10)]
    public GameObject camAccueil, camBrut, camHistoire;
    [Space(10)]
    public float delayBeforeActivationFadeImg = 3f;

    GameObject currentPanel;
    GameObject currentCam;
    Coroutine co;

    public bool PanelReconstructionActif { get => camBrut.activeSelf; }
    [HideInInspector] public bool isTransitioning = false; //Passé à true puis à false avant et aprè sun fondu en noir

    [Space(20)]

    public float delayBeforeReturnToMenu = 10f;
    float returnTimer;
    public Color normalBtnColor, selectedBtnColor;


    [Space(10)]
    [Header("Analytics : ")]
    [Space(10)]

    float timeSpentOnAccueil;
    float timeSpentOnLibre;
    float timeSpentOnHistoire;


    public static PanelManager instance;


    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }

        instance = this;


        if (!PlayerPrefs.HasKey("langue"))
        {
            PlayerPrefs.SetString("langue", "fr");
        }
    }






    // Start is called before the first frame update
    void Start()
    {
        panelCredits.SetActive(false);
        panelBrut.SetActive(false);

        camBrut.SetActive(false);
        camHistoire.SetActive(false);

        if (co == null)
            co = StartCoroutine(SceneFader.instance.ActiverNouveauPanel(null, panelAccueil, delayBeforeActivationFadeImg, true, null, camAccueil, 0));

        currentPanel = panelAccueil;
        currentCam = camAccueil;

        LocalizationManager.instance.LoadLocalizedText(PlayerPrefs.GetString("langue", "fr"));
        ShowNavigationButton(0);
        ShowLanguageButton(PlayerPrefs.GetString("langue", "fr"));

    }

    public void ShowNavigationButton(int index)
    {

        foreach (Transform child in buttonsLeft.transform)
        {
            Button b = child.GetComponent<Button>();
            Image i = child.GetChild(1).GetComponent<Image>();
            Text t = child.GetChild(0).GetComponent<Text>();


            b.interactable = child.GetSiblingIndex() != index;
            i.color = child.GetSiblingIndex() == index ? selectedBtnColor : normalBtnColor;
            t.color = child.GetSiblingIndex() == index ? selectedBtnColor : normalBtnColor;
        }
    }

    public void ShowLanguageButton(string s)
    {

        foreach (Transform child in buttonsRight.transform)
        {
            if (child.childCount > 0)
            {
                TextMeshProUGUI t = child.GetChild(0).GetComponent<TextMeshProUGUI>();
                if(t)
                    t.color = t.text.ToLower().Equals(s.ToLower()) ? selectedBtnColor : normalBtnColor;
            }
        }
    }

    public void MakeNavigationsBtnsInteractable()
    {
        Button[] btns = buttonsLeft.transform.GetComponentsInChildren<Button>();

        for (int i = 0; i < btns.Length; i++)
        {
            btns[i].interactable = false;
        }
    }

    private void Update()
    {
        if(Input.touchCount == 0 && !DialogueSystem.instance.isPlaying && !Input.GetMouseButtonDown(0) && !panelAccueil.activeSelf)
        {
            if(returnTimer < delayBeforeReturnToMenu)
            {
                returnTimer += Time.deltaTime;
            }
            else
            {
                AnalyticsBtnEvents.OnModeVeilleActivated();
                RetourAccueil();
            }
        }
        else
        {
            returnTimer = 0f;
        }

        if (panelAccueil.activeSelf && !panelCredits.activeSelf)
        {
            timeSpentOnAccueil += Time.deltaTime;
        }
        else if (panelReconstruction.activeSelf && !panelCredits.activeSelf)
        {
            timeSpentOnLibre += Time.deltaTime;
        }
        else if (panelHistoire.activeSelf && !panelCredits.activeSelf)
        {
            timeSpentOnHistoire += Time.deltaTime;
        }
    }









    public void ToggleCredits()
    {
        if (isTransitioning)
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
                if(a.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
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

        if (isTransitioning)
            return;


        if (b)
        {
            panelCredits.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Animator a = panelCredits.GetComponent<Animator>();

            if (a)
            {
                if(a.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                    a.Play("hide");
            }
            else
            {
                panelCredits.SetActive(false);
            }

            Time.timeScale = 1f;
        }
    }



    public void RetourAccueil()
    {
        SceneFader.instance.FadeToScene(0);

        if (panelCredits.activeSelf)
        {
            ToggleCredits(false);
        }

        AnalyticsBtnEvents.OnHomeButtonPressed();

        if (panelReconstruction.activeSelf)
        {
            AnalyticsBtnEvents.TimeSpentOnModeLibre(timeSpentOnLibre);
        }
        else if (panelHistoire.activeSelf)
        {
            if(DialogueSystem.instance.timeline.time < DialogueSystem.instance.timeline.duration) //Si la video n'a pas été finie, alors on enregistre le temps écoulé sur la vidéo
                 AnalyticsBtnEvents.TimeSpentOnVideo((float)DialogueSystem.instance.timeline.time);
        }
    }

    public void ChangerLangue(string fileLanguage)
    {
        if (PlayerPrefs.GetString("langue") != fileLanguage)
        {
            PlayerPrefs.SetString("langue", fileLanguage);
            ShowLanguageButton(fileLanguage);
            //PlayerPrefs.Save();       //Décommenter cette ligne si l'on ne veut plus que la langue revienne au français au prochain lancement de l'application
            RetourAccueil();

            AnalyticsBtnEvents.OnTranslateButtonPressed(fileLanguage);
        }
    }


    private void OnLevelWasLoaded(int level)
    {
        Time.timeScale = 1f;

        LocalizationStartupManager.instance.textsToModify = (LocalizedText[])Resources.FindObjectsOfTypeAll(typeof(LocalizedText));
        ShowLanguageButton(PlayerPrefs.GetString("langue", "fr"));
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("langue");
        AnalyticsBtnEvents.TotalTimeSpentOnApp(Time.time);
    }

    public void ModeLibre()
    {

        StopCoroutine(co);
        SceneFader.instance.StopAllCoroutines();

        co = StartCoroutine(SceneFader.instance.ActiverNouveauPanel(currentPanel, panelReconstruction, delayBeforeActivationFadeImg, false, currentCam, camBrut, 1));
        currentPanel = panelReconstruction;
        currentCam = camBrut;

        if (panelCredits.activeSelf)
        {
            ToggleCredits(false);
        }


        AnalyticsBtnEvents.OnModeLibreButtonPressed();
        AnalyticsBtnEvents.TimeSpentOnModeAccueil(timeSpentOnAccueil);

    }
    public void ModeHistoire()
    {

        StopCoroutine(co);
        SceneFader.instance.StopAllCoroutines();
        panelBrut.SetActive(false);

        co = StartCoroutine(SceneFader.instance.ActiverNouveauPanel(currentPanel, panelHistoire, delayBeforeActivationFadeImg, false, currentCam, camHistoire, 2));

        currentPanel = panelHistoire;
        currentCam = camHistoire;

        if (panelCredits.activeSelf)
        {
            ToggleCredits(false);
        }


        AnalyticsBtnEvents.OnModeHistoireButtonPressed();
        AnalyticsBtnEvents.TimeSpentOnModeAccueil(timeSpentOnAccueil);

    }




    public void ResetModes()
    {
        PanelGisanteManager.instance.ResetModeLibre();
        DialogueSystem.instance.StopDialogue();
    }


    public void ShowDescriptionPointInteret(int index)
    {
        panelBrut.SetActive(true);
        PanelGisanteManager.instance.ActiverDescription(index);
        CameraGisante.instance.ChangerCible(index);

        AnalyticsBtnEvents.OnPointInteretButtonPressed();

    }
    public void HideDescriptionPointInteret()
    {
        Animator a = panelBrut.GetComponent<Animator>();

        if (a)
        {
            a.Play("hide");
        }
        else
        {
            panelBrut.SetActive(false);
        }

        CameraGisante.instance.ChangerCible();

    }


}
