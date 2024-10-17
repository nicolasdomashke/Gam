using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.Search;
using UnityEngine;

public class QuestHandler : MonoBehaviour
{
    public Parameters PlayerParameters;

    public QuestManager manager;
    public QuestInfo questInfo;

    //private const int maxTitleLength = 45; //if 1280*720 , 36 pt
    //private const int maxTaleLength = 500; //if 1280*720 , 36 pt

    public void ShowTale()
    {
        Time.timeScale = 0f;
        manager.questUI.SetActive(true);
        foreach (TMP_Text text in manager.questUI.GetComponentsInChildren<TMP_Text>())
        {
            if(text.gameObject.name == "title")
            {
                text.text = questInfo.title;
            }
            else
            {
                text.text = questInfo.tale;
            }
        }

    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        bool isEPressed = Input.GetKey(KeyCode.E);
        if (isEPressed && collision.tag == "Player")
        {
            ShowTale();
            foreach (var modName in questInfo.modifiers.Keys)
            {
                switch (modName)
                {
                    case "thirst":
                        PlayerParameters.thirst += questInfo.modifiers[modName];
                        break;
                    case "hunger":
                        PlayerParameters.hunger += questInfo.modifiers[modName];
                        break;
                    case "happiness":
                        PlayerParameters.happiness += questInfo.modifiers[modName];
                        break;
                    case "time":
                        PlayerParameters.time += questInfo.modifiers[modName];
                        break;
                    case "lastSleep":
                        PlayerParameters.lastSleep = (int)(PlayerParameters.time % Parameters.ms_in_day) + 1 - questInfo.modifiers[modName];
                        break;
                    default:
                        break;
                }
            }
            manager.activeQuests.Remove(questInfo.id);
            foreach (var addr in questInfo.nextQuestsAdress)
            {
                manager.CreateNewQuest(addr);
            }
            Destroy(Array.Find(this.gameObject.GetComponentsInParent<Component>(), go => go.gameObject.name.Contains("QuestObject")).gameObject);
        }
    }
}
