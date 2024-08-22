using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cOrbit : MonoBehaviour
{
    public GameObject target;
    public float orbitSpeed;
    Vector3 radiusVec;
    // Start is called before the first frame update
    void Start()
    {
        radiusVec = this.transform.position - target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Orbit();
    }

    void Orbit()
    {
        this.transform.position = target.transform.position + radiusVec;
        this.transform.RotateAround(target.transform.position, Vector3.up, orbitSpeed * Time.deltaTime);
        radiusVec = this.transform.position - target.transform.position;
    }
}
