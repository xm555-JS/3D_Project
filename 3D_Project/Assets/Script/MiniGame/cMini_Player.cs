using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class cMini_Player : MonoBehaviour
{
    public float speed;

    Camera miniCam;
    cMini_Weapon weapon;
    Animator anim;
    Transform targetEnemy;

    Vector3 moveDir;
    Vector3 destinationPos;
    Quaternion lookTarget;

    bool isSelected;
    bool isClick;
    bool isMove;

    float rotateTime;
    float fireTime;

    // 현재 선택된 플레이어를 다른 플레이어가 알 수 있게 static으로 알리기
    private static cMini_Player currentlySelectedPlayer;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        weapon = GetComponentInChildren<cMini_Weapon>();
    }

    void Start()
    {
        miniCam = GameObject.FindWithTag("Mini_Camera").GetComponent<Camera>();
    }

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
        int layerMask = 1 << LayerMask.NameToLayer("Player");
        if (Physics.Raycast(ray, out hit, maxDistance, layerMask) && isClick)
        {
            // 현재 클릭한 플레이어를 선택
            if (hit.transform == this.transform)
            {
                // 다른 플레이어가 선택된 상태라면 해당 플레이어의 선택 해제
                if (currentlySelectedPlayer != null && currentlySelectedPlayer != this)
                {
                    currentlySelectedPlayer.isSelected = false;
                }

                currentlySelectedPlayer = this;
                isSelected = true;

                return;
            }
        }

        // 선택된 플레이어가 이 플레이어인 경우에만 목적지를 변경할 수 있게
        if (isSelected)
        {
            int floorLayerMask = 1 << LayerMask.NameToLayer("Mini_Floor_Player");
            if (Physics.Raycast(ray, out hit, maxDistance, floorLayerMask) && isClick)
            {
                destinationPos = hit.point;
                moveDir = destinationPos - transform.position;
                lookTarget = Quaternion.LookRotation(moveDir);
                isMove = true;
            }
        }
    }

    void Move()
    {
        if (isMove)
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
        {
            anim.SetBool("isRun", false);
        }

        float posX = Mathf.Clamp(transform.position.x, -24f, 24f);
        float posZ = Mathf.Clamp(transform.position.z, -24f, 24f);
        transform.position = new Vector3(posX, transform.position.y, posZ);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("MiniMonster"))
        {
            if (!targetEnemy)
                targetEnemy = other.transform;
            else if (targetEnemy)
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