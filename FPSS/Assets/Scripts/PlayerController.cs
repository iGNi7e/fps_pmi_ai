using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour { //Input commands

    [SerializeField]
    float speed = 5f;
    [SerializeField]
    float sensivity = 2f;

    private PlayerMovement movement;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        //input keys from keyboard
        float _xMov = Input.GetAxis("Horizontal"); 
        float _zMov = Input.GetAxis("Vertical");

        //vectors for movement player
        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;
        
        Vector3 _velocity = (_movHorizontal + _movVertical).normalized * speed; //main movement vector for method Move

        movement.Move(_velocity); //set movement vector

        float _yRot = Input.GetAxisRaw("Mouse X"); //input mouse rotation to left and right

        Vector3 _rotation = new Vector3(0f,_yRot,0f) * sensivity; //create a vector for rotate person and camera to left and right

        movement.Rotate(_rotation); //set a vector for rotate person and camera to left and right

        float _xRot = Input.GetAxisRaw("Mouse Y") * -1; //input mouse rotation to up and down

        Vector3 _cameraRotation = new Vector3(_xRot,0f,0f) * sensivity; //create a vector for rotate ONLY camera to up and down

        movement.RotateCamera(_cameraRotation);  //set a vector for rotate ONLY camera to left and right
    }

}
