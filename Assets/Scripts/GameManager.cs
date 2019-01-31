using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    private static GameState gameState;

    private static bool paused = false;

    public static bool Paused
    {
        get
        {
            return paused;
        }
        set
        {
            paused = value;

            if (value)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;

        }
    }

    //These functions are called by UI buttons
    //------------------------------------------
    public static void LoadMainMenu()
    {
        gameState = GameState.START;
        SceneManager.LoadScene("StartScene");
    }

    public static void LoadRules()
    {
        gameState = GameState.RULES;
        SceneManager.LoadScene("Rules");
    }
    public static void StartGame()
    {
        gameState = GameState.IN_GAME;
        SceneManager.LoadScene("Game");
    }

    public static void LoadScores()
    {
        gameState = GameState.SCOREBOARD;
        SceneManager.LoadScene("Scoreboard");
    }
    //------------------------------------------
}
public enum GameState { START, RULES, IN_GAME, SCOREBOARD };
