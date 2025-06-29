using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//################# --- MATERIAL CLASS --- #####################
[System.Serializable]
public class SkinMaterial
{
    public string name;
    public Material material;
}


[CreateAssetMenu(fileName = "newSkin", menuName = "Skin")]
public class Skin : ScriptableObject
{
    [SerializeField] private string skinName;
    [SerializeField] public SkinnedMeshRenderer[] skinMeshes;
    public SkinMaterial[] skinMaterials;
    [SerializeField]
    public bool hasHair = true, hasBeard = false, hasHead = true, hasChest = true, hasBelly = true,
                                    hasArms = true, hasForearms = true, hasHands = true, hasHips = true,
                                    hasLegs = true, hasAnkles = true, hasFeet = true;

    public string getName() => skinName;

    public SkinnedMeshRenderer[] getMeshes() => skinMeshes;

    public int materialsSize() => skinMaterials.Length;

    public string getMaterialName(int index) => skinMaterials[index].name;

    public void changeMaterial(GameObject piece, int materialIndex)
    {
        SkinnedMeshRenderer mesh = piece.GetComponent<SkinnedMeshRenderer>();
        Material[] materials = mesh.sharedMaterials;
        materials[0] = skinMaterials[materialIndex].material;
        mesh.sharedMaterials = materials;
    }
}
