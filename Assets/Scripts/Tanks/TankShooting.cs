using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;              // Used to identify the different players.
    public Rigidbody m_Shell;                   // Prefab of the shell.
    public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
    public float m_MinLaunchForce = 0.1f;        // The force given to the shell if the fire button is not held.
    public float m_MaxLaunchForce = 15f;        // The force given to the shell if the fire button is held for the max charge time.
    public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.
    public ControlMode m_ControlMode;           // Computer/Human controlled player
    [HideInInspector] public Vector3 opponentPos;
    private float m_CPUAccuracy = 0.5f;          // some latency when CPU decides where to move to

    private string m_FireButton;                // The input axis that is used for launching shells.
    private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
    private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool m_Fired;                       // Whether or not the shell has been launched with this button press.
    private bool m_CanFire = false;             // Whether or not you are allowed to fire right now
    private float m_CPUThinkTime = 3f;          // some latency when CPU decides where to move to

    private void OnEnable()
    {
        // When the tank is turned on, reset the launch force and the UI
        m_CurrentLaunchForce = m_MinLaunchForce;
    }


    private void Start ()
    {
        // The fire axis is based on the player number.
        m_FireButton = "Jump";

        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

        GameEvents.current.onTankReadyFireStart += onTankReadyFireStart;
    }


    private void Update ()
    {
        if(!m_CanFire)
        {
            return;
        }

        // If the max force has been exceeded and the shell hasn't yet been launched...
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            // ... use the max force and launch the shell.
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire ();
        }
        // Otherwise, if the fire button has just started being pressed...
        else if (Input.GetButtonDown (m_FireButton))
        {
            // ... reset the fired flag and reset the launch force.
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

        }
        // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
        else if (Input.GetButton (m_FireButton) && !m_Fired)
        {
            // Increment the launch force and update the slider.
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
        }
        // Otherwise, if the fire button is released and the shell hasn't been launched yet...
        else if (Input.GetButtonUp (m_FireButton) && !m_Fired)
        {
            // ... launch the shell.
            Fire ();
        }
    }


    private void Fire ()
    {
        GameEvents.current.TankFireStart(m_PlayerNumber);

        // Set the fired flag so only Fire is only called once.
        m_Fired = true;
        m_CanFire = false;

        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
            Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        var shellExplosion = shellInstance.gameObject.GetComponent<ShellExplosion>();

        shellExplosion.m_PlayerNumber = m_PlayerNumber;

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; 

        // Reset the launch force.  This is a precaution in case of missing button events.
        m_CurrentLaunchForce = m_MinLaunchForce;
    }

    private void onTankReadyFireStart(int id)
    {
        if (id == m_PlayerNumber)
        {
            if (m_ControlMode == ControlMode.Player)
            {
                // allow player to fire
                m_CanFire = true;
            }
            else
            {
                //CPU fires
                Invoke("CPUFire", m_CPUThinkTime);
            }
        }
    }

    public void CPUFire()
    {

        float gravity = 9.81f;
        float distToOpponent = Vector3.Distance(transform.position, opponentPos);

        // RANGE = ((Velocity * Velocity) * sin(2 * angle)) / gravity
        //
        // our angle 45 deg so sin(2 * angle) = 1
        // g = 9.81 (earths gravity)
        // re-arrange this to get launch force 

        m_CurrentLaunchForce = Mathf.Sqrt(distToOpponent * gravity);

        // make the shot inaccurate
        float rndRange = Random.Range(-m_MaxLaunchForce, m_MaxLaunchForce);
        float offset = (1f - m_CPUAccuracy) * rndRange;

        m_CurrentLaunchForce += offset;

        // cap at max/min
        m_CurrentLaunchForce = Mathf.Min(m_CurrentLaunchForce, m_MaxLaunchForce);
        m_CurrentLaunchForce = Mathf.Max(m_CurrentLaunchForce, m_MinLaunchForce);

        Fire();        
    }

    public void onDestroy()
    {
        GameEvents.current.onTankReadyFireStart -= onTankReadyFireStart;
    }
}