using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;

public class PanelHistoireButtons : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public Text texteDialogue;
    public Text characterNameText;
    public Text timeSpent;
    public Text videoDurationText, nomVideoText;
    public TextMeshProUGUI ecouteursText;
    public Button playButton, replayButton, muteButton;

    [Space(20)]

    public RectTransform videoDurationRect;
    public PlayableDirector timeline;
    public Animator barreDialogueAnim;
    public Sprite muteImg, unmuteImg;


    [Space(10)]
    [Header("Dialogue & Video : ")]
    [Space(10)]

    public CanvasGroup content;
    public float fadeSpeed = 2f;
    public AnimationCurve fadeCurve;


    [Space(10)]
    public Dialogue dialogue;
    int currentindex = 0;


    bool videoFinie = false;
    bool mute = false;
    public bool isPlaying = false;


    [Space(10)]
    [Header("Audio : ")]
    [Space(10)]


    public float volumeFadeSpeed = .5f;
    public AnimationCurve volumeFadeCurve;
    public AudioSource dialogueVoicesAud;


    public static PanelHistoireButtons instance;
    Coroutine co;


    [Space(10)]
    [Header("Analytics : ")]
    [Space(10)]

    public float delayBeforeReturnToVeille = 300f;
    float timeSpentOnHistoire;
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

    // Start is called before the first frame update
    void Start()
    {
        ChangerLangue(PlayerPrefs.GetString("langue", "fr"));
        panelCredits.SetActive(false);

        content.alpha = 0f;
        timeline.enabled = false;

        replayButton.interactable = false;
        playButton.interactable = true;

        if (timeline)
        {
            string duration = ToMinutes((float)timeline.duration);
            videoDurationText.text = string.Format("{0} {1}", videoDurationText.text, duration);
        }

    }



    #region Dialogue
    private string ToMinutes(float f)
    {
        int min = Mathf.FloorToInt(f / 60f);
        int sec = Mathf.RoundToInt(f % 60f);

        if (sec == 60)
        {
            sec = 0;
            min++;
        }

        string s = min.ToString("0") + ":" + sec.ToString("00");
        return s;
    }


    private void Update()
    {
        if (timeline)
        {


            if (isPlaying)
            {
                videoDurationRect.sizeDelta = new Vector2(Mathf.Lerp(1574f, 0f, (float)(timeline.time / timeline.duration)), videoDurationRect.sizeDelta.y);
                timeSpent.text = ToMinutes((float) timeline.time);
            }
        }

        if (Mathf.Approximately(videoDurationRect.sizeDelta.x, 0f))
            OnVideoFinished();



        if (Input.touchCount == 0 && !isPlaying && !Input.GetMouseButtonDown(0))
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
            timeSpentOnHistoire += Time.deltaTime;
        }

    }

    private void RetourAccueil()
    {

        SceneFader.instance.FadeToScene(0);
        //AnalyticsBtnEvents.OnModeVeilleActivated();

        if (timeline.time < timeline.duration && isPlaying) //Si la video n'a pas été finie, alors on enregistre le temps écoulé sur la vidéo
            AnalyticsBtnEvents.TimeSpentOnVideo((float)DialogueSystem.instance.timeline.time);
        
    }








    public void PlayVideo()
    {

        isPlaying = true;
        dialogue = Resources.Load<Dialogue>($"Dialogues/{PlayerPrefs.GetString("langue", "fr")}"); 
        dialogueVoicesAud.Play();


        currentindex = 0;
        timeline.enabled = true;
        timeline.Play();

        playButton.gameObject.SetActive(false);
        videoDurationText.enabled = false;
        nomVideoText.enabled = false;
        ecouteursText.enabled = false;

        barreDialogueAnim.Play("show");
        muteButton.interactable = true;
        replayButton.interactable = true;

        texteDialogue.text = null;
        characterNameText.text = null;

        AnalyticsBtnEvents.OnPlayVideoButtonPressed();
    }


    public void ReplayVideo()
    {
        StartCoroutine(ResetPanelHistoire());

        AnalyticsBtnEvents.OnReplayVideoButtonPressed();

    }


    public void MuteVideo()
    {
        mute = !mute;
        MuteAudioSources(mute);
        //muteButton.GetComponent<Image>().sprite = mute ? muteImg : unmuteImg;
        //AudioManager.instance.Mute(mute);


        if (mute)
            AnalyticsBtnEvents.OnMuteButtonPressed();
    }

    private void MuteAudioSources(bool mute)
    {
        muteButton.GetComponent<Image>().sprite = mute ? muteImg : unmuteImg;

        //AudioListener[] als = Resources.FindObjectsOfTypeAll<AudioListener>();

        //for (int i = 0; i < als.Length; i++)
        //{
        //    als[i].enabled = !mute;
        //}


        dialogueVoicesAud.mute = mute;

        //for (int i = 0; i < dialogue.repliques.Length; i++)
        //{
        //    Son sv = AudioManager.instance.GetSonFromClip(dialogue.repliques[i].voiceClip);
        //    if (sv != null)
        //        sv.source.volume = mute ? 0f : sv.volume;

        //    print(sv);

        //    Son sb = AudioManager.instance.GetSonFromClip(dialogue.repliques[i].bruitageClip);

        //    if (sb != null)
        //        sb.source.volume = mute ? 0f : sb.volume;
        //}
    }


    public void StopDialogue()
    {
        StartCoroutine(ResetPanelHistoire());
    }







    private IEnumerator ResetPanelHistoire()
    {
        //barreDialogueAnim.Play("hide");
        dialogue = Resources.Load<Dialogue>($"Dialogues/{PlayerPrefs.GetString("langue", "fr")}");

        content.alpha = 0f;
        Color c = texteDialogue.color;
        texteDialogue.color = new Color(c.r, c.g, c.b, 1f);
        timeSpent.text = ToMinutes(0f);

        //yield return StartCoroutine(SceneFader.instance.ActiverNouveauPanel(null, null, .5f, false, null, null, 2));
        yield return null;

        //playButton.gameObject.SetActive(true);
        //replayButton.interactable = false;
        //videoDurationText.enabled = true;
        //nomVideoText.enabled = true;

        timeline.time = timeline.initialTime;
        //timeline.Stop();
        timeline.Evaluate();
        timeline.Play();

        currentindex = 0;
        videoFinie = false;
        dialogue.fini = false;
        isPlaying = true;

        mute = false;
        MuteAudioSources(mute); 
        muteButton.interactable = true;
        //replayButton.interactable = false;
        dialogueVoicesAud.Play();

        RepliqueSuivante();


        //print(currentindex);
    }


    public void OnVideoFinished()
    {
        isPlaying = false;
        videoFinie = true;
        replayButton.interactable = true;
        muteButton.interactable = false;

        mute = false;
        MuteAudioSources(false);

        barreDialogueAnim.Play("hide");

        currentindex = dialogue.repliques.Length;

        RepliqueSuivante();


        AnalyticsBtnEvents.OnVideoCompleted();
        AnalyticsBtnEvents.TimeSpentOnVideo((float)timeline.duration);
        SceneFader.instance.FadeToScene(SceneFader.GetActiveSceneIndex());
    }



    public void RepliqueSuivante()
    {
        if (co == null)
        {
            co = StartCoroutine(ChangerDialogue());
        }
    }


    private IEnumerator ChangerDialogue()
    {
        float t = 1f;


        if (!dialogue.fini)
        {

            while (t > 0f)
            {
                t -= Time.deltaTime * fadeSpeed;
                float a = fadeCurve.Evaluate(t);


                if (currentindex == dialogue.repliques.Length || videoFinie)
                {
                    content.alpha = a;
                }
                if (videoFinie)
                {
                    muteButton.GetComponent<Button>().interactable = false;
                }
                if (currentindex <= dialogue.repliques.Length)
                {
                    Color c = texteDialogue.color;
                    Color cc = characterNameText.color;
                    texteDialogue.color = new Color(c.r, c.g, c.b, a);
                    characterNameText.color = new Color(cc.r, cc.g, cc.b, a);
                }

                yield return 0;
            }
        }


        dialogue.fini = currentindex == dialogue.repliques.Length;





        if (currentindex < dialogue.repliques.Length && !videoFinie)
        {
            Replique r = dialogue.repliques[currentindex];

            if (r.bruitageClip)
            {
                Son sb = AudioManager.instance.GetSonFromClip(r.bruitageClip);

                if (!sb.source.isPlaying)
                    AudioManager.instance.Play(r.bruitageClip);

                if (sb != null)
                {
                    float timerBruitage = 0f;
                    while (timerBruitage < 1f)
                    {
                        timerBruitage += Time.deltaTime * volumeFadeSpeed;
                        float a = volumeFadeCurve.Evaluate(timerBruitage);
                        sb.source.volume = r.endBruitage ? 1 - a : a;
                        yield return null;

                    }
                }
            }





            texteDialogue.text = r.text;
            characterNameText.text = r.characterName;

            if (r.voiceClip)
                AudioManager.instance.Play(r.voiceClip);

            muteButton.GetComponent<Button>().interactable = true;

            currentindex++;



            t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * fadeSpeed;
                float a = fadeCurve.Evaluate(t);
                if (currentindex == 1)
                {
                    content.alpha = a;
                }
                Color c = texteDialogue.color;
                Color cc = characterNameText.color;
                texteDialogue.color = new Color(c.r, c.g, c.b, a);
                characterNameText.color = new Color(cc.r, cc.g, cc.b, a);

                yield return 0;
            }

        }

        co = null;
    }


    #endregion




    #region Boutons bas



    public void Restartlevel()
    {
        videoDurationText.text = string.Format("{0} {1}", LocalizationManager.instance.GetLocalizedData(videoDurationText.GetComponent<LocalizedText>().key), ToMinutes((float)timeline.duration));

        if(isPlaying)
            SceneFader.instance.FadeToScene(SceneFader.GetActiveSceneIndex());
    }



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
            dialogueVoicesAud.Pause();
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
                {
                    a.Play("hide");
                    dialogueVoicesAud.UnPause();

                }
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
            dialogueVoicesAud.Pause();
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
                {
                    a.Play("hide");
                    dialogueVoicesAud.UnPause();
                }
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
        LocalizationStartupManager.instance.ChangeAllTextsInScene();
        ShowLanguageButton(PlayerPrefs.GetString("langue", "fr"));
    }




    #endregion
}