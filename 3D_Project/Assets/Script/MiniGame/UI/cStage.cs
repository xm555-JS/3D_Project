using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cStage : MonoBehaviour
{
    [SerializeField] Text stateTxt;
    [SerializeField] int stageNum;
    int preStageNum;
    public void NextStage() { stageNum++; }

    void Update()
    {
        StageTxtUpdate();
    }

    void StageTxtUpdate()
    {
        // 스테이지 Text 변경
        if (stageNum == preStageNum)
            return;

        stateTxt.text = "Stage " + stageNum;
        preStageNum = stageNum;
    }
}
