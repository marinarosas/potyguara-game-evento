using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.XR;
using System.Net.Sockets;
using Unity.VisualScripting;

public class SalesCenterController : MonoBehaviour
{
    [Header("Button Prefab")]
    [SerializeField] public GameObject buttonPrefab;

    [Header("Scriptable Objects")]
    [SerializeField] public Product[] potycoins;
    [SerializeField] public Product[] shows;
    [SerializeField] public Product[] meditationClasses;
    [SerializeField] public Product[] skinsFEM;
    [SerializeField] public Product[] skinsMASC;

    [Header("Menu Container")]
    [SerializeField] private Transform content;
    public bool controlMenu = false;
    public bool changedStatus = false;
    public bool isPurshing = false;

    private List<InputDevice> devices = new List<InputDevice>();

    public void AddNewButton(Sprite image, string id, string description, string category, int index)
    {
        GameObject newButton = Instantiate(buttonPrefab, content);
        newButton.GetComponent<Image>().sprite = image;
        newButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = description;
        if (category == "skin")
        {
            if (FindFirstObjectByType<PotyPlayerController>().VerifSkins(index))
            {
                newButton.GetComponent<Button>().interactable = false;
                return;
            }
        }
        if(category == "class")
        {
            if (description == "FREE")
            {
                if (FindFirstObjectByType<PotyPlayerController>().VerifSessions(id))
                {
                    newButton.GetComponent<Button>().interactable = false;
                    return;
                }
                newButton.GetComponent<Button>().onClick.AddListener(() => BuyFreeProduct(category, id, index, newButton.GetComponent<Button>()));
                return;
            }
        }
        if(category == "show")
        {
            if (description == "FREE")
            {
                if (FindFirstObjectByType<PotyPlayerController>().VerifTickets(id))
                {
                    newButton.GetComponent<Button>().interactable = false;
                    return;
                }
                newButton.GetComponent<Button>().onClick.AddListener(() => BuyFreeProduct(category, id, index, newButton.GetComponent<Button>()));
                return;
            }
        }
        newButton.GetComponent<Button>().onClick.AddListener(() => BuyProduct(id, description, category, newButton.GetComponent<Button>()));
    }

    public void CheckSessions()
    {
        List<string> sessions = FindFirstObjectByType<PotyPlayerController>().GetSessions();
        if (sessions.Count != 0)
        {
            foreach (string session in sessions)
            {
                foreach (Product product in meditationClasses)
                {
                    if (product.id == session)
                        FindFirstObjectByType<MeditationRoomController>().AddButton(product.index);
                }
            }
        }
    }

    private void BuyFreeProduct(string category, string id, int index, Button btn)
    {
        if (!isPurshing)
        {
            if (category == "show")
            {
                FindFirstObjectByType<MenuShowController>().UnclockShow(id);
                NetworkManager.Instance.SendTicket(id);
                FindFirstObjectByType<PotyPlayerController>().AddTicket(id);
            }
            if (category == "class")
            {
                FindFirstObjectByType<MeditationRoomController>().AddButton(index);
                NetworkManager.Instance.SendSession(id);
                FindFirstObjectByType<PotyPlayerController>().AddSession(id);
            }
            btn.interactable = false;
            isPurshing = true;
            Invoke("ResetBoolean", 0.7f);
        }
    }

    private void ResetBoolean()
    {
        isPurshing = false;
    }

    public Product[] GetShows()
    {
        return shows;
    }

    public int GetIndexSkin(string id)
    {
        if(int.Parse(id) <= 4012)
        {
            foreach (Product item in skinsFEM)
                if (item.id == id)
                    return item.index;
        }
        else
        {
            foreach (Product item in skinsMASC)
                if (item.id == id)
                    return item.index;
        }
        return 0;
    }

    public void UpdateSalesCenter(string category)
    {
        foreach (Transform child in content) {
            Destroy(child.gameObject);
        }
        if (category == "moeda")
        {
            foreach (Product item in potycoins)
                AddNewButton(item.image, item.id, item.description, item.category, item.index);
        }
        if(category == "show")
        {
            foreach (Product item in shows)
                AddNewButton(item.image, item.id, item.description, item.category, item.index);
        }
        if (category == "class")
        {
            foreach (Product item in meditationClasses)
                AddNewButton(item.image, item.id, item.description, item.category, item.index);
        }
        if (category == "skin")
        {
            if(FindFirstObjectByType<PotyPlayerController>().GetGender() == 1)
                foreach (Product item in skinsFEM)
                    AddNewButton(item.image, item.id, item.description, item.category, item.index);
            else
                foreach (Product item in skinsMASC)
                    AddNewButton(item.image, item.id, item.description, item.category, item.index);
        }
    }

    private void BuyProduct(string id, string description, string category, Button btn)
    {
        if (!isPurshing)
        {
            Microtransaction.Instance.InitSale(id, description, category);
            if (!category.Equals("moeda"))
                btn.interactable = false;
            isPurshing = true;
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("MainCamera"))
        {
            InputDeviceCharacteristics rightHandCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(rightHandCharacteristics, devices);
            if (devices.Count != 0)
            {
                devices[0].TryGetFeatureValue(CommonUsages.secondaryButton, out bool Bbutton);
                if (Bbutton) // B button pressed
                {
                    GameObject menu = transform.GetChild(0).gameObject;
                    if (menu != null)
                    {
                        if (!changedStatus)
                        {
                            controlMenu = !controlMenu;
                            menu.SetActive(controlMenu);
                            changedStatus = true;
                            Invoke("ChangeStatus", .3f);
                        }
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space)) // B button pressed
                {
                    GameObject menu = transform.GetChild(0).gameObject;
                    if (menu != null)
                    {
                        if (!changedStatus)
                        {
                            controlMenu = !controlMenu;
                            menu.SetActive(controlMenu);
                            changedStatus = true;
                            Invoke("ChangeStatus", .3f);
                        }
                    }
                }
            }
        }
    }

    private void ChangeStatus()
    {
        changedStatus = !changedStatus;
    }
}
