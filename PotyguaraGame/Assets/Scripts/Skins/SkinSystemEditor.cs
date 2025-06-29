using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

enum ArrowType { Mesh, Material }

#if UNITY_EDITOR
[CustomEditor(typeof(EditSkin))]
public class EditSystemEditor : Editor
{
    #region SerializedProps
    SerializedProperty skins;
    SerializedProperty currentSkin;
    SerializedProperty hair, head, chest, belly, arms, forearms, hands, hips, legs, feet, beard;
    SerializedProperty skinContainer, rootBone;

    #endregion

    private void OnEnable(){
        skins = serializedObject.FindProperty("skins");
        currentSkin = serializedObject.FindProperty("currentSkin");

        hair = serializedObject.FindProperty("hair");
        head = serializedObject.FindProperty("head");
        chest = serializedObject.FindProperty("chest");
        belly = serializedObject.FindProperty("belly");
        arms = serializedObject.FindProperty("arms");
        forearms = serializedObject.FindProperty("forearms");
        hands = serializedObject.FindProperty("hands");
        hips = serializedObject.FindProperty("hips");
        legs = serializedObject.FindProperty("legs");
        //ankles = serializedObject.FindProperty("ankles");
        feet = serializedObject.FindProperty("feet");
        beard = serializedObject.FindProperty("beard");

        skinContainer = serializedObject.FindProperty("skinContainer");
        rootBone = serializedObject.FindProperty("rootBone");
        currentSkin = skins.GetArrayElementAtIndex(0);
    }

    public override void OnInspectorGUI()
    {
        EditSkin _editSkin = (EditSkin)target;

        //base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((EditSkin)target), typeof(EditSkin), false);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.PropertyField(rootBone, new GUIContent("Root bone - Rig armature"));
        EditorGUILayout.PropertyField(skinContainer, new GUIContent("Skin container"));
        GUILayout.Space(15);

        //EditorGUILayout.PropertyField(ankles, new GUIContent("ankles"));
        EditorGUILayout.PropertyField(arms, new GUIContent("arms"));
        EditorGUILayout.PropertyField(beard, new GUIContent("beard"));
        EditorGUILayout.PropertyField(belly, new GUIContent("belly"));
        EditorGUILayout.PropertyField(chest, new GUIContent("chest"));
        EditorGUILayout.PropertyField(feet, new GUIContent("feet"));
        EditorGUILayout.PropertyField(forearms, new GUIContent("forearms"));
        EditorGUILayout.PropertyField(hair, new GUIContent("hair"));
        EditorGUILayout.PropertyField(hands, new GUIContent("hands"));
        EditorGUILayout.PropertyField(head, new GUIContent("head"));
        EditorGUILayout.PropertyField(hips, new GUIContent("hips"));
        EditorGUILayout.PropertyField(legs, new GUIContent("legs"));
        GUILayout.Space(15);

        EditorGUILayout.LabelField("SKIN SET", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(skins, new GUIContent("Skins", "Texture swapping is enabled when adding more than one material"));

        GUILayout.Space(15);
        EditorGUILayout.LabelField("SELECT SKIN", EditorStyles.boldLabel);

        #region POPUP FIELD (ENUM-LIKE)
        List<string> skinNames = new List<string>();
            for (int i = 0; i < skins.arraySize; i++){
                SerializedProperty skin = skins.GetArrayElementAtIndex(i);
                string skinName = _editSkin.getSkinName(i);
                skinNames.Add(skinName);
            }
            int selectedIndex = skinNames.IndexOf(_editSkin.currentSkin.getName());
            if (selectedIndex < 0) selectedIndex = 0;
            selectedIndex = EditorGUILayout.Popup(selectedIndex, skinNames.ToArray());

            if (selectedIndex >= 0 && selectedIndex < skinNames.Count)
            {
                _editSkin.changeMesh(selectedIndex);
                EditorUtility.SetDirty(_editSkin);
            }
        #endregion


        EditorGUILayout.LabelField("Or use arrows");

        #region ARROWS
            GUILayout.BeginHorizontal();
                Arrows(ArrowType.Mesh);
                //GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            //GUILayout.BeginHorizontal();
            //    GUILayout.Label("", GUILayout.Width(20));
            //    GUILayout.BeginVertical();
            //        EditorGUILayout.LabelField("EDIT CURRENT SKIN:");
            //        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentSkin"));
            //    GUILayout.EndVertical();
            //GUILayout.EndHorizontal();

        #endregion

        //CAMPO CONDICIONAL
        if (_editSkin.currentSkin.materialsSize() > 1)
        {
            GUILayout.Space(10);
            int index = _editSkin.getMaterialIndex();
            EditorGUILayout.LabelField("SELECT TEXTURE/COLOR: " + _editSkin.currentSkin.getMaterialName(index), EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            Arrows(ArrowType.Material);

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);
        GUILayout.Label("Reset skin system:", EditorStyles.boldLabel);
        if (GUILayout.Button("Reset"))
        {
            currentSkin = skins.GetArrayElementAtIndex(0);
            //_editSkin.disableMeshes();
            _editSkin.changeMesh(0); //reset
            _editSkin.currentSkin = _editSkin.skins[0];
            _editSkin.changeMaterial(0);
            _editSkin.setMaterialIndex(0);
            Debug.Log("Skin system reseted for this character");

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(_editSkin);
        }

        GUILayout.Space(10);
        GUILayout.Label("Save skin:", EditorStyles.boldLabel);
        if (GUILayout.Button("Save"))
        {
            _editSkin.saveSkin();
        }

        EditorUtility.SetDirty(_editSkin);
        serializedObject.ApplyModifiedProperties();
    }

    private void Arrows(ArrowType type)
    {
        EditSkin _editSkin = (EditSkin)target;
        int currentIndex = _editSkin.getIndex();
        int skinCount = skins.arraySize;
        int materialIndex = _editSkin.getMaterialIndex();
        int materialCount = _editSkin.currentSkin.materialsSize();

        if (type == ArrowType.Mesh)
        {
            GUI.enabled = currentIndex > 0;
        }
        else if (type == ArrowType.Material)
        {
            GUI.enabled = materialIndex > 0;
        }

        if (GUILayout.Button("<", GUILayout.Width(45), GUILayout.Height(45)))
        {
            if (type == ArrowType.Mesh)
                _editSkin.changeMesh(DIRECTION.Decrease);
            else
                _editSkin.changeMaterial(DIRECTION.Decrease);
        }

        if (type == ArrowType.Mesh)
        {
            GUI.enabled = currentIndex < skinCount - 1;
        }
        else if (type == ArrowType.Material)
        {
           GUI.enabled = materialIndex < materialCount - 1;
        }

        if (GUILayout.Button(">", GUILayout.Width(45), GUILayout.Height(45)))
        {
            if (type == ArrowType.Mesh)
                _editSkin.changeMesh(DIRECTION.Increase);
            else
                _editSkin.changeMaterial(DIRECTION.Increase);
        }

        EditorUtility.SetDirty(_editSkin);
        Repaint();
        GUI.enabled = true;
    }

}
#endif