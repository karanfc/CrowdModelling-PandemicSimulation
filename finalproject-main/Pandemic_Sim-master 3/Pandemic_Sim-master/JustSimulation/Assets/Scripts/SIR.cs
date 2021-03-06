using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SIR : MonoBehaviour
{
    private Text text;
    private PandemicArea pandemicArea;

    // Start is called before the first frame update
    void Start()
    {
        pandemicArea = GetComponentInParent<PandemicArea>();
        text = GetComponent<Text>();      
    }

    // Update is called once per frame
    void LateUpdate()
    {
        text.text = "Total Healthy Agents = " + pandemicArea.healthyCounter + "\n" +
                    "Total Infected Agents = " + pandemicArea.infectedCounter + "\n"+
                    "Total Recovered Agents = " + pandemicArea.recoveredCounter + "\n" +
                    "Total Agents who have expired = " + pandemicArea.expiredCounter + "\n"+
                    "Total Vaccinated Agents = " + pandemicArea.vaccinatedCounter + "\n";
        
    }
}
