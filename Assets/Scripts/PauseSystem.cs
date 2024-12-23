using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class PauseSystem : MonoBehaviour
{
    public GameObject pauseMenu;
    public Button returnButton;
    public Button exitButton;
    public UnityEvent returnButtonEvent; //Referenciamos a la función de ActiveMenu del interactionSystem

    public string MainMenuScene = "MainMenu";

    void Start()
    {
        pauseMenu.SetActive(false);
        returnButton.onClick.AddListener(ReturnGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    public void ToggleVisibilityMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    void ReturnGame()
    {
        returnButtonEvent.Invoke(); //Invocamos la función
    }

    void ExitGame()
    {
        SceneManager.LoadScene(MainMenuScene);
    }

}
