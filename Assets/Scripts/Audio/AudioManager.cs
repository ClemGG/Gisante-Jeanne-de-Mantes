using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Son[] sons;
    AudioListener thisAudioListener;
    AudioListener[] audioListeners;

    public static AudioManager instance;

#if UNITY_EDITOR

    private void OnValidate()
    {
        for (int i = 0; i < sons.Length; i++)
        {
            sons[i].tag = sons[i].clip.name;
        }
    }

#endif




    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        audioListeners = (AudioListener[])Resources.FindObjectsOfTypeAll(typeof(AudioListener));
        if(audioListeners.Length == 0 && !thisAudioListener)
        {
            thisAudioListener = gameObject.AddComponent<AudioListener>();
        }

        for (int i = 0; i < sons.Length; i++)
        {
            if(sons[i].source == null)
            {
                sons[i].source = gameObject.AddComponent<AudioSource>();
                sons[i].source.clip = sons[i].clip;
                sons[i].source.volume = sons[i].volume;
                sons[i].source.loop = sons[i].loop;
                sons[i].source.playOnAwake = sons[i].playOnAwake;

                if (sons[i].playOnAwake)
                {
                    Play(sons[i].clip.name);
                }
            }
        }
    }


    public void Play(string name)
    {
        Son s = Array.Find(sons, son => son.clip.name == name);

        if (s != null)
            s.source.Play();
        else
            Debug.LogError($"Erreur : Le nom \"{name}\" n'existe pas dans la liste des sons.");
    }
    public void Stop(string name)
    {
        Son s = Array.Find(sons, son => son.clip.name == name);

        if (s != null)
            s.source.Stop();
        else
            Debug.LogError($"Erreur : Le nom \"{name}\" n'existe pas dans la liste des sons.");
    }

    public void Pause(string name, bool shouldPause)
    {
        Son s = Array.Find(sons, son => son.clip.name == name);

        if (s != null)
        {
            if (shouldPause)
            {
                s.source.Pause();
            }
            else
            {
                s.source.UnPause();
            }
        }
        else
            Debug.LogError($"Erreur : Le nom \"{name}\" n'existe pas dans la liste des sons.");
    }
    public void PauseAll(bool shouldPause)
    {
        foreach (Son s in sons)
        {
            if (s != null)
            {
                if (shouldPause)
                {
                    s.source.Pause();
                }
                else
                {
                    s.source.UnPause();
                }
            }
            else
                Debug.LogError($"Erreur : Le nom \"{name}\" n'existe pas dans la liste des sons.");
        }

        
    }

    public void StopAll()
    {
        foreach (Son s in sons)
        {
            s.source.Stop();
        }
    }





    public void Play(AudioClip clip)
    {
        Son s = Array.Find(sons, son => son.clip == clip);

        if (s != null)
            s.source.Play();
        else
            Debug.LogError($"Erreur : Le nom \"{name}\" n'existe pas dans la liste des sons.");
    }
    public void Stop(AudioClip clip)
    {
        Son s = Array.Find(sons, son => son.clip == clip);

        if (s != null)
            s.source.Stop();
        else
            Debug.LogError($"Erreur : Le nom \"{name}\" n'existe pas dans la liste des sons.");
    }
    public void Pause(AudioClip clip, bool shouldPause)
    {
        Son s = Array.Find(sons, son => son.clip == clip);

        if (s != null)
        {
            if (shouldPause)
            {
                s.source.Pause();
            }
            else
            {
                s.source.UnPause();
            }
        }
        else
            Debug.LogError($"Erreur : Le nom \"{name}\" n'existe pas dans la liste des sons.");
    }







    public void PlayRandomSoundFromList(params int[] indexs)
    {
        int alea = UnityEngine.Random.Range(0, indexs.Length);
        Son s = sons[indexs[alea]];

        if (s != null)
            s.source.Play();
        else
            Debug.LogError($"Erreur : L'ID n° \"{indexs[alea]}\" n'existe pas dans la liste des sons.");
    }


    public void PlayRandomSoundFromList(params string[] noms)
    {
        int alea = UnityEngine.Random.Range(0, noms.Length);
        Son s = Array.Find(sons, son => son.clip.name == noms[alea]);

        if (s != null)
            s.source.Play();
        else
            Debug.LogError($"Erreur : Le nom \"{noms[alea]}\" n'existe pas dans la liste des sons.");
    }


    public void Mute(bool shouldMute)
    {
        //if (audioListeners.Length == 0)
        //    thisAudioListener.enabled = !shouldMute;
        //else
        //    for (int i = 0; i < audioListeners.Length; i++)
        //    {
        //        audioListeners[i].enabled = !shouldMute;
        //    }


        for (int i = 0; i < sons.Length; i++)
        {
            print(sons[i].source.volume);
            //sons[i].source.mute = shouldMute;
            sons[i].source.volume = shouldMute ? 0f : sons[i].volume;
            print(sons[i].source.volume);
        }
    }


    public Son GetSonFromClip(AudioClip clip)
    {
        if(clip != null)
        {

            Son s = Array.Find(sons, son => son.clip == clip);

            if (s != null)
                return s;
            else
            {
                Debug.LogError($"Erreur : Le clip \"{clip.name}\" n'existe pas dans la liste des sons.");
                return null;
            }
        }
        else
        {
            return null;
        }

    }

    public Son GetSonFromClip(string name)
    {
        if (name != null)
        {

            Son s = Array.Find(sons, son => son.clip.name == name);

            if (s != null)
                return s;
            else
            {
                Debug.LogError($"Erreur : Le clip \"{name}\" n'existe pas dans la liste des sons.");
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}