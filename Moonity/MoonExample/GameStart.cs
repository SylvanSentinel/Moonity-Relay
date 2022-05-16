using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public static GameStart instance;

    public string playerName = "Borpa";

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }



    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void StartGameWithName()
    {
        Invoke("Go", 1);
    }

    void Go()
    {
        playerName = FindObjectOfType<PlayerID>().playerName;
        StartGame();
    }

}
