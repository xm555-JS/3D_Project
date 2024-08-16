using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class cMini_Player : MonoBehaviour
{
    public float speed;

    Camera miniCam;
    Animator anim;
    cMini_Weapon weapon;
    Transform selectedPlayer;
    Transform targetEnemy;
    Vector3 moveDir;
    Vector3 destinationPos;
    Quaternion lookTarget;
    bool isSelected;
    bool isClick;
    bool isMove;
    float rotateTime;
    float fireTime;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        weapon = GetComponentInChildren<cMini_Weapon>();
    }

    // Start is called before the first frame update
    void Start()
    {
        miniCam = GameObject.FindWithTag("Mini_Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        KeyDown();
        Move();
        Select();
    }

    void KeyDown()
    {
        // 마우스 클릭으로 플레이어 선택
        isClick = Input.GetButtonDown("Fire1");
    }

    void Select()
    {
        // Ray Info
        Ray ray = miniCam.ScreenPointToRay(Input.mousePosition);
        float maxDistance = 50f;
        RaycastHit hit;

        // 플레이어 선택
        if (isSelected)
        {
            int floorLayerMask = 1 << LayerMask.NameToLayer("Mini_Floor_Player");
            if (Physics.Raycast(ray, out hit, maxDistance, floorLayerMask) && isClick)
            {
                if (selectedPlayer == this.transform)
                {
                    destinationPos = hit.point;
                    moveDir = destinationPos - transform.position;
                    lookTarget = Quaternion.LookRotation(moveDir);
                    isMove = true;
                }
            }
        }
        // 플레이어 선택한 상태에서 땅 클릭
        int layerMask = 1 << LayerMask.NameToLayer("Player");
        if (Physics.Raycast(ray, out hit, maxDistance, layerMask) && isClick)
        {
            if (hit.transform == this.transform)
            {
                selectedPlayer = this.transform;
                isSelected = true;
            }
            else
                isSelected = false;
        }
    }

    void Move()
    {
        if (isMove && isSelected)
        {
            anim.SetBool("isRun", true);

            float rotationPower = 0.05f;
            rotateTime += Time.deltaTime;
            Mathf.Clamp01(rotateTime);
            transform.position += moveDir.normalized * Time.deltaTime * speed;
            transform.rotation = Quaternion.Lerp(transform.rotation, lookTarget, rotateTime * rotationPower);
            isMove = (transform.position - destinationPos).magnitude > 1f;
        }
        else
            anim.SetBool("isRun", false);
        float posX = Mathf.Clamp(transform.position.x, -32.5f, 32.5f);
        float posZ = Mathf.Clamp(transform.position.z, -32.5f, 32.5f);
        transform.position = new Vector3(posX, transform.position.y, posZ);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("MiniMonster"))
        {
            if (!targetEnemy)
                targetEnemy = other.transform;
            else if(targetEnemy)
            {
                if (other.transform != targetEnemy)
                    return;

                Vector3 dir = (targetEnemy.transform.position + targetEnemy.transform.forward) - transform.position;
                transform.LookAt(transform.position + dir.normalized);

                fireTime += Time.deltaTime;
                if (fireTime >= weapon.rate)
                {
                    anim.SetTrigger("doShot");
                    weapon.Use();
                    fireTime = 0f;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == targetEnemy)
            targetEnemy = null;
    }
}








//-----------------------------------------------------------------------------------------------------------------------------------------------------------

//Camera miniCam;
//public Transform selectedPlayer;
//bool isClick;
//Vector3 destinationPos;
//bool isMove;
//Vector3 moveDir;
//Quaternion lookTarget;
//bool isSelected;
//float time;

//// Start is called before the first frame update
//void Start()
//{
//    miniCam = GameObject.FindWithTag("Mini_Camera").GetComponent<Camera>();
//}

//// Update is called once per frame
//void Update()
//{
//    KeyDown();
//    Move();
//    Select();
//}

//void KeyDown()
//{
//    // 마우스 클릭으로 플레이어 선택
//    isClick = Input.GetButtonDown("Fire1");
//}

//void Select()
//{
//    // Ray Info
//    Ray ray = miniCam.ScreenPointToRay(Input.mousePosition);
//    float maxDistance = 50f;
//    RaycastHit hit;

//    // 플레이어 선택
//    if (isSelected)
//    {
//        int floorLayerMask = 1 << LayerMask.NameToLayer("Mini_Floor_Player");
//        if (Physics.Raycast(ray, out hit, maxDistance, floorLayerMask) && isClick)
//        {
//            if (selectedPlayer == this.transform)
//            {
//                destinationPos = hit.point;
//                isMove = true;
//                moveDir = destinationPos - transform.position;
//                lookTarget = Quaternion.LookRotation(moveDir);
//            }
//        }
//    }
//    // 플레이어 선택한 상태에서 땅 클릭
//    int layerMask = 1 << LayerMask.NameToLayer("Player");
//    if (Physics.Raycast(ray, out hit, maxDistance, layerMask) && isClick)
//    {
//        if (hit.transform == this.transform)
//        {
//            selectedPlayer = this.transform;
//            isSelected = true;
//        }
//        else
//            isSelected = false;
//    }
//}

//void Move()
//{
//    if (isMove && isSelected)
//    {
//        time += Time.deltaTime;
//        Mathf.Clamp01(time);
//        transform.position += moveDir.normalized * Time.deltaTime * 5f;
//        transform.rotation = Quaternion.Lerp(transform.rotation, lookTarget, time * 0.05f);
//        Debug.Log((transform.position - destinationPos).magnitude);
//        isMove = (transform.position - destinationPos).magnitude > 1f;
//    }
//}









//------------------------------------------------------------------------------------------------------------------------------------------------------------------
//Camera miniCam;
//Transform selectedPlayer;
//Vector3 playerVec;
//Vector3 destinationVec;
//Quaternion lookTarget;
//bool isSelected;
//bool isMove;
//Vector3 dir;
//            // 플레이어 선택
//        if (selectedPlayer)
//        {
//            int floorLayerMask = 1 << LayerMask.NameToLayer("Mini_Floor_Player");
//            if (Physics.Raycast(ray, out hit, maxDistance, floorLayerMask) && isSelected && !isMove)
//            {
//                playerVec = selectedPlayer.transform.position;
//                destinationVec = hit.point;
//                dir = destinationVec - playerVec;
//                lookTarget = Quaternion.LookRotation(dir);
//                isMove = true;
//            }
//        }
//        // 플레이어 선택한 상태에서 땅 클릭
//        if (Physics.Raycast(ray, out hit, maxDistance, layerMask) && isSelected)
//{
//    //if (isMove)
//    //{
//    //    isMove = false;
//    //}
//    selectedPlayer = hit.transform;
//}
//    }

//    void Move()
//{
//    if (isMove)
//    {
//        transform.position += dir.normalized * Time.deltaTime * 2f;
//        transform.rotation = Quaternion.Lerp(transform.rotation, lookTarget, 0.25f);

//        isMove = (transform.position - destinationVec).magnitude > 0.05f;
//    }
//}
