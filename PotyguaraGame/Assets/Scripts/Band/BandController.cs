using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;

[Serializable]
public class TimeEvent
{
    [SerializeField] public int min;
    [SerializeField] public int sec;
    [SerializeField] public bool vocal;
} 

public class BandController : MonoBehaviour
{
    [SerializeField] private BandMember[] members;
    [SerializeField] int silenceThreshold = 10;
    [SerializeField] private int numberOfSamples = 64;

    [SerializeField] public GameObject micBackingVocal;

    [SerializeField] GameObject[] reactiveObjects;
    [Range(1f, 100f)]
    [SerializeField] private float smoothFactor = 4f;

    [SerializeField] private TimeEvent[] vocalEvents;

    private BandMember vocalist;

    private AudioSource audioBand;
    [SerializeField] private VideoPlayer video;
    private bool isSilence = false;
    private bool hasVocal = true;
    private bool showStarted = false;

    private float[] samples;
    private float[] spectrum;
    private float checkInterval = .25f;
    private float lastCheckTime = 0f;

    void Start()
    {
        samples = new float[numberOfSamples];
        spectrum = new float[numberOfSamples];

        audioBand = GetComponent<AudioSource>();
    }

    public void StartShow(AudioClip clip)
    {
        video.Play();
        foreach (var member in members)
        {
            member.setAnimator(member.GetComponent<Animator>());
            member.IniciateMember();

            if (member.getInstrument() == Instrument.VOCALS)
                vocalist = member;
        }
        showStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Time.time - lastCheckTime >= checkInterval)
        if (showStarted)
        {
            /*if(video.time == video.length)
            {
                Achievement.Instance.firstCompleteEvent = true;
            }*/
            int volume = GetVolume(audioBand);
            if (volume < silenceThreshold)
            {
                isSilence = true;
                StartCoroutine(SetSilence(0.5f));
            }
            else if (isSilence)
            {
                StopAllCoroutines();
                Play();
            }
            audioBand.Play();

            float frequency = GetSpectrum(audioBand);
            //float mappedValue = mapValue(frequency, 0, 2);
            //if (volume < silenceThreshold) mappedValue = 0;
            float mappedValue = mapValue(volume, 0, 3, 0, 100);

            audioBand.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

            foreach (var timeEvent in vocalEvents)
                timeAction(audioBand, timeEvent.min, timeEvent.sec, timeEvent.vocal);

            foreach (var obj in reactiveObjects)
            {
                Transform control = FindControl(obj.transform);
                control.localScale = Vector3.Lerp(control.localScale, new Vector3(control.localScale.x, mappedValue, control.localScale.z), Time.deltaTime * smoothFactor);
            }
        }
    }

    private IEnumerator SetSilence(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var member in members)
            member.pauseAnimation();
    }

    private void Play()
    {
        isSilence = false;
        foreach (var member in members)
            member.playAnimation();
    }

    private float mapValue(float frequency, float min, float max, float minOriginal, float maxOriginal)
    {
        return min + (frequency - minOriginal) * (max - min) / (minOriginal - maxOriginal);
    }

    private Transform FindControl(Transform parent)
    {
        if (parent.name == "Control")
            return parent;

        foreach (Transform child in parent)
        {
            Transform found = FindControl(child);
            if (found != null)
                return found;
        }

        return null;
    }

    private void timeAction(AudioSource audioSource, int minTarget, int secTarget, bool vocal)
    {
        int currentMin = Mathf.FloorToInt(audioSource.time / 60);
        int currentSec = Mathf.FloorToInt(audioSource.time % 60);

        if (currentMin == minTarget && currentSec == secTarget) { 
            hasVocal = vocal;
            if (vocal)
                vocalist.playAnimation();
            else
                vocalist.pauseAnimation();
            //Debug.Log("Ação ativada no tempo: " + currentMin + ":" + currentSec +" - "+ vocal);
        }
    }

    #region audio
    private int GetVolume(AudioSource source)
    {
        source.GetOutputData(samples, 0);

        float sum = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += Mathf.Abs(samples[i]);
        }
        float averageVolume = sum / samples.Length;

        int volume = Mathf.RoundToInt(averageVolume * 100);

        volume = Mathf.Clamp(volume, 0, 100);

        return volume;
    }

    private float GetSpectrum(AudioSource source)
    {
        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        float sampleRate = AudioSettings.outputSampleRate;

        float sumWeighted = 0f;
        float sumIntensity = 0f;

        for (int i = 0; i < spectrum.Length; i++)
        {
            float frequency = i * (sampleRate / 2) / spectrum.Length;
            sumWeighted += frequency * spectrum[i];
            sumIntensity += spectrum[i];
            //print("Índice: " + i + ", Frequência: " + frequency + " Hz, Intensidade: " + spectrum[i]);
        }

        float averageFrequency = sumWeighted / sumIntensity;
        //print("-------------------- Frequência média: " + averageFrequency + " Hz");

        return averageFrequency;
    }

    private bool DetectFrequencyRange(float[] spectrum, float minFreq, float maxFreq)
    {
        float sampleRate = AudioSettings.outputSampleRate;
        int spectrumSize = spectrum.Length;

        int minIndex = Mathf.Clamp(FrequencyToIndex(minFreq, sampleRate), 0, spectrumSize - 1);
        int maxIndex = Mathf.Clamp(FrequencyToIndex(maxFreq, sampleRate), 0, spectrumSize - 1);

        float energy = 0f;
        float totalEnergy = 0f;

        for (int i = 0; i < spectrumSize; i++)
        {
            totalEnergy += spectrum[i];
            if (i >= minIndex && i <= maxIndex)
            {
                energy += spectrum[i];
            }
        }

        float relativeThreshold = totalEnergy * 0.1f;
        return energy > relativeThreshold;
    }

    private int FrequencyToIndex(float frequency, float sampleRate)
    {
        return Mathf.FloorToInt(frequency * numberOfSamples / (sampleRate / 2));
    }
    #endregion
}
