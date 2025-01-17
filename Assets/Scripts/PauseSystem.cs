using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.Audio;
using TMPro;

public class PauseSystem : MonoBehaviour
{
    [Header("Menú Principal")]
    public GameObject pauseMenu;
    public Button returnButton;
    public Button optionButton;
    public Button exitButton;
    public UnityEvent returnButtonEvent; //Referenciamos a la función de ActiveMenu del interactionSystem

    [Header("Configuración")]
    public AudioMixer audioMixer;
    public GameObject ConfigMenu;
    public Button optionReturnButton;
    public Slider mouseSensiblity;
    public Slider volume;
    public TMP_Text sensibilityText;
    public TMP_Text volumeText;

    [Header("Escenas")]
    public string MainMenuScene = "MainMenu";

    void Start()
    {
        pauseMenu.SetActive(false);
        //Botones Menú Principal:
        returnButton.onClick.AddListener(ReturnGame);
        optionButton.onClick.AddListener(OptionMenu);
        exitButton.onClick.AddListener(ExitGame);

        ConfigMenu.SetActive(false);
        //Configuración
        optionReturnButton.onClick.AddListener(OptionMenu);
        mouseSensiblity.onValueChanged.AddListener(HandleSensitivityChange);
        volume.onValueChanged.AddListener(HandleVolumeChange);
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

    void OptionMenu()
    {
        ToggleVisibilityMenu();
        ConfigMenu.SetActive(!ConfigMenu.activeSelf);
    }

    void HandleSensitivityChange(float value)
    {
        FirstPersonCamera.mouseSensitivity = value;
        sensibilityText.text = value.ToString();
    }
    void HandleVolumeChange(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(0.0001f, value)) * 20f);
        volumeText.text = value.ToString();
    }
}
