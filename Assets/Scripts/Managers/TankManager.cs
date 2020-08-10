using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    
    public Color m_PlayerColor;            
    public Transform m_SpawnPoint;
    public ControlMode m_ControlMode;
    public GameObject destinationMarker;
    [HideInInspector] public int m_PlayerNumber;             
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;          
    [HideInInspector] public int m_Wins;                     


    private TankMovement m_Movement;       
    private TankShooting m_Shooting;
    //private GameObject m_CanvasGameObject;
    private bool m_IsActivePlayer = false;
    


    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        //m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Movement.destinationMarker = destinationMarker;
        m_Movement.m_ControlMode = m_ControlMode;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_ControlMode = m_ControlMode;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }

        GameEvents.current.onMoveTurnStart += onMoveTurnStart;
        GameEvents.current.onMoveTurnEnd += onMoveTurnEnd;
        GameEvents.current.onCameraModeChangeEnd += onCameraModeChangeEnd;
        GameEvents.current.onSetTankPositionStart += onSetTankPositionStart;
        GameEvents.current.onTankMoveStart += onTankMoveStart;
        GameEvents.current.onTankMoveEnd += onTankMoveEnd;
        GameEvents.current.onExplosionAt += onExplosionAt;
    }

    public void onMoveTurnStart(int id)
    {
        m_IsActivePlayer = m_PlayerNumber == id;

        if (m_IsActivePlayer)
        {
            Debug.Log("TURN STARTED");
            Debug.Log(id);

            GameEvents.current.CameraModeChangeStart(CameraMode.TopDown);
        }
    }

    private void onMoveTurnEnd(int id, Vector3 pos)
    {
        if(m_PlayerNumber != id)
        {
            m_Shooting.opponentPos = pos; // store where your opponent is
        }
    }

    public void onCameraModeChangeEnd(CameraMode mode)
    {
        // we have successfully transitioned the camera to the top down view
        if (mode == CameraMode.TopDown && m_IsActivePlayer)
        {
            // now we can pick a location on the grid to click
            GameEvents.current.SetTankPositionStart(m_PlayerNumber);
        }

        if (mode == CameraMode.Players && m_IsActivePlayer)
        {
            // the top down view has reset back to player view, now tell the tank to move
            GameEvents.current.TankMoveStart(m_PlayerNumber);
        }
    }

    public void onSetTankPositionStart(int id)
    {
        if(m_ControlMode == ControlMode.Player && m_PlayerNumber == id)
        {
            m_Movement.m_EnableClickToMove = true;
        }

        if (m_ControlMode == ControlMode.CPU && m_PlayerNumber == id)
        {
            m_Movement.WaitThenSetRandomDestination();
        }        
    }

    public void onTankMoveStart(int id)
    {
        if(m_PlayerNumber == id)
        {
            m_Movement.MoveTank();
        }
    }

    public void onTankMoveEnd(int id)
    {
        if (m_PlayerNumber == id)
        {
            // todo: shooting logic
            GameEvents.current.MoveTurnEnd(m_PlayerNumber, m_Instance.transform.position);
        }
    }

    public void onExplosionAt(Vector3 pos, ExplosionType explosionType, int id)
    {
        if(explosionType == ExplosionType.Tank && pos == m_Instance.transform.position)
        {
            MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = false;
            }

            if (m_PlayerNumber == id)
            {
                // oops we killed ourself
                Debug.Log("KILLED YOURSELF");

                // if cpu died we won
                GameEvents.current.GameOver(m_PlayerNumber == 2);
            }
            else
            {
                // opponent defeated
                Debug.Log("KILLED OPPONENT");

                // if player died we lost
                GameEvents.current.GameOver(m_PlayerNumber == 2);
            }
        }
    }

    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        //m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        //m_CanvasGameObject.SetActive(true);
    }

    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }

    public void onDestroy()
    {
        GameEvents.current.onMoveTurnStart -= onMoveTurnStart;
        GameEvents.current.onMoveTurnEnd -= onMoveTurnEnd;
        GameEvents.current.onCameraModeChangeEnd -= onCameraModeChangeEnd;
        GameEvents.current.onSetTankPositionStart -= onSetTankPositionStart;
        GameEvents.current.onTankMoveStart -= onTankMoveStart;
        GameEvents.current.onTankMoveEnd -= onTankMoveEnd;
        GameEvents.current.onExplosionAt -= onExplosionAt;
    }
}
