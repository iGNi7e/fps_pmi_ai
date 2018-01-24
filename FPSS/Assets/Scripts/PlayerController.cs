using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour { //Input commands

    [SerializeField]
    float speed = 5f;
    [SerializeField]
    float sensivity = 2f;
    [SerializeField]
    private float thrusterForce = 1000f;

    [Header("Spring settings: ")]
    [SerializeField]
    private JointDriveMode jointMode = JointDriveMode.Position;
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    private ConfigurableJoint joint;
    private PlayerMovement movement;

    private void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
        movement = GetComponent<PlayerMovement>();
        SetJointSettings(jointSpring);
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

        float _cameraRotationX = _xRot * sensivity; //create a vector for rotate ONLY camera to up and down

        movement.RotateCamera(_cameraRotationX);  //set a vector for rotate ONLY camera to left and right

        Vector3 _thrusterForce = Vector3.zero;
        if (Input.GetButton("Jump"))
        {
            _thrusterForce = Vector3.up * thrusterForce;
            SetJointSettings(0f);
        }
        else
        {
            SetJointSettings(jointSpring);
        }
        movement.ApplyThruster(_thrusterForce);
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            mode = jointMode,
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }
}
