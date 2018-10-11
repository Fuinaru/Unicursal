using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SavaAndLoad : MonoBehaviour {


    public static SavaAndLoad saveload;
	// Use this for initialization
	void Awake () {
        saveload = this;

    }
	
	// Update is called once per frame
	void Update () {

    }

    public void SaveGame()
    {
        // 创建了一个 SaveData 对象，同时当前游戏的所有数据都会保存到这个对象中。
        SaveData saveData = CreateSaveGameObject();

        // 创建了一个 BinaryFormatter，然后创建一个 FileStream，在创建时指定文件路径和要保存的 Save 对象。它会序列化数据（转换成字节），然后写磁盘，关闭 FileStream。现在在电脑上会多一个名为 gamesave.save 的文件。.save 后缀只是一个示例，你可以使用任意扩展名
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, saveData);
        file.Close();



    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            //文件读取，读取savadata
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            SaveData saveData = (SaveData)bf.Deserialize(file);
            file.Close();

            //savadata中的值赋予至游戏中
            for (int i = 0; i < saveData.bestTimes.GetLength(0); i++)
            {
                try
                { GameManager.bestTimes[i] = saveData.bestTimes[i]; }
                catch { Debug.Log("bestTimes存储越界"); }
            }

            for (int i = 0; i < saveData.historyTimes.GetLength(0); i++)
            {
                for (int j = 0; j < saveData.historyTimes.GetLength(1); j++)
                    try { GameManager.historyTimes[i, j] = saveData.historyTimes[i, j]; }
                    catch { Debug.Log("historyTimes存储越界"); }
            }

        }
        else
        {
            Debug.Log("No game saved!");
        }
    }



    private SaveData CreateSaveGameObject()
    {
        //储存

        SaveData saveData = new SaveData();
        saveData.bestTimes =new float[GameManager.bestTimes.Length];
        for (int i = 0; i < GameManager.bestTimes.Length; i++)
        {
            saveData.bestTimes[i] = GameManager.bestTimes[i];
        }
            saveData.historyTimes = new float[GameManager.historyTimes.GetLength(0), GameManager.historyTimes.GetLength(1)];
        for (int i = 0; i < GameManager.historyTimes.GetLength(0); i++) {
            for (int j = 0; j < GameManager.historyTimes.GetLength(1); j++)
                saveData.historyTimes[i, j] = GameManager.historyTimes[i, j];
        }

        return saveData;
    }

}






