using System.Collections;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class LiftController : MonoBehaviour
{
    private int currentFloor = 0;
    private Animator ani;
    private GameObject leftDoor;
    private GameObject rightDoor;
    private Transform player;

    private void Start()
    {
        ani = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        leftDoor = transform.GetChild(0).gameObject;
        rightDoor = transform.GetChild(1).gameObject;
    }

    public void OpenTheDoors()
    {
        leftDoor.GetComponent<Animator>().Play("OpenLeft");
        rightDoor.GetComponent<Animator>().Play("OpenRight");
    }

    public void CloseTheDoors()
    {
        leftDoor.GetComponent<Animator>().Play("CloseLeft");
        rightDoor.GetComponent<Animator>().Play("CloseRight");
    }

    public void ChangeFloor(int value)
    {
        if(currentFloor != value)
        {
            GameObject.FindWithTag("MainCamera").transform.GetChild(3).GetComponent<FadeController>().FadeInForFadeOutWithAnimator(3f, ani);
            FindObjectOfType<HeightController>().SetBool(true);
            if (value == 0)
                ani.Play("DownTerreo");
            else
                ani.Play("MoveUpTerreo");

            currentFloor = value;
        }
        else
            OpenTheDoors();
    }
}
