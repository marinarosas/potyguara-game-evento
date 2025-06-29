using UnityEngine;
using UnityEngine.UI;

public class WallController : MonoBehaviour
{
    public Material forceField;
    private bool receivedDamage = false;
    private Renderer wallRenderer;
    private Material material;

    public void setDamage(bool value)
    {
        receivedDamage = value;
    }

    private void Start()
    {
        forceField = new Material(forceField);
        wallRenderer = GetComponent<Renderer>();
        material = wallRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] collisions = Physics.OverlapSphere(transform.position, 1f);

        bool hasBodyCollision = false;
        foreach (Collider collision in collisions){
            if (collision.CompareTag("Body")){
                hasBodyCollision = true;
                break;
            }
        }

        if (!hasBodyCollision)
            receivedDamage = false;

        if (receivedDamage)
            material.SetFloat("_isDamage", 1.0f);
        else
            material.SetFloat("_isDamage", 0.0f);
    }

    public void resetWall()
    {
        material.SetFloat("_isDamage", 0.0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!receivedDamage)
        {
            if (collision.gameObject.CompareTag("Body"))
            {
                setDamage(true);
            }
        }
    }
}
