using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelGisanteManager : MonoBehaviour
{
    public Animator uiAnim, panelBrut;
    public Transform gisanteBrutModel, gisanteReconstructionModel;
    public Button btnBrut, btnRes;
    public CanvasGroup CanvasGroupBtnBrut, CanvasGroupBtnRes;
    public Image fadeImg;

    [Space(10)]
    public Scrollbar scrollBarDescription;
    public ScrollRect scrollRectDescription;
    [Space(10)]
    public AnimationCurve scaleCurve;
    public bool modeBrut = true;
    public float fadeSpeed = 2f;
    [HideInInspector] public bool transitionIsFinished = false;


    [Space(10)]

    public LocalizedTextPointInteret titrePointInteret;

    [Space(10)]

    public List<PointInteretButton> buttonsBrut;
    public List<PointInteretButton> buttonsReconstruction;

    [Space(20)]

    public GameObject[] descriptions;


    public static PanelGisanteManager instance;




    [Space(10)]
    [Header("Analytics : ")]
    [Space(10)]

    public float delayBeforeReturnToVeille = 300f;
    float timeSpentOnLibre;
    float returnTimer;






    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }

        instance = this;

    }





    private void Start()
    {

        ChangerLangue(PlayerPrefs.GetString("langue", "fr"));
        panelCredits.SetActive(false);

        //modeBrut = !modeBrut;
        gisanteBrutModel.localScale = Vector3.one;
        //gisanteBrutModel.localScale = modeBrut ? Vector3.one : Vector3.zero;
        //gisanteReconstructionModel.localScale = modeBrut ? Vector3.zero : Vector3.one;
        //ChangerModeGisante(); 
        //SetModeBrutAtStart();
        transitionIsFinished = true;
        fadeImg.gameObject.SetActive(false);
        scrollBarDescription.value = 1f;
    }




    private void Update()
    {
        SortInHierarchy();


        if (Input.touchCount == 0 && !Input.GetMouseButtonDown(0))
        {
            if (returnTimer < delayBeforeReturnToVeille)
            {
                returnTimer += Time.deltaTime;
            }
            else
            {
                RetourAccueil();
            }
        }
        else
        {
            returnTimer = 0f;
        }

        if (!panelCredits.activeSelf)
        {
            timeSpentOnLibre += Time.deltaTime;
        }

    }




    private void RetourAccueil()
    {
        SceneFader.instance.FadeToScene(0);
        AnalyticsBtnEvents.OnModeVeilleActivated();
        //AnalyticsBtnEvents.OnHomeButtonPressed(); 
        AnalyticsBtnEvents.TimeSpentOnModeLibre(timeSpentOnLibre);

    }


    #region Gisante

    private void SortInHierarchy()
    {
        if (modeBrut)
        {
            List<PointInteretButton> b = buttonsBrut;

            b.Sort((x, y) => x.depth.CompareTo(y.depth));
            for (int i = 0; i < b.Count; i++)
            {
                b[i].transform.SetSiblingIndex(i);
            }
        }
        else
        {
            List<PointInteretButton> r = buttonsReconstruction;

            r.Sort((x, y) => x.depth.CompareTo(y.depth));
            for (int i = 0; i < r.Count; i++)
            {
                r[i].transform.SetSiblingIndex(i);
            }
        }

    }






    public void ShowDescriptionPointInteret(int index)
    {
        panelBrut.gameObject.SetActive(true);
        ActiverDescription(index);
        CameraGisante.instance.ChangerCible(index);

        AnalyticsBtnEvents.OnPointInteretButtonPressed();

        btnBrut.gameObject.SetActive(false);
        btnRes.gameObject.SetActive(false);
        //CanvasGroupBtnBrut.alpha = CanvasGroupBtnRes.alpha = 0f; //Pour cacher les gros boutons pendant le panel de description

    }

    public void ActiverDescription(int index)
    {
        index = Mathf.Clamp(index, 0, descriptions.Length);

        titrePointInteret.ChangeDescrptionTitle(index);
        scrollRectDescription.content = descriptions[index].GetComponent<RectTransform>();

        for (int i = 0; i < descriptions.Length; i++)
        {
            descriptions[i].SetActive(i == index);
        }
        scrollBarDescription.value = 1f;
    }



    //Appelé par le SceneFader quand le mode libre est activé, pour nettoyer le mode histoire et réinitialiser la Gisante
    public void ResetModeLibre()
    {
        modeBrut = false;
        gisanteBrutModel.localScale = modeBrut ? Vector3.one : Vector3.zero;
        gisanteReconstructionModel.localScale = modeBrut ? Vector3.zero : Vector3.one;
        ChangerModeGisante();
        PanelManager.instance.HideDescriptionPointInteret();

    }


    public void HideDescriptionPointInteret()
    {
        if (panelBrut)
        {
            panelBrut.Play("hide");
        }

        scrollBarDescription.value = 1f;
        btnBrut.gameObject.SetActive(true);
        btnRes.gameObject.SetActive(true);
        //CanvasGroupBtnBrut.alpha = CanvasGroupBtnRes.alpha = 1f; //Pour afficher les gros boutons pendant le panel de description
        CameraGisante.instance.ChangerCible();
    }





    public void ChangerModeGisante()
    {
        scrollBarDescription.value = 1f;
        modeBrut = !modeBrut;

        //uiAnim.Play(modeBrut ? "show_brut" : "show_restitution");


        StopAllCoroutines();
        //StartCoroutine(ChangeMaterialsAlpha());
        StartCoroutine(ChangeModelSize());

    }

    public void SetModeBrutAtStart()
    {
        scrollBarDescription.value = 1f;
        modeBrut = !modeBrut;

        uiAnim.Play(modeBrut ? "show_brut" : "show_restitution");


        StopAllCoroutines();
        //StartCoroutine(ChangeMaterialsAlpha());
        StartCoroutine(ChangeModelSize());
        //StartCoroutine(ReduceModelSize());

    }








    private IEnumerator ChangeModelSize()
    {
        transitionIsFinished = false;


        yield return StartCoroutine(SceneFader.instance.FadeImgOut(fadeImg));
        gisanteBrutModel.localScale = modeBrut ? Vector3.one : Vector3.zero;
        gisanteReconstructionModel.localScale = !modeBrut ? Vector3.one : Vector3.zero;

        gisanteBrutModel.gameObject.SetActive(modeBrut);
        gisanteReconstructionModel.gameObject.SetActive(!modeBrut);

        CanvasGroupBtnBrut.alpha = modeBrut ? 1f : 0f;
        yield return StartCoroutine(SceneFader.instance.FadeImgIn(fadeImg));


        //btnBrut.interactable = btnRes.interactable = false;

        //Transform currentModel = !modeBrut ? gisanteBrutModel : gisanteReconstructionModel;
        //Vector3 scale = currentModel.localScale;
        //float t = 0f;


        //currentModel = !modeBrut ? gisanteBrutModel : gisanteReconstructionModel;
        //scale = currentModel.localScale;

        //while (t < 1f)
        //{
        //    t += Time.deltaTime;

        //    scale = Vector3.Slerp(Vector3.one, Vector3.zero, scaleCurve.Evaluate(t));
        //    currentModel.localScale = scale;
        //    yield return null;
        //}


        //yield return new WaitForSeconds(.5f);

        //t = 0f;
        //currentModel = modeBrut ? gisanteBrutModel : gisanteReconstructionModel;
        //scale = currentModel.localScale;


        //while (t < 1f)
        //{
        //    t += Time.deltaTime;

        //    scale = Vector3.Slerp(Vector3.zero, Vector3.one, scaleCurve.Evaluate(t));
        //    currentModel.localScale = scale;
        //    yield return null;
        //}


        //btnBrut.interactable = btnRes.interactable = true;

        transitionIsFinished = true;

    }




    private IEnumerator ReduceModelSize()
    {
        transitionIsFinished = false;

        btnBrut.interactable = btnRes.interactable = false;

        Transform currentModel = modeBrut ? gisanteBrutModel : gisanteReconstructionModel;
        Vector3 scale = currentModel.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;

            scale = Vector3.Slerp(Vector3.zero, Vector3.one, scaleCurve.Evaluate(t));
            currentModel.localScale = scale;
            yield return null;
        }



        transitionIsFinished = true;
        btnBrut.interactable = btnRes.interactable = true;

    }

    //private IEnumerator ChangeMaterialsAlpha()
    //{
    //    float t = 0f;

    //    brutMat.ChangeRenderMode(StandardShaderUtils.BlendMode.Transparent);
    //    reconstructionMat.ChangeRenderMode(StandardShaderUtils.BlendMode.Transparent);

    //    while (t < 1f)
    //    {
    //        t += Time.deltaTime;

    //        Color c = brutMat.color;
    //        brutMat.color = new Color(c.r, c.g, c.b, modeBrut ? t : 1f - t);
    //        c = reconstructionMat.color;
    //        reconstructionMat.color = new Color(c.r, c.g, c.b, modeBrut ? 1f - t : t);

    //        yield return null;
    //    }

    //    brutMat.ChangeRenderMode(modeBrut ? StandardShaderUtils.BlendMode.Opaque : StandardShaderUtils.BlendMode.Cutout);
    //    reconstructionMat.ChangeRenderMode(!modeBrut ? StandardShaderUtils.BlendMode.Opaque : StandardShaderUtils.BlendMode.Cutout);
    //}


    #endregion



    #region Boutons bas

    public GameObject panelCredits;
    public Transform buttonsRight;

    [Space(10)]

    public Color selectedBtnColor, normalBtnColor;



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
        //LocalizationStartupManager.instance.ChangeAllTextsInScene();
        ChangerLangue(PlayerPrefs.GetString("langue", "fr"));
    }

    #endregion
}
