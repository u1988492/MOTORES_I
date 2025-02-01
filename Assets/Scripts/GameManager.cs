using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isGuardianVisionUnlocked = false;
    public bool isGuardianVisionActive = false;

    public Vector3 positionBunker;
    public Quaternion rotationBunker;
    public Vector3 positionGuardian;
    public Quaternion rotationGuardian;

    public CanvasGroup transitionCanvas;
    public float fadeDuration = 1.5f;

    public GameObject guardianVisionObjects;
    public GameObject finalDoor;
    public TutorialManager tutorialManager;
    public FinalScene dialogue;

    private bool morsecode;
    private bool lightcode;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        guardianVisionObjects.SetActive(false);
    }

    public bool IsVisionUnlocked()
    {
        return isGuardianVisionUnlocked;
    }

    public bool IsVisionActive()
    {
        return isGuardianVisionActive;
    }

    public void ActiveVision()
    {
        isGuardianVisionActive = true;
        guardianVisionObjects.SetActive(true);
    }

    public void DesactiveVision()
    {
        isGuardianVisionActive = false;
        guardianVisionObjects.SetActive(false);
    }

    public void SaveBunkerCoords(Vector3 pos)
    {
        positionBunker = pos;
    }

    public void SaveBunkerRotation(Quaternion rot)
    {
        rotationBunker = rot;
    }

    public Vector3 GetBunkerCoords()
    {
        return positionBunker;
    }

    public Quaternion GetBunkerRotation()
    {
        return rotationBunker;
    }

    public void SaveGuardianCoords(Vector3 pos)
    {
        positionGuardian = pos;
    }

    public void SaveGuardianRotation(Quaternion rot)
    {
        rotationGuardian = rot;
    }

    public Vector3 GetGuardianCoords()
    {
        return positionGuardian;
    }

    public Quaternion GetGuardianRotation()
    {
        return rotationGuardian;
    }


    public void tpGuardian()
    {
        
        SaveBunkerCoords(GameObject.FindGameObjectWithTag("Player").transform.position);
        SaveBunkerRotation(GameObject.FindGameObjectWithTag("Player").transform.rotation);
        StartCoroutine(Transition("AstralPlane", GetGuardianCoords(), GetGuardianRotation()));
    }

    public void tpBunker()
    {
        //SaveGuardianCoords(GameObject.FindGameObjectWithTag("Player").transform.position);
        //SaveGuardianRotation(GameObject.FindGameObjectWithTag("Player").transform.rotation);
        Debug.Log(GetBunkerCoords());
        StartCoroutine(Transition("Bunker", GetBunkerCoords(), GetBunkerRotation()));

    }

    private IEnumerator Transition(string sceneName, Vector3 newPosition, Quaternion newRotation)
    {
        transitionCanvas.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(1));

        if (sceneName == "AstralPlane")
        {
            Debug.Log("Guardando escena...");
            BunkerState.Instance.SaveBunkerState();
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Añade un pequeño delay para asegurar que todo está cargado
        yield return new WaitForSeconds(0.1f);

        if (sceneName == "Bunker")
        {
            isGuardianVisionUnlocked = true;
            BunkerState.Instance.RestoreBunkerState();
            yield return new WaitForEndOfFrame(); // Espera a que termine el frame

            if (PlayerManager.Instance != null)
            {
                Debug.Log($"Intentando mover jugador a: {newPosition}");
                PlayerManager.Instance.SetPosition(newPosition, newRotation);
            }
        }
        else
        {
            // Para el caso del plano astral
            yield return new WaitForSeconds(0.1f);
            if (PlayerManager.Instance != null)
            {
                PlayerManager.Instance.transform.position = newPosition;
                PlayerManager.Instance.transform.rotation = newRotation;
            }
        }

        yield return StartCoroutine(Fade(0));
        transitionCanvas.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = transitionCanvas.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            transitionCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        transitionCanvas.alpha = targetAlpha;
    }

    public void Morse()
    {
        morsecode = true;
        EndGame();
    }

    public void Lights()
    {
        lightcode = true;
        EndGame();
    }

    private void EndGame()
    {
        if(lightcode && morsecode)
        {
            finalDoor.SetActive(false);
            tutorialManager.Exit();
        }
    }

    public void ShowEnd()
    {
        StartCoroutine(End());
        
    }

    IEnumerator End()
    {
        transitionCanvas.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(4));
        dialogue.gameObject.SetActive(true);
        dialogue.StartEnding();
    }
    public void RemoveFinalText()
    {
        tutorialManager.gameObject.SetActive(false);
    }
}
