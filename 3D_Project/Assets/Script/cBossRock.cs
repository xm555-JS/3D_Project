using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cBossRock : cBullet
{
    Rigidbody rigid;
    float angularPower;
    float scaleValue;
    bool isShot;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine("GainPowerTime");
        StartCoroutine("GainPower");
    }

    IEnumerator GainPowerTime()
    {
        yield return new WaitForSeconds(2.2f);
        isShot = true;
    }

    IEnumerator GainPower()
    {
        while (!isShot)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;
            this.transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(Vector3.right * angularPower, ForceMode.Acceleration);

            yield return null;
        }
    }
}
