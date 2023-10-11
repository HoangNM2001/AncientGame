#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pancake;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    [SerializeField] private GameObject fieldPrefab;
    [SerializeField] private int rowNum;
    [SerializeField] private int columnNum;
    [SerializeField] private float rowOffset;
    [SerializeField] private float columnOffset;
    [SerializeField] private float space;

    public GameObject FieldPrefab => fieldPrefab;
    public int RowNum => rowNum;
    public int ColumnNum => columnNum;
    public float RowOffset => rowOffset;
    public float ColumnOffset => columnOffset;
    public float Space => space;
}

[CustomEditor(typeof(FieldGenerator))]
public class FieldGeneratorEditor : Editor
{
    private FieldGenerator fieldGenerator;
    private GameObject fieldPrefab;
    private int rowNum;
    private int columnNum;
    private float rowOffset;
    private float columnOffset;
    private float space;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        fieldGenerator = (FieldGenerator)target;
        fieldPrefab = fieldGenerator.FieldPrefab;
        rowNum = fieldGenerator.RowNum;
        columnNum = fieldGenerator.ColumnNum;
        rowOffset = fieldGenerator.RowOffset;
        columnOffset = fieldGenerator.ColumnOffset;
        space = fieldGenerator.Space;

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
        extendField.layer = LayerMask.NameToLayer("Interact");

        ExtendField extendFieldScript = extendField.AddComponent<ExtendField>();

        var sizeOfBoxPerBigField = 6.2f;
        BoxCollider boxCollider = extendField.AddComponent<BoxCollider>();
        // boxCollider.size = new Vector3(1.6f * columnNum, 1.0f, 1.6f * rowNum);
        // boxCollider.center = new Vector3(0.0f, 0.5f, 0.0f);
        boxCollider.size = new Vector3(sizeOfBoxPerBigField * columnNum / 4, 1.0f, sizeOfBoxPerBigField * rowNum / 4);
        boxCollider.center = new Vector3(sizeOfBoxPerBigField * (columnNum / 4 - 1) / 2, 0.5f,
            sizeOfBoxPerBigField * (rowNum / 4 - 1) / 2);
        boxCollider.isTrigger = true;

        GameObject tempField = (GameObject)PrefabUtility.InstantiatePrefab(fieldPrefab, extendField.transform);

        MeshFilter fieldMesh = tempField.GetComponentInChildren<MeshFilter>();
        float fieldSizeX = fieldMesh.sharedMesh.bounds.size.x - columnOffset;
        float fieldSizeY = fieldMesh.sharedMesh.bounds.size.y - rowOffset;
        
        // Vector3 startPos = new Vector3(-fieldSizeX * (columnNum - 1) / 2.0f, tempField.transform.localPosition.y,
        //     -fieldSizeY * (rowNum - 1) / 2.0f);
        Vector3 startPos = new Vector3(-fieldSizeX * 1.5f, tempField.transform.localPosition.y, -fieldSizeY * 1.5f);

        DestroyImmediate(tempField);

        for (int row = 0; row < rowNum; row++)
        {
            for (int column = 0; column < columnNum; column++)
            {
                GameObject newField = (GameObject)PrefabUtility.InstantiatePrefab(fieldPrefab, extendField.transform);
                newField.name = "Field " + row + "x" + column;

                newField.transform.localPosition = new Vector3(startPos.x + fieldSizeX * column + space * (column / 4),
                    startPos.y,
                    startPos.z + fieldSizeY * row + space * (row / 4));

                newField.GetComponent<Field>().ResetUniqueID();
            }
        }
    }
}
#endif