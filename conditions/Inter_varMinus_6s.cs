using UnityEngine;

public class Inter_varMinus_6s : MonoBehaviour
{
    private trafficController controller;
    private void Start()
    {
        controller = FindObjectOfType<trafficController>();
    }

    private void Update()
    {
        if (controller != null)
        {
            foreach (GameObject VehicleGroup in controller.CarsGroups)
            {
                switch (VehicleGroup.tag)
                {
                    case "C1":
                        SetSpeedForCarGroup(VehicleGroup, 30f);
                        // SetSpawnSpacingForCarGroup(VehicleGroup, 0f);
                        break;
                    case "C2":
                        SetSpeedForCarGroup(VehicleGroup, 40f);
                        // SetSpawnSpacingForCarGroup(VehicleGroup, 9f+3f);
                        break;
                    case "C3":
                        SetSpeedForCarGroup(VehicleGroup, 50f);
                        // SetSpawnSpacingForCarGroup(VehicleGroup, 6.6f+3f);
                        break;
                    case "C4":
                        SetSpeedForCarGroup(VehicleGroup, 60f);
                        // SetSpawnSpacingForCarGroup(VehicleGroup, 5.4f+3f);
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

    private void SetSpeedForCarGroup(GameObject carGroup, float speed)
    {
        Car[] cars = carGroup.GetComponentsInChildren<Car>();
        foreach (Car car in cars)
        {
            car.SpeedMPS = speed / 3.6f;
        }
    }
    // private void SetSpawnSpacingForCarGroup(GameObject carGroup, float timeSpacing)
    // {
    //     Car[] cars = carGroup.GetComponentsInChildren<Car>();
    //     foreach (Car car in cars)
    //     {
    //         car.LatentTimeToStart = timeSpacing;
    //     }   
    // }
}
