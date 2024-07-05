using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inter_varPlus_6s : MonoBehaviour
{
    private trafficController controller;
    private int ThirdCheckPoint;
    private int SecondCheckPoint;
    private int FirstCheckPoint=50;
    private float SmoothSpeedTime=3f;
    private Dictionary<Car, Coroutine> decelerationCoroutines = new Dictionary<Car, Coroutine>();
    void Start()
    {
        controller = FindObjectOfType<trafficController>();
        SecondCheckPoint = -1;
        ThirdCheckPoint = -1;
    }
    private void Update()
    {
        if (controller != null)
        {
            foreach (GameObject VehicleGroup in controller.CarsGroups)
            {
                switch (VehicleGroup.tag)
                {
                    case "C2":
                        Car[] C2cars = VehicleGroup.GetComponentsInChildren<Car>();
                        foreach (Car C2car in C2cars)
                        {
                            if (C2car.CurrentElement == FirstCheckPoint && !decelerationCoroutines.ContainsKey(C2car))
                            {
                                Coroutine coroutine = StartCoroutine(DecelerateCoroutine(C2car, VehicleGroup,50f, SmoothSpeedTime));
                                UnityEngine.Debug.Log("Car starting slowing down to 50km/h");
                                decelerationCoroutines.Add(C2car, coroutine);
                            }
                        }
                        break;
                    case "C3":
                        Car[] C3cars = VehicleGroup.GetComponentsInChildren<Car>();
                        foreach (Car C3car in C3cars)
                        {
                            if (C3car.CurrentElement == FirstCheckPoint && !decelerationCoroutines.ContainsKey(C3car))
                            {
                                Coroutine coroutine = StartCoroutine(DecelerateCoroutine(C3car, VehicleGroup,50f, SmoothSpeedTime));
                                UnityEngine.Debug.Log("Car starting slowing down to 50km/h");
                                decelerationCoroutines.Add(C3car, coroutine);
                            }
                            if (C3car.CurrentElement == SecondCheckPoint-3 && !decelerationCoroutines.ContainsKey(C3car))
                            {
                                Coroutine coroutine = StartCoroutine(DecelerateCoroutine(C3car, VehicleGroup,40f, SmoothSpeedTime));
                                UnityEngine.Debug.Log("Car starting slowing down to 40km/h");
                                decelerationCoroutines.Add(C3car, coroutine);
                            }
                        }
                        break;
                    case "C4":
                        Car[] C4cars = VehicleGroup.GetComponentsInChildren<Car>();
                        foreach (Car C4car in C4cars)
                        {
                            if (C4car.CurrentElement == FirstCheckPoint && !decelerationCoroutines.ContainsKey(C4car))
                            {
                                Coroutine coroutine = StartCoroutine(DecelerateCoroutine(C4car, VehicleGroup,50f, SmoothSpeedTime));
                                UnityEngine.Debug.Log("Car starting slowing down to 50km/h");
                                decelerationCoroutines.Add(C4car, coroutine);
                            }

                            if(SecondCheckPoint!=-1){
                                if (C4car.CurrentElement == SecondCheckPoint-3 && !decelerationCoroutines.ContainsKey(C4car))
                                {
                                    Coroutine coroutine = StartCoroutine(DecelerateCoroutine(C4car, VehicleGroup,40f, SmoothSpeedTime));
                                    UnityEngine.Debug.Log("Car starting slowing down to 40km/h");
                                    decelerationCoroutines.Add(C4car, coroutine);
                                }
                                if(ThirdCheckPoint!=-1){
                                    if (C4car.CurrentElement == ThirdCheckPoint-3 && !decelerationCoroutines.ContainsKey(C4car))
                                    {
                                        Coroutine coroutine = StartCoroutine(DecelerateCoroutine(C4car, VehicleGroup,30f, SmoothSpeedTime));
                                        UnityEngine.Debug.Log("Car starting slowing down to 30km/h");
                                        decelerationCoroutines.Add(C4car, coroutine);
                                    }
                                }
                            }
                        }   
                        break;
                    case "C1":
                        break;
                    default:
                        Debug.LogWarning($"Unknown tag found: {VehicleGroup.tag}");
                        break;
                }
            }
        }
        else
        {
            Debug.LogError("Cannot find trafficController component!");
        }
    }
    private IEnumerator DecelerateCoroutine(Car GivenCar, GameObject VehicleGroup, float targetSpeedKMH, float duration)
    {
        float startTime = Time.time;
        float startSpeed = GivenCar.SpeedMPS; // 记录当前速度作为初始速度

        while (Time.time - startTime < duration)
        {
            float elapsedTime = Time.time - startTime;
            float t = elapsedTime / duration;
            float newSpeed = Mathf.Lerp(startSpeed, targetSpeedKMH / 3.6f, t); // 使用记录的初始速度作为起始值
            GivenCar.SpeedMPS = newSpeed;
            Car GroupComponent = VehicleGroup.GetComponent<Car>();
            if (GroupComponent != null)
            {
                GroupComponent.SpeedMPS = newSpeed;
            }
            yield return null;
        }
        if (VehicleGroup.tag == "C3" && GivenCar.gameObject.tag == "V1" && SecondCheckPoint == -1)
        {
            if (ThirdCheckPoint == -1) 
            {
                SecondCheckPoint = GivenCar.CurrentElement;
                UnityEngine.Debug.Log("2nd check point element : " + SecondCheckPoint);
            }
        }
        else if (VehicleGroup.tag == "C4" && GivenCar.gameObject.tag == "V1" && ThirdCheckPoint == -1 && SecondCheckPoint!=-1)
        {
            if(SecondCheckPoint!=GivenCar.CurrentElement)
            {
                ThirdCheckPoint = GivenCar.CurrentElement;
                UnityEngine.Debug.Log("3rd check point element : " + ThirdCheckPoint);
            }
        }
        GivenCar.SpeedMPS = targetSpeedKMH / 3.6f;
        decelerationCoroutines.Remove(GivenCar);
    }
}
