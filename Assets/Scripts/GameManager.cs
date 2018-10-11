using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public  class GameManager : MonoBehaviour {

    public static int lineNum = 0;
    public float  currentTime = 0;
    public Text currentTimeUI ;
    public static float[] bestTimes;
    public static float[,] historyTimes;
    public Text historyTimeUI;
    public Text bestTimeUI;
    private static int currentStage=1;
    private static int MaxStage;
    public Text stageNum;
    public Text completeGame;
    public static GameManager GM;

    public static void RestartCurrentStage()
    {
        if (lineNum== 0) return;   //赢时不执行
        Point.points.Clear();
        SceneManager.LoadScene(currentStage-1);
    }
    public  void RestartGame()
    {
        currentStage = 1;
        Point.points.Clear();
        SceneManager.LoadScene(currentStage - 1);
    }



    //防止由于DontDestroyOnLoad重复生成EffectManager
    public static bool isClone;
    public GameObject effectManager;
    private GameObject cloneEffectManager;
    void Awake()
    {
        lineNum = 0;
        if (!isClone)
        {
            cloneEffectManager = Instantiate(effectManager, effectManager.transform.position, effectManager.transform.rotation) as GameObject;
            isClone = true;
        }
        DontDestroyOnLoad(cloneEffectManager);
    }


    private void Start()
    {
        //初始化

        
        GM = this;
        currentTime = 0;
        MaxStage = SceneManager.sceneCountInBuildSettings;
        currentStage = int.Parse(SceneManager.GetActiveScene().name);
        //根据关卡数更新大小
        bestTimes = new float[MaxStage];

        historyTimes = new float[MaxStage, 5];

        //读取显示（最佳时间,历史时间）
        SavaAndLoad.saveload.LoadGame();
        UpdateUI();
        UpdateHistoryTimeUI();


    }
    private void Update()
    {
        UpdateTime();
    }
    private void UpdateTime() {
        //计时，UI更新，若完成则不再计时
        if (lineNum > 0)
        {
            currentTime += Time.deltaTime;
            currentTimeUI.text = "时间：" + ToTime(currentTime);
        }
    }
//更新关卡以及最佳时间的UI
    private void UpdateUI()
    {
        stageNum.text = "关卡：" + currentStage.ToString();
        if (bestTimes[currentStage - 1] > 0) bestTimeUI.text = "最佳："+ToTime(bestTimes[currentStage - 1]);
    }

    //完成一条线，当为0时代表通关
    public  void LineComplete()
    {
    lineNum--;
    if (lineNum == 0) YouWin();
     }

//初始化points，进入下一关,用协程两秒后进入下一关
   private  void YouWin() {

        Point.points.Clear();

        //通关后的动画
        completeGame.gameObject.SetActive(true);
        completeGame.GetComponent<Animator>().Play("Complete");

        //更新历史事件
        UpdateHistoryTime();

        //更新最佳时间
        if (bestTimes[currentStage - 1] > currentTime || bestTimes[currentStage - 1] == 0)
        {
            bestTimes[currentStage - 1] = currentTime;
            UpdateUI();
            completeGame.text = "新纪录：" + ToTime(currentTime);
        }
        else completeGame.text = "用时：" + ToTime(currentTime);
        if (currentStage < MaxStage)
        {
            StartCoroutine(GoToNextStage());
        }
        //全部关卡完成，重新开始按钮激活
        else completeGame.transform.GetChild(0).gameObject.SetActive(true);
    }

    IEnumerator GoToNextStage() {
        currentStage += 1;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(currentStage - 1);
    }

    //将float格式的时间转为标准时间格式的字符串
    private string ToTime(float time) {
        int hour = (int)time / 3600;
        int minute = (int)(time - hour * 3600) / 60;
        int second = (int)(time - hour * 3600 - minute * 60);
        int milliSecond = (int)((time - (int)time)*1000);
        //小时不常用到，但以防万一留着
        if(hour==0)return string.Format("{0:D2}:{1:D2}:{2:D3}", minute, second, milliSecond);
        return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}",hour,minute,second,milliSecond);

    }

    private void OnDestroy()
    {
        //储存
        SavaAndLoad.saveload.SaveGame();
    }
    public int GetMaxStage() {
        return MaxStage;
    }
    public void UpdateHistoryTime() {

        for (int j = 4; j > 0; j--)
        {
            historyTimes[currentStage - 1, j] = historyTimes[currentStage - 1, j - 1];
        }
        historyTimes[currentStage - 1, 0] = currentTime;
        UpdateHistoryTimeUI();
    }
    public void UpdateHistoryTimeUI()
    {
        historyTimeUI.text = "关卡" + currentStage + "历史用时：";
        for (int j = 0; j < 5; j++)
        {
            historyTimeUI.text += "\n" +ToTime(historyTimes[currentStage - 1, j]);
        }
        historyTimeUI.text.Replace("\\n", "\n");
    }
    public void CleanRecord (){
        Array.Clear(bestTimes, 0, bestTimes.GetLength(0));
        Array.Clear(historyTimes, 0, historyTimes.GetLength(0) * historyTimes.GetLength(1));
        SavaAndLoad.saveload.SaveGame();
        Point.points.Clear();
        SceneManager.LoadScene(currentStage - 1);
    }
}
