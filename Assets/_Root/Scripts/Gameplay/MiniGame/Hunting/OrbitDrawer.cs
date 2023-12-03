using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Pancake;
using Unity.Mathematics;
using UnityEngine;

public class OrbitDrawer : GameComponent
{
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private int numberOfPoint;

    private const float spaceBetweenPoints = 0.04f;
    private List<Transform> pointList = new List<Transform>();

    private void Awake()
    {
        for (var i = 0; i < numberOfPoint; i++)
        {
            pointList.Add(Instantiate(pointPrefab, transform).transform);
        }
    }

    public void DrawOrbit(Vector3 force)
    {
        // Debug.Log("Force: " + force);
        for (int i = 0; i < numberOfPoint; i++)
        {
            pointList[i].gameObject.SetActive(true);
            pointList[i].localPosition = CalculatePosition((float)(i * spaceBetweenPoints), force);
            // Debug.LogError(i + "---" + pointList[i].localPosition);
        }
    }

    private Vector3 CalculatePosition(float t, Vector3 force)
    {
        return new Vector3(force.x * t, force.y * t + Physics.gravity.y * t * t * 0.5f, 0.0f);
    }

    public void EraseOrbit()
    {
        gameObject.SetActive(false);
    }
}
