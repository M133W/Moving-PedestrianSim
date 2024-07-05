using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class LogCSV : MonoBehaviour
{
    private GameManager gameManager;
    private StreamWriter writer;
    private trafficController trafficControllerInstance; 
    private PlayerController playerController;
    private CameraController cameraController;
    private ScreenFader screenFader;
    private float timer = 0f;
    private float samplingRate =0.02f;
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerController = FindObjectOfType<PlayerController>();
        trafficControllerInstance = FindObjectOfType<trafficController>();
        cameraController = FindObjectOfType<CameraController>();
        screenFader=FindObjectOfType<ScreenFader>();

        if (trafficControllerInstance == null)
        {
            Debug.LogError("未找到 trafficController 组件");
        }

        if (gameManager == null || playerController == null || cameraController == null)
        {
            Debug.LogError("未找到 GameManager, PlayerController, 或 CameraController 组件");
        }

        GenerateCSV();
    }

    void GenerateCSV()
    {
        //  get the date and time as the name of the file
        string fileName = DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";

        // get path to Downloads folder
        string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Downloads";

        // create the complete path
        string filePath = Path.Combine(downloadsPath, fileName);

        // creat a StreamWriter
        writer = new StreamWriter(filePath);

        // write the headlines of the CSV file 
        writer.WriteLine("TotalTime;TrialTime;InTransition;ExtraSceneShowing;TypeCond;NbTrial;UpcomingCar : dist;GapTime;GapVerification;Accident;TTC;CanMove;PedMove;PedTurn;DistWorldCar2Ped;ScreenCoordinate;WorldCoordinate;PedEyeWorldPos");

        Debug.Log("CSV file generated: " + filePath);
    }

    void Update()
    {
        // trafficController.Update();
        //the elaspedTime gets reset after each transportation

        timer += Time.deltaTime; // 增加计时器
        if (timer >= samplingRate)
        {
            if (trafficControllerInstance != null)
            {
                writer.WriteLine(
                gameManager.generalTime+";"+
                gameManager.elapsedTime + ";"+
                screenFader.isFading.ToString()+";"+
                trafficControllerInstance.ExtraSceneShowing.ToString()+";"+
                gameManager.currentCondition+";"+
                // gameManager.nbTry+";"+
                gameManager.conditionTries[gameManager.currentCond]+";"+
                trafficControllerInstance.UpcomingCar+";"+
                trafficControllerInstance.GapCrossTime.ToString()+";"+
                trafficControllerInstance.GapVerification.ToString()+";"+
                playerController.hasCrashed.ToString()+";"+
                trafficControllerInstance.TTC.ToString()+";"+
                GameManager.canMove.ToString()+";"+
                playerController.PedMove.ToString()+";"+
                cameraController.normalizedRotation.ToString()+";"+
                trafficControllerInstance.Dist2PedCarList+";"+
                trafficControllerInstance.CarScreenCoordinates+";"+
                trafficControllerInstance.CarWorldCoordinates+';'+
                cameraController.PedEyeWorldPos.ToString()
                );
            }
            timer=0f;
        }
    }

    void OnDestroy()
    {
        // closing StreamWriter
        if (writer != null)
        {
            writer.Close();
        }
    }
}
