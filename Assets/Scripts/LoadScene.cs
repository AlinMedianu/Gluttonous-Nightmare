using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LoadScene : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] uIButtonSounds;
    private AudioSource audioSource;
    private delegate void FunctionContainer();
    private FunctionContainer functionContainer = null;       //A way to hold the some function, in order to call each, when needed

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        functionContainer += GameManager.StartGame;
        functionContainer += GameManager.LoadRules;
        functionContainer += GameManager.LoadMainMenu;
        functionContainer += GameManager.LoadScores;
        functionContainer +=  () =>
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        };
    }

    private void OnDestroy()
    {
        functionContainer = null;
    }

    // Loads the Game Scene
    public void StartGame()
    {
        StartCoroutine(WaitForClip(0));
    }

    // Loads the Rules Scene
    public void Rules()
    {
        StartCoroutine(WaitForClip(1));
    }

    // Loads the Main Menu Scene
    public void MainMenu()
    {
        StartCoroutine(WaitForClip(2));
    }

    // Loads the Scoreboard Scene
    public void Scoreboard()
    {
        StartCoroutine(WaitForClip(3));
    }

    // Quits the game
    public void Quit()
    {
        StartCoroutine(WaitForClip(4));
    }

    private IEnumerator WaitForClip(int functionID)
    {
        audioSource.clip = uIButtonSounds[Random.Range(0, uIButtonSounds.Length)];
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);                   //Wait for the clip to finish, before invoking a function 
        functionContainer.GetInvocationList()[functionID].DynamicInvoke();          //that will change the scene or close the game entirely
    }
}
