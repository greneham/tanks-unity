using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;
    public float m_ScreenEdgeBuffer = 4f;
    public float m_MinSize = 6.5f;
    public Transform m_TopDown;
    [HideInInspector] public Transform[] m_Targets;
    [HideInInspector] public Camera m_Camera;

    private float m_ZoomSpeed;
    private Vector3 m_MoveVelocity;
    private Vector3 m_DesiredPosition;
    private bool m_IsTopDown = false;
    private float m_PrevCamSize;
    private float m_TopDownCamSize = 22;
    private Vector3 prevPos;
    private Vector3 prevRot;
    private float prevCamSize;

    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }

    public void Start()
    {
        GameEvents.current.onCameraModeChangeStart += onCameraModeChangeStart;
    }

    public void onCameraModeChangeStart(CameraMode mode)
    {
        m_IsTopDown = mode == CameraMode.TopDown;

        Vector3 pos, rot;
        float camSize;
        float tweenDuration = 1f;

        // tween to top down camera position
        if (m_IsTopDown)
        {
            pos = m_TopDown.transform.position;
            rot = m_TopDown.transform.rotation.eulerAngles;
            camSize = m_TopDownCamSize;

            // store cam settings for when we tween back later
            prevPos = m_Camera.transform.position;
            prevRot = m_Camera.transform.rotation.eulerAngles;
            prevCamSize = m_Camera.orthographicSize;
        }
        else
        {
            // apply stored cam settings
            pos = prevPos;
            rot = prevRot;
            camSize = prevCamSize;
        }

        LeanTween.move(m_Camera.gameObject, pos, tweenDuration);
        LeanTween.rotate(m_Camera.gameObject, rot, tweenDuration);
        LeanTween.value(m_Camera.gameObject, m_Camera.orthographicSize, camSize, tweenDuration).setOnUpdate((float flt) => {
            m_Camera.orthographicSize = flt;
        }).setOnComplete(onTweenComplete);
    }

    public void onTweenComplete()
    {
        if(m_IsTopDown)
        {
            GameEvents.current.CameraModeChangeEnd(CameraMode.TopDown);
        }
        else
        {
            GameEvents.current.CameraModeChangeEnd(CameraMode.Players);
        }
    }

    public void onDestroy()
    {
        GameEvents.current.onCameraModeChangeStart -= onCameraModeChangeStart;
    }

    private void FixedUpdate()
    {
        if(!m_IsTopDown)
        {
            Move();
            Zoom();
        }
        
    }


    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        size += m_ScreenEdgeBuffer;

        size = Mathf.Max(size, m_MinSize);

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}