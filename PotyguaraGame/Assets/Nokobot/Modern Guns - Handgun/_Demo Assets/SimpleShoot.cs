using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    private int maxBullets = 10;
    private int currentBullets;
    private InputDevice targetDevice;
    private bool isLeft = false;
    private bool isRight = false;
    private float timeout = 0.5f;

    public AudioSource shotSound;
    public AudioSource reloadSound;
    public TextMeshProUGUI bullets;

    [SerializeField] float nextTimeForFire = 0f; // tempo para o proximo tiro
    [SerializeField] private float delayBetweenShots = 0.3f;  // Delay entre cada disparo em segundos
    [SerializeField] private bool canShoot = true; // booleano para saber se pode ou não atirar

    [Header("Prefab Refrences")]
    //public GameObject line;
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 803.5f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 250f;
    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();

        Reload();
    }

    public void setLeftHand(bool value)
    {
        isLeft = value;
    }

    public void setRightHand(bool value)
    {
        isRight = value;
    }

    void Update()
    {
        // controle do tempo entre disparos
        if (!canShoot)
            nextTimeForFire += Time.deltaTime;

        if (nextTimeForFire >= delayBetweenShots)
        {
            canShoot = true;
            nextTimeForFire = 0f;
        }

        if (isLeft)
        {
            targetDevice = FindFirstObjectByType<LeftHandController>().GetTargetDevice();
        }
        else if (isRight) { 
            targetDevice = FindFirstObjectByType<RightHandController>().GetTargetDevice();
        }

        if (isRight != false || isLeft != false)
        {
            targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
            if (triggerValue > 0.1f || Input.GetKeyDown(KeyCode.F))
            {
                if (currentBullets > 0)
                {
                    if (canShoot)
                    {
                        gunAnimator.SetTrigger("Fire");
                        shotSound.Play();
                        //Calls animation on the gun that has the relevant animation events that will fire
                        if (isRight)
                            FindFirstObjectByType<RightHandController>().AnimationFinger();
                        else if (isLeft)
                            FindFirstObjectByType<LeftHandController>().AnimationFinger();
                        canShoot = false;
                    }
                }
                else
                {
                    reloadSound.Play();
                    Reload();
                }
            }
        }
        bullets.text = currentBullets.ToString();
    }

    public void Reload()
    {
        currentBullets = maxBullets;
        bullets.text = currentBullets.ToString();
    }

    //This function creates the bullet behavior
    void Shoot()
    {
        currentBullets--;
        if (muzzleFlashPrefab)
        {
            //Create the muzzle flash
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

            //Destroy the muzzle flash effect
            Destroy(tempFlash, destroyTimer);
        }
        RaycastHit hitInfo;
        bool hasHit = Physics.Raycast(barrelLocation.position, barrelLocation.forward, out hitInfo, 100);
        /*if (line)
        {
            GameObject liner = Instantiate(line);
            liner.GetComponent<LineRenderer>().SetPositions(new Vector3[] {barrelLocation.position, hasHit ? hitInfo.point : barrelLocation.position + barrelLocation.forward*100});
            Destroy(liner, 0.5f);
        }*/
        //cancels if there's no bullet prefeb
        if (!bulletPrefab)
        { return; }

        // Create a bullet and add force on it in direction of the barrel
        GameObject bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
        //bullet.GetComponent<BloodVisualEffectController>().stopEffect();

    }

    //This function creates a casing at the ejection slot
    void CasingRelease()
    {
        //Cancels function if ejection slot hasn't been set or there's no casing
        if (!casingExitLocation || !casingPrefab)
        { return; }

        //Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer);
    }

}
