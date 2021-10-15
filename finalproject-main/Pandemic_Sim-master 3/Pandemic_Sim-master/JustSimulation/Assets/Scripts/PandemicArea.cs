using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PandemicArea : MonoBehaviour
{
    public float range;

    public GameObject dummyBot;
    public int healthyBotCount;
    public int infectedBotCount;

    //new counts
    public int vaccinatedBotCount;
    public int expiredBotCount;

    //List of DummyBots
    private List<GameObject> dummyBotList = new List<GameObject>();

    public GameObject spawnPosList;


  
    public float exposureRadius = 4f;

    [Range(100f, 1f)]
    public float infectionCoeff = 50f;                      //check if the infection coefficient is ever changed.

    public float recoverTime = 50f;

    private GameObject exportObj;
    [System.NonSerialized]
    public int healthyCounter;
    [System.NonSerialized]
    public int infectedCounter = 0;

    [System.NonSerialized]
    public int recoveredCounter = 0;

    [System.NonSerialized]
    public int vaccinatedCounter = 0;

    [System.NonSerialized]
    public int expiredCounter = 0;        

    public int HealthyCounter
    {
        get => healthyCounter;
        set
        {
            healthyCounter = value;
            exportObj.GetComponent<ExportCsv>().record();
        }
    }
    public int InfectedCounter
    {
        get => infectedCounter;
        set
        {
            infectedCounter = value;
            exportObj.GetComponent<ExportCsv>().record();
        }
    }

    public int RecoveredCounter
    {
        get => recoveredCounter;
        set
        {
            recoveredCounter = value;
            exportObj.GetComponent<ExportCsv>().record();
        }
    }
     
    public int VaccinatedCounter
    {
        get => vaccinatedCounter;
        set
        {
            vaccinatedCounter = value;
            exportObj.GetComponent<ExportCsv>().record();
        }
    }


    public int ExpiredCounter
    {
        get => expiredCounter;
        set
        {
            expiredCounter = value;
            exportObj.GetComponent<ExportCsv>().record();
        }
    }

    
    public void CreateObjectAtRandomPosition(GameObject obj, int healthyNum, int infectedNum, int vaccinatedNum)
    {
        //Add default healthy bots
        for (int i = 0; i < healthyNum; i++)
        {
            int spawn_index = Random.Range(1, 35);
            Vector3 spawn_position = ChooseRandomSpawnPosition(spawn_index);
            GameObject f = Instantiate(obj, spawn_position, Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f)), transform);
            f.GetComponent<SphereCollider>().radius = exposureRadius;
            f.GetComponent<DummyBot>().infectionCoeff = infectionCoeff;
            f.GetComponent<DummyBot>().SpawnPosition = spawn_position;
            if(i == 0){
                Debug.Log(f.GetComponent<DummyBot>().SpawnPosition);
            }
            dummyBotList.Add(f);

        }
        //Add default starter infected bots
        for (int i = 0; i < infectedNum; i++)
        {
            int spawn_index = Random.Range(1, 35);
            Vector3 spawn_position = ChooseRandomSpawnPosition(spawn_index);
            GameObject b = Instantiate(obj, spawn_position, Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f)), transform);
            b.GetComponent<DummyBot>().m_InfectionStatus = DummyBot.agentStatus.INFECTED;
            b.GetComponent<DummyBot>().changeAgentStatus();
            b.GetComponent<SphereCollider>().radius = exposureRadius;
            b.GetComponent<DummyBot>().SpawnPosition = spawn_position;
            dummyBotList.Add(b);
        }

        //Add default starter vaccinated bots
        for (int i = 0; i < vaccinatedNum; i++)
        {
            int spawn_index = Random.Range(1, 35);
            Vector3 spawn_position = ChooseRandomSpawnPosition(spawn_index);
            GameObject v = Instantiate(obj, spawn_position, Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f)), transform);
            v.GetComponent<DummyBot>().m_InfectionStatus = DummyBot.agentStatus.VACCINATED;
            v.GetComponent<DummyBot>().changeAgentStatus();
            v.GetComponent<SphereCollider>().radius = exposureRadius;
            v.GetComponent<DummyBot>().SpawnPosition = spawn_position;
            dummyBotList.Add(v);
        }


    }
    public Vector3 ChooseRandomPosition()
    {
        return new Vector3(Random.Range(-range, range), 2f,
                Random.Range(-range, range)) + transform.position;
    }

    public Vector3 ChooseRandomSpawnPosition(int spawn_index)
    {
        Vector3 spawnPosition = spawnPosList.transform.GetChild(spawn_index).position;
        return new Vector3(spawnPosition.x + Random.Range(-5, 5), 2f,
                spawnPosition.z + Random.Range(-5, 5)) + transform.position;
    }


    public void ResetPandemicArea()
    {
        infectedCounter = 0;
        HealthyCounter = healthyBotCount + infectedBotCount; 
        recoveredCounter = 0;
        VaccinatedCounter = vaccinatedBotCount;
        ExpiredCounter = 0;


        if (dummyBotList.Count == 0)
        {
            CreateObjectAtRandomPosition(dummyBot, healthyBotCount, infectedBotCount, vaccinatedBotCount);
        }
        else
        {
            //Reset every dummyBot in the list
            for (int i = 0; i < dummyBotList.Count; i++)
            {

                if (i < healthyBotCount)
                {
                    int spawn_index = Random.Range(1, 35);
                    Vector3 spawn_position = ChooseRandomSpawnPosition(spawn_index);
                    dummyBotList[i].GetComponent<DummyBot>().m_InfectionStatus = DummyBot.agentStatus.HEALTHY;
                    dummyBotList[i].GetComponent<DummyBot>().changeAgentStatus();
                    dummyBotList[i].transform.position = spawn_position;
                    dummyBotList[i].GetComponent<DummyBot>().nextActionTime = -1f;
                    dummyBotList[i].GetComponent<DummyBot>().SpawnPosition = spawn_position;
                    dummyBotList[i].GetComponent<DummyBot>().recoverTime = recoverTime; //Reset the recoverTime also
                }
                else
                {
                    int spawn_index = Random.Range(1, 35);
                    Vector3 spawn_position = ChooseRandomSpawnPosition(spawn_index);
                    dummyBotList[i].GetComponent<DummyBot>().m_InfectionStatus = DummyBot.agentStatus.INFECTED;
                    dummyBotList[i].GetComponent<DummyBot>().changeAgentStatus();
                    dummyBotList[i].transform.position = spawn_position;
                    dummyBotList[i].GetComponent<DummyBot>().nextActionTime = -1f;
                    dummyBotList[i].GetComponent<DummyBot>().SpawnPosition = spawn_position;
                    dummyBotList[i].GetComponent<DummyBot>().recoverTime = recoverTime; //Reset the recoverTime also
                }

            }
        }

    }

    public void Awake()
    {
        exportObj = GetComponentInChildren<ExportCsv>().gameObject;
        exportObj.GetComponent<ExportCsv>().addHeaders();
        
        ResetPandemicArea();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPandemicArea();
        }
    }

}
