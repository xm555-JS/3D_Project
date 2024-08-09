using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0414

public class Player : MonoBehaviour
{
    [Header("[Component]")]
    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshRenderders;

    [Header("[PlayerInfo]")]
    public Camera followCamere;
    public GameObject[] grenades;
    public int ammo;
    public int coin;
    public int health;
    public int hasGrenades;
    bool isDamage;

    [Header("[PlayerInfo_Max]")]
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    [Header("[Move]")]
    public float speed;
    public float jumpPower;

    float hAxis;
    float vAxis;
    float defaultSpeed;

    Vector3 moveDir;
    Vector3 inputVec;
    Vector3 dodgeDir;

    bool isDown;
    bool isJump;
    bool isAir;
    bool isDodge;
    bool isRoll;

    bool isBorder;

    [Header("[Weapon]")]
    public GameObject[] weapons;
    public bool[] hasWeapon;
    public GameObject grenadeObj;
    cWeapon curWeapon;
    GameObject nearObject;

    bool isWeaponTrigger;
    bool isSwap;
    bool isFire;
    bool isFireReady = true;
    bool isReload;
    bool isLoading;
    bool isGrenade;
    bool isThrow;

    float fireDelay;

    int weaponIndex;
    int preWeaponIndex;

    public Vector3 GetPlayerDir() { return moveDir; }

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshRenderders = GetComponentsInChildren<MeshRenderer>();
    }

    void Start()
    {
        defaultSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        GetDir();
        Turn();
        Jump();
        Attack();
        Grenade();
        Reload();
        Dodge();
        Interection();
        Swap();
    }

    void FixedUpdate()
    {
        Move();
        FreezeRotation();
        StopToWall();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isAir = false;
            anim.SetBool("isJump", false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.gameObject.CompareTag("Item"))
        {
            cItem item = other.GetComponent<cItem>();

            switch (item.type)
            {
                case cItem.Type.AMMO:
                    ammo += item.value;
                    if (ammo >= maxAmmo)
                        ammo = maxAmmo;
                    break;

                case cItem.Type.COIN:
                    coin += item.value;
                    if (coin >= maxCoin)
                        coin = maxCoin;
                    break;

                case cItem.Type.HEART:
                    health += item.value;
                    if (health >= maxHealth)
                        health = maxHealth;
                    break;

                case cItem.Type.GRENADE:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades >= maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }

            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            if (!isDamage)
            {
                cBullet enemyBullet = other.GetComponent<cBullet>();
                health -= enemyBullet.damage;
                StartCoroutine("OnDamage");
            }

            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
            nearObject = null;
    }

    IEnumerator OnDamage()
    {
        isDamage = true;

        foreach (MeshRenderer meshRenderer in meshRenderders)
        {
            meshRenderer.material.color = Color.yellow;
        }

        yield return new WaitForSeconds(1f);

        isDamage = false;
        foreach (MeshRenderer meshRenderer in meshRenderders)
        {
            meshRenderer.material.color = Color.white;
        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5f, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5f, LayerMask.GetMask("Wall"));
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        isDown = Input.GetButton("Walk");
        isJump = Input.GetButtonDown("Jump");
        isFire = Input.GetButton("Fire1");
        isGrenade = Input.GetButton("Fire2");
        isReload = Input.GetButtonDown("Reload");
        isDodge = Input.GetButtonDown("Dodge");
        isWeaponTrigger = Input.GetButtonDown("Interection");
    }

    void GetDir()
    {
        moveDir = new Vector3(hAxis, 0f, vAxis).normalized;
        //transform.position += moveVec * speed * Time.deltaTime;

        // 회피할 때 dir 고정
        if (isRoll)
            moveDir = dodgeDir;
        if (isSwap || !isFireReady || isLoading)
            moveDir = Vector3.zero;

        // 애니메이션
        anim.SetBool("isRun", moveDir != Vector3.zero);
        anim.SetBool("isWalk", isDown);
    }

    void Turn()
    {
        // 회전
        transform.LookAt(transform.position + moveDir);

        // 마우스 회전
        if (isFire)
        {
            Ray ray = followCamere.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 moveDir = rayHit.point - transform.position;
                moveDir.y = 0f;
                transform.LookAt(transform.position + moveDir);
            }
        }
    }

    void Move()
    {
        // rigidybody를 이용한 플레이어 이동
        // Rigidbody를 이용하여 플레이어를 움직이게 바꿈
        // FixedUpdate에서 처리함으로 같은 프레임률에서 충돌을 처리할 수 있어 안전하게 물리 상호작용이 가능
        if (!isBorder)
        {
            inputVec = moveDir * speed * (isDown ? 0.3f : 1f) * Time.deltaTime;
            rigid.MovePosition(rigid.position + inputVec);
        }
    }

    void Jump()
    {
        if (isJump && !isAir && !isDodge)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isAir = true;

            // 애니메이션
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
        }
    }

    void Dodge()
    {
        if (isDodge && !isAir && !isJump)
        {
            // 플레이어 Dir 방향으로 이동
            dodgeDir = moveDir;

            speed *= 3f;
            isRoll = true;

            // 애니메이션
            anim.SetTrigger("doDodge");

            Invoke("FinishDodge", 0.5f);
        }
    }

    void FinishDodge()
    {
        speed = defaultSpeed;
        isRoll = false;
    }

    void Interection()
    {
        if (isWeaponTrigger && nearObject != null)
        {
            if (nearObject.CompareTag("Weapon"))
            {
                cItem item = nearObject.GetComponent<cItem>();
                int weaponIndex = item.value;
                hasWeapon[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    void Swap()
    {
        bool _swapHammer = Input.GetButtonDown("Swap1");
        bool _swapHandGun = Input.GetButtonDown("Swap2");
        bool _swapSubMachineGun = Input.GetButtonDown("Swap3");

        if (_swapHammer) { weaponIndex = 0; }
        else if (_swapHandGun) { weaponIndex = 1; }
        else if (_swapSubMachineGun) { weaponIndex = 2; }

        // 무기를 쓸 수 있는지 없는지 check
        if (!hasWeapon[weaponIndex])
            return;

        if ((_swapHammer || _swapHandGun || _swapSubMachineGun) && !isJump && !isDodge && !isSwap)
        {
            if (!curWeapon)
                preWeaponIndex = weaponIndex;
            else if (curWeapon)
            {
                // 만약에 같은 무기 인덱스라면 Swap X
                if (preWeaponIndex == weaponIndex)
                    return;
                else
                {
                    curWeapon.gameObject.SetActive(false);
                    preWeaponIndex = weaponIndex;
                }
            }

            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("FinishSwap", 0.5f);

            curWeapon = weapons[weaponIndex].GetComponent<cWeapon>();
            curWeapon.gameObject.SetActive(true);
        }
    }

    void FinishSwap()
    {
        isSwap = false;
    }

    void Attack()
    {
        if (!curWeapon)
            return;

        fireDelay += Time.deltaTime;

        if (fireDelay >= curWeapon.rate)
            isFireReady = true;

        if (isFire && isFireReady && !isDodge && !isSwap)
        {
            curWeapon.Use();

            if (curWeapon.type == cWeapon.Type.MELEE)
                anim.SetTrigger("doSwing");
            else
                anim.SetTrigger("doShot");

            fireDelay = 0f;
            isFireReady = false;
        }
    }

    void Grenade()
    {
        if (hasGrenades == 0)
            return;

        if (isGrenade && !isLoading && !isSwap)
        {
            RaycastHit hit;
            // 결과 광선은 월드 공간에 있으며, 카메라의 근거리 평면에서 시작하여 화면의 위치(x,y) 픽셀 좌표를 통과합니다
            // 화면 공간은 픽셀로 정의됩니다. 화면의 왼쪽 하단은 (0,0)이고 오른쪽 상단은 ( pixelWidth -1, pixelHeight -1)입니다.
            Ray ray = followCamere.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100f))
            {
                Vector3 moveDir = hit.transform.position - this.transform.position;
                moveDir.y = 10f;

                GameObject grenade = Instantiate(grenadeObj, this.transform.position, this.transform.rotation);
                Rigidbody grenadeRigid = grenade.GetComponent<Rigidbody>();
                grenadeRigid.AddForce(moveDir, ForceMode.Impulse);
                //grenadeRigid.AddForce(Vector3.back * 10f, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }

    void Reload()
    {
        if (curWeapon == null)
            return;

        if (curWeapon.type == cWeapon.Type.MELEE)
            return;

        if (ammo == 0)
            return;


        if (isReload && !isJump && !isDodge && !isSwap && isFireReady)
        {
            isLoading = true;
            anim.SetTrigger("doReload");
            Invoke("FinishReload", 3f);
        }
    }

    void FinishReload()
    {
        curWeapon.curAmmo = curWeapon.maxAmmo;
        ammo -= curWeapon.maxAmmo;
        isLoading = false;
    }
}
