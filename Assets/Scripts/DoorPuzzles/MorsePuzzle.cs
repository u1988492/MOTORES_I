using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public enum States
{
    SHORT,
    LONG,
}

public class Letter
{
    public int num;
    public char let;

    public Letter()
    {
        num = 1;
        let = 'A';
    }
}

public class MorsePuzzle : MonoBehaviour
{

    Dictionary<char, List<States>> morseCode = new Dictionary<char, List<States>>()
    {
        {'A', new List<States>() { States.SHORT, States.LONG} },
        {'B', new List<States>() { States.LONG, States.SHORT, States.SHORT, States.SHORT }},
        {'C', new List<States>() { States.LONG, States.SHORT, States.LONG, States.SHORT }},
        {'D', new List<States>() { States.LONG, States.SHORT, States.SHORT }},
        {'E', new List<States>() { States.SHORT }},
        {'F', new List<States>() { States.SHORT, States.SHORT, States.LONG, States.SHORT }},
        {'G', new List<States>() { States.LONG, States.LONG, States.SHORT }},
        {'H', new List<States>() { States.SHORT, States.SHORT, States.SHORT, States.SHORT }},
        {'I', new List<States>() { States.SHORT, States.SHORT }},
        {'J', new List<States>() { States.SHORT, States.LONG, States.LONG, States.LONG }},
        {'K', new List<States>() { States.LONG, States.SHORT, States.LONG }},
        {'L', new List<States>() { States.SHORT, States.LONG, States.SHORT, States.SHORT }},
        {'M', new List<States>() { States.LONG, States.LONG }},
        {'N', new List<States>() { States.LONG, States.SHORT }},
        {'O', new List<States>() { States.LONG, States.LONG, States.LONG }},
        {'P', new List<States>() { States.SHORT, States.LONG, States.LONG, States.SHORT }},
        {'Q', new List<States>() { States.LONG, States.LONG, States.SHORT, States.LONG }},
        {'R', new List<States>() { States.SHORT, States.LONG, States.SHORT }},
        {'S', new List<States>() { States.SHORT, States.SHORT, States.SHORT }},
        {'T', new List<States>() { States.LONG }},
        {'U', new List<States>() { States.SHORT, States.SHORT, States.LONG }},
        {'V', new List<States>() { States.SHORT, States.SHORT, States.SHORT, States.LONG }},
        {'W', new List<States>() { States.SHORT, States.LONG, States.LONG }},
        {'X', new List<States>() { States.LONG, States.SHORT, States.SHORT, States.LONG }},
        {'Y', new List<States>() { States.LONG, States.SHORT, States.LONG, States.LONG }},
        {'Z', new List<States>() { States.LONG, States.LONG, States.SHORT, States.SHORT }},
    };

    Dictionary<int, char> letterPos = new Dictionary<int, char>() {
        {1,'A'},
        {2,'B'},
        {3,'C'},
        {4,'D'},
        {5,'E'},
        {6,'F'},
        {7,'G'},
        {8,'H'},
        {9,'I'},
        {10,'J'},
        {11,'K'},
        {12,'L'},
        {13,'M'},
        {14,'N'},
        {15,'O'},
        {16,'P'},
        {17,'Q'},
        {18,'R'},
        {19,'S'},
        {20,'T'},
        {21,'U'},
        {22,'V'},
        {23,'W'},
        {24,'X'},
        {25,'Y'},
        {26,'Z'},
    };

    public List<char> word;
    public List<TMP_Text> lettersText;
    public float timeBetweenWord = 5f;
    public float timeBetweenLetters = 3f;
    public float timeBetweenStates = 2f;
    public float timeLong = 2f;
    public float timeShort = 1f;

    public GameObject lightMain;

    private List<Letter> userInput;
    public Light pointLight;
    private Material lightMaterial;
    private MeshRenderer meshRenderer;

    private bool isWordShowing = false;

    void Start()
    {
        //pointLight = lightMain.GetComponentInChildren<Light>();
        meshRenderer = lightMain.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            lightMaterial = meshRenderer.material;
        }

        userInput = new List<Letter>();
        for (int i = 0; i < word.Count; i++)
        {
            userInput.Add(new Letter());
            lettersText[i].text = "A";
        }

        StartCoroutine(ShowWord());
    }

    void Update()
    {
        if (!isWordShowing)
        {
            StartCoroutine(ShowWord());
        }
    }

    IEnumerator ShowWord()
    {
        isWordShowing = true;
        foreach(var l in word)
        {
            List<States> listLights = morseCode[l];
            foreach(var l2 in listLights)
            {
                float time;
                if (l2.Equals(States.LONG))
                {
                    time = timeLong;
                }
                else
                {
                    time = timeShort;
                }

                OperateLight(true);
                yield return new WaitForSeconds(time);

                OperateLight(false);
                yield return new WaitForSeconds(timeBetweenStates);
            }
            OperateLight(false);
            yield return new WaitForSeconds(timeBetweenLetters);
        }
        OperateLight(false);
        yield return new WaitForSeconds(timeBetweenWord);
        isWordShowing = false;
    }

    void OperateLight(bool state)
    {
        if (GameManager.instance.IsVisionActive())
        {
            pointLight.enabled = state;

            if (state) lightMaterial.EnableKeyword("_EMISSION");
            else lightMaterial.DisableKeyword("_EMISSION");
        }
        else
        {
            pointLight.enabled = false;
            lightMaterial.DisableKeyword("_EMISSION");
        }
    }

    public void LetterUp(int pos)
    {
        Debug.Log("Pulsaste pos: " + pos + " tiene número: " + userInput[pos].num);
        if (userInput[pos].num > 1)
        {
            userInput[pos].num -= 1;
            userInput[pos].let = letterPos[userInput[pos].num];
            lettersText[pos].text = letterPos[userInput[pos].num].ToString();

        }
        Debug.Log("Y ahora tiene número: " + userInput[pos].num);
        CheckSolution();
    }

    public void LetterDown(int pos)
    {
        Debug.Log("Pulsaste pos: " + pos + " tiene número: " + userInput[pos].num);
        if (userInput[pos].num < 26)
        {
            userInput[pos].num += 1;
            userInput[pos].let = letterPos[userInput[pos].num];
            lettersText[pos].text = letterPos[userInput[pos].num].ToString();

        }
        Debug.Log("Y ahora tiene número: " + userInput[pos].num);
        CheckSolution();
    }

    public char GetLetter(int pos)
    {
        return userInput[pos].let;
    }

    void CheckSolution()
    {
        if (GameManager.instance.IsVisionUnlocked())
        {
            bool isSolutionCorrect = true;
            for (int i = 0; i < word.Count; i++)
            {
                if (userInput[i].let != word[i])
                {
                    isSolutionCorrect = false;
                    break;
                }
            }

            if (isSolutionCorrect) Debug.Log("Ganaste");
        }
    }
}

