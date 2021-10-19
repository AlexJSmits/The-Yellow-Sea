using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverPlane2 : MonoBehaviour
{

    private Rigidbody rB;
    private float deadZone = 0.1f;
    private float currThrust = 0;
    private float currTurn = 0;
    private int layerMask;

    public float forwardAcl = 100f;
    public float backwardsAcl = 25f;
    public float turnStrength = 10;

    public float hoverForce = 9;
    public float hoverHeight = 2;
    public GameObject[] hoverPoints;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rB = GetComponent<Rigidbody>();

        layerMask = 1 << LayerMask.NameToLayer("Ship");
        layerMask = ~layerMask;
    }

    // Update is called once per frame
    void Update()
    {
        currThrust = 0;
        float aclAxis = Input.GetAxis("Vertical");
        if(aclAxis > deadZone)
        {
            currThrust = aclAxis * forwardAcl;
        }
        else if (aclAxis < -deadZone)
        {
            currThrust = aclAxis * backwardsAcl;
        }

        currTurn = 0;
        float turnAxis = Input.GetAxis("Horizontal");
        if (Mathf.Abs(turnAxis) > deadZone)
        {
            currTurn = turnAxis;
        }
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        for (int i = 0; i < hoverPoints.Length; i++)
        {
            var hoverPoint = hoverPoints[i];
            if(Physics.Raycast(hoverPoint.transform.position, Vector3.down, out hit, hoverHeight, layerMask))
            {
                rB.AddForceAtPosition(Vector3.up * hoverForce * (1 - (hit.distance / hoverHeight)), hoverPoint.transform.position);
            }
            else
            {
                if (transform.position.y > hoverPoint.transform.position.y)
                {
                    rB.AddForceAtPosition(hoverPoint.transform.up * hoverForce, hoverPoint.transform.position);
                }
                else
                {
                    rB.AddForceAtPosition(hoverPoint.transform.up * -hoverForce, hoverPoint.transform.position);
                }
            }
        }

        if (Mathf.Abs(currThrust) > 0)
        {
            rB.AddForce(transform.forward * currThrust);
        }

        if(currTurn > 0)
        {
            rB.AddRelativeTorque(Vector3.up * currTurn * turnStrength);
        }
        else if (currTurn < 0)
        {
            rB.AddRelativeTorque(Vector3.up * currTurn * turnStrength);
        }
    }
}
