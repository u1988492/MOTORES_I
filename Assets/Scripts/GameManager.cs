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
    public Vector3 positionGuardian;

    public CanvasGroup transitionCanvas;
    public float fadeDuration = 1.5f;

    public GameObject guardianVisionObjects;

    private string currentAmbientSound;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } 
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        guardianVisionObjects.SetActive(false);
        UpdateAmbientSound(SceneManager.GetActiveScene().name); // initialize ambient sound
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

    public Vector3 GetBunkerCoords()
    {
        return positionBunker;
    }

    public void SaveGuardianCoords(Vector3 pos)
    {
        positionGuardian = pos;
    }

    public Vector3 GetGuardianCoords()
    {
        return positionGuardian;
    }


    public void tpGuardian()
    {
        SaveBunkerCoords(GameObject.FindGameObjectWithTag("Player").transform.position);
        StartCoroutine(Transition("AstralPlane", GetBunkerCoords()));
    }

    public void tpBunker()
    {
        SaveGuardianCoords(GameObject.FindGameObjectWithTag("Player").transform.position);
        StartCoroutine(Transition("Bunker", GetBunkerCoords())); 
    }

    private IEnumerator Transition(string sceneName, Vector3 newPosition)
    {
        transitionCanvas.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(1));

        if(!string.IsNullOrEmpty(currentAmbientSound)){
            SoundManager.Instance.StopAmbientSound(currentAmbientSound); // stop sound between scenes
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = newPosition;
        }

        UpdateAmbientSound(sceneName);

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

    // updates the ambient sound according to the scene
    private void UpdateAmbientSound(string sceneName){
        string newAmbientSound = sceneName == "Bunker" ? "BunkerAmbient" : "AstralAmbient";

        if(currentAmbientSound != newAmbientSound){
            if(!string.IsNullOrEmpty(currentAmbientSound)){
                SoundManager.Instance.StopAmbientSound(currentAmbientSound); // stop sound if there is no set sound
            }
            currentAmbientSound = newAmbientSound;
            SoundManager.Instance.PlayAmbientSound(currentAmbientSound);
        }
    }
}
