using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverPlane : MonoBehaviour
{

    public float upwardThrust = 250;
    public float forwardThrust = 400f;
    public float turnThrust = 300f;
    public float hoverSensorRange;
    //public float hoverHeight;
    [Space]
    public GameObject propulsion;
    public GameObject centerMass;
    public GameObject[] sensors;

    private Rigidbody rB;
    private RaycastHit hit;

    private void Start()
    {
        rB = GetComponent<Rigidbody>();
        rB.centerOfMass = centerMass.transform.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        rB.AddForceAtPosition(Time.deltaTime * transform.TransformDirection(Vector3.forward) * Input.GetAxis("Vertical") * forwardThrust, propulsion.transform.position);
        rB.AddTorque(Time.deltaTime * transform.TransformDirection(Vector3.up) * Input.GetAxis("Horizontal") * turnThrust);

    }

    private void FixedUpdate()
    {

        foreach (GameObject Sensor in sensors)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, hoverSensorRange, LayerMask.GetMask("Ground")))
            {
                rB.AddForceAtPosition(Time.deltaTime * transform.TransformDirection(Vector3.up) * Mathf.Pow(3f - hit.distance, 2) / 3f * upwardThrust, Sensor.transform.position);
            }
        }
        rB.AddForce(-Time.deltaTime * transform.TransformVector(Vector3.right) * transform.InverseTransformVector(rB.velocity).x * 5f);
    }

}
