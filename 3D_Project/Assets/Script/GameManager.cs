using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("[Camera]")]
    public GameObject menuCam;
    public GameObject gameCam;

    [Header("[Shop]")]
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;
    public GameObject miniGameZone;

    [Header("[Player]")]
    public Player player;

    [Header("[Boss]")]
    public cBoss boss;

    [Header("[Enemy_Zone]")]
    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;

    [Header("[Game_Info]")]
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCountA;
    public int enemyCountB;
    public int enemyCountC;
    public int enemyCountD;

    [Header("[MiniGame_Info]")]
    public bool isMiniGame;

    [Header("[UI_Group]")]
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;

    [Header("[Game_UI]")]
    public Text maxScoreTxt;
    public Text scoreTxt;
    public Text stageTxt;
    public Text playerTimeTxt;
    public Text playerHealthTxt;
    public Text playerAmmoTxt;
    public Text playerCoinTxt;
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weapon4Img;
    public Text enemyTxtA;
    public Text enemyTxtB;
    public Text enemyTxtC;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    public Text curScoreText;
    public Text bestText;

    //bossHealthBar
    Image healthBarImg;
    float rate = 1f;
    bool isTwinkling;
    Animator healthBarAnim;

    // PlayerHealthImg
    public Image playerHealthImg;
    Animator playerHealthAnim;
    bool isShaking;

    void Awake()
    {
        //int maxScore = PlayerPrefs.GetInt("MaxScore");
        //string maxScoreStr = maxScore.ToString();
        //maxScoreTxt.text = maxScoreStr;
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        enemyList = new List<int>();

        if (!PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);

        healthBarImg = bossHealthBar.GetComponent<Image>();
        healthBarAnim = bossHealthBar.GetComponent<Animator>();

        playerHealthAnim = playerHealthImg.gameObject.GetComponent<Animator>();
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        curScoreText.text = scoreTxt.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");

        if (player.score > maxScore)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void StageStart()
    {
        isBattle = true;

        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);
        miniGameZone.SetActive(false);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        StartCoroutine("InBattle");
    }

    public void StageEnd()
    {
        player.transform.position = Vector3.up * 1.4f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);
        miniGameZone.SetActive(true);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false;
        stage++;
    }

    public void MiniGameStart()
    {
        isMiniGame = true;

        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);
        miniGameZone.SetActive(false);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);
    }

    public void MiniGameEnd()
    {
        player.transform.position = Vector3.up * 1.4f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);
        miniGameZone.SetActive(true);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false;
    }

    IEnumerator InBattle()
    {
        if (stage % 5 == 0)
        {
            enemyCountD++;
            GameObject instantBoss = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            boss = instantBoss.GetComponent<cBoss>();
            boss.manager = this;
        }

        for (int i = 0; i < stage; i++)
        {
            int rand = Random.Range(0, 3);
            enemyList.Add(rand);

            switch (rand)
            {
                case 0:
                    enemyCountA++;
                    Debug.Log("A 증가" + enemyCountA);
                    break;
                case 1:
                    enemyCountB++;
                    Debug.Log("B 증가" + enemyCountB);
                    break;
                case 2:
                    enemyCountC++;
                    Debug.Log("C 증가" + enemyCountC);
                    break;
            }
        }

        while (enemyList.Count > 0)
        {
            int randZone = Random.Range(0, 4);
            GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[randZone].position, enemyZones[randZone].rotation);
            cEnemy enemy = instantEnemy.GetComponent<cEnemy>();
            enemy.manager = this;
            enemyList.RemoveAt(0);
            yield return new WaitForSeconds(4f);
        }

        while (enemyCountA + enemyCountB + enemyCountC + enemyCountD > 0)
        {
            yield return null;
        }

        if (enemyCountA + enemyCountB + enemyCountC + enemyCountD <= 0)
            Debug.Log("3초뒤 클리어");

        yield return new WaitForSeconds(3f);

        boss = null;
        StageEnd();
    }

    // Update is called once per frame
    void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;
    }

    void LateUpdate()
    {
        scoreTxt.text = string.Format("{0:n0}", player.score);
        stageTxt.text = "STAGE" + stage;

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - (hour * 3600)) / 60);
        int sec = (int)(playTime % 60);
        playerTimeTxt.text = string.Format("{0:00}", hour) + "/" + string.Format("{0:00}", min) + "/" + string.Format("{0:00}", sec);

        playerHealthTxt.text = player.health + "/" + player.maxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);

        if (!player.curWeapon)
            playerAmmoTxt.text = "-/" + player.ammo;
        else if (player.curWeapon.type == cWeapon.Type.MELEE)
            playerAmmoTxt.text = "-/" + player.ammo;
        else
            playerAmmoTxt.text = player.curWeapon.curAmmo + "/" + player.ammo;

        if (player.hasWeapon[0])
            weapon1Img.color = new Color(1f, 1f, 1f, 1f);
        else
            weapon1Img.color = new Color(1f, 1f, 1f, 0f);

        if (player.hasWeapon[1])
            weapon2Img.color = new Color(1f, 1f, 1f, 1f);
        else
            weapon2Img.color = new Color(1f, 1f, 1f, 0f);

        if (player.hasWeapon[2])
            weapon3Img.color = new Color(1f, 1f, 1f, 1f);
        else
            weapon3Img.color = new Color(1f, 1f, 1f, 0f);

        if (player.hasGrenades > 0)
            weapon4Img.color = new Color(1f, 1f, 1f, 1f);
        else
            weapon4Img.color = new Color(1f, 1f, 1f, 0f);

        enemyTxtA.text = enemyCountA.ToString();
        enemyTxtB.text = enemyCountB.ToString();
        enemyTxtC.text = enemyCountC.ToString();

        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30f;

            float bossHealthRate = (float)boss.curHealth / boss.maxHealth;
            if (rate > bossHealthRate)
            {
                rate -= Time.deltaTime;
                rate = Mathf.Max(rate, bossHealthRate);

                if (!isTwinkling)
                {
                    //StartCoroutine("Twinking");
                    healthBarAnim.SetBool("isTwinkling", true);
                    isTwinkling = true;
                }
            }
            else
            {
                //StopCoroutine("Twinking");
                healthBarAnim.SetBool("isTwinkling", false);
                isTwinkling = false;

                healthBarImg.color = new Color(1f, 0f, 0f, 1f);
            }


            float bossRate = Mathf.Lerp(0f, 1f, rate);
            bossHealthBar.localScale = new Vector3(bossRate, 1f, 1f);
        }
        else
            bossHealthGroup.anchoredPosition = Vector3.up * 200f;

        // PlayerHealthImg
        if (player.health <= (player.maxHealth / 2) && !isShaking)
        {
            Debug.Log("isShaking");
            playerHealthAnim.SetBool("isShaking", true);
            isShaking = true;
        }
        else if (player.health > (player.maxHealth / 2) && isShaking)
        {
            Debug.Log("Not isShaking");
            playerHealthAnim.SetBool("isShaking", false);
            isShaking = false;
        }
            
    }

    //IEnumerator Twinking()
    //{
    //    isTwinkling = true;

    //    while (true)
    //    {
    //        healthBarImg.color = new Color(0.5f, 0f, 0f, 1f);
    //        yield return new WaitForSeconds(0.3f);

    //        healthBarImg.color = new Color(1f, 0f, 0f, 1f);
    //        yield return new WaitForSeconds(0.3f);
    //    }
    //}
}
