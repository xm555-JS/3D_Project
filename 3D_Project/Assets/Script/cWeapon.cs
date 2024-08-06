using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cWeapon : MonoBehaviour
{
    public enum Type { MELEE, RANGE }
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailRenderer;

    public void Use()
    {
        if (type == Type.MELEE)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
    }

    IEnumerator Swing()
    {
        yield return null;
        meleeArea.enabled = true;
        trailRenderer.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailRenderer.enabled = false;
    }
}
