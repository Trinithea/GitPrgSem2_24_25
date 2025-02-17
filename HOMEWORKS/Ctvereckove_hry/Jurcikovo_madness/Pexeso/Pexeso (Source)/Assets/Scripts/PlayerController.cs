using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 3.0f;
    private bool isGrounded;
    
    [Header("Mouse Look")]
    public float mouseSensitivity = 2.0f;
    public float verticalRotationLimit = 90f;
    
    private float verticalRotation = 0f;
    private Camera playerCamera;
    private CharacterController characterController;
    
    
    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }
    
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        transform.Rotate(0, mouseX, 0);
        
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);
        
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
    
    private void HandleMovement()
    {
        
        float moveForward = Input.GetAxis("Vertical") * speed;
        float moveSide = Input.GetAxis("Horizontal") * speed;
        
        Vector3 move = transform.forward * moveForward + transform.right * moveSide;
        move *= Time.deltaTime;
        
        if (Input.GetAxis("Jump") > 0)
        {
            move.y += jumpHeight * Time.deltaTime;
        }
        else
            move.y += gravity * Time.deltaTime;
        characterController.Move(move);
    }
}
