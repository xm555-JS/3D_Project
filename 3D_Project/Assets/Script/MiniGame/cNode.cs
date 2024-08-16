using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cNode : MonoBehaviour
{
    public enum Point { LEFT_UP, LEFT_DOWN, RIGHT_DOWN, RIGHT_UP }
    public Point position;

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("MiniMonster"))
        {
            switch (position)
            { 
                case Point.LEFT_UP:
                    if (other.transform.position.x <= -30f)
                        other.transform.rotation = this.transform.rotation;
                    break;
                case Point.LEFT_DOWN:
                    if (other.transform.position.z <= -30f)
                        other.transform.rotation = this.transform.rotation;
                    break;
                case Point.RIGHT_DOWN:
                    if (other.transform.position.x >= 30f)
                        other.transform.rotation = this.transform.rotation;
                    break;
                case Point.RIGHT_UP:
                    if (other.transform.position.z >= 30f)
                        other.transform.rotation = this.transform.rotation;
                    break;
            }
        }
    }
}
