using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cChase : MonoBehaviour
{
    public GameObject target;
    void Update()
    {
        this.transform.position = target.transform.position;
    }
}
