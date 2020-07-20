using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LocalizationManager : MonoBehaviour {

    public Dictionary<string, string> localizedText;
    public string fileGenericName = "traduction_", fileExtension = "..json", missingtextString = "Texte localisé non trouvé !";
    public static LocalizationManager instance;
    public string currentLanguage;



    private bool isReady = false; //On n'en a pas besoin, mais on le laise si jamais on veut activer un truc automatiquement après l'appui sur le bouton

    public bool CheckIfReady()
    {
        return isReady;
    }



    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }





    public void LoadLocalizedText(string fileLanguage)
    {
        currentLanguage = fileLanguage;
        //print(LocalizationManager.instance.currentLanguage);

        string fileName = string.Format("{0}{1}{2}", fileGenericName, fileLanguage, fileExtension);

        localizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName).Replace('\\', '/');


        if (!File.Exists(filePath))
        {
            Debug.Log("Erreur : Le fichier \"" + fileName + "\" est introuvable dans le dossier \"" + filePath + "\".");
        }
        else
        {
            string dataAsJson = File.ReadAllText(filePath);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }

            //Debug.Log("Données chargées, le dictionnaire contient " + localizedText.Count + " entrées.");
            LocalizationStartupManager.instance.ChangeAllTextsInScene();
        }

        isReady = true;
    }



    public string GetLocalizedData(string key)
    {
        string result = missingtextString;
        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }

        return result;
    }
}
