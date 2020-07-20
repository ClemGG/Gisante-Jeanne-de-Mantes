using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SceneFader : MonoBehaviour {

    public static bool isTransitioning;
    public Image fadeImg, videoFadeImg;

    [Tooltip("Plus la valeur est haute, plus le fondu sera rapide, et inversement plus la valeur est basse.")]
    public float fadeAlphaSpeed = 1f;
    public AnimationCurve fadeCurve;


    public static SceneFader instance;

    private void Awake()
    {
        if (instance != null)
        {
            print("More than one SceneFader in scene !");
            return;
        }

        instance = this;
        
    }





    private void Start()
    {
        if (fadeImg.gameObject.activeSelf == false)
        {
            fadeImg.gameObject.SetActive(true);
        }

        if(!isTransitioning)
        StartCoroutine(FadeIn());
    }



    public static int GetActiveSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    public static string GetActiveSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }






    /// <summary>
    /// Permet de réaliser un fondu entre les scènes
    /// </summary>
    public void FadeToScene(int sceneIndex)
    {
        if (!isTransitioning)
        StartCoroutine(FadeOut(sceneIndex));
    }



    /// <summary>
    /// Permet de réaliser un fondu avant de quitter le jeu.
    /// </summary>
    public void FadeToQuitScene()
    {
        if(!isTransitioning)
        StartCoroutine(FadeQuit());
    }




    public IEnumerator ActiverNouveauPanel(GameObject ancienPanel, GameObject nouveauPanel, float delayBeforeActivation, bool hideOnStart, GameObject ancienneCam, GameObject nouvelleCam, int buttonToHighlight)
    {
        //PanelManager.instance.isTransitioning = true;
        //PanelManager.instance.MakeNavigationsBtnsInteractable();
        isTransitioning = true;


        videoFadeImg.gameObject.SetActive(true);
        if (hideOnStart)
        {
            videoFadeImg.color = new Color(0,0,0,1);
        }
        else
        {
            FadeOutToVideo();
        }

        yield return new WaitForSeconds(1f / fadeAlphaSpeed);

        if (ancienPanel)
            ancienPanel.SetActive(false);

        if (nouveauPanel)
            nouveauPanel.SetActive(true);

        if (ancienneCam)
            ancienneCam.SetActive(false);

        if (nouvelleCam)
            nouvelleCam.SetActive(true);

        //PanelManager.instance.ShowNavigationButton(buttonToHighlight);
        //PanelManager.instance.ResetModes();

        yield return new WaitForSeconds(delayBeforeActivation);
        FadeInToVideo();

        //PanelManager.instance.isTransitioning = false;
        isTransitioning = false;
    }


    public void FadeInToVideo()
    {
        if(!isTransitioning)
        StartCoroutine(FadeInVideo());
    }

    private IEnumerator FadeInVideo()
    {
        isTransitioning = true;

        float t = 1f;
        videoFadeImg.color = Color.black;


        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime * fadeAlphaSpeed;
            float a = fadeCurve.Evaluate(t);
            videoFadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, a);
            yield return 0;
        }

        isTransitioning = false;
        videoFadeImg.gameObject.SetActive(false);
    }






    public void FadeOutToVideo()
    {
        if(isTransitioning)
        StartCoroutine(FadeOutVideo());
    }

    private IEnumerator FadeOutVideo()
    {
        isTransitioning = true;

        float t = 0f;
        videoFadeImg.gameObject.SetActive(true);


        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeAlphaSpeed;
            float a = fadeCurve.Evaluate(t);
            videoFadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, a);
            yield return 0;
        }

        isTransitioning = false;

    }



















    /// <summary>
    /// Diminue l'alpha du fondu pour faire apparaître la scène
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeIn()
    {

        isTransitioning = true;
        float t = 1f;


        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime * fadeAlphaSpeed;
            float a = fadeCurve.Evaluate(t);
            fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, a);
            yield return 0;
        }

        isTransitioning = false;
        fadeImg.gameObject.SetActive(false);



    }








    /// <summary>
    /// Augmente l'alpha du fondu pour faire disparaître la scène
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeOut(int sceneIndex)
    {
        isTransitioning = true;
        fadeImg.gameObject.SetActive(true);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeAlphaSpeed;
            float a = fadeCurve.Evaluate(t);
            fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, a);
            yield return 0;
        }

        isTransitioning = false;
        AudioManager.instance.StopAll();
        SceneManager.LoadScene(sceneIndex);
    }







    /// <summary>
    /// Augmente l'alpha du fondu pour faire disparaître la scène et quitter le jeu.
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeQuit()
    {
        isTransitioning = true;
        fadeImg.gameObject.SetActive(true);
        float t = 0f;



        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeAlphaSpeed;
            float a = fadeCurve.Evaluate(t);
            fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, a);
            yield return 0;
        }


        isTransitioning = false;
        Application.Quit();
    }

    public IEnumerator FadeImgOut(Image fadeImg)
    {
        isTransitioning = true;
        float t = 0f;
        fadeImg.gameObject.SetActive(true);


        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeAlphaSpeed;
            float a = fadeCurve.Evaluate(t);
            fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, a);
            yield return 0;
        }

        isTransitioning = false;
    }

    public IEnumerator FadeImgIn(Image fadeImg)
    {
        isTransitioning = false;
        float t = 1f;


        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime * fadeAlphaSpeed;
            float a = fadeCurve.Evaluate(t);
            fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, a);
            yield return 0;
        }

        fadeImg.gameObject.SetActive(false);
        isTransitioning = false;
    }
}
