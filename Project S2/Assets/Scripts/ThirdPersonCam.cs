using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    public Transform playerPos;
    
    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.Find("Player/Orientation").transform;
        this.transform.position = playerPos.position;
        Debug.Log("Camera position set to player position");
        Debug.Log("Camera position: " + this.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = playerPos.position;
    }
}
