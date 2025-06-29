using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "newProduct", menuName = "Product")]
public class Product : ScriptableObject
{
    public Sprite image;
    public string description;
    public string category;
    public string id;
    public int index;

}
