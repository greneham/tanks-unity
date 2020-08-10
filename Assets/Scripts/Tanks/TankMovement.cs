using UnityEngine;
using UnityEngine.AI;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public AudioSource m_MovementAudio;
    public AudioClip m_EngineIdling;
    public AudioClip m_EngineDriving;
    public float m_PitchRange = 0.2f;
    public NavMeshAgent agent;
    public bool m_EnableClickToMove = false;
    public GameObject destinationMarker;
    public ControlMode m_ControlMode;

    private Camera cam;
    private Rigidbody m_Rigidbody;
    private float m_OriginalPitch;
    private bool m_IsEnroute = false;
    private RaycastHit nextPosition;
    private float m_CPUThinkTime = 1f; // some latency when CPU decides where to move to


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    private void OnEnable()
    {
        m_Rigidbody.isKinematic = false;
    }

    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;
    }

    private void Start()
    {
        //m_OriginalPitch = m_MovementAudio.pitch;
    }

    public void MoveTank()
    {
        // MOVE OUR AGENT
        agent.SetDestination(nextPosition.point);
        m_IsEnroute = true;
    }

    private void Update()
    {

        if (m_EnableClickToMove)
        {
            bool m_isValid = isValidPosition(Input.mousePosition);

            destinationMarker.SetActive(m_isValid);

            if (m_isValid)
            {
                destinationMarker.transform.position = new Vector3(nextPosition.point.x, destinationMarker.transform.position.y, nextPosition.point.z);

                if (Input.GetMouseButtonDown(0))
                {
                    m_EnableClickToMove = false;

                    SetPositionEnd();
                }
            }
        }

        if (m_IsEnroute)
        {
            CheckReachedDestination();
        }

        EngineAudio();
    }

    private bool isValidPosition(Vector3 p)
    {
        Ray ray = cam.ScreenPointToRay(p);

        if (!Physics.Raycast(ray, out nextPosition))
        {
            return false;
        }

        NavMeshPath path = new NavMeshPath();

        agent.CalculatePath(nextPosition.point, path);

        return path.status != NavMeshPathStatus.PathInvalid;
    }

    public void WaitThenSetRandomDestination()
    {
        Invoke("SetRandomDestination", m_CPUThinkTime);
    }

    public void SetRandomDestination()
    {
        bool isValidDestination = false;
        Vector3 pos;
        float rndX;
        float rndY;

        while (!isValidDestination)
        {
            rndX = Random.Range(0f, Screen.width);
            rndY = Random.Range(0f, Screen.height);

            pos = new Vector3(rndX, rndY, 0f);

            isValidDestination = isValidPosition(pos);
        }

        destinationMarker.SetActive(true);
        destinationMarker.transform.position = new Vector3(nextPosition.point.x, destinationMarker.transform.position.y, nextPosition.point.z);

        Invoke("SetPositionEnd", m_CPUThinkTime);
    }

    private void SetPositionEnd()
    {
        GameEvents.current.SetTankPositionEnd(m_PlayerNumber);
        GameEvents.current.CameraModeChangeStart(CameraMode.Players);
    }

    private void CheckReachedDestination()
    {
        if (m_IsEnroute && !agent.hasPath)
        {
            // Done
            m_IsEnroute = false;

            destinationMarker.SetActive(false);

            GameEvents.current.TankMoveEnd(m_PlayerNumber);
        }
    }

    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        //if(Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f) 
        //{
        //    if(m_MovementAudio.clip == m_EngineDriving)
        //    {
        //        m_MovementAudio.clip = m_EngineIdling;
        //        m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
        //        m_MovementAudio.Play();
        //    }
        //} else
        //{
        //    if (m_MovementAudio.clip == m_EngineIdling)
        //    {
        //        m_MovementAudio.clip = m_EngineDriving;
        //        m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
        //        m_MovementAudio.Play();
        //    }
        //}
    }
}