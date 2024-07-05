using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;

public class MarkerEvent : MonoBehaviour
{
    string StreamName = "cortivision_markers";// prendre le meme nom avec l4ordi serveur
    string StreamType = "Markers";
    private StreamOutlet outlet;
    private GameManager gameManager;
    private PlayerController playerController;
    private int[] sample={0};

    void Start()
    {
        gameManager=FindObjectOfType<GameManager>();
        playerController=FindObjectOfType<PlayerController>();
        var hash = new Hash128();
        hash.Append(StreamName);
        hash.Append(StreamType);
        hash.Append(gameObject.GetInstanceID());
        StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, 1, LSL.LSL.IRREGULAR_RATE,
            channel_format_t.cf_int8, hash.ToString());//hash est source id
        outlet = new StreamOutlet(streamInfo);
        // RecordStartSimPedVrM();
        // RecordStartSimPedVA();
    }

    public void RecordRestartTry()
    {
        if (outlet != null)
        {
            sample[0] = 1;//"NewTry";
            outlet.push_sample(sample);
            Debug.Log("New trial marker recorded for " + gameObject.name);
        }
    }

    public void RecordHasCrashed()
    {
        if (outlet != null)
        {
            sample[0] = 2;//"HasCrashed";
            outlet.push_sample(sample);
            Debug.Log("Car crash marker recorded for " + gameObject.name);
        }
    }

    public void RecordHasArrived()
    {
        if (outlet != null)
        {
            sample[0] = 3;//"HasArrived";
            outlet.push_sample(sample);
            Debug.Log("Successful crossing marker recorded for " + gameObject.name);
        }
    }
    public void RecordV1EnterCrossing()
    {
        if (outlet != null)
        {
            sample[0] = 4;//"V1EnterCrossing";
            outlet.push_sample(sample);
            Debug.Log("V1EnterCrossing marker recorded for " + gameObject.name);
        }
    }
    public void RecordV3ExitCrossing()
    {
        if (outlet != null)
        {
            sample[0] = 5;//"V3ExitCrossing";
            outlet.push_sample(sample);
            Debug.Log("V3ExitCrossing marker recorded for " + gameObject.name);
        }
    }

    public void RecordIsPaused()
    {
        if (outlet != null)
        {
            sample[0] = 6;//"Paused";
            outlet.push_sample(sample);
            Debug.Log("IsPaused marker recorded for " + gameObject.name);
        }
    }
    public void RecordHasEnded()
    {
        if (outlet != null)
        {
            sample[0] = 7;//"HasEnded";
            outlet.push_sample(sample);
            Debug.Log("HasEnded marker recorded for " + gameObject.name);
        }
    }
    public void RecordCondBaseline()
    {
        if (outlet != null)
        {
            sample[0] = 8;//"CondBaseline";
            outlet.push_sample(sample);
            Debug.Log("Baseline cond marker recorded for " + gameObject.name);
        }
    }
    public void RecordCondInter_const_3s()
    {
        if (outlet != null)
        {
            sample[0] = 9;//"CondInter_const_3s";
            outlet.push_sample(sample);
            Debug.Log("Const_3s cond marker recorded for " + gameObject.name);
        }
    }
    
    // public void RecordCondInter_const_6s()
    // {
    //     if (outlet != null)
    //     {
    //         sample[0] = 10;//"CondInter_const_6s";
    //         outlet.push_sample(sample);
    //         Debug.Log("Const_6s cond marker recorded for " + gameObject.name);
    //     }
    // }

    public void RecordCondInter_varMinus_3s()
    {
        if (outlet != null)
        {
            sample[0] = 11;//"CondInter_varMinus_3s";
            outlet.push_sample(sample);
            Debug.Log("Minus_3s cond marker recorded for " + gameObject.name);
        }
    }
    // public void RecordCondInter_varMinus_6s()
    // {
    //     if (outlet != null)
    //     {
    //         sample[0] = 12;//"CondInter_varMinus_6s";
    //         outlet.push_sample(sample);
    //         Debug.Log("Minus_6s cond marker recorded for " + gameObject.name);
    //     }
    // }
    public void RecordCondInter_varPlus_3s()
    {
        if (outlet != null)
        {
            sample[0] = 13;//"CondInter_varPlus_3s";
            outlet.push_sample(sample);
            Debug.Log("Plus_3s cond marker recorded for " + gameObject.name);
        }
    }
    // public void RecordCondInter_varPlus_6s()
    // {
    //     if (outlet != null)
    //     {
    //         sample[0] = 14;//"CondInter_varPlus_6s";
    //         outlet.push_sample(sample);
    //         Debug.Log("Plus_6s cond marker recorded for " + gameObject.name);
    //     }
    // }

    ////// nouveau ci-après /////
    public void RecordExtraSceneEnd()
    {
        if (outlet != null)
        {
            sample[0] =15;//"ExtraSceneEnd";
            outlet.push_sample(sample);
            Debug.Log("ExtraSceneEnd marker recorded for " + gameObject.name);
        }
    }
/////////// VA ////////////////////
    public void RecordStartSimPedVA()
    {
        if (outlet != null)
        {
            sample[0] =16;//"SimPedVA_everyStart";
            outlet.push_sample(sample);
            Debug.Log("SimPed-VA start marker recorded for " + gameObject.name);
        }
    }


////////// VrM ////////////////////
    public void RecordStartSimPedVrM()
    {
        if (outlet != null)
        {
            sample[0] =17;//"SimPedVrM_everyStart";
            outlet.push_sample(sample);
            Debug.Log("SimPed-VrM start marker recorded for " + gameObject.name);
        }
    }

/////// sim endings ////////
    public void RecordEndSimPedVA()
    {
        if (outlet != null)
        {
            sample[0] =18;//"SimPedVA_end";
            outlet.push_sample(sample);
            Debug.Log("SimPed-VA end marker recorded for " + gameObject.name);
        }
    }
    public void RecordEndSimPedVrM()
    {
        if (outlet != null)
        {
            sample[0] =19;//"SimPedVrM_end";
            outlet.push_sample(sample);
            Debug.Log("SimPed-VrM end marker recorded for " + gameObject.name);
        }
    }


}