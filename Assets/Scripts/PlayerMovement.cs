using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    bool isCheatMode = false; // Modo chetos desactivado al inicio

    void Update()
    {
        // Alternar modo chetos con F3 + F4
        if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.F4))
        {
            isCheatMode = !isCheatMode;
            velocity = Vector3.zero; // Reinicia la velocidad

            if (isCheatMode)
                controller.enabled = false; // Desactiva colisiones
            else
                controller.enabled = true; // Activa colisiones
        }

        if (isCheatMode)
        {
            CheatModeMovement();
        }
        else
        {
            NormalMovement();
        }
    }

    void NormalMovement()
    {
        // Comprueba si el jugador está en el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Obtiene la entrada del teclado
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calcula el movimiento
        Vector3 move = transform.right * x + transform.forward * z;

        // Aplica el movimiento
        controller.Move(move * speed * Time.deltaTime);

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Aplica la gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void CheatModeMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float y = 0f;

        if (Input.GetKey(KeyCode.Space)) y = 1f; // Subir
        if (Input.GetKey(KeyCode.LeftShift)) y = -1f; // Bajar

        // Movimiento libre sin colisiones
        Vector3 move = (transform.right * x + transform.forward * z + transform.up * y) * speed * Time.deltaTime;
        transform.position += move;
    }
}
