using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public enum RotationType
{
    Left,
    Right
}

[System.Serializable]
public class Position
{
    public int n;
    public RotationType rot;
}

public class SafeboxPuzzle : MonoBehaviour, IPuzzle
{

    public GameObject door;
    public Collider interactionZone;

    public float rotationSpeed = 50f;
    public int numbersAvailable = 12;
    public Position[] solution;

    private RotationType orientation;
    private int verificationpoint = 0;
    private bool Interactable = false;
    private bool isUnlocked = false;
    private bool Rotating = false;
    private float varemo;
    private float acumulatedRotation = 0f;
    private Coroutine verificationCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        varemo = 360f / numbersAvailable; //As� sabemos el varemo entre los n�meros
    }

    // Update is called once per frame
    void Update()
    {
        if (Interactable)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                // Si estaba verificando, cancelamos la verificaci�n
                if (verificationCoroutine != null)
                {
                    StopCoroutine(verificationCoroutine);
                    verificationCoroutine = null;
                }

                // Rotamos seg�n la tecla presionada
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    Rotate('l');
                }
                else
                {
                    Rotate('r');
                }
            }
            else if (Rotating)
            {
                // Si no hay ninguna verificaci�n en curso, iniciamos una nueva
                if (verificationCoroutine == null)
                {
                    verificationCoroutine = StartCoroutine(VerifyPosition());
                }
            }
        }
    }

    public void Interact(InventorySystem inventory)
    {
        Debug.Log("Se llamo a interact");
        Interactable = true;
    }

    public void StopInteract()
    {
        Debug.Log("Se sali� del Puzzle");
        Interactable = false;
    }

    void Rotate(char r)
    {
        orientation = (r == 'l') ? RotationType.Left : RotationType.Right; //Depende de la letra guardamos su orientaci�n

        Rotating = true; 
        float Rotation = rotationSpeed * Time.deltaTime; //Calculamos la rotaci�n
        if (r == 'r')
        {
            transform.Rotate(Vector3.down, Rotation, Space.Self); // Rota en el eje Y en sentido horario
            acumulatedRotation += Rotation;
        }
        else
        {
            transform.Rotate(Vector3.up, Rotation, Space.Self); // Rota en el eje Y en sentido antihorario
            acumulatedRotation -= Rotation;
        }

        while (acumulatedRotation < 0)
        {
            acumulatedRotation += 360f;
        }
        while (acumulatedRotation >= 360f)
        {
            acumulatedRotation -= 360f;
        }

        //DEBUG:
        int numero = (int)Mathf.Round(acumulatedRotation / varemo) % (numbersAvailable + 1);

        Debug.Log(numero);
    }

    private IEnumerator VerifyPosition()
    {
        Rotating = false;
        yield return new WaitForSeconds(1f); //En un segundo

        int numeroFinal = (int)Mathf.Round(acumulatedRotation / varemo) % (numbersAvailable + 1); //Conseguimos tras 1 segundo el n�mero actual
        Debug.Log($"Posici�n final: {numeroFinal}");

        if (solution[verificationpoint].n == numeroFinal &&
        ((solution[verificationpoint].rot == RotationType.Left && orientation == RotationType.Left) ||
        (solution[verificationpoint].rot == RotationType.Right && orientation == RotationType.Right))) //Si n�mero actual y rotaci�n actual es correcta
        {
            SoundManager.Instance.PlaySFX("click");
            verificationpoint++;
        }
        else
        {
            verificationpoint = 0;
        }

        if (verificationpoint == solution.Length) //Si se consigue todo, se abre la puerta
        {
            OpenLock();
        }

        verificationCoroutine = null;
    }

    private void OpenLock()
    {
        door.GetComponent<DoorObject>().OpenDoorWithoutPivot(Vector3.back);
        isUnlocked = true;
        interactionZone.tag = "Untagged";
        interactionZone.enabled = false;
    }
}
