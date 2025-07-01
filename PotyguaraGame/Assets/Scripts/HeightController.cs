using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeightController : MonoBehaviour
{
    [SerializeField] private float height = 0f;
    private GameObject player;
    public bool insideLift = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        GameObject.FindWithTag("MainCamera").transform.GetChild(4).GetComponent<SteamProfileManager>().UpdatePotycoins(FindFirstObjectByType<PotyPlayerController>().GetPotycoins());
        // Se não for Atalhos de Menu, coloca o player na posição inicial da Cena
        if (!TransitionController.Instance.GetIsSkip())
        {
            Transform initialPosition = GameObject.Find("InitialPosition").transform;
            player.transform.position = initialPosition.position;
            FindFirstObjectByType<HeightController>().NewHeight(player.transform.position.y);
            player.transform.eulerAngles = new Vector3(0, initialPosition.eulerAngles.y, 0);
        }
    }

    public void NewHeight(float value)
    {
        height = value;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (!insideLift)
            {
                LiftShowController show = FindFirstObjectByType<LiftShowController>();
                if (show != null)
                {
                    if (show.isInsideLift)
                        VariableHeight(player);
                    else
                        FixedHeight(player);
                }
                else
                    FixedHeight(player);
            }
            else
                VariableHeight(player);
        }
    }

    public void SetBool(bool value)
    {
        insideLift = value;
    }

    public bool GetBool()
    {
        return insideLift;
    }

    public void FixedHeight(GameObject obj)
    {
        obj.transform.position = new Vector3(obj.transform.position.x, height, obj.transform.position.z);
    }

    public void VariableHeight(GameObject obj)
    {
        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
    }
}
