using UnityEngine;
using UnityEngine.UI;

public class PlankDeceleration : MonoBehaviour
{
    public Rigidbody plankRigidbody;
    public Transform board;
    public float deceleration = 5f;
    public float minimumSpeed = 0.7f;
    public GameObject menu;

    private bool isInDecelerationZone = false;

    private void Start()
    {
        menu.transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(() => FindFirstObjectByType<HoverController>().RestartTheGame());
        menu.transform.GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(() => TransitionController.Instance.LoadSceneAsync(2));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("MainCamera"))
        {
            isInDecelerationZone = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("MainCamera")) && isInDecelerationZone)
        {
            Vector3 currentVelocity = plankRigidbody.velocity;
            if (currentVelocity.magnitude > minimumSpeed)
            {
                Vector3 oppositeDirection = -currentVelocity.normalized;
                plankRigidbody.AddForce(oppositeDirection * deceleration, ForceMode.Acceleration);
            }
            else
            {
                plankRigidbody.velocity = Vector3.zero;
                plankRigidbody.angularVelocity = Vector3.zero;
                isInDecelerationZone = false;
                if (menu != null)
                {
                    board.eulerAngles = new Vector3(0f, board.eulerAngles.y, 0f);
                    menu.SetActive(true);
                    plankRigidbody.isKinematic = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("MainCamera"))
        {
            isInDecelerationZone = false;
        }
    }
}
