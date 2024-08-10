using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cShop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos;
    public Text talkText;
    public string[] talkData;

    Player enterPlayer;

    // Start is called before the first frame update
    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    // Update is called once per frame
    public void Exit()
    {
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000f;
    }

    public void Buy(int index)
    {
        int price = itemPrice[index];

        if (price > enterPlayer.coin)
        {
            StopCoroutine("Talk");
            StartCoroutine("Talk");
            return;
        }
        
        enterPlayer.coin -= price;

        Instantiate(itemObj[index], itemPos[index].position, itemPos[index].rotation);
    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
