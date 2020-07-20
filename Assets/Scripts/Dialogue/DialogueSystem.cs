using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Space(10)]
    [Header("Scripts & Components : ")]
    [Space(10)]

    public Text texteDialogue;
    public Text characterNameText;
    public Text videoDurationText, nomVideoText;
    public GameObject playButton, replayButton, muteButton;

    [Space(20)]

    public Image videoDurationFill;
    public GameObject barreVideo;
    public PlayableDirector timeline;
    public Sprite muteImg, unmuteImg;


    [Space(10)]
    [Header("Dialogue & Video : ")]
    [Space(10)]

    public CanvasGroup gameobjectDialogue;
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



    public static DialogueSystem instance;
    Coroutine co;

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
        timeline.enabled = false;

        gameobjectDialogue.alpha = 0f;
        replayButton.SetActive(false);

        if (timeline)
        {
            string duration = ToMinutes((float)timeline.duration);
            videoDurationText.text = string.Format("{0}{1}", videoDurationText.text, duration);
        }
    }

    private string ToMinutes(float f)
    {
        int min = Mathf.FloorToInt(f / 60f);
        int sec = Mathf.RoundToInt(f % 60f);

        if(sec == 60)
        {
            sec = 0;
            min++;
        }

        string s = min.ToString("0") + ":" + sec.ToString("00");
        return s;
    }


    private void Update()
    {
        if(timeline)
            videoDurationFill.fillAmount = (float)(timeline.time / timeline.duration);

        if (Mathf.Approximately(videoDurationFill.fillAmount, 1f))
            OnVideoFinished();
    }






    public void PlayVideo()
    {

        isPlaying = true;
        dialogue = Resources.Load<Dialogue>($"Dialogues/{PlayerPrefs.GetString("langue", "fr")}");

        currentindex = 0;
        timeline.enabled = true;
        timeline.Play();

        playButton.SetActive(false);
        videoDurationText.enabled = false;
        nomVideoText.enabled = false;

        barreVideo.GetComponent<Animator>().Play("show");
        muteButton.GetComponent<Button>().interactable = false;


        AnalyticsBtnEvents.OnPlayVideoButtonPressed();
    }


    public void ReplayVideo()
    {
        StartCoroutine(SceneFader.instance.ActiverNouveauPanel(null, null, .5f, false, null, null, 2));
        StartCoroutine(ResetPanelHistoire());

        AnalyticsBtnEvents.OnReplayVideoButtonPressed();

    }


    public void MuteVideo()
    {
        mute = !mute;
        MuteAudioSources(mute);

        if(mute)
            AnalyticsBtnEvents.OnMuteButtonPressed();
    }

    private void MuteAudioSources(bool mute)
    {
        muteButton.GetComponent<Image>().sprite = mute ? muteImg : unmuteImg;

        for (int i = 0; i < dialogue.repliques.Length; i++)
        {
            Son sv = AudioManager.instance.GetSonFromClip(dialogue.repliques[i].voiceClip);
            if(sv!= null)
                sv.source.volume = mute ? 0f : sv.volume;

            Son sb = AudioManager.instance.GetSonFromClip(dialogue.repliques[i].bruitageClip);

            if(sb != null)
                sb.source.volume = mute ? 0f : sb.volume;
        }
    }


    public void StopDialogue()
    {
        StartCoroutine(ResetPanelHistoire());
    }







    private IEnumerator ResetPanelHistoire()
    {
        barreVideo.GetComponent<Animator>().Play("hide");

        gameobjectDialogue.alpha = 0f;
        Color c = texteDialogue.color;
        texteDialogue.color = new Color(c.r, c.g, c.b, 1f);

        yield return new WaitForSeconds(1.2f);

        playButton.SetActive(true);
        replayButton.SetActive(false);
        videoDurationText.enabled = true;
        nomVideoText.enabled = true;

        timeline.time = timeline.initialTime;
        timeline.Stop();
        timeline.Evaluate();


        currentindex = 0;
        videoFinie = false;
        dialogue.fini = false;

        mute = false;
        MuteAudioSources(mute);

        AudioManager.instance.StopAll();


        //print(currentindex);
    }


    public void OnVideoFinished()
    {
        isPlaying = false;
        videoFinie = true;
        replayButton.SetActive(true);

        mute = false;
        MuteAudioSources(false);

        barreVideo.GetComponent<Animator>().Play("hide");

        currentindex = dialogue.repliques.Length;

        RepliqueSuivante();


        AnalyticsBtnEvents.OnVideoCompleted();
        AnalyticsBtnEvents.TimeSpentOnVideo((float)timeline.duration);
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
                    muteButton.GetComponent<Button>().interactable = false;
                    gameobjectDialogue.alpha = a;
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

                if(!sb.source.isPlaying)
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

            if(r.voiceClip)
                AudioManager.instance.Play(r.voiceClip);

            muteButton.GetComponent<Button>().interactable = true;

            currentindex++;



            t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * fadeSpeed;
                float a = fadeCurve.Evaluate(t);
                if(currentindex == 1)
                {
                    gameobjectDialogue.alpha = a;
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
}
