using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5;
    [SerializeField]
    private float maxAngVelocity = 10;

    private Rigidbody rigidbody;
    private bool forwardKeyPressed;
    private bool backwardKeyPressed;
    private bool leftKeyPressed;
    private bool rightKeyPressed;

    private bool _isControllable = true;

    public Transform cameraTransform; 
	
	public bool useGravity = true;


    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.maxAngularVelocity = maxAngVelocity;
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
		//gravity for paper
		/*BallMaterial ballMaterial = GetComponent<MaterialController>().ballMaterial;

		if (ballMaterial.name == "Paper")
		{
			rigidbody.useGravity = false;

			Debug.Log(rigidbody.mass*rigidbody.mass);
			if (useGravity) {
				rigidbody.AddForce(Physics.gravity*0.000048f);
			}
		}else
		{
			rigidbody.useGravity = true;

		}*/
		
		
        // check for key presses
        forwardKeyPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        backwardKeyPressed = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        leftKeyPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        rightKeyPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // y=0 => horizontal movement
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        // ensure consistent speed
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movement = Vector3.zero;

        // movement relative to the camera
        if (forwardKeyPressed)
        {
            movement += cameraForward; 
        }
        if (backwardKeyPressed)
        {
            movement -= cameraForward; 
        }
        if (leftKeyPressed)
        {
            movement -= cameraRight; 
        }
        if (rightKeyPressed)
        {
            movement += cameraRight; 
        }

        if (_isControllable)
        {
            rigidbody.AddForce(movement * _speed, ForceMode.Force);
        } else
        {
            Debug.Log("YOO");
        }
    }

    public void setSpeed(float speed)
    {
        _speed = speed;
    }

    public void setIsControllable(bool isControllable)
    {
        _isControllable = isControllable;
    }
}
