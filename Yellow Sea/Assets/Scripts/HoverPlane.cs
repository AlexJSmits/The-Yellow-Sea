using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class HoverPlane : MonoBehaviour
{
  Rigidbody m_body;
  float m_deadZone = 0.1f;

      public float m_hoverForce = 9.0f;
      public float m_hoverHeight = 2.0f;
      public float m_maxHoverHeight = 10f;
    public float m_minHoverHeight = 3f;
      public GameObject[] m_hoverPoints;
    public GameObject m_exitPoint;

      public float m_forwardAcl = 100.0f;
      public float m_backwardAcl = 25.0f;
      float m_currThrust = 0.0f;

      public float m_turnStrength = 10f;
      float m_currTurn = 0.0f;

      int m_layerMask;

    public GameObject m_player;
    public GameObject m_playerCam;

    [Space]

    public GameObject playerVisual;

  void Start()
  {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_playerCam = GameObject.FindGameObjectWithTag("PlayerCam");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_body = GetComponent<Rigidbody>();

        m_layerMask = 1 << LayerMask.NameToLayer("Ship");
        m_layerMask = ~m_layerMask;

        this.enabled = false;

    }

  void OnDrawGizmos()
  {
  
        //  Hover Force
        RaycastHit hit;
        for (int i = 0; i < m_hoverPoints.Length; i++)
        {
              var hoverPoint = m_hoverPoints [i];
              if (Physics.Raycast(hoverPoint.transform.position, 
                                  -Vector3.up, out hit,
                                  m_hoverHeight, 
                                  m_layerMask))
              {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(hoverPoint.transform.position, hit.point);
                Gizmos.DrawSphere(hit.point, 0.5f);
              } else
              {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(hoverPoint.transform.position, 
                               hoverPoint.transform.position - Vector3.up * m_hoverHeight);
              }
        }
  }
	
  void Update()
  {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExitShip();
        }

        // Main Thrust
        m_currThrust = 0.0f;
        float aclAxis = Input.GetAxis("Vertical");
        if (aclAxis > m_deadZone)
          m_currThrust = aclAxis * m_forwardAcl;
        else if (aclAxis < -m_deadZone)
          m_currThrust = aclAxis * m_backwardAcl;

        // Turning
        m_currTurn = 0.0f;
        float turnAxis = Input.GetAxis("Horizontal");
        if (Mathf.Abs(turnAxis) > m_deadZone)
          m_currTurn = turnAxis;

        if (Input.GetKey(KeyCode.Space) && m_hoverHeight < m_maxHoverHeight)
        {
            m_hoverHeight += (3f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftControl) && m_hoverHeight > m_minHoverHeight)
        {
            m_hoverHeight -= (3f * Time.deltaTime);
        }
    }

  void FixedUpdate()
  {

        //  Hover Force
        RaycastHit hit;
        for (int i = 0; i < m_hoverPoints.Length; i++)
        {
              var hoverPoint = m_hoverPoints [i];
              if (Physics.Raycast(hoverPoint.transform.position, 
                                  -Vector3.up, out hit,
                                  m_hoverHeight,
                                  m_layerMask))
                m_body.AddForceAtPosition(Vector3.up 
                  * m_hoverForce
                  * (1.0f - (hit.distance / m_hoverHeight)), 
                                          hoverPoint.transform.position);
          else
          {
                if (transform.position.y > hoverPoint.transform.position.y)
                  m_body.AddForceAtPosition(
                    hoverPoint.transform.up * m_hoverForce,
                    hoverPoint.transform.position);
                else
                  m_body.AddForceAtPosition(
                    hoverPoint.transform.up * -m_hoverForce,
                    hoverPoint.transform.position);
          }
        }

        // Forward
            if (Mathf.Abs(m_currThrust) > 0)
                m_body.AddForce(transform.forward * m_currThrust);

        // Turn
        if (m_currTurn > 0)
        {
            m_body.AddRelativeTorque(Vector3.up * m_currTurn * m_turnStrength);
        } else if (m_currTurn < 0)
        {
            m_body.AddRelativeTorque(Vector3.up * m_currTurn * m_turnStrength);
        }
  }

    void ExitShip()
    {
        //overlap sphere on left and right hand side of the ship 
        //if no collision detected then player is moved to location and toggled on
        //ship script is turned off


        Collider[] hitColliders = Physics.OverlapSphere(m_exitPoint.transform.position, 1.0f, m_layerMask);

        if (hitColliders.Length == 0)
        {

            m_player.transform.position = m_exitPoint.transform.position;
            m_player.transform.rotation = m_exitPoint.transform.rotation;

            m_player.SetActive(true);
            m_playerCam.SetActive(true);


            this.enabled = false;
        }
        
    }

    private void OnEnable()
    {
        playerVisual.SetActive(true);
    }

    private void OnDisable()
    {
        playerVisual.SetActive(false);
    }
}
