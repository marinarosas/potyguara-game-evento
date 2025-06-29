using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryController : MonoBehaviour
{
    public List<Image> images;
    public List<Sprite> sprites;
    // Start is called before the first frame update
    void Start()
    {
        for (int ii = 0; ii < images.Count; ii++)
            images[ii].sprite = sprites[ii]; 
    }
}
