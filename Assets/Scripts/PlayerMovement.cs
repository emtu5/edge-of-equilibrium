using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player1 : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.maxAngularVelocity = maxAngVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            forwardKeyPressed = true;
            //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + _speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            backwardKeyPressed = true;
            //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - _speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            leftKeyPressed = true;
            //transform.position = new Vector3(transform.position.x - _speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rightKeyPressed = true;
            //transform.position = new Vector3(transform.position.x + _speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        if(forwardKeyPressed)
        {
            rigidbody.AddForce(new Vector3(0, 0, _speed));
            forwardKeyPressed = false;
        }
        if(leftKeyPressed)
        {
            rigidbody.AddForce(new Vector3(-_speed, 0, 0));
            leftKeyPressed = false;
        }
        if(rightKeyPressed)
        {
            rigidbody.AddForce(new Vector3(_speed, 0, 0));
            rightKeyPressed = false;
        }
        if (backwardKeyPressed)
        {
            rigidbody.AddForce(new Vector3(0, 0, -_speed));
            backwardKeyPressed = false;
        }
    }
}
