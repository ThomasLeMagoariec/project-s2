using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;
    public Transform combatLookAt;
    
    [Header("Cameras")]
    public GameObject basicCamera;
    public GameObject combatCamera;
    public GameObject topdownCamera;

    public KeyCode basicCameraKey;
    public KeyCode combatCameraKey;
    public KeyCode topdownCameraKey;
    

    [Header("Other")]
    public float rotationSpeed;
    


    public CameraStyle currentStyle;
    public enum CameraStyle 
    {
        Basic,
        Combat,
        TopDown
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() 
    {
        if(Input.GetKeyDown(basicCameraKey)) SwitchCameraStyle(CameraStyle.Basic);
        if(Input.GetKeyDown(combatCameraKey)) SwitchCameraStyle(CameraStyle.Combat);
        if(Input.GetKeyDown(topdownCameraKey)) SwitchCameraStyle(CameraStyle.TopDown);
        

        Vector3 viewDirection = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDirection.normalized;

        if(currentStyle == CameraStyle.Basic || currentStyle == CameraStyle.TopDown)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            if(inputDirection != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDirection.normalized, Time.deltaTime * rotationSpeed);
            }
        }
        else if(currentStyle == CameraStyle.Combat)
        {
            Vector3 combatDirection = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = combatDirection.normalized;

            playerObj.forward = combatDirection.normalized;
        }
        

    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        basicCamera.SetActive(false);
        combatCamera.SetActive(false);
        topdownCamera.SetActive(false);

        if(newStyle == CameraStyle.Basic) basicCamera.SetActive(true);
        if(newStyle == CameraStyle.Combat) combatCamera.SetActive(true);
        if(newStyle == CameraStyle.TopDown) topdownCamera.SetActive(true);
        
        currentStyle = newStyle;
    }
}
