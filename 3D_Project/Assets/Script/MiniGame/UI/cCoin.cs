using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cCoin : MonoBehaviour
{
    [SerializeField] Text coinTxt;
    int coin = 1000;
    int preCoin;

    void Update()
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

    public void GetCoin(int getCoin) { coin -= getCoin; }
}
