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
    public GameManager manager;
    public Camera followCamere;
    public GameObject[] grenades;
    public int score;
    public int ammo;
    public int coin;
    public int health;
    public int hasGrenades;
    bool isDamage;
    bool isDead;

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
    bool isRollCool;

    bool isBorder;
    bool isShop;

    [Header("[Weapon]")]
    public GameObject[] weapons;
    public bool[] hasWeapon;
    public GameObject grenadeObj;
    public cWeapon curWeapon;
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

        PlayerPrefs.SetInt("MaxScore", 12345);
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
        if (other.gameObject.CompareTag("Weapon") || other.gameObject.CompareTag("Shop"))
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
            nearObject = null;
        if (other.gameObject.CompareTag("Shop"))
        {
            cShop shop = other.GetComponent<cShop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }

    IEnumerator OnDamage()
    {
        isDamage = true;

        foreach (MeshRenderer meshRenderer in meshRenderders)
        {
            meshRenderer.material.color = Color.yellow;
        }

        if (health <= 0 && !isDead)
            OnDie();

        yield return new WaitForSeconds(1f);

        isDamage = false;
        foreach (MeshRenderer meshRenderer in meshRenderders)
        {
            meshRenderer.material.color = Color.white;
        }
    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
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

        // ȸ���� �� dir ����
        if (isRoll)
            moveDir = dodgeDir;
        if (isSwap || !isFireReady || isLoading || isDead)
            moveDir = Vector3.zero;

        // �ִϸ��̼�
        anim.SetBool("isRun", moveDir != Vector3.zero);
        anim.SetBool("isWalk", isDown);
    }

    void Turn()
    {
        // ȸ��
        transform.LookAt(transform.position + moveDir);

        // ���콺 ȸ��
        if (isFire && !isDead)
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
        // rigidybody�� �̿��� �÷��̾� �̵�
        // Rigidbody�� �̿��Ͽ� �÷��̾ �����̰� �ٲ�
        // FixedUpdate���� ó�������� ���� �����ӷ����� �浹�� ó���� �� �־� �����ϰ� ���� ��ȣ�ۿ��� ����
        if (!isBorder)
        {
            inputVec = moveDir * speed * (isDown ? 0.3f : 1f) * Time.deltaTime;
            rigid.MovePosition(rigid.position + inputVec);
        }
    }

    void Jump()
    {
        if (isJump && !isAir && !isRoll && !isDead)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isAir = true;

            // �ִϸ��̼�
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
        }
    }

    void Dodge()
    {
        if (isDodge && !isRollCool && !isAir && !isJump && !isSwap && !isDead)
        {
            // �÷��̾� Dir �������� �̵�
            dodgeDir = moveDir;

            speed *= 3f;
            isRoll = true;
            isRollCool = true;

            // �ִϸ��̼�
            anim.SetTrigger("doDodge");

            Invoke("FinishDodge", 0.5f);
        }
    }

    void FinishDodge()
    {
        speed = defaultSpeed;
        isRoll = false;
        StartCoroutine("RollCoolTime");
    }

    IEnumerator RollCoolTime()
    {
        float rollCoolTime = 1f;
        float coolTime = 0;
        while (coolTime <= rollCoolTime)
        {
            coolTime += Time.deltaTime;
            yield return null;
        }
        isRollCool = false;
    }

    void Interection()
    {
        if (isWeaponTrigger && nearObject != null && !isDead)
        {
            if (nearObject.CompareTag("Weapon"))
            {
                cItem item = nearObject.GetComponent<cItem>();
                int weaponIndex = item.value;
                hasWeapon[weaponIndex] = true;

                Destroy(nearObject);
            }
            else if (nearObject.CompareTag("Shop"))
            {
                cShop shop = nearObject.GetComponent<cShop>();
                shop.Enter(this);
                isShop = true;
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

        // ���⸦ �� �� �ִ��� ������ check
        if (!hasWeapon[weaponIndex])
            return;

        if ((_swapHammer || _swapHandGun || _swapSubMachineGun) && !isJump && !isRoll && !isSwap)
        {
            if (!curWeapon)
                preWeaponIndex = weaponIndex;
            else if (curWeapon)
            {
                // ���࿡ ���� ���� �ε������ Swap X
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

        if (isFire && isFireReady && !isRoll && !isSwap && !isShop)
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
            // ��� ������ ���� ������ ������, ī�޶��� �ٰŸ� ��鿡�� �����Ͽ� ȭ���� ��ġ(x,y) �ȼ� ��ǥ�� ����մϴ�
            // ȭ�� ������ �ȼ��� ���ǵ˴ϴ�. ȭ���� ���� �ϴ��� (0,0)�̰� ������ ����� ( pixelWidth -1, pixelHeight -1)�Դϴ�.
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


        if (isReload && !isJump && !isRoll && !isSwap && isFireReady)
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
