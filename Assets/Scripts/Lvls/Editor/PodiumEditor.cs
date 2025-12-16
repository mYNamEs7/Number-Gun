using System;
using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Podium), true)]
public class PodiumEditor : Editor
{
    private Podium _podium;
    SerializedProperty hp, height, width, color;
    SerializedProperty item, podiumModel, hpTxt, podiumRenderer;
    GameData gameData;
    int oldHP;

    void OnEnable()
    {
        _podium = target as Podium;
        gameData = AssetDatabase.LoadAssetAtPath<GameData>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Game Data", new[] { "Assets/Data" })[0]));
        Undo.undoRedoPerformed += UpdateAll;

        hp = serializedObject.FindProperty("hp");
        oldHP = hp.intValue;
        color = serializedObject.FindProperty("color");
        hpTxt = serializedObject.FindProperty("hpTxt");
        item = serializedObject.FindProperty("item");
        podiumRenderer = serializedObject.FindProperty("podiumRenderer");

        if (!(_podium is ResizePodium)) return;
        podiumModel = serializedObject.FindProperty("podiumModel");
        height = serializedObject.FindProperty("height");
        width = serializedObject.FindProperty("width");
    }

    void OnDisable()
    {
        Undo.undoRedoPerformed -= UpdateAll;
    }

    void UpdateAll()
    {
        UpdateHP();
        UpdateColor();

        if (!(_podium is ResizePodium)) return;
        UpdateHeight();
        UpdateWidth();
    }

    void UpdateHP()
    {
        TextMeshPro txt = hpTxt.objectReferenceValue as TextMeshPro;
        int hp = this.hp.intValue;
        if (oldHP == hp) return;
        EditorUtility.SetDirty(txt);
        txt.text = hp < 1000 ? hp.ToString() : Math.Round(hp / 1000f, 1).ToString() + "K";
        oldHP = hp;
    }

    void UpdateColor()
    {
        for (int i = 0; i < podiumRenderer.arraySize; i++)
        {
            Renderer renderer = podiumRenderer.GetArrayElementAtIndex(i).objectReferenceValue as Renderer;
            if (renderer.sharedMaterial == gameData.podiumMaterials[color.enumValueIndex]) return;
            EditorUtility.SetDirty(renderer);
            renderer.sharedMaterial = gameData.podiumMaterials[color.enumValueIndex];
        }
    }

    void UpdateHeight()
    {
        Transform podium = podiumModel.objectReferenceValue as Transform;
        if (podium.localScale.y != height.floatValue)
        {
            EditorUtility.SetDirty(podium);
            podium.localScale = new Vector3(podium.localScale.x, height.floatValue, podium.localScale.z);
        }
        TextMeshPro txt = hpTxt.objectReferenceValue as TextMeshPro;
        if (txt.transform.localPosition.y != height.floatValue + ResizePodium.AddHPHeight)
        {
            EditorUtility.SetDirty(txt);
            txt.transform.localPosition = new Vector3(txt.transform.localPosition.x, height.floatValue + ResizePodium.AddHPHeight, txt.transform.localPosition.z);
        }
        if (!item.objectReferenceValue) return;
        Transform itemTransform = (item.objectReferenceValue as Item).transform;
        if (itemTransform.parent != _podium.transform)
        {
            EditorUtility.SetDirty(itemTransform);
            itemTransform.parent = _podium.transform;
        }
        if (itemTransform.localPosition != new Vector3(0, height.floatValue + ResizePodium.AddItemHeight, 0))
        {
            EditorUtility.SetDirty(itemTransform);
            itemTransform.localPosition = new Vector3(0, height.floatValue + ResizePodium.AddItemHeight, 0);
        }
    }

    void UpdateWidth()
    {
        Transform podium = podiumModel.objectReferenceValue as Transform;
        if (width.floatValue != podium.localScale.x || width.floatValue != podium.localScale.z)
        {
            EditorUtility.SetDirty(podium);
            podium.localScale = new Vector3(width.floatValue, podium.localScale.y, width.floatValue);
        }
        BoxCollider collider = _podium.GetComponent<BoxCollider>();
        if (width.floatValue * ResizePodium.ColliderWidth != collider.size.x || width.floatValue * ResizePodium.ColliderLength != collider.size.z)
        {
            EditorUtility.SetDirty(collider);
            collider.size = new Vector3(width.floatValue * ResizePodium.ColliderWidth, collider.size.y, width.floatValue * ResizePodium.ColliderLength);
        }
        TextMeshPro txt = hpTxt.objectReferenceValue as TextMeshPro;
        if (txt.transform.localPosition.z != -1.2f * width.floatValue)
        {
            EditorUtility.SetDirty(txt);
            txt.transform.localPosition = new Vector3(txt.transform.localPosition.x, txt.transform.localPosition.y, -1.2f * width.floatValue);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(hp);
        if (EditorGUI.EndChangeCheck()) UpdateHP();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(color);
        if (EditorGUI.EndChangeCheck()) UpdateColor();

        if (_podium is ResizePodium)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(height);
            if (EditorGUI.EndChangeCheck()) UpdateHeight();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(width);
            if (EditorGUI.EndChangeCheck()) UpdateWidth();
        }
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(item);
        if (EditorGUI.EndChangeCheck()) UpdateHeight();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawDefaultInspector();
    }
}
