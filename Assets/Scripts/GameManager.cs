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
        SaveGuardianCoords(GameObject.FindGameObjectWithTag("Player").transform.position);
        SaveGuardianRotation(GameObject.FindGameObjectWithTag("Player").transform.rotation);
        StartCoroutine(Transition("AstralPlane", GetBunkerCoords(), GetBunkerRotation()));
    }

    private IEnumerator Transition(string sceneName, Vector3 newPosition, Quaternion newRotation)
    {
        transitionCanvas.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(1));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = newPosition;
            player.transform.rotation = newRotation;
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
}
