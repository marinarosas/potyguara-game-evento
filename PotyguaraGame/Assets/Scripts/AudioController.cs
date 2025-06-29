using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private Transform player;

    private float distanceToPlayer;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if(distanceToPlayer > 40)
        {
            GetComponent<AudioSource>().volume = 0.1f;
        }else if(distanceToPlayer > 20)
        {
            GetComponent<AudioSource>().volume = 0.2f;
        }else if(distanceToPlayer > 10)
        {
            GetComponent<AudioSource>().volume = 0.45f;
        }else if(distanceToPlayer > 0)
        {
            GetComponent<AudioSource>().volume = 0.7f;
        }
    }
}
