using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DIRECTION { Decrease, Increase }

public class SkinSystem : MonoBehaviour{
    [SerializeField] public Transform rootBone;
    [SerializeField] public Transform skinContainer;
    [SerializeField] public GameObject hair, head, chest, belly, arms, forearms, hands, hips, legs, feet, beard;
    [SerializeField] public List<Skin> skins;
    [SerializeField] public Skin currentSkin = null;
    
    protected int indexSkin = 0;
    protected int oldIndexSkin = -1;
    protected int indexMaterial = 0;

    #region publicFunctions

    public void disableMeshes()
    {
        for (int i = 0; i < skins.Count; i++)
            if (i != indexSkin)
                skins[i].skinMeshes[0].gameObject?.SetActive(false);
    }

    public bool changeMesh(DIRECTION direction)
    {
        try {
            if ((direction == DIRECTION.Decrease && indexSkin > 0) ||
                (direction == DIRECTION.Increase && indexSkin < skins.Count - 1))
            {
                toggleVisibleMeshes(false);

                indexSkin += (direction == DIRECTION.Increase) ? 1 : -1;
                oldIndexSkin = indexSkin;

                toggleVisibleMeshes(true);

                currentSkin = skins[indexSkin];
                UpdateBodyMesh(skins[indexSkin]);

                changeMaterial(0);

                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error when trying to change skins ({direction}): {ex.Message}");
            return false;
        }
    }

    public void changeMesh(int index)
    {
        try {
            if (oldIndexSkin == index)
                return;

            oldIndexSkin = index;
            toggleVisibleMeshes(false);
            indexSkin = index;
            toggleVisibleMeshes(true);
            currentSkin = skins[indexSkin];

            UpdateBodyMesh(currentSkin);

            changeMaterial(0);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error when trying to change skins: {ex.Message}");
        }
    }

    protected void UpdateBodyMesh(Skin skin){
        hair?.SetActive(skin.hasHair);
        head?.SetActive(skin.hasHead);
        chest?.SetActive(skin.hasChest);
        belly?.SetActive(skin.hasBelly);
        arms?.SetActive(skin.hasArms);
        forearms?.SetActive(skin.hasForearms);
        hands?.SetActive(skin.hasHands);
        hips?.SetActive(skin.hasHips);
        legs?.SetActive(skin.hasLegs);
        //ankles?.SetActive(skin.hasAnkles);
        feet?.SetActive(skin.hasFeet);
        if (beard != null)
            beard?.SetActive(skin.hasBeard);
    }

    public bool changeMaterial(DIRECTION direction)
    {
        try{
            if ((direction == DIRECTION.Decrease && indexMaterial > 0) ||
                (direction == DIRECTION.Increase && indexMaterial < currentSkin.materialsSize() - 1))
            {
                indexMaterial += (direction == DIRECTION.Increase) ? 1 : -1;
                changeMaterial(indexMaterial);

                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error when trying to change material ({direction}): {ex.Message}");
            return false;
        }
    }

    public void changeMaterial(int index)
    {
        if (index < 0)
            index = 0;
        StartCoroutine(changeMaterialDelayed(index));
    }

    public void setMaterialIndex(int index) => indexMaterial = index;
    
    IEnumerator changeMaterialDelayed(int index)
    {
        yield return null;
        if (skinContainer != null && skinContainer.childCount > 0)
            foreach (Transform child in skinContainer.GetChild(0))
                currentSkin.changeMaterial(child.gameObject, index);
    }

    protected void toggleVisibleMeshes(bool state)
    {
        if (state)
            StartCoroutine(DelayedCreateMeshes());
        else
            if (skinContainer.childCount > 0)
                if (!Application.isPlaying)
                    DestroyImmediate(skinContainer.GetChild(0).gameObject);
                else
                    Destroy(skinContainer.GetChild(0).gameObject);
    }

    IEnumerator DelayedCreateMeshes()
    {
        yield return null;
        CreateMeshes();
    }

    private void CreateMeshes()
    {
        SkinnedMeshRenderer[] meshes = skins[indexSkin].getMeshes();
        GameObject skinNameContainer = new GameObject(skins[indexSkin].getName());
        skinNameContainer.transform.SetParent(skinContainer, true);
        foreach (var mesh in meshes){
            SkinnedMeshRenderer newMesh = Instantiate(mesh, rootBone.localPosition, Quaternion.identity, skinNameContainer.transform);
            newMesh.bones = chest.GetComponent<SkinnedMeshRenderer>().bones;
            newMesh.rootBone = rootBone;
        }
       
    }

    public string getSkinName(int index) => skins[index].getName();
    public int getIndex() => indexSkin;
    public int getMaterialIndex() => indexMaterial;

    #endregion
}
