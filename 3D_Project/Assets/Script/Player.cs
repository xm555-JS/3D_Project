using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // component
    Rigidbody rigid;
    Animator anim;

    // move
    float hAxis;
    float vAxis;

    Vector3 moveDir;
    Vector3 dodgeDir;

    bool isDown;
    bool isJump;
    bool isAir;

    bool isDodge;
    bool isRoll;

    public float speed;
    public float jumpPower;

    float defaultSpeed;


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
        Dodge();
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

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        isDown = Input.GetButton("Walk");
        isJump = Input.GetButtonDown("Jump");
        isDodge = Input.GetButtonDown("Dodge");
    }

    void GetDir()
    {
        moveDir = new Vector3(hAxis, 0f, vAxis).normalized;
        //transform.position += moveVec * speed * Time.deltaTime;

        // 회피할 때 dir 고정
        if (isRoll)
            moveDir = dodgeDir;

        // 애니메이션
        anim.SetBool("isRun", moveDir != Vector3.zero);
        anim.SetBool("isWalk", isDown);
    }

    void Turn()
    {
        // 회전
        transform.LookAt(transform.position + moveDir);
    }

    void Move()
    {
        // rigidybody를 이용한 플레이어 이동
        // Rigidbody를 이용하여 플레이어를 움직이게 바꿈
        // FixedUpdate에서 처리함으로 같은 프레임률에서 충돌을 처리할 수 있어 안전하게 물리 상호작용이 가능
        Vector3 InputVec = moveDir * speed * (isDown ? 0.3f : 1f) * Time.deltaTime;
        rigid.MovePosition(rigid.position + InputVec);
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
}
