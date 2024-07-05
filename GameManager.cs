// using System.ComponentModel;
// using System.Diagnostics;
// using System.Timers;
// using System;
// using System.Threading;
// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine.UI;


// public class GameManager : MonoBehaviour
// {
//     public static bool canMove=false;
//     private ScreenFader screenFader;
//     private PlayerController playerController;
//     private trafficController trafficControllerInstance;
//     public float tryStartTime;
//     public float elapsedTime;
//     public int nbTry = 1;  
//     public int maxNbTry = 6;
//     private float gameStartTime;
//     public float generalTime;
//     private GameCondition selectedCondition;
//     public List<GameCondition> availableConditions;
//     private bool isInitialized = false;
//     public string currentCondition;
//     private MarkerEvent markerEvent;
//     public Text ConditionText;

//     public void Awake()
//     {
//         if (!isInitialized)
//         {
//             isInitialized = true;
//             tryStartTime = Time.time;
//             screenFader = FindObjectOfType<ScreenFader>();
//             playerController = FindObjectOfType<PlayerController>();
//             markerEvent = FindObjectOfType<MarkerEvent>();
//             if (markerEvent == null)
//             {
//                 UnityEngine.Debug.LogError("MarkerEvent component not found!");
//                 return;
//             }
//             canMove = false;
//             gameStartTime = Time.time;
//             availableConditions = Enum.GetValues(typeof(GameCondition)).OfType<GameCondition>().ToList();
//             trafficControllerInstance = FindObjectOfType<trafficController>();
//             // currentCondition = selectedCondition.ToString();
//             SelectRandomCondition();
//             ExecuteConditionLogic(); // 记录条件
//         }
//     }

//     // private vs
//     private void ExecuteConditionLogic()
//     {
//         switch (selectedCondition)
//         {
//             case GameCondition.Baseline:
//                 ActivateScriptByName("Baseline");
//                 markerEvent.RecordCondBaseline();
//                 break;
//             case GameCondition.Inter_const_3s:
//                 ActivateScriptByName("Inter_const_3s");
//                 markerEvent.RecordCondInter_const_3s();
//                 break;
//             case GameCondition.Inter_const_6s:
//                 ActivateScriptByName("Inter_const_6s");
//                 markerEvent.RecordCondInter_const_6s();
//                 break;
//             case GameCondition.Inter_varMinus_3s:
//                 ActivateScriptByName("Inter_varMinus_3s");
//                 markerEvent.RecordCondInter_varMinus_3s();
//                 break;
//             case GameCondition.Inter_varMinus_6s:
//                 ActivateScriptByName("Inter_varMinus_6s");
//                 markerEvent.RecordCondInter_varMinus_6s();
//                 break;
//             case GameCondition.Inter_varPlus_3s:
//                 ActivateScriptByName("Inter_varPlus_3s");
//                 markerEvent.RecordCondInter_varPlus_3s();
//                 break;
//             case GameCondition.Inter_varPlus_6s:
//                 ActivateScriptByName("Inter_varPlus_6s");
//                 markerEvent.RecordCondInter_varPlus_6s();
//                 break;
//             default:
//                 UnityEngine.Debug.LogError("Invalid game condition selected");
//                 break;
//         }
//         currentCondition = selectedCondition.ToString(); 
//     }
//     private void ActivateScriptByName(string scriptName)
//     {
//         MonoBehaviour scriptComponent = GetComponent(scriptName) as MonoBehaviour;

