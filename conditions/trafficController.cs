using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyRoads3Dv3;

public class trafficController : MonoBehaviour
{
    public bool ExtraSceneShowing=false;
    public string UpcomingCar;
    public float spawnInterval;
    private GameManager gameManager;
    private Coroutine spawnCoroutine;
    public float instantSpeed;
    private float groupSpacing=0f;
    public int currentGroup;
    public float GapCrossTime; 
    public float GapVerification;
    public string Dist2PedCarList; 
    public string CarScreenCoordinates; 
    public string CarWorldCoordinates;
    public float TTC;
    public bool CarInCrossing = false;
    public float PreCarExitTime;
    private int totalGroups;
    public GameObject chaineVoiture;
    public GameObject OneCar;
    private ERRoadNetwork roadNetwork;
    private ERRoad road;
    private UnityEngine.Vector3[] centerPoints;
    private float carLength;
    private int MetersPerElement;
    private float maxCenterPoint;
    private float roadLength;
    public int PedElementPos;
    public int InGroupeCarGap;//en metre par rapport au centre, donc 10m en vrai
    private int startCurrentPoint; // 1 element = 4 metres  = 53
    private float minDistance;
    private GameObject selectedVehicleGroup;
    private Car selectedCar;
    public float givenSpeed=0f;
    public float initialSpeedMPS;
    public float carGroupLength;
    public float Time4CarGroupLength;
    public List<GameObject> CarsGroups; 
    public List<GameObject> CarPool;
    private Coroutine WaitToBalanceCoroutine;
    private readonly object carsGroupsLock = new object();
    private readonly object carPoolLock = new object();
    private MarkerEvent markerEvent;

    public void Awake()
    { 
        markerEvent=FindObjectOfType<MarkerEvent>();
        PedElementPos = 2;
        InGroupeCarGap = 16;
        totalGroups = 4;
        startCurrentPoint = 53;
        carLength= 5.7f;
        CarInCrossing = false;
        gameManager=FindObjectOfType<GameManager>();
        roadNetwork = new ERRoadNetwork();
        if (roadNetwork == null)
        {
            UnityEngine.Debug.LogError("Road Network object not found.");
        }
        road=roadNetwork.GetRoadByName("200mRoad");
        if (road == null)
        {
            UnityEngine.Debug.LogError("Road object not found.");
        }
        centerPoints=road.GetSplinePointsCenter();
        maxCenterPoint = centerPoints.Length;
        roadLength=road.GetDistance();
        MetersPerElement=(int)(roadLength/maxCenterPoint);
        carGroupLength = 3 * carLength + 2 * (InGroupeCarGap-carLength);
        UnityEngine.Debug.Log("CarGroupLength: "+carGroupLength);
        UnityEngine.Debug.Log("Meters per element: " + MetersPerElement);
        UnityEngine.Debug.Log("Road length: " + roadLength);
        UnityEngine.Debug.Log("Max Center Point Value: " + maxCenterPoint);
        minDistance = float.MaxValue;
    }
    public void Start()
    {
        gameManager.RestartTry();
        // InitializeScene();
    }
    public void InitializeScene()
    {
        // if(spawnCoroutine!=null)
        // {
        //     StopCoroutine(spawnCoroutine);
        //     if(WaitToBalanceCoroutine!=null)
        //     {
        //         StopCoroutine(WaitToBalanceCoroutine);
        //     }
        // }
        currentGroup = 0;
        GapCrossTime = 0f;
        GapVerification = 0f;
        Dist2PedCarList = "";
        CarScreenCoordinates = ""; 
        CarWorldCoordinates=""; 
        UpcomingCar = "";
        TTC = 0f;
        // CarsGroups = new List<GameObject>();
        // CarPool=new List<GameObject>();
        PreCarExitTime=Time.time;
        selectedVehicleGroup= null;
        selectedCar=null;
        // playerController.hasCrashed=false;
        CarInCrossing=false;
        initialSpeedMPS=givenSpeed/3.6f;
        GiveGroupSpacing();
        if (gameManager.currentCondition == "Inter_varPlus_3s" )//|| gameManager.currentCondition == "Inter_varPlus_6s")
        {
            StartCoroutine(WaitToBalance(12.4f));
        }
        else
        {
            startSpawnCoroutine();
        }
    }
    private IEnumerator WaitToBalance(float delay)
    {
        WaitToBalanceCoroutine=StartCoroutine(StartSpawnCoroutineWithDelay(delay));
        // ExtraSceneShowing=false;
        yield return WaitToBalanceCoroutine;
    }

