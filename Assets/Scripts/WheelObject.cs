/*Este script controla la rotación individual del objeto 'WHEEL'
El numberShown maximo dependerña dek número de caras que tenga la rueda - cambiar cuando sea necesario
La Rotation de la rueda dependerá de los atributos  Transform attributes de la rueda - cambiar cuando sea necesario
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Wheel : MonoBehaviour, IPuzzle {

    public static event Action<string, int> Rotated = delegate {}; //envía la información necesaria al script WheelPuzzleControl
    private bool coroutineAllowed; //si la rueda no está rodando, será true
    private int numberShown; //número que se muestra en la parte frontal de la rueda

    private void Start() {
        coroutineAllowed = true;
        numberShown = 0;
    }

    public void Interact(InventorySystem inventory) { //Llama a RotateWheel solo cuando esté en la interfaz IPuzzle
        if(coroutineAllowed){
            StartCoroutine("RotateWheel");
        }
    }

    private IEnumerator RotateWheel() { //función que rota la rueda

        coroutineAllowed = false;

        for (int i = 0; i <= 11; i++){
            transform.Rotate( 0f, 3f, 0f);
            yield return new WaitForSeconds(0.01f);
        }

        coroutineAllowed = true;
        numberShown += 1;

        if (numberShown > 9){
            numberShown = 0;
        }

        Rotated(name, numberShown); //se actualizan los valores en WheelPuzzleControl
    }
}