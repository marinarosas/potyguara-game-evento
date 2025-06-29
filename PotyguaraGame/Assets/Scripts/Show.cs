using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
[CreateAssetMenu(fileName = "newShow", menuName = "Show")]
public class Show : ScriptableObject
{
    public Sprite image;
    public GameObject banda;
    public string description;
    public AudioClip clip;
    public GameObject[] extras;
    public string id;
}
