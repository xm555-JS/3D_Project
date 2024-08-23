using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cSpawn : MonoBehaviour
{
    [SerializeField] GameObject[] players;
    [SerializeField] float[] playerProbs;

    List<GameObject> playerNormal = new List<GameObject>();
    List<GameObject> playerRare = new List<GameObject>();
    List<GameObject> playerLegend = new List<GameObject>();

    WaitForSeconds drawCoolTime = new WaitForSeconds(0.5f);

    public void DrawPlayer()
    {
        InstantePlayer();
    }

    public void DrawPlayerFive()
    {
        StartCoroutine(Draw());
    }

    IEnumerator Draw()
    {
        int count = 0;

        while (count < 5)
        {
            InstantePlayer();
            yield return drawCoolTime;
            count++;
        }
    }

    public void RemoveAll()
    {
        for (int i = 0; i < playerNormal.Count; i++)
        {
            Destroy(playerNormal[i]);
        }

        for (int i = 0; i < playerRare.Count; i++)
        {
            Destroy(playerRare[i]);
        }

        for (int i = 0; i < playerLegend.Count; i++)
        {
            Destroy(playerLegend[i]);
        }   

        playerNormal.Clear();
        playerRare.Clear();
        playerLegend.Clear();
    }

    public void RemoveNormal()
    {
        int lastIndex = playerNormal.Count - 1;
        if (playerNormal.Count == 0)
            return;
        Destroy(playerNormal[lastIndex]);
        playerNormal.RemoveAt(lastIndex);
    }

    public void RemoveRare()
    {
        int lastIndex = playerRare.Count - 1;
        if (playerRare.Count == 0)
            return;
        Destroy(playerRare[lastIndex]);
        playerRare.RemoveAt(lastIndex);
    }

    public void RemoveLegend()
    {
        int lastIndex = playerLegend.Count - 1;
        if (playerLegend.Count == 0)
            return;
        Destroy(playerLegend[lastIndex]);
        playerLegend.RemoveAt(lastIndex);
    }

    void InstantePlayer()
    {
        int playerIndex = Choose(playerProbs);
        GameObject playerInstante = Instantiate(players[playerIndex], transform.position +
                                        new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)), transform.rotation);
        PlayerClassification(playerIndex, playerInstante);
    }


    void PlayerClassification(int playerIndex, GameObject instante)
    {
        if (playerIndex == 0)
            playerNormal.Add(instante);
        else if (playerIndex == 1)
            playerRare.Add(instante);
        else if (playerIndex == 2)
            playerLegend.Add(instante);
    }

    int Choose(float[] probs)
    {
        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;
        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
                return i;
            else
                randomPoint -= probs[i];
        }

        return probs.Length - 1;
    }
}
