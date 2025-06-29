using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("MainCamera"))
        {
            Debug.Log("Entrou no elevador");
            other.gameObject.transform.parent = transform.parent;
            FindObjectOfType<LiftController>().CloseTheDoors();
            FindFirstObjectByType<HeightController>().SetBool(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("MainCamera"))
        {
            Debug.Log("Saiu do elevador");
            other.gameObject.transform.parent = null;
            FindObjectOfType<LiftController>().CloseTheDoors();
            FindFirstObjectByType<HeightController>().SetBool(false);
        }
    }
}
