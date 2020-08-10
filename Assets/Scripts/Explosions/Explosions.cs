using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosions : MonoBehaviour
{
    public GameObject explosion1;
    public GameObject explosion2;
    public float explodeDuration = 3;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.onExplosionAt += onExplosionAt;
    }

    private void onExplosionAt(Vector3 pos, ExplosionType explosionType, int id)
    {
        transform.position = pos;

        if(explosionType == ExplosionType.Tank)
        {
            Debug.Log("HIT TANK");
            explosion1.SetActive(true);
        }

        if (explosionType == ExplosionType.Terrain)
        {
            Debug.Log("HIT TERRAIN");
            explosion2.SetActive(true);
        }

        Invoke("ResetExplosion", explodeDuration);
    }

    private void ResetExplosion()
    {
        explosion1.SetActive(false);
        explosion2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onDestroy()
    {
        GameEvents.current.onExplosionAt -= onExplosionAt;
    }
}
