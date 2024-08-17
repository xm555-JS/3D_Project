using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    [Header("[Coin]")]
    [SerializeField] Text coinTxt;
    int coin = 1000;
    int preCoin;

    [Header("[Stage]")]
    [SerializeField] Text stateTxt;
    [SerializeField] int stageNum;
    int preStageNum;

    [Header("[TimmerView]")]
    [SerializeField] Image timerImg;
    [SerializeField] Text timerTxt;

    [Header("[TimmerController]")]
    [SerializeField] float timer;
    [SerializeField] bool isStageFinish;
    float saveTimer;
    Animator anim;

    [Header("[EnemySpawn]")]
    [SerializeField] cSpawnMonster enemySpawn;

    [Header("[AOE]")]
    [SerializeField] GameObject Aoe;

    WaitForSeconds BreakTime = new WaitForSeconds(3f);

    public void GetCoin(int getCoin) { coin += getCoin; }
    public void StartTimmer() { isStageFinish = true; }
    public void NextStage() { stageNum++; }

    void Awake()
    {
        InitializeTimer();
    }

    void Start()
    {
        StartEnemySpawn();
    }

    void LateUpdate()
    {
        CoinUpdate();
        StageTxtUpdate();
        if (!isStageFinish)
        {
            Timer();
            TimerControll();
            HandleAnimation();
        }
    }

    void CoinUpdate()
    {
        if (coin == preCoin)
            return;

        coinTxt.text = coin.ToString();
        preCoin = coin;
    }

    public void UseCoin(int useCoin)
    {
        if (useCoin > coin)
            return;

        coin -= useCoin;
    }

    void StageTxtUpdate()
    {
        // 스테이지 Text 변경
        if (stageNum == preStageNum)
            return;

        stateTxt.text = "Stage " + stageNum;
        preStageNum = stageNum;
    }

    void InitializeTimer()
    {
        anim = timerImg.GetComponent<Animator>();
        saveTimer = timer;
    }

    void Timer()
    {
        // 시간 Update
        if (timer <= 0f && !isStageFinish)
        {
            timer = 0f;
            isStageFinish = true;
            FinishedAnimation();
            StartCoroutine(StageStart());
            DontEnemySpawn();
        }

        if (!isStageFinish)
            timer -= Time.deltaTime;
    }

    IEnumerator StageStart()
    {
        yield return BreakTime;

        NextStage();
        timer = saveTimer;
        isStageFinish = false;
        StartEnemySpawn();
    }

    void TimerControll()
    {
        // 타이머 Text 변경
        const float oneHour = 3600f;
        int hour = (int)(timer / oneHour);
        int min = (int)((timer - (oneHour * hour)) / 60f);
        int sec = (int)(timer % 60f);

        timerTxt.text = $"{hour:00}/{min:00}/{sec:00}";
    }

    void HandleAnimation()
    {
        if (timer <= 10f && !isStageFinish)
            anim.SetBool("isShaking", true);
    }

    void FinishedAnimation()
    {
        anim.SetBool("isShaking", false);
    }

    void StartEnemySpawn()
    {
        enemySpawn.SpawnEnemy(stageNum);
    }

    void DontEnemySpawn()
    {
        enemySpawn.DontSpawnEnemy();
    }

    public void MiniGameOver()
    {
        Aoe.SetActive(true);
        StartCoroutine(AoeSetactiveFalse());
        stageNum = 0;
        timer = 0f;
        isStageFinish = true;
        FinishedAnimation();
        StartCoroutine(StageStart());
        DontEnemySpawn();
    }

    IEnumerator AoeSetactiveFalse()
    {
        yield return new WaitForSeconds(2f);
        Aoe.SetActive(false);
    }
}
