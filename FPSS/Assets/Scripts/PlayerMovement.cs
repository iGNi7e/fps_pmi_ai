using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float cameraRotationLimit = 85f;

    private float currentCamRotationX = 0f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float rotationCameraX = 0;
    private Vector3 thrusterForce = Vector3.zero;

    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Get a movement vector
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }
    public void RotateCamera(float _rotationCameraX)
    {
        rotationCameraX = _rotationCameraX;
    }
    // Get a force vector for our player
    public void ApplyThruster(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }

    // Run every physics iteration
    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.transform.position + velocity * Time.fixedDeltaTime);
        }
        if (thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    void PerformRotation()
    {
        if (rotation != Vector3.zero)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        }
        if (cam != null)
        {
            currentCamRotationX += rotationCameraX;
            currentCamRotationX = Mathf.Clamp(currentCamRotationX, -cameraRotationLimit, cameraRotationLimit);
            cam.transform.localEulerAngles = new Vector3(currentCamRotationX, 0f, 0f);
        }
    }
    

}
