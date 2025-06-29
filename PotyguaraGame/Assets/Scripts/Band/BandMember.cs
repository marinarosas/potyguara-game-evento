using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Instrument
{
    VOCALS,
    GUITAR,
    BASS,
    DRUMS,
    KEYBOARD
}

public class BandMember : MonoBehaviour
{
    [Header("Member")]
    [SerializeField] protected string name;
    [SerializeField] protected bool isBackingVocal;
    [SerializeField] private Instrument instrument;
    protected Animator animator;

    public void IniciateMember()
    {
        if (isBackingVocal && instrument != Instrument.VOCALS)
            setBackingVocalMic();
    }

    protected void setBackingVocalMic()
    {
        GameObject mic = transform.parent.gameObject.GetComponent<BandController>().micBackingVocal;
        GameObject micInstance = Instantiate(mic, transform.parent);
        micInstance.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 0.55f);
    }
    public void setAnimator(Animator anim)
    {
        animator = anim;
    }

    public Instrument getInstrument() => instrument;

    public void pauseAnimation() => animator.SetBool("isIdle", true);
    public void playAnimation() => animator.SetBool("isIdle", false);
}
