#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEditor;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    [SerializeField] private GameObject fieldPrefab;
    [SerializeField] private int rowNum;
    [SerializeField] private int columnNum;

    public GameObject FieldPrefab => fieldPrefab;
    public int RowNum => rowNum;
    public int ColumnNum => columnNum;
}

[CustomEditor(typeof(FieldGenerator))]
public class FieldGeneratorEditor : Editor
{
    private FieldGenerator fieldGenerator;
    private GameObject fieldPrefab;
    private int rowNum;
    private int columnNum;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        fieldGenerator = (FieldGenerator)target;
        fieldPrefab = fieldGenerator.FieldPrefab;
        rowNum = fieldGenerator.RowNum;
        columnNum = fieldGenerator.ColumnNum;

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Extend Field"))
        {
            GenerateExtendField();
        }
    }

    private void GenerateExtendField()
    {
        fieldGenerator.transform.RemoveAllChildren();
        GameObject extendField = new GameObject("ExtendField " + rowNum + "x" + columnNum);
        extendField.transform.SetParent(fieldGenerator.transform);

        GameObject tempField = (GameObject)PrefabUtility.InstantiatePrefab(fieldPrefab, extendField.transform);

        MeshFilter fieldMesh = tempField.GetComponentInChildren<MeshFilter>();
        float fieldSizeX = fieldMesh.sharedMesh.bounds.size.x;
        float fieldSizeY = fieldMesh.sharedMesh.bounds.size.y;

        Vector3 startPos = new Vector3(-fieldSizeX * (columnNum - 1) / 2.0f, tempField.transform.localPosition.y,
            -fieldSizeY * (rowNum - 1) / 2.0f);

        DestroyImmediate(tempField);

        for (var row = 0; row < rowNum; row++)
        {
            for (var column = 0; column < columnNum; column++)
            {
                GameObject newField = (GameObject)PrefabUtility.InstantiatePrefab(fieldPrefab, extendField.transform);
                newField.name = "Field " + row + "x" + column;
                newField.transform.localPosition = new Vector3(startPos.x + fieldSizeX * column, startPos.y,
                    startPos.z + fieldSizeY * row);
            }
        }
    }
}
#endif