    private IEnumerator StartSpawnCoroutineWithDelay(float delay)
    {
        ExtraSceneShowing=true;
        yield return new WaitForSeconds(delay);
        markerEvent.RecordExtraSceneEnd();
        ExtraSceneShowing=false;
        startSpawnCoroutine();
    }

    private void startSpawnCoroutine()
    {
        // spawnCoroutine = StartCoroutine(SpawnCarGroup(spawnInterval));
        spawnCoroutine = StartCoroutine(SpawnCarGroup());

    }
    void GiveGroupSpacing()
    {
        Component[] components = GetComponents(typeof(Component));
        foreach (Component component in components)
        {
            MonoBehaviour monoBehaviour = component as MonoBehaviour;
            if (monoBehaviour == null || !monoBehaviour.isActiveAndEnabled)
                continue;
            string componentName = component.GetType().Name;
            switch (componentName)
            {
                case "Inter_const_3s":
                    groupSpacing = 3.14f;
                    givenSpeed=30f;
                    UnityEngine.Debug.Log("Group spacing selon Inter_const_3s");
                    break;
                // case "Inter_const_6s":
                //     groupSpacing = 6.14f;
                //     givenSpeed=30f;
                //     UnityEngine.Debug.Log("Group spacing selon Inter_const_6s");
                //     break;
                case "Inter_varMinus_3s":
                    groupSpacing =0f;
                    givenSpeed=30f;
                    UnityEngine.Debug.Log("Group spacing selon Inter_varMinus_3s");
                    break;
                // case "Inter_varMinus_6s":
                //     groupSpacing =0f;
                //     givenSpeed=30f;
                //     UnityEngine.Debug.Log("Group spacing selon Inter_varMinus_6s");
                //     break;
                case "Inter_varPlus_3s":
                    groupSpacing =0.7f;
                    givenSpeed=60f;
                    UnityEngine.Debug.Log("Group spacing selon Inter_varPlus_3s");
                    break;
                // case "Inter_varPlus_6s":
                //     groupSpacing =0.7f;
                //     givenSpeed=60f;
                //     UnityEngine.Debug.Log("Group spacing selon Inter_varPlus_6s");
                //     break;
                default:
                    groupSpacing =0f;
                    givenSpeed=30f;
                    UnityEngine.Debug.Log("Group spacing selon Baseline");
                    break;
            }
        }
    }
    public void Update()
    {
        CarScreenCoordinates = "";
        CarWorldCoordinates="";
        GetCor();
        float minDistance = float.MaxValue;
        GameObject closestVehicleGroup = null;
        Car closestCar = null;
        Dist2PedCarList="";
        foreach (GameObject vehicleGroup in CarsGroups)
        {
            Car carComponent = vehicleGroup.GetComponent<Car>(); 
            if (!carComponent.HasReachedDestination)
            {
                Car v1Component = vehicleGroup.transform.Find("V1").GetComponent<Car>();
                Car v2Component = vehicleGroup.transform.Find("V2").GetComponent<Car>();
                Car v3Component = vehicleGroup.transform.Find("V3").GetComponent<Car>();
                MoveCar(v1Component);
                MoveCar(v2Component);
                MoveCar(v3Component);
                minDistance = CheckAndSetMinDistance(v3Component, minDistance, vehicleGroup, ref closestVehicleGroup, ref closestCar);
                minDistance = CheckAndSetMinDistance(v2Component, minDistance, vehicleGroup, ref closestVehicleGroup, ref closestCar);
                minDistance = CheckAndSetMinDistance(v1Component, minDistance, vehicleGroup, ref closestVehicleGroup, ref closestCar);
                UnityEngine.Debug.Log($"{vehicleGroup.tag+v1Component.tag}.Dis2Ped: {v1Component.DistanceToPed}");
                UnityEngine.Debug.Log($"{vehicleGroup.tag+v2Component.tag}.Dis2Ped: {v2Component.DistanceToPed}");
                UnityEngine.Debug.Log($"{vehicleGroup.tag+v3Component.tag}.Dis2Ped: {v3Component.DistanceToPed}");
                Dist2PedCarList+=vehicleGroup.tag+v1Component.tag+" : "+v1Component.DistanceToPed.ToString()+", ";
                Dist2PedCarList+=vehicleGroup.tag+v2Component.tag+" : "+v2Component.DistanceToPed.ToString()+", ";
                Dist2PedCarList+=vehicleGroup.tag+v3Component.tag+" : "+v3Component.DistanceToPed.ToString()+", ";
            }
        }
        if (closestVehicleGroup != null && closestCar != null)
        {
            string vehicleGroupTag = closestVehicleGroup.tag;
            string specificCarTag = closestCar.tag;
            TTC = closestCar.DistanceToPed / closestCar.SpeedMPS;
            UpcomingCar = vehicleGroupTag + specificCarTag +" : "+minDistance;
        } 
        else
        {
            UpcomingCar = "NA"; 
        }
        UnityEngine.Debug.Log("UpcomingCar: " + UpcomingCar);

        if(UpcomingCar!="NA")
        {
            GapCrossTime = Time.time-PreCarExitTime;
        }
        else
        {
            GapCrossTime = 0f;
            CarScreenCoordinates ="NA";
            CarWorldCoordinates="NA";
            Dist2PedCarList="NA";
        }
        Debug.LogFormat("<color=yellow> GapCrossTime: {0}</color>", GapCrossTime);
    }
    public void SetGroupSpeed(float speed)
    {
        givenSpeed = speed;
        initialSpeedMPS = givenSpeed / 3.6f;
        UnityEngine.Debug.Log("Group speed set to: " + speed);
    }
    public GameManager.GameCondition GetCondition()
    {
        var scriptType = this.GetType();
        var scriptName = scriptType.Name;
        if (System.Enum.TryParse<GameManager.GameCondition>(scriptName, out var condition))
        {
            return condition;
        }
        else
        {
            UnityEngine.Debug.LogError($"Failed to parse enum for script: {scriptName}");
            return GameManager.GameCondition.Baseline; 
        }
    }
    public void DisableAllChildScripts()
    {
        Component[] allComponents = GetComponents<Component>();

        foreach (Component component in allComponents)
        {
            if (!(component is trafficController) && !(component is GameManager)&& !(component is LogCSV))
            {
                if (component.GetType() != typeof(Transform))
                {
                    if (component is MonoBehaviour monoBehaviour)
                    {
                        monoBehaviour.enabled = false;
                    }
                }
            }
        }
    }
    public void ClearScene()
    {
        if(spawnCoroutine!=null)
        {
            StopCoroutine(spawnCoroutine);
            if(WaitToBalanceCoroutine!=null)
            {
                StopCoroutine(WaitToBalanceCoroutine);
            }
        }
        Car[] cars = FindObjectsOfType<Car>();
        foreach (Car car in cars)
        {
            if (car != null && car.gameObject != null)
            {
                Destroy(car.gameObject);
            }
        }
        // CarPool.Clear();
        // CarsGroups.Clear();
        lock (carsGroupsLock)
        {
            CarsGroups.Clear();
        }

        lock (carPoolLock)
        {
            CarPool.Clear();
        }
        CarsGroups = new List<GameObject>();
        CarPool=new List<GameObject>();
    }
    float CheckAndSetMinDistance(Car carComponent, float currentMinDistance, GameObject vehicleGroup, ref GameObject selectedGroup, ref Car selectedCar)
    {
        float minDistance = currentMinDistance;

        if (!carComponent.HasReachedDestination)
        {
            if (carComponent.DistanceToPed < minDistance || minDistance == float.MaxValue )
            {
                minDistance = carComponent.DistanceToPed;
                selectedGroup = vehicleGroup;
                selectedCar = carComponent;
            }
        }
        return minDistance;
    }
    void GetCor()
    {
        foreach (GameObject VehicleGroup in CarsGroups)
        {
            if (VehicleGroup != null && VehicleGroup.gameObject.activeSelf)
            {
                string vehicleGroupTag = VehicleGroup.tag;
                foreach (Transform carTransform in VehicleGroup.transform)
                {
                    Vector3 worldCoordinate = carTransform.position;
                    CarWorldCoordinates += vehicleGroupTag + carTransform.tag + " : " + worldCoordinate.ToString("F2") + ",";
                    UnityEngine.Vector3 screenCoordinate = GetScreenCoordinate(carTransform.position);
                    if (screenCoordinate.x >= 0 && screenCoordinate.x <= Screen.width && screenCoordinate.y >= 0 && screenCoordinate.y <= Screen.height)
                    {
                        CarScreenCoordinates += vehicleGroupTag + carTransform.tag + " : " + screenCoordinate.ToString("F2") + ",";
                    }
                    else
                    {
                        CarScreenCoordinates += vehicleGroupTag + carTransform.tag + " : NA,";
                    }
                }
            }
        }
        UnityEngine.Debug.Log("ScreenCoordinate list : "+ CarScreenCoordinates);
        UnityEngine.Debug.Log("WorldCoordinate list : "+ CarWorldCoordinates);
    }
    private UnityEngine.Vector3 GetScreenCoordinate(UnityEngine.Vector3 worldPos)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return UnityEngine.Vector2.zero;
        }
        // Check if the worldPos is within the camera's frustum
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        if (!GeometryUtility.TestPlanesAABB(frustumPlanes, new Bounds(worldPos, Vector3.one)))
        {
            // Object is not within the camera's frustum
            return UnityEngine.Vector2.zero;
        }
        UnityEngine.Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        // float dpiScaleX = Screen.dpi > 0 ? Screen.dpi / 96.0f : 1.0f;
        // float dpiScaleY = Screen.dpi > 0 ? Screen.dpi / 96.0f : 1.0f;
        return new UnityEngine.Vector2(screenPos.x/Screen.width, screenPos.y/ Screen.height); //* dpiScaleX * dpiScaleY
    }

    private IEnumerator SpawnCarGroup()//float timer
    {
        if(gameManager.currentCondition!="Baseline")
        {
            while (currentGroup < totalGroups)
            {
                GameObject VehicleGroup=null;
                string tag;
                Debug.Log("Center point at index 53: " + centerPoints[53]);
                if (centerPoints.Length > 53)
                {
                    VehicleGroup = Instantiate(chaineVoiture, centerPoints[53], Quaternion.identity);
                }
                else
                {
                    Debug.LogError("CenterPoints list does not contain enough elements to access index 53.");
                }
                tag = "C" + (currentGroup + 1);
                VehicleGroup.tag = tag;
                Car carComponents = VehicleGroup.AddComponent<Car>();
                SetGroupSpeed(givenSpeed);
                // AddCarGroup(VehicleGroup);
                lock (carsGroupsLock)
                {
                    // CarsGroups.Add(VehicleGroup);
                    AddCarGroup(VehicleGroup);
                }
                Car v1Component = VehicleGroup.transform.Find("V1").gameObject.AddComponent<Car>();
                Car v2Component = VehicleGroup.transform.Find("V2").gameObject.AddComponent<Car>();
                Car v3Component = VehicleGroup.transform.Find("V3").gameObject.AddComponent<Car>();

                v1Component.Initialize(startCurrentPoint + 0,initialSpeedMPS); // ça fait 1.3s de gap interieur (10m de dist entres les VA de meme groupe)max
                // AddCar(VehicleGroup.transform.Find("V1").gameObject);
                v2Component.Initialize(startCurrentPoint + 4,initialSpeedMPS); // pour vitesse de 8.33m/s
                // AddCar(VehicleGroup.transform.Find("V2").gameObject);
                v3Component.Initialize(startCurrentPoint + 8,initialSpeedMPS); // 2.83-2.96s de gap exterieur
                // AddCar(VehicleGroup.transform.Find("V3").gameObject);
                lock (carPoolLock)
                {
                    // CarPool.Add(V1);
                    // CarPool.Add(V2);
                    // CarPool.Add(V3);
                    AddCar(VehicleGroup.transform.Find("V1").gameObject);
                    AddCar(VehicleGroup.transform.Find("V2").gameObject);
                    AddCar(VehicleGroup.transform.Find("V3").gameObject);
                }
                currentGroup+=1;
                UnityEngine.Debug.Log("currentGroup : " + currentGroup);
                Time4CarGroupLength = carGroupLength / initialSpeedMPS;

                float intervalToWait = 0f; // timer;
                if (gameManager.currentCondition==("Inter_varMinus_3s"))
                {
                    // 根据currentGroup的值选择对应的SpawnInterval
                    List<float> spawnIntervals = new List<float>() {9.0f+4.86f, 6.6f+3.64f, 5.4f+2.9f};
                    if (currentGroup - 1 < spawnIntervals.Count)
                    {
                        intervalToWait = spawnIntervals[currentGroup - 1];
                        UnityEngine.Debug.Log("SpawningVarCarGroup at : "+intervalToWait);
                    }
                }
                // else if(gameManager.currentCondition==("Inter_varMinus_6s"))
                // {
                //     List<float> spawnIntervals = new List<float>() {9.0f+7.88f, 6.6f+6.62f, 5.4f+5.9f};
                //     if (currentGroup - 1 < spawnIntervals.Count)
                //     {
                //         intervalToWait = spawnIntervals[currentGroup - 1];
                //         UnityEngine.Debug.Log("SpawningVarCarGroup at : "+intervalToWait);
                //     }
                // }
                else if(gameManager.currentCondition==("Inter_varPlus_3s"))
                {
                    List<float> spawnIntervals = new List<float>() {groupSpacing + Time4CarGroupLength+0.33f, groupSpacing + Time4CarGroupLength+0.35f, groupSpacing + Time4CarGroupLength+0.4f};
                    if (currentGroup - 1 < spawnIntervals.Count)
                    {
                        intervalToWait = spawnIntervals[currentGroup - 1];
                        UnityEngine.Debug.Log("SpawningVarCarGroup at : "+intervalToWait);
                    }
                }
                // else if(gameManager.currentCondition==("Inter_varPlus_6s"))
                // {
                //     List<float> spawnIntervals = new List<float>() {groupSpacing + Time4CarGroupLength+3.33f, groupSpacing + Time4CarGroupLength+3.35f, groupSpacing + Time4CarGroupLength+3.4f};
                //     if (currentGroup - 1 < spawnIntervals.Count)
                //     {
                //         intervalToWait = spawnIntervals[currentGroup - 1];
                //         UnityEngine.Debug.Log("SpawningVarCarGroup at : "+intervalToWait);
                //     }
                // }
                else
                {
                    intervalToWait=groupSpacing + Time4CarGroupLength;
                }
                UnityEngine.Debug.Log("Vehicle Group Speed:" + initialSpeedMPS);
                UnityEngine.Debug.Log("Time4CarGroupLength:" + Time4CarGroupLength);
                yield return new WaitForSeconds(intervalToWait);
            }
        }
        else //spawn for baseline
        {
            // intervalToWait=0f;
            while (currentGroup < 1)
            {
                SetGroupSpeed(givenSpeed);
                GameObject OnlyCar=null;
                OnlyCar = Instantiate(OneCar, centerPoints[53], Quaternion.identity);
                Car OnlyCarGroupComponent = OnlyCar.AddComponent<Car>();
                Car v1Component = OnlyCar.transform.Find("V1").gameObject.AddComponent<Car>();
                v1Component.Initialize(startCurrentPoint,initialSpeedMPS); 
                // AddCarGroup(OnlyCar);
                // AddCar(v1Component.gameObject);
                lock (carsGroupsLock)
                {
                    // CarsGroups.Add(OnlyCar);
                    AddCarGroup(OnlyCar);
                }

                lock (carPoolLock)
                {
                    // CarPool.Add(V1);
                    AddCar(v1Component.gameObject);
                }
                currentGroup+=1;
                float intervalToWait = 0f;
                yield return new WaitForSeconds(intervalToWait);
                // yield return null;
            }
        }
       
    }
    void DisableCarComponents(GameObject carObject)
    {
        UnityEngine.Component[] components = carObject.GetComponents<UnityEngine.Component>();
        (carObject as GameObject)?.SetActive(false);
    }
    void MoveCar(Car car)
    {
        if(!car.HasReachedDestination){
            if (centerPoints.Length > 0)
            {
                car.Distance += car.DistanceToMove;

                if (car.Distance > UnityEngine.Vector3.Distance(centerPoints[car.CurrentElement], centerPoints[car.CurrentElement+1]))
                {
                    car.Distance -= UnityEngine.Vector3.Distance(centerPoints[car.CurrentElement], centerPoints[car.CurrentElement + 1]);
                    car.CurrentElement--;
                    if (car.CurrentElement < 0)
                    {
                        car.CurrentElement = 0;//centerPoints.Length - 2; // Adjusted to be the second-to-last element
                        car.HasReachedDestination=true;
                        DisableCarComponents(car.gameObject);
                    }
                }
                UnityEngine.Debug.Log($"currentElement: {car.CurrentElement},currentVehicleGroup: {car.tag}");

                UnityEngine.Vector3 position = UnityEngine.Vector3.Lerp(centerPoints[car.CurrentElement + 1], centerPoints[car.CurrentElement], car.Distance / UnityEngine.Vector3.Distance(centerPoints[car.CurrentElement], centerPoints[car.CurrentElement + 1]));
                UnityEngine.Vector3 normal = UnityEngine.Vector3.up; // 初始化为向上的法线

                // 根据道路倾斜值调整法线// normal = UnityEngine.Quaternion.AngleAxis(tilting, centerPoints[car.CurrentElement + 1] - centerPoints[car.CurrentElement]) * normal;
                //pour VA
                //position += normal;
                //pour VrM *0.15f ;// * 1.1f; // 调整值根据需要调整 // float offset=1.1f; // position = Vector3.ProjectOnPlane(position, normal*offset);
                position += normal*0.15f;
                car.transform.position = position;

                UnityEngine.Vector3 direction = centerPoints[car.CurrentElement + 1] - centerPoints[car.CurrentElement];
                
                UnityEngine.Quaternion rotation = UnityEngine.Quaternion.LookRotation(-direction.normalized, normal);
                car.transform.rotation = rotation;

                float distanceToMove = car.SpeedMPS * Time.deltaTime;
                car.DistanceToMove = distanceToMove;

                instantSpeed = car.SpeedMPS * 3.6f;
                UnityEngine.Debug.Log("trafficControllerBase instant speed: " + instantSpeed);

                car.DistanceToPed = (car.CurrentElement - PedElementPos) * MetersPerElement-(carLength/2);
                car.DistanceToPed = Mathf.Lerp(car.DistanceToPed, car.DistanceToPed-MetersPerElement, Time.deltaTime);
                // TTC = car.DistanceToPed / car.SpeedMPS;
            }
        }
    }
    private void AddCarGroup(GameObject VehicleGroup)
    {
        CarsGroups.Add(VehicleGroup);
    }
    private void AddCar(GameObject car)
    {
        CarPool.Add(car);
    }
}
public class Car : MonoBehaviour
{
    public float Distance { get; set; }
    public float DistanceToMove { get; set; }
    public int CurrentElement { get; set; }
    public bool HasReachedDestination { get; set; }
    public float SpeedMPS { get; set; }
    public float InstantSpeed;
    public float DistanceToPed { get; set; }
    private float carLength= 5.7f;
    private trafficController trafficControllerInstance;
    private GameManager gameManager;
    private MarkerEvent markerEvent;
    private void Start()
    {
        markerEvent=FindObjectOfType<MarkerEvent>();
        trafficControllerInstance=FindObjectOfType<trafficController>();
        gameManager=FindObjectOfType<GameManager>();
        SpeedMPS=trafficControllerInstance.givenSpeed/3.6f;
    }
    public void Initialize(int startCurrentPoint, float speedMPS)
    {
        Distance = 0;
        SpeedMPS = speedMPS; // 设置车辆速度
        DistanceToMove = SpeedMPS * Time.deltaTime;
        CurrentElement = startCurrentPoint;
        HasReachedDestination = false;
        DistanceToPed = (CurrentElement-2)*4-(carLength/2);// trafficControllerInstance.MetersPerElement
//hard codinggggggggggg 4 meters
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("crossing"))
        {
        
            trafficControllerInstance.GapVerification=Time.time-trafficControllerInstance.PreCarExitTime;
            UnityEngine.Debug.Log("Gap verification : "+ trafficControllerInstance.GapVerification);
            UnityEngine.Debug.Log("<color=green> passing through !</color>");
            trafficControllerInstance.CarInCrossing=true;
            if(this.CompareTag("V1"))
            {
                GameManager.canMove=true;
                markerEvent.RecordV1EnterCrossing();
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("crossing"))
        {
            UnityEngine.Debug.Log("<color=red> leaving !</color>");
            trafficControllerInstance.PreCarExitTime=Time.time;
            trafficControllerInstance.CarInCrossing=false;
            if(this.CompareTag("V3"))
            {
                markerEvent.RecordV3ExitCrossing();
            }
            if(this.transform.parent.gameObject.tag=="C4" && this.CompareTag("V3"))
            {
                UnityEngine.Debug.Log("Try ending");
                markerEvent.RecordHasEnded();
                gameManager.RestartTry();
            }
        }
    }
    private void Update(){
        InstantSpeed=SpeedMPS*3.6f;
    }
}