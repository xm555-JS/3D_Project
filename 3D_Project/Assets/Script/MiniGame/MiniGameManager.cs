using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    [Header("[StartButtonGroup]")]
    [SerializeField] GameObject startButtonGroup;

    [Header("[Player]")]
    [SerializeField] cSpawn playerSpawn;

    [Header("[Coin]")]
    [SerializeField] Text coinTxt;
    int coin = 1000;
    int preCoin;

    [Header("[Stage]")]
    [SerializeField] Text stateTxt;
    [SerializeField] int stageNum;
    int preStageNum;
    bool isEnd;

    [Header("[TimmerView]")]
    [SerializeField] Image timerImg;
    [SerializeField] Text timerTxt;

    [Header("[TimmerController]")]
    [SerializeField] float timer;
    [SerializeField] float saveTimer = 20f;
    [SerializeField] bool isStageFinish;
    Animator anim;

    [Header("[EnemySpawn]")]
    [SerializeField] cSpawnMonster enemySpawn;

    [Header("Boss")]
    [SerializeField] RectTransform bossHealthGroup;
    [SerializeField] RectTransform bossHealthBar;
    Image healthBarImg;
    Animator healthBarAnim;
    float rate = 1f;
    bool isTwinkling;

    [Header("[AOE]")]
    [SerializeField] GameObject Aoe;

    [Header("[Reward]")]
    [SerializeField] GameObject reward;

    WaitForSeconds BreakTime = new WaitForSeconds(3f);

    public void GetCoin(int getCoin) { coin += getCoin; }
    public void StartTimmer() { isStageFinish = true; }
    public void NextStage() { stageNum++; }

    void Awake()
    {
        InitializeTimer();
        healthBarImg = bossHealthBar.GetComponent<Image>();
        healthBarAnim = bossHealthBar.GetComponent<Animator>();
    }

    void OnEnable()
    {
        enemySpawn.OnGetCoin += HandleGetCoin;
        Initialize();
    }

    void OnDisable()
    {
        enemySpawn.OnGetCoin -= HandleGetCoin;
    }

    void Initialize()
    {
        startButtonGroup.SetActive(true);
        Aoe.SetActive(false);
        coin = 1000;
        timer = saveTimer;
        isStageFinish = false;
        enemySpawn.ResetSpawn();
    }

    public void MiniGameStart()
    {
        StartEnemySpawn();
    }

    void LateUpdate()
    {
        CoinUpdate();
        StageTxtUpdate();

        MiniGameOver();
        BossHpBar();

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
        timer = saveTimer;
    }

    void Timer()
    {
        // 시간 Update
        if (timer <= 0f && !isStageFinish && stageNum < 5 && !isEnd)
        {
            timer = 0f;
            isStageFinish = true;
            FinishedAnimation();
            StartCoroutine(StageStart());
            DontEnemySpawn();
        }

        bool isButtonActive = startButtonGroup.activeSelf;
        if (!isStageFinish && !isEnd && !isButtonActive)
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

    public void MiniGameExit()
    {
        Aoe.SetActive(true);
        StartCoroutine(AoeSetactiveFalse());

        playerSpawn.RemoveAll();

        stageNum = 1;
        timer = 0f;
        isStageFinish = true;
        if (isEnd)
            isEnd = false;

        FinishedAnimation();
        DontEnemySpawn();
    }

    void MiniGameOver()
    {
        bool isGameOver = enemySpawn.CheckGameOver();
        if (isGameOver && !isStageFinish && !isEnd)
            Restart();
        if (enemySpawn.GetBossObject() != null)
        {
            if (enemySpawn.GetBossCurHealth() <= 0 && !isStageFinish)
            {
                isEnd = true;
                reward.SetActive(true);
            }
            if (enemySpawn.GetBossCurHealth() >= 0 && isStageFinish && stageNum >= 4)
                Restart();
        }
    }

    public void Restart()
    {
        Aoe.SetActive(true);
        StartCoroutine(AoeSetactiveFalse());

        FinishedAnimation();
        DontEnemySpawn();

        playerSpawn.RemoveAll();

        stageNum = 0;
        timer = 0f;
        if (isEnd)
            isEnd = false;
    }

    IEnumerator AoeSetactiveFalse()
    {
        yield return new WaitForSeconds(1f);
        Aoe.SetActive(false);
        coin = 1000;
        enemySpawn.ResetSpawn();
    }

    void HandleGetCoin(cSpawnMonster spawn)
    {
        coin += 100;
    }

    void BossHpBar()
    {
        GameObject boss = enemySpawn.GetBossObject();
        if (boss)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30f;

            float bossHealthRate = (float)enemySpawn.GetBossCurHealth() / enemySpawn.GetBossMaxHealth();
            if (rate > bossHealthRate)
            {
                rate -= Time.deltaTime;
                rate = Mathf.Max(rate, bossHealthRate);

                if (!isTwinkling)
                {
                    healthBarAnim.SetBool("isTwinkling", true);
                    isTwinkling = true;
                }
            }
            else
            {
                healthBarAnim.SetBool("isTwinkling", false);
                isTwinkling = false;

                healthBarImg.color = new Color(1f, 0f, 0f, 1f);
            }


            float bossRate = Mathf.Lerp(0f, 1f, rate);
            bossHealthBar.localScale = new Vector3(bossRate, 1f, 1f);
        }
        else
            bossHealthGroup.anchoredPosition = Vector3.up * 200f;
    }

    public void CloseUI(GameObject ui)
    {
        ui.SetActive(false);
    }
}

