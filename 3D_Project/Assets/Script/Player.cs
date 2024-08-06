using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header ("[Component]")]
    Rigidbody rigid;
    Animator anim;

    [Header("[PlayerInfo]")]
    public GameObject[] grenades;
    public int ammo;
    public int coin;
    public int health;
    public int hasGrenades;

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
    Vector3 dodgeDir;
    bool isDown;
    bool isJump;
    bool isAir;
    bool isDodge;
    bool isRoll;

    [Header("[Weapon]")]
    public GameObject[] weapons;
    public bool[] hasWeapon;
    cWeapon curWeapon;
    GameObject nearObject;
    bool isWeaponTrigger;
    bool isSwap;
    bool isFire;
    bool isFireReady = true;
    float fireDelay;
    int weaponIndex;
    int preWeaponIndex;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
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
        Dodge();
        Interection();
        Swap();
    }

    void FixedUpdate()
    {
        Move();
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

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        isDown = Input.GetButton("Walk");
        isJump = Input.GetButtonDown("Jump");
        isFire = Input.GetButtonDown("Fire1");
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
        if (isSwap || !isFireReady)
            moveDir = Vector3.zero;

        // �ִϸ��̼�
        anim.SetBool("isRun", moveDir != Vector3.zero);
        anim.SetBool("isWalk", isDown);
    }

    void Turn()
    {
        // ȸ��
        transform.LookAt(transform.position + moveDir);
    }

    void Move()
    {
        // rigidybody�� �̿��� �÷��̾� �̵�
        // Rigidbody�� �̿��Ͽ� �÷��̾ �����̰� �ٲ�
        // FixedUpdate���� ó�������� ���� �����ӷ����� �浹�� ó���� �� �־� �����ϰ� ���� ��ȣ�ۿ��� ����
        Vector3 InputVec = moveDir * speed * (isDown ? 0.3f : 1f) * Time.deltaTime;
        rigid.MovePosition(rigid.position + InputVec);
    }

    void Jump()
    {
        if (isJump && !isAir && !isDodge)
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
        if (isDodge && !isAir && !isJump)
        {
            // �÷��̾� Dir �������� �̵�
            dodgeDir = moveDir;

            speed *= 3f;
            isRoll = true;

            // �ִϸ��̼�
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

        // ���⸦ �� �� �ִ��� ������ check
        if (!hasWeapon[weaponIndex])
            return;

        if ((_swapHammer || _swapHandGun || _swapSubMachineGun) && !isJump && !isDodge && !isSwap)
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

        if (isFire && isFireReady && !isDodge && !isSwap)
        {
            curWeapon.Use();
            anim.SetTrigger("doSwing");
            fireDelay = 0f;
            isFireReady = false;
        }
    }
}