//         if (scriptComponent != null)
//         {
//             scriptComponent.enabled = true;
//         }
//         else
//         {
//             UnityEngine.Debug.LogError("Component not found: " + scriptName);
//         }
//     }
//     public bool RestartTry()
//     {
//         if (nbTry < maxNbTry)
//         {
//             trafficControllerInstance.CarInCrossing = false;
//             markerEvent.RecordRestartTry();
//             screenFader.ShowScreenFader();
//             playerController.ResetPed();
//             trafficControllerInstance.ClearScene();
//             trafficControllerInstance.InitializeScene();
//             tryStartTime = Time.time;
//             nbTry++;
//             return true;
//         }else{
//             NextCondition();
//         }
//         return false;
//     }
//     public void NextCondition()
//     {
//         DeactivateCondScripts();
//         markerEvent.RecordNextCondition();
//         markerEvent.RecordRestartTry();
//         if (availableConditions.Count > 0 )
//         {
//             SelectRandomCondition();
//             ExecuteConditionLogic();
//             playerController.ResetPed();
//             trafficControllerInstance.ClearScene();
//             trafficControllerInstance.InitializeScene();
//             tryStartTime = Time.time;
//             nbTry=1;
//             screenFader.ShowScreenFader();
//         }
//         else
//         {
//             SessionOver();
//             UnityEngine.Debug.LogWarning("No available conditions left.");
//         }
//         // currentCondition = selectedCondition.ToString(); 
//         // ConditionText.text = "Condition : "+  currentCondition; 
//     }
//     private void DeactivateCondScripts()
//     {
//         MonoBehaviour[] allComponents = GetComponents<MonoBehaviour>();

//         foreach (MonoBehaviour component in allComponents)
//         {
//             if (component != trafficControllerInstance && component != this && component!=FindObjectOfType<LogCSV>())
//             {
//                 component.enabled = false;
//             }
//         }
//     }

//     public void Update()
//     {
//         ConditionText.text = "Condition : " + currentCondition + "; nbTry : " + nbTry +"/6"; // 更新条件文本
//         // ConditionText.text = $"Condition: {currentCondition}, Try: {conditionTries[currentCondition]} / {maxTries}";
//         elapsedTime = Time.time - tryStartTime;
//         generalTime = Time.time - gameStartTime;
//         if (Input.GetKeyDown(KeyCode.N))
//         {
//             NextCondition();
//         }

//         if (Input.GetKeyDown(KeyCode.Escape))
//         {
//             SessionOver();
//         }
//         if (availableConditions.Count==0 && nbTry > maxNbTry)
//         {
//             UnityEngine.Debug.Log("<color=green>Session OVER !</color>");
//             SessionOver();
//         }
//         if (Input.GetKeyDown(KeyCode.C))
//         {
//             canMove=true;
//         }
//         if (Input.GetKeyDown(KeyCode.R))
//         {
//             RestartTry();
//         }
//     }
//     public enum GameCondition
//     {
//         Baseline,
//         Inter_const_3s,
//         Inter_const_6s,
//         Inter_varMinus_3s,
//         Inter_varMinus_6s,
//         Inter_varPlus_3s,
//         Inter_varPlus_6s
//     }

