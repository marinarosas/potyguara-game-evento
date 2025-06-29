using DigitalRuby.RainMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class RainController : MonoBehaviour
{
    [Tooltip("Camera the rain should hover over, defaults to main camera")]
    public Camera Camera;

    [Tooltip("Whether rain should follow the camera. If false, rain must be moved manually and will not follow the camera.")]
    public bool FollowCamera = true;

    [Tooltip("Light rain looping clip")]
    public AudioClip RainSoundLight;

    [Tooltip("Medium rain looping clip")]
    public AudioClip RainSoundMedium;

    [Tooltip("Heavy rain looping clip")]
    public AudioClip RainSoundHeavy;

    [Tooltip("Intensity of rain in mm)")]
    [Range(0f, 100f)]
    public float precip;

    [Tooltip("Rain particle system")]
    public ParticleSystem RainFallParticleSystem;

    [Tooltip("Wind looping clip")]
    public AudioClip WindSound;

    [Tooltip("Wind sound volume modifier, use this to lower your sound if it's too loud.")]
    [Range(0f, 80f)]
    public float WindSoundVolumeModifier = 0.5f;

    [Tooltip("Wind zone that will affect and follow the rain")]
    public WindZone WindZone;

    [Tooltip("X = minimum wind speed. Y = maximum wind speed. Z = sound multiplier. Wind speed is divided by Z to get sound multiplier value. Set Z to lower than Y to increase wind sound volume, or higher to decrease wind sound volume.")]
    public Vector3 WindSpeedRange = new Vector3(50.0f, 500.0f, 500.0f);

    [Tooltip("How often the wind speed and direction changes (minimum and maximum change interval in seconds)")]
    public Vector2 WindChangeInterval = new Vector2(5.0f, 30.0f);

    [Tooltip("Whether wind should be enabled.")]
    public bool EnableWind = true;

    protected LoopingAudioSource audioSourceRainLight;
    protected LoopingAudioSource audioSourceRainMedium;
    protected LoopingAudioSource audioSourceRainHeavy;
    protected LoopingAudioSource audioSourceRainCurrent;
    protected LoopingAudioSource audioSourceWind;

    private float lastRainIntensityValue = -1.0f;
    private float nextWindTime;

    private void UpdateWind()
    {
        if (EnableWind && WindZone != null && WindSpeedRange.y > 1.0f)
        {
            WindZone.gameObject.SetActive(true);
            if (FollowCamera)
            {
                WindZone.transform.position = Camera.transform.position;
            }
            if (!Camera.orthographic)
            {
                WindZone.transform.Translate(0.0f, WindZone.radius, 0.0f);
            }
            if (nextWindTime < Time.time)
            {
                WindZone.windMain = UnityEngine.Random.Range(WindSpeedRange.x, WindSpeedRange.y);
                WindZone.windTurbulence = UnityEngine.Random.Range(WindSpeedRange.x, WindSpeedRange.y);
                if (Camera.orthographic)
                {
                    int val = UnityEngine.Random.Range(0, 2);
                    WindZone.transform.rotation = Quaternion.Euler(0.0f, (val == 0 ? 90.0f : -90.0f), 0.0f);
                }
                else
                {
                    WindZone.transform.rotation = Quaternion.Euler(Random.Range(-30.0f, 30.0f), Random.Range(0.0f, 360.0f), 0.0f);
                }
                nextWindTime = Time.time + Random.Range(WindChangeInterval.x, WindChangeInterval.y);
                audioSourceWind.Play((WindZone.windMain / WindSpeedRange.z) * WindSoundVolumeModifier);
            }
        }
        else
        {
            if (WindZone != null)
            {
                WindZone.gameObject.SetActive(false);
            }
            audioSourceWind.Stop();
        }

        audioSourceWind.Update();
    }

    private void CheckForRainChange()
    {
        if (lastRainIntensityValue != precip)
        {
            lastRainIntensityValue = precip;
            if (precip <= 0.5f)
            {
                if (audioSourceRainCurrent != null)
                {
                    audioSourceRainCurrent.Stop();
                    audioSourceRainCurrent = null;
                }
                if (RainFallParticleSystem != null)
                {
                    ParticleSystem.EmissionModule e = RainFallParticleSystem.emission;
                    e.enabled = false;
                    RainFallParticleSystem.Stop();
                }
            }
            else
            {
                LoopingAudioSource newSource;
                if (precip > 15f)
                {
                    newSource = audioSourceRainHeavy;
                }
                else if (precip > 3f && precip <= 15f)
                {
                    newSource = audioSourceRainMedium;
                }
                else
                {
                    newSource = audioSourceRainLight;
                }
                if (audioSourceRainCurrent != newSource)
                {
                    if (audioSourceRainCurrent != null)
                    {
                        audioSourceRainCurrent.Stop();
                    }
                    audioSourceRainCurrent = newSource;
                    audioSourceRainCurrent.Play(1.0f);
                }
                if (RainFallParticleSystem != null)
                {
                    ParticleSystem.EmissionModule e = RainFallParticleSystem.emission;
                    e.enabled = RainFallParticleSystem.GetComponent<Renderer>().enabled = true;
                    if (!RainFallParticleSystem.isPlaying)
                    {
                        RainFallParticleSystem.Play();
                    }
                    ParticleSystem.MinMaxCurve rate = e.rateOverTime;
                    rate.mode = ParticleSystemCurveMode.Constant;
                    rate.constantMin = rate.constantMax = RainFallEmissionRate();
                    e.rateOverTime = rate;
                }
            }
        }
    }

    protected virtual float RainFallEmissionRate()
    {
        return (RainFallParticleSystem.main.maxParticles / RainFallParticleSystem.main.startLifetime.constant) * precip;
    }

    protected virtual void Start()
    {

#if DEBUG

        if (RainFallParticleSystem == null)
        {
            Debug.LogError("Rain fall particle system must be set to a particle system");
            return;
        }

#endif

        if (Camera == null)
        {
            Camera = Camera.main;
        }

        audioSourceRainLight = new LoopingAudioSource(this, RainSoundLight);
        audioSourceRainMedium = new LoopingAudioSource(this, RainSoundMedium);
        audioSourceRainHeavy = new LoopingAudioSource(this, RainSoundHeavy);
        audioSourceWind = new LoopingAudioSource(this, WindSound);

        if (RainFallParticleSystem != null)
        {
            ParticleSystem.EmissionModule e = RainFallParticleSystem.emission;
            e.enabled = false;
            Renderer rainRenderer = RainFallParticleSystem.GetComponent<Renderer>();
            rainRenderer.enabled = false;
        
        }
    }

    protected virtual void Update()
    {

#if DEBUG

        if (RainFallParticleSystem == null)
        {
            Debug.LogError("Rain fall particle system must be set to a particle system");
            return;
        }

#endif
        if (FindFirstObjectByType<DayController>() != null)
        {
            if (NetworkManager.Instance.modeWeatherOn)
                precip = FindFirstObjectByType<DayController>().GetPrecip();
            else
                precip = 0;
        }

        CheckForRainChange();
        UpdateWind();
        audioSourceRainLight.Update();
        audioSourceRainMedium.Update();
        audioSourceRainHeavy.Update();
    }

    public class LoopingAudioSource
    {
        public AudioSource AudioSource { get; private set; }
        public float TargetVolume { get; private set; }

        public LoopingAudioSource(MonoBehaviour script, AudioClip clip)
        {
            AudioSource = script.gameObject.AddComponent<AudioSource>();

            AudioSource.loop = true;
            AudioSource.clip = clip;
            AudioSource.playOnAwake = false;
            AudioSource.volume = 0.0f;
            AudioSource.Stop();
            TargetVolume = 1.0f;
        }

        public void Play(float targetVolume)
        {
            if (!AudioSource.isPlaying)
            {
                AudioSource.volume = 0.0f;
                AudioSource.Play();
            }
            TargetVolume = targetVolume;
        }

        public void Stop()
        {
            TargetVolume = 0.0f;
        }

        public void Update()
        {
            if (AudioSource.isPlaying && (AudioSource.volume = Mathf.Lerp(AudioSource.volume, TargetVolume, Time.deltaTime)) == 0.0f)
            {
                AudioSource.Stop();
            }
        }
    }
}

public class Rain2Script : RainController
{
    [Tooltip("The height above the camera that the rain will start falling from")]
    public float RainHeight = 25.0f;

    [Tooltip("How far the rain particle system is ahead of the player")]
    public float RainForwardOffset = -7.0f;

    private void UpdateRain()
    {
        // keep rain and mist above the player
        if (RainFallParticleSystem != null)
        {
            if (FollowCamera)
            {
                var s = RainFallParticleSystem.shape;
                s.shapeType = ParticleSystemShapeType.ConeVolume;
                RainFallParticleSystem.transform.position = Camera.transform.position;
                RainFallParticleSystem.transform.Translate(0.0f, RainHeight, RainForwardOffset);
                RainFallParticleSystem.transform.rotation = Quaternion.Euler(0.0f, Camera.transform.rotation.eulerAngles.y, 0.0f);
            }
            else
            {
                var s = RainFallParticleSystem.shape;
                s.shapeType = ParticleSystemShapeType.Box;
            }
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        UpdateRain();
    }
}
