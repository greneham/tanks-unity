using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;
    public GameObject destinationMarker;
    public GameObject m_TankPrefab;         
    public TankManager[] m_Tanks;

    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;       

    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

        Invoke("StartGame", 1); // 1 sec delay

        GameEvents.current.onMoveTurnEnd += onMoveTurnEnd;
        GameEvents.current.onTankFireEnd += onTankFireEnd;

    }

    private void StartGame()
    {
        GameEvents.current.MoveTurnStart(1);
    }

    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].destinationMarker = destinationMarker;
            m_Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }

    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }

    private void onMoveTurnEnd(int id, Vector3 pos)
    {
        int nextPlayerId = id + 1;

        // start from P1 once we get to end of player list
        if(nextPlayerId > m_Tanks.Length)
        {

            //this is hard coded to work for a x2 player game for now
            m_Tanks[0].m_Instance.transform.LookAt(m_Tanks[1].m_Instance.transform);
            m_Tanks[1].m_Instance.transform.LookAt(m_Tanks[0].m_Instance.transform);

            //todo: UI says TAKE AIM

            Invoke("StartShootingRound", 1f); // a little latency before shooting round

            return;
        }

        GameEvents.current.MoveTurnStart(nextPlayerId);
    }

    private void onTankFireEnd(int id)
    { 
        int nextPlayerId = id + 1;

        if (nextPlayerId <= m_Tanks.Length)
        {
            GameEvents.current.TankReadyFireStart(nextPlayerId);
        } 
        else
        {
            // no hits, start new round
            GameEvents.current.MoveTurnStart(1);
        }
    }

    private void StartShootingRound()
    {
        GameEvents.current.TankReadyFireStart(1);
    }

    public void onDestroy()
    {
        GameEvents.current.onMoveTurnEnd -= onMoveTurnEnd;
        GameEvents.current.onTankFireEnd -= onTankFireEnd;
    }
}