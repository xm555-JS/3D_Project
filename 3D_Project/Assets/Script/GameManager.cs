using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("[Camera]")]
    public GameObject menuCam;
    public GameObject gameCam;

    [Header("[Player]")]
    public Player player;

    [Header("[Boss]")]
    public cBoss boss;

    [Header("[Game_Info]")]
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCountA;
    public int enemyCountB;
    public int enemyCountC;

    [Header("[UI_Group]")]
    public GameObject menuPanel;
    public GameObject gamePanel;

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

    void Awake()
    {
        //int maxScore = PlayerPrefs.GetInt("MaxScore");
        //string maxScoreStr = maxScore.ToString();
        //maxScoreTxt.text = maxScoreStr;
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBattle)
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
            weapon4Img.color = new Color(1f, 1f, 1f, 0f);

        if (player.hasGrenades > 0)
            weapon4Img.color = new Color(1f, 1f, 1f, 1f);
        else
            weapon4Img.color = new Color(1f, 1f, 1f, 0f);

        enemyTxtA.text = enemyCountA.ToString();
        enemyTxtB.text = enemyCountB.ToString();
        enemyTxtC.text = enemyCountC.ToString();

        bossHealthBar.localScale = new Vector3((float)boss.curHealthBoss / boss.maxHealthBoss, 1f, 1f);
    }
}
