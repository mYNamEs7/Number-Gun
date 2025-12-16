using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SuperCard))]
public class SuperCardEditor : Editor
{
    SuperCard superCard;
    SerializedProperty upgrades;

    void OnEnable()
    {
        superCard = target as SuperCard;
        upgrades = serializedObject.FindProperty("upgrades");
    }

    // public override void OnInspectorGUI()
    // {
    //     serializedObject.ApplyModifiedProperties();
    //     serializedObject.Update();

    //     EditorGUILayout.PropertyField(upgrades);

    //     serializedObject.ApplyModifiedProperties();
    //     EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    //     DrawDefaultInspector();
    // }
}