//     private void SelectRandomCondition()
//     {
//         int randomIndex = UnityEngine.Random.Range(0, availableConditions.Count);
//         selectedCondition = availableConditions[randomIndex];
//         availableConditions.RemoveAt(randomIndex);
//     }
//     private void SessionOver()
//     {
//         UnityEngine.Debug.Log("Session Over");
// #if UNITY_EDITOR
//         // if playing in the editor, stop playing
//         UnityEditor.EditorApplication.isPlaying = false;
// #else
//         // if running in an built app, close the app
//         Application.Quit();
// #endif
//     }
// }

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float elapsedTime;
    public Text ConditionText;
    public static bool canMove=false;
    private List<GameCondition> conditionsRemaining = new List<GameCondition>();
    public Dictionary<GameCondition, int> conditionTries = new Dictionary<GameCondition, int>();
    public GameCondition currentCond;
    public string currentCondition;
    private int maxTries = 6;
    public int totalTries = 0;
    private MarkerEvent markerEvent;
    private float tryStartTime;
    private float gameStartTime;
    public float generalTime;
    private trafficController trafficControllerInstance;
    private PlayerController playerController;
    private ScreenFader screenFader;
    private void Awake()
    {
        EnumerateConditions();
        markerEvent = FindObjectOfType<MarkerEvent>();
        trafficControllerInstance=FindObjectOfType<trafficController>();
        screenFader=FindObjectOfType<ScreenFader>();
        playerController = FindObjectOfType<PlayerController>();
    }
    private void EnumerateConditions()
    {
        conditionsRemaining = Enum.GetValues(typeof(GameCondition)).Cast<GameCondition>().ToList();
        foreach (var condition in conditionsRemaining)
        {
            conditionTries[condition] = 0;
        }
    }
    public void Update()
    {
        // ConditionText.text = "Condition : " + currentCondition + "; nbTry : " + nbTry +"/6"; // 更新条件文本
        ConditionText.text = $"Condition: {currentCondition}, Try: {conditionTries[currentCond]} / {maxTries}, Total tries: {totalTries} / 42";
        elapsedTime = Time.time - tryStartTime;
        generalTime = Time.time - gameStartTime;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SessionOver();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            canMove=true;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartTry();
        }
    }
    private void DeactivateCondScripts()
    {
        MonoBehaviour[] allComponents = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in allComponents)
        {
            if (component != trafficControllerInstance && component != this && component!=FindObjectOfType<LogCSV>())
            {
                component.enabled = false;
            }
        }
    }
    private void SelectRandomCondition()
    {
        List<GameCondition> availableConditions = conditionsRemaining.Where(cond => conditionTries[cond] < maxTries).ToList();
        if (availableConditions.Count == 0)
        {
            SessionOver();
            return;
        }
        int randomIndex = UnityEngine.Random.Range(0, availableConditions.Count); // 使用 availableConditions 列表的长度
        currentCond = availableConditions[randomIndex];
        conditionTries[currentCond]++;
        currentCondition = currentCond.ToString(); 
        totalTries++;
        // ExecuteConditionLogic(currentCond);
    }

    private void ActivateScriptByName(string scriptName)
    {
        MonoBehaviour scriptComponent = GetComponent(scriptName) as MonoBehaviour;

        if (scriptComponent != null)
        {
            scriptComponent.enabled = true;
        }
        else
        {
            UnityEngine.Debug.LogError("Component not found: " + scriptName);
        }
    }
    private void ExecuteConditionLogic(GameCondition condition)
    {
        switch (condition)
        {
            case GameCondition.Baseline:
                ActivateScriptByName("Baseline");
                markerEvent.RecordCondBaseline();
                break;
            case GameCondition.Inter_const_3s:
                ActivateScriptByName("Inter_const_3s");
                markerEvent.RecordCondInter_const_3s();
                break;
            // case GameCondition.Inter_const_6s:
            //     ActivateScriptByName("Inter_const_6s");
            //     markerEvent.RecordCondInter_const_6s();
            //     break;
            case GameCondition.Inter_varMinus_3s:
                ActivateScriptByName("Inter_varMinus_3s");
                markerEvent.RecordCondInter_varMinus_3s();
                break;
            // case GameCondition.Inter_varMinus_6s:
            //     ActivateScriptByName("Inter_varMinus_6s");
            //     markerEvent.RecordCondInter_varMinus_6s();
            //     break;
            case GameCondition.Inter_varPlus_3s:
                ActivateScriptByName("Inter_varPlus_3s");
                markerEvent.RecordCondInter_varPlus_3s();
                break;
            // case GameCondition.Inter_varPlus_6s:
            //     ActivateScriptByName("Inter_varPlus_6s");
            //     markerEvent.RecordCondInter_varPlus_6s();
            //     break;
            default:
                UnityEngine.Debug.LogError("Invalid game condition selected");
                break;
        }
        currentCondition = condition.ToString(); 
    }

    public void RestartTry()
    {
        DeactivateCondScripts();
        tryStartTime = Time.time;
        SelectRandomCondition();
        ExecuteConditionLogic(currentCond);
        playerController.ResetPed();
        trafficControllerInstance.ClearScene();
        trafficControllerInstance.InitializeScene();
        screenFader.ShowScreenFader();// markerEvent.RecordStartSimPedVrM();
    }

    private void SessionOver()
    {
        //markerEvent.RecordEndSimPedVA();
        markerEvent.RecordEndSimPedVrM();
        markerEvent.RecordHasEnded();
        Debug.Log("Session Over");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public enum GameCondition
    {
        Baseline,//0
        Inter_const_3s,//1
        // Inter_const_6s,//2
        Inter_varMinus_3s,//3
        // Inter_varMinus_6s,//4
        Inter_varPlus_3s,//5
        // Inter_varPlus_6s//6
    }
}
