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
    [Header("Men� Principal")]
    public GameObject pauseMenu;
    public Button returnButton;
    public Button optionButton;
    public Button exitButton;
    public UnityEvent returnButtonEvent; //Referenciamos a la funci�n de ActiveMenu del interactionSystem

    [Header("Configuraci�n")]
    public AudioMixer audioMixer;
    public GameObject configMenu;
    public Button optionReturnButton;
    public Slider mouseSensiblity;
    public Slider volume;
    public TMP_Text sensibilityText;
    public TMP_Text volumeText;

    public static bool optionMenu = false;

    [Header("Escenas")]
    public string MainMenuScene = "MainMenu";

    void Start()
    {
        pauseMenu.SetActive(false);
        //Botones Men� Principal:
        returnButton.onClick.AddListener(ReturnGame);
        optionButton.onClick.AddListener(OptionMenu);
        exitButton.onClick.AddListener(ExitGame);

        configMenu.SetActive(false);
        //Configuraci�n
        optionReturnButton.onClick.AddListener(OptionMenu);
        mouseSensiblity.onValueChanged.AddListener(HandleSensitivityChange);
        volume.onValueChanged.AddListener(HandleVolumeChange);
    }

    public bool isESCPressed()
    {
        if((configMenu.activeSelf == false && pauseMenu.activeSelf == false) || (configMenu.activeSelf == true))
        {
            pauseMenu.SetActive(true);
            configMenu.SetActive(false);
            return true;
        }
        else
        {
            pauseMenu.SetActive(false);
            return false;
        }
    }
    void ReturnGame()
    {
        returnButtonEvent.Invoke(); //Invocamos la funci�n
    }

    void ExitGame()
    {
        SceneManager.LoadScene(MainMenuScene);
    }

    void OptionMenu()
    {
        if (pauseMenu.activeSelf == false)
        {
            pauseMenu.SetActive(true);
            configMenu.SetActive(false);
        }
        else
        {
            pauseMenu.SetActive(false);
            configMenu.SetActive(true);
        }
        
    }

    void HandleSensitivityChange(float value)
    {
        EnhancedFirstPersonCamera.mouseSensitivity = value;
        sensibilityText.text = value.ToString();
    }
    void HandleVolumeChange(float value)
    {
        SoundManager.Instance.UpdateMasterVolume(value);
//        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(0.0001f, value)) * 20f); 
        volumeText.text = value.ToString();
    }
}
