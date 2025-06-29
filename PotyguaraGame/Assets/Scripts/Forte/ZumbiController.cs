using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.VFX;

public class ZumbiController : MonoBehaviour
{
    private Transform player = null;
    private NavMeshAgent navMesh;
    private bool MarkedPontuacion = false;

    private float velocityWalking = 0.7f, velocityPersecution = 3f;
    private float distanceFollow = 30f, distanceAttack = 3f;

    private float distanceForPlayer;
    private bool isDead = false;
    private SpawnerController spawner;
    private Animator ani;

    // Start is called before the first frame update
    void Start()
    {
        spawner = FindFirstObjectByType<SpawnerController>();
        navMesh = transform.parent.GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            if(spawner.GetCurrentLevel() == 2)
            {
                player = GameObject.FindWithTag("Player").transform;
            }
            else if (spawner.GetCurrentLevel() == 1)
            {
                WallController[] walls = FindObjectsByType<WallController>(FindObjectsSortMode.InstanceID);
                player = walls[Random.Range(0, walls.Length - 1)].transform;
            }
        }

        if (GameForteController.Instance.GetMode() == 0)
        {
            distanceForPlayer = Vector3.Distance(player.transform.position, transform.position);

            if (!isDead)
            {
                if(distanceForPlayer < distanceFollow)
                {
                    Follow();
                    if (distanceForPlayer <= distanceAttack) // for check if the enemy can attack the player
                        Attack();
                }
                else
                    Walking();

                if(spawner.GetCurrentLevel() == 1)
                    if (distanceForPlayer < 0.5f)
                        ChangeTarget();
            }
        }  
    }

    void Walking()
    {
        ani.SetBool("IsWalking", true);
        ani.SetBool("IsRunning", false);
        navMesh.acceleration = 5f;
        navMesh.speed = velocityWalking;
        navMesh.destination = player.position;
    }

    public void Dead()
    {
        navMesh.isStopped = true;
        ani.SetBool("IsWalking", false);
        ani.SetBool("IsRunning", false);

        if (!isDead)
        {
            ani.SetBool("IsDead", true);
            isDead = true;
            if (transform.childCount > 0){
                Transform firstChild = transform.GetChild(0);
                firstChild.tag = "Untagged";
            }//CONSIDERAR MORTO PRAS PAREDES
        }
    }

    void Idle()
    {
        ani.SetBool("IsWalking", false);
        ani.SetBool("IsRunning", false);
        ani.SetBool("isShouting", false);
        navMesh.acceleration = 0f;
        navMesh.speed = 0f;
    }

    void Follow()
    {
        ani.SetBool("IsWalking", false);
        ani.SetBool("IsRunning", true);
        navMesh.acceleration = 8f;
        navMesh.speed = velocityPersecution;
        navMesh.destination = player.position;
    }

    void Attack()
    {
        navMesh.isStopped = true;
        ani.SetBool("isShouting", true);
    }

    public void DestroyZumbi()
    {
        Destroy(transform.parent.gameObject);
    }

    private void ChangeTarget()
    {
        WallController[] walls = FindObjectsByType<WallController>(FindObjectsSortMode.InstanceID);
        player = walls[Random.Range(0, walls.Length - 1)].transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (!MarkedPontuacion)
            {
                MarkedPontuacion = true;
                collision.gameObject.GetComponent<BulletController>().playEffect();
                collision.gameObject.GetComponent<BulletController>().StartBloodEffect(2f, 0.5f);
                GameForteController.Instance.SetCurrentScore(1);
                Achievement.Instance.zombies++;

                if (Achievement.Instance.partidas_defesaForte == 100)
                    Achievement.Instance.UnclockAchievement("kill_100");
                if (Achievement.Instance.partidas_defesaForte == 200)
                    Achievement.Instance.UnclockAchievement("zumbinator");

                Achievement.Instance.SetStat("zombies", Achievement.Instance.zombies);

                GetComponent<ZumbiController>().Dead();
                Destroy(collision.gameObject, 3f);
            }
        }
    }
}
