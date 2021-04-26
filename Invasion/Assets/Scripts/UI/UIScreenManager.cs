using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        FindObjectOfType<GameManager>().SetGameUIActive(false);
    }

    public void StartGame()
    {
        FindObjectOfType<GameManager>().LoadNextLevel();
    }

    public void Exit()
    {
        FindObjectOfType<GameManager>().ExitGame();
    }

    public void RestartGame()
    {
        FindObjectOfType<GameManager>().ReturnToMain();
    }
}
