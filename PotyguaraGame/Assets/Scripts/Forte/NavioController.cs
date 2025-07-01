using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;

public class NavioController : MonoBehaviour
{
    public GameObject cannonBallPrefab;

    private float timeBetweenShoots = 5f;
    private float count = 0;
    private bool canShoot = true;

    private int health = 3;
    private bool receivedDamage = false;

    private Animator ani;

    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    private void Shoot()
    {
        Transform parent = transform.parent;
        foreach(Transform t in parent)
        {
            if (t.name != "navio")
            {
                float force = Random.Range(3000f, 7500f);
                GameObject cannonBall = Instantiate(cannonBallPrefab, t.position, t.rotation);
                cannonBall.GetComponent<CannonBallController>().SetIsNavio(true);
                cannonBall.GetComponent<CannonBallController>().wasInstantiatedForNavio = true;
                cannonBall.GetComponent<Rigidbody>().velocity = t.forward * force * Time.deltaTime;
                Destroy(cannonBall, 5f);
            }
        }
        canShoot = false;
    }

    void Update()
    {
        if (!canShoot)
        {
            count += Time.deltaTime;
            if(count >= timeBetweenShoots)
            {
                canShoot = true;
                Shoot();
                count = 0;
            }
        }
        if(canShoot)
        {
            Shoot();
        }
    }

    public void Death()
    {
        Destroy(transform.parent.gameObject, 0.5f);
    }

    private void ResetBoolean()
    {
        receivedDamage = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("CannonBall"))
        {
            if (!collision.gameObject.GetComponent<CannonBallController>().wasInstantiatedForNavio)
            {
                if (!receivedDamage)
                {
                    health--;
                    receivedDamage = true;
                    if (health <= 0)
                    {
                        ani.SetTrigger("isDestroyed");
                        return;
                    }
                    Invoke("ResetBoolean", 0.4f);
                }
            }
        }
    }
}
