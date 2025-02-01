using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPuzzle : MonoBehaviour
{
    public int numberLights = 8;
    public List<bool> solutionLights;
    
    private List<bool> userInput;
    
    // Start is called before the first frame update
    void Start()
    {
        userInput = new List<bool>();
        for(var i = 0; i < numberLights; i++)
        {
            userInput.Add(false);
        }
    }

    public void LightClicked(int number)
    {
        userInput[number] = !userInput[number];

        VerifySolution();
    }

    void VerifySolution()
    {
        if (GameManager.instance.IsVisionUnlocked())
        {
            bool isCorrect = true;
            for (int i = 0; i < numberLights; i++)
            {
                if (solutionLights[i] != userInput[i])
                {
                    isCorrect = false;
                    break;
                }
            }

            if (isCorrect)
            {
                Debug.Log("Felicidades");
            }
            else
            {
                Debug.Log("Vuelve a casa");
            }
        }
        
    }

}
