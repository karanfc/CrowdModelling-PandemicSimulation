using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

/// <summary>
/// Manages a single DummyBot who acts randomly on the environment.
/// </summary>
public class DummyBot : MonoBehaviour
{
    public Material healthyMaterial;

    public Material infectiousMaterial;

    [Tooltip("The material when the bot is recovered")]
    public Material recoveredMaterial;

    public Material vaccinatedMaterial;

    public Material expiredMaterial;    

    [HideInInspector]
    public float exposureRadius = 8f;

    [HideInInspector]
    public float infectionCoeff;

    public Vector3 SpawnPosition;

    public GameObject spawnPosList;


    public GameObject targetPosList;


    [Tooltip("The probability of exposure at that maximum distance")]
    [Range(0.0f, 0.001f)]
    public float probability; //immunity + all the factors that influence transmission

    //The PandemicArea
    private PandemicArea pandemicArea;

    //The gameObject of the Pandemic Area
    private GameObject pandemicAreaObj;

    // Speed of agent rotation.
    public float turnSpeed = 300;

    // Speed of agent movement.
    public float moveSpeed = 1;
    

    //Check if agent is frozen or not;
    public bool isFrozen = false;
    
   
    [HideInInspector]
    public float nextActionTime = -1f;
    public Vector3 origin = new Vector3(0f,0f,0f);
    private Vector3 targetPosition;
    private NavMeshAgent navMeshagent;
        


    
    public enum agentStatus
    {
        HEALTHY,
        INFECTED,
        RECOVERED,
        VACCINATED,
        EXPIRED
    }
    public float recoverTime;

    public agentStatus m_InfectionStatus = agentStatus.HEALTHY;

    public void changeAgentStatus()
    {
        switch (m_InfectionStatus)
        {
            case agentStatus.HEALTHY:
                GetComponentInChildren<Renderer>().material = healthyMaterial;
                break;


            case agentStatus.VACCINATED:
                GetComponentInChildren<Renderer>().material = vaccinatedMaterial;
                break;

            case agentStatus.INFECTED:
                GetComponentInChildren<Renderer>().material = infectiousMaterial;
                pandemicAreaObj.GetComponent<PandemicArea>().healthyCounter--;
                pandemicAreaObj.GetComponent<PandemicArea>().InfectedCounter++;
                break;

            case agentStatus.RECOVERED:
                GetComponentInChildren<Renderer>().material = recoveredMaterial;
                pandemicAreaObj.GetComponent<PandemicArea>().InfectedCounter--;
                pandemicAreaObj.GetComponent<PandemicArea>().RecoveredCounter++;               
                break;


            case agentStatus.EXPIRED:
                GetComponentInChildren<Renderer>().material = expiredMaterial;
                pandemicAreaObj.GetComponent<PandemicArea>().InfectedCounter--;
                pandemicAreaObj.GetComponent<PandemicArea>().ExpiredCounter++;               
                break;

        }
    }


    private void moveRandomTarget()
    {

        if (targetPosition == origin){
                // Pick a random target
                int target_index = Random.Range(0, 14);
                targetPosition = targetPosList.transform.GetChild(target_index).position;

                //Randomize target position
                targetPosition = new Vector3(targetPosition.x + Random.Range(-10f, 10f), targetPosition.y, targetPosition.z + Random.Range(-10f, 10f));

                // Rotate toward the target
                transform.rotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);

                navMeshagent.destination = targetPosition;    
        }

        else {

            if(Vector3.Distance(GetComponent<Rigidbody>().transform.position, targetPosition) < 1.0f) {
                 if (m_InfectionStatus == agentStatus.INFECTED) {
                    // int randomQuarantin 
                    targetPosition = SpawnPosition;
                } else {
                    // Pick a random target
                    int target_index = Random.Range(0, 14);
                    targetPosition = targetPosList.transform.GetChild(target_index).position;

                    //Randomize target position
                    targetPosition = new Vector3(targetPosition.x + Random.Range(-10f, 10f), targetPosition.y, targetPosition.z + Random.Range(-10f, 10f));

                    // Rotate toward the target
                    transform.rotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);

                    navMeshagent.destination = targetPosition;    


                }
            }
            else {
                navMeshagent.destination = targetPosition;    
            }
        }
    }


    
    private void OnTriggerEnter(Collider other)
    {
        TriggerEnterOrStay(other);
    }

    
    private void OnTriggerStay(Collider other)
    {
        TriggerEnterOrStay(other);
    }

    
    private void TriggerEnterOrStay(Collider collider)
    {
        
        //Check if our agent is healthy, otherwise there is nothing like reinfection
        if (m_InfectionStatus == agentStatus.HEALTHY)
        {
           
            //Check if its a dummyBot   
            if (collider.CompareTag("dummyBot"))
            {
                //If it is infected 
                if (collider.gameObject.GetComponent<DummyBot>().m_InfectionStatus == agentStatus.INFECTED)
                {
                    if (NotAtSpawnPosition(collider.gameObject.transform.position)){
                        exposeInfection(collider.gameObject);
                    }
                    
                }
            }
        }

    }

    private bool NotAtSpawnPosition(Vector3 currentPosition)
    {
        for (int i = 1; i < 35; i++)
        {
            if (Vector3.Distance(currentPosition, spawnPosList.transform.GetChild(i).position) < 10f)
            {
                return false;
            }
        }
        return true;      
    }



    
    private void exposeInfection(GameObject infector)
    {
        //Distance between two agents
        float distance = Vector3.Distance(infector.transform.position, transform.position);
       

        probability = Mathf.InverseLerp(exposureRadius, 0, distance) / infectionCoeff;   //Does not depend on biological factors yet

        probability = probability + Random.Range(-0.1f, 0.1f); //Add biological noise


        if (Random.Range(0f, 1f) < probability)
        {
            m_InfectionStatus = agentStatus.INFECTED;
            changeAgentStatus();
        }
    }

    private void Awake()
    {
        //Get the PandemicArea
        pandemicArea = GetComponentInParent<PandemicArea>();
        pandemicAreaObj = pandemicArea.gameObject;
        navMeshagent = GetComponent<NavMeshAgent>();
        targetPosition = pandemicArea.ChooseRandomPosition();
        GetComponent<SphereCollider>().radius = exposureRadius;
        recoverTime = pandemicArea.recoverTime;
        var randomizeRecoverTime = probability + Random.Range(-50f, 50f);
        recoverTime = recoverTime + randomizeRecoverTime;

        moveSpeed = moveSpeed + Random.Range(-0.5f, 0.5f);
        spawnPosList = GameObject.Find("SpawnArea");
        targetPosList = GameObject.Find("Targets");
        targetPosition = origin;


    }
    private void FixedUpdate()
    {
        if (!isFrozen)
        {
            moveRandomTarget();
        }
        moveRandomTarget();

        if(m_InfectionStatus == agentStatus.INFECTED)
        {

            if(recoverTime <= 0)
            {
                var probabilityOfFatality = Random.Range(-1f, 1f);

                //some infected patients die with a probability of 0.048
                if (probabilityOfFatality < -0.952f) {
                    m_InfectionStatus = agentStatus.EXPIRED;

                    //add code to remove the bots

                } else {
                    m_InfectionStatus = agentStatus.RECOVERED;
                }
                
                changeAgentStatus();
            }
            else
            {
                recoverTime -= Time.deltaTime;
            }
        }

    }
}
