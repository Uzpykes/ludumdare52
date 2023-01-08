using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void Start()
    {
        transform.LookAt(Camera.main.gameObject.transform);
    }
}
