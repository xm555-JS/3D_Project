using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cTimer : MonoBehaviour
{
    [Header("[View]")]
    [SerializeField] Image timerImg;
    [SerializeField] Text gameTimer;

    [Header("[Controller]")]
    [SerializeField] float timer;
    [SerializeField] bool isFinish;

    Animator anim;

    public void StartTimmer() { isFinish = true; }

    void Awake()
    {
        InitializeTimer();
    }

    void InitializeTimer()
    {
        anim = timerImg.GetComponent<Animator>();
    }

    void Update()
    {
        if (!isFinish)
        {
            Timer();
            TimerControll();
            HandleAnimation();
        }
    }

    void Timer()
    {
        // 시간 Update
        if (timer <= 0f)
        {
            timer = 0f;
            isFinish = true;
            FinishedAnimation();
        }
        
        if (!isFinish)
            timer -= Time.deltaTime;
    }

    void TimerControll()
    {
        // 타이머 Text 변경
        const float oneHour = 3600f;
        int hour = (int)(timer / oneHour);
        int min = (int)((timer - (oneHour * hour)) / 60f);
        int sec = (int)(timer % 60f);

        gameTimer.text = $"{hour:00}/{min:00}/{sec:00}";
    }

    void HandleAnimation()
    {
        if (timer <= 10f && !isFinish)
            anim.SetBool("isShaking", true);
    }

    void FinishedAnimation()
    {
        anim.SetBool("isShaking", false);
    }
}
