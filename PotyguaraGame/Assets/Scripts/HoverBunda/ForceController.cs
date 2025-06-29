using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class ForceController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions; 
    private InputAction leftJoystick;
    private InputAction rightJoystick;

    private Rigidbody rb;

    private bool gameStarted = false;
    private InputAction moveAction;

    public float aceleração = 5f;  // Aceleração constante do hoverboard
    public float velocidadeMaxima = 10f;  // Limite de velocidade

    /* private void OnEnable()
     {
         var actionMap = inputActions.FindActionMap("Joysticks");
         leftJoystick = actionMap.FindAction("Primary2DAxis");
         rightJoystick = actionMap.FindAction("Secondary2DAxis");

         leftJoystick.Enable();
         rightJoystick.Enable();
     }

     private void OnDisable()
     {
         leftJoystick.Disable();
         leftJoystick.Disable();
     }*/

    public void SetBoolean()
    {
        gameStarted = true;
        transform.position = new Vector3(540f, 56.2f, -509f);
        transform.rotation = new Quaternion(0, -0.361624479f, 0, 0.932323813f);
    }

    /*void Update()
    {
        if (gameStarted)
        {
            if (rb.velocity.magnitude < velocidadeMaxima)
            {
                rb.AddForce(transform.forward * aceleração, ForceMode.Acceleration);
            }

            // Chame a função para alterar a direção do movimento
            AlterarDirecao();
        }
    }*/
    public float forwardSpeed = 10f;  // Velocidade automática para frente
    public float turnSpeed = 60f;     // Velocidade de rotação
    public float hoverHeight = 2f;    // Altura de levitação
    public float hoverForce = 10f;    // Força de sustentação


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;  // Desativar gravidade para efeito de levitação

        // Configurar a ação do OpenXR para o thumbstick
        moveAction = new InputAction("Move", binding: "<XRController>{LeftHand}/thumbstick");
        moveAction.Enable();
    }

    void FixedUpdate()
    {
        // Simular levitação usando Raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, hoverHeight))
        {
            float forceAmount = hoverForce * (1f - (hit.distance / hoverHeight));
            rb.AddForce(Vector3.up * forceAmount, ForceMode.Acceleration);
        }

        // Movimento automático para frente
        rb.velocity = transform.forward * forwardSpeed;

        // Controle de direção via thumbstick do controle esquerdo
        Vector2 input = moveAction.ReadValue<Vector2>();
        float turn = input.x * turnSpeed * Time.fixedDeltaTime;
        transform.Rotate(Vector3.up, turn);
    }



    void HandleMovementWithOculus()
    {
        Vector2 leftInput = leftJoystick.ReadValue<Vector2>();
        Vector2 rightInput = rightJoystick.ReadValue<Vector2>();

        Vector3 movement = new Vector3(leftInput.x, 0, leftInput.y);
        transform.Translate(movement * Time.deltaTime, Space.World);

        Vector3 rotation = new Vector3(0, rightInput.x, 0);
        transform.Rotate(rotation * Time.deltaTime, Space.World);
    }

    void AlterarDirecao()
    {

        Vector2 leftInput = leftJoystick.ReadValue<Vector2>();
        Vector2 rightInput = rightJoystick.ReadValue<Vector2>();

        Vector3 direcao = new Vector3(leftInput.x, 0, leftInput.y).normalized;

        if (direcao.magnitude > 0)
        {
            // Rotaciona o hoverboard para a direção do movimento
            Quaternion novaRotacao = Quaternion.LookRotation(direcao);
            transform.rotation = Quaternion.Slerp(transform.rotation, novaRotacao, Time.deltaTime * 10);
        }
    }
}
