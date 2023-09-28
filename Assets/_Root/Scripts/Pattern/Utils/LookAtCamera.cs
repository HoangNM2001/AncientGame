using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform mainCamTrans;

    private void Start()
    {
        if (Camera.main != null) mainCamTrans = Camera.main.transform;
        transform.rotation = Quaternion.Euler(mainCamTrans.localEulerAngles.x,mainCamTrans.localEulerAngles.y, transform.rotation.z);
    }
}
