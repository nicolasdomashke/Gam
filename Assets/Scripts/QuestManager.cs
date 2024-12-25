using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public class QuestInfo
{
    public string title;
    public int id;
    public string tale;
    public string minigame;
    public Dictionary<string,int> modifiers = new Dictionary<string,int>();
    public int scene;
    public float[] spawnPos = new float[3];
    public List<string> nextQuestsAdress = new List<string>();
    public QuestInfo(string jsonString)
    {
        JsonConvert.PopulateObject(jsonString, this);
    }
}
public struct InactiveQuestsStruct
{
    public static Dictionary<int, HashSet<string>> inactiveQuestsForOtherscene = new Dictionary<int, HashSet<string>>();
    public static QuestInfo currentQuest;
}
public class QuestManager : MonoBehaviour
{
    [SerializeField] private GameObject questPrefab; 
    [SerializeField] public GameObject questUI;
    [SerializeField] private Parameters parameters;

    [SerializeField] private string startQuestAdress = "./Assets/questlines/start.json";
    public Dictionary<int, GameObject> activeQuests = new Dictionary<int, GameObject>();
    public void CreateNewQuest(string addr)
    {
        string jsonString = File.ReadAllText(addr);
        QuestInfo questData = new QuestInfo(jsonString);
        if(questData.scene!= SceneManager.GetActiveScene().buildIndex)
        {
            if (!InactiveQuestsStruct.inactiveQuestsForOtherscene.ContainsKey(questData.scene))
            {
                InactiveQuestsStruct.inactiveQuestsForOtherscene.Add(questData.scene, new HashSet<string>());
            }
            InactiveQuestsStruct.inactiveQuestsForOtherscene[questData.scene].Add(addr);
            return;
        }

        if (!activeQuests.ContainsKey(questData.id))
        {
            GameObject newQuest = Instantiate(questPrefab, new Vector3(questData.spawnPos[0], questData.spawnPos[1], questData.spawnPos[2]), new Quaternion(0, 0, 0, 0));
            //newQuest.transform.position = questData.spawnPos;
            Debug.Log("Created quest: " + questData.title);
            QuestHandler newQuestHandler = newQuest.gameObject.GetComponentInChildren<QuestHandler>();

            newQuestHandler.questInfo = questData;

            newQuestHandler.PlayerParameters = parameters;
            newQuestHandler.manager = this;

            activeQuests.Add(questData.id, newQuest);
        }
    }

    void Start()
    {
        string jsonString = File.ReadAllText(startQuestAdress);
        QuestInfo startQuestData = new QuestInfo(jsonString);
        foreach (var addr in startQuestData.nextQuestsAdress)
        {
            CreateNewQuest(addr);
        }
        if (InactiveQuestsStruct.inactiveQuestsForOtherscene.ContainsKey(SceneManager.GetActiveScene().buildIndex))
        {
            foreach (var addr in InactiveQuestsStruct.inactiveQuestsForOtherscene[SceneManager.GetActiveScene().buildIndex])
            {
                CreateNewQuest(addr);
            }
        }
    }
}
