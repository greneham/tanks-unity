using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public AudioSource m_ExplosionAudio;
    public int m_PlayerNumber;
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_ExplosionRadius = 5f;      
    
    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 hitTankPosition = Vector3.zero;

        // Find all the tanks in an area around the shell and damage them.

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for(int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidBody = colliders[i].GetComponent<Rigidbody>();

            if(!targetRigidBody)
            {
                continue;
            }

            hitTankPosition = targetRigidBody.transform.position;
        }


        if(hitTankPosition == Vector3.zero)
        {
            // hit terrain
            GameEvents.current.ExplosionAt(transform.position, ExplosionType.Terrain, m_PlayerNumber);
            GameEvents.current.TankFireEnd(m_PlayerNumber); // we keep playing if missed
        }
        else
        {
            // hit tank, game over for a player
            GameEvents.current.ExplosionAt(hitTankPosition, ExplosionType.Tank, m_PlayerNumber);
        }
        
        m_ExplosionAudio.Play();

        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        return 0f;
    }

    public void Update()
    {
        if (transform.position.z < 0)
        {
            // todo: handle out of bounds
            Debug.Log("OUT OF BOUNDS");
        }
    }

}