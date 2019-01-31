using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;                                               //for the Serializable attribute
using System.IO;                                            //for the File and FileStream classes
using System.Runtime.Serialization.Formatters.Binary;       //for the BinaryFormatter class
using UnityEngine.SceneManagement;                          //for the SceneManager class

//This class is shared among two separate gameobjects, one that starts in the "Game" scene and persists in the "Scoreboard"
//scene, the Score Manager, and one that always exists in the "Scoreboard" scene, but is inactive if Game Manager exists, the Scoreboard
[RequireComponent(typeof(AudioSource))]
public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text namesText;
    [SerializeField]
    private Text scoresText;
    [SerializeField]
    private InputField nameField;
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private AudioClip[] uIButtonSounds;
    private float score;
    private string highScoreName = "Type your name here";
    private KeyValuePair<string, float> scoreAndName = new KeyValuePair<string, float>();
    private Dictionary<string, float> scoresAndNames = new Dictionary<string, float>();
    private AudioSource audioSource;

    private void Start()
    {
        //Score Manager's Start in the "Scoreboard" scene
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex((int)GameState.IN_GAME))
        {
            DontDestroyOnLoad(gameObject);
            //DeleteData();
            LoadData();
            audioSource = GetComponent<AudioSource>();
        }
        //Scoreboard's Start
        else if (GameObject.Find("Score Manager") == null)
        {
            LoadData();
            UpdateScoreboard();
        }
    }

    private void Update ()
    {
        //Score Manager's first frame in the "Scoreboard" scene
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex((int)GameState.SCOREBOARD) && namesText == null)
        {
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByBuildIndex((int)GameState.SCOREBOARD));
            GameObject.Find("Scoreboard").SetActive(false);
            namesText = GameObject.Find("Canvas").transform.Find("Names").GetComponent<Text>();
            scoresText = GameObject.Find("Canvas").transform.Find("Scores").GetComponent<Text>();
            nameField = GameObject.Find("Canvas").transform.Find("Name Field").GetComponent<InputField>();
            nameField.onValueChanged.AddListener((name) => EnableButton(name));
            submitButton = GameObject.Find("Canvas").transform.Find("Submit Button").GetComponent<Button>();
            submitButton.onClick.AddListener(() => UpdateData());
            //Scored a high score
            if (scoresAndNames.Count < 10 || score >= scoreAndName.Value)
            {
                nameField.gameObject.SetActive(true);
                submitButton.gameObject.SetActive(true);
            }
            else
                UpdateScoreboard();
        }
        //Score Manager's Update in the "Game" scene
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex((int)GameState.IN_GAME) && !PlayerController.dead)
        {
            score += Time.deltaTime;
            scoreText.text = "You survived: " + (int)score + " seconds";
        }
    }
    //This function writes in the file
    private void SaveData()
    {
        BinaryFormatter encrypter = new BinaryFormatter();
        FileStream file;
        //Creates a file at the specified address if it doesm't exist, if it does, it opens the file
        if (File.Exists(Application.persistentDataPath + "/data.dat"))            
            file = File.Open(Application.persistentDataPath + "/data.dat", FileMode.Open);
        else
            file = File.Create(Application.persistentDataPath + "/data.dat");
        //------------------------------------------------------------------------------------------
        Data fileData = new Data(); //the Scores class was made as a midpoint between this class and the file
        fileData.FileScoresAndNames = scoresAndNames;
        encrypter.Serialize(file, fileData); //the Scores instance can be serialized to binary because of the Serializable attribute
        file.Close();
    }
    //This function reads from the file
    private void LoadData()
    {
        if(File.Exists(Application.persistentDataPath + "/data.dat"))
        {
            BinaryFormatter encrypter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/data.dat", FileMode.Open);
            Data fileData = (Data)encrypter.Deserialize(file);
            file.Close();
            scoresAndNames = fileData.FileScoresAndNames;
        }
    }

    //A function to delete the file for testing purposes
    private void DeleteData()
    {
        if (File.Exists(Application.persistentDataPath + "/data.dat"))
            File.Delete(Application.persistentDataPath + "/data.dat");
    }

    //Called by the input field "Name Field" after scoring a highscore
    public void EnableButton(string name)
    {
        submitButton.interactable = true;
    }

    //Called by the button "Submit Button" upon clicking it
    public void UpdateData()
    {
        audioSource.clip = uIButtonSounds[UnityEngine.Random.Range(0, uIButtonSounds.Length)];
        audioSource.Play();
        foreach (KeyValuePair<string, float> kVP in scoresAndNames) //find the last key-value pair in the dictionary
            scoreAndName = kVP;
        highScoreName = nameField.text;
        scoresAndNames.Add(highScoreName, score);
        List<float> scores = new List<float>();
        foreach (float score in scoresAndNames.Values)
            scores.Add(score);
        for (int i = 0; i < scores.Count; i++)              //A basic way of sorting
            for (int j = i + 1; j < scores.Count; j++)      //the scores so it shows
                if (scores[i] < scores[j])                  //the largest scores at
                {                                           //the top and the smallest
                    float tempScore = scores[i];            //at the bottom
                    scores[i] = scores[j];                  //
                    scores[j] = tempScore;                  //
                }                                           //
        if (scores.Count == 11)
            scores.RemoveAt(10);
        Dictionary<string, float> newScoresAndNames = new Dictionary<string, float>();
        foreach (float score in scores)
            foreach (string name in scoresAndNames.Keys)
                if (score == scoresAndNames[name])
                    newScoresAndNames.Add(name, score);
        scoresAndNames = newScoresAndNames;
        UpdateScoreboard();
        SaveData();
    }

    //Write the high scores on the screen
    private void UpdateScoreboard()
    {
        if(nameField != null)
            nameField.gameObject.SetActive(false);
        if (submitButton != null)
            submitButton.gameObject.SetActive(false);
        foreach (string name in scoresAndNames.Keys)
        {
            namesText.text += "\t\t\t" + name + "\n";
            scoresText.text += (int)scoresAndNames[name] + " seconds\t\t\t\n";
        }
            
    }
}

[Serializable]
class Data
{
    public Dictionary<string, float> FileScoresAndNames { get; set; }
}