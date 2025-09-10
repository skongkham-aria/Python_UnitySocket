using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandTracking : MonoBehaviour
{
    public UDPReceive udpReceive;
    public GameObject[] handpoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        string data = udpReceive.data;
        if (string.IsNullOrEmpty(data)) return;

        // Remove brackets
        data = data.Trim('[', ']');
        string[] points = data.Split(',');

        if (points.Length < 63) return; // 21 points * 3 coords

        for (int i = 0; i < 21; i++)
        {
            float x = 5- float.Parse(points[i * 3]) / 70f; // Adjust divisor for your scene
            float y = float.Parse(points[i * 3 + 1]) / 70f;
            float z = float.Parse(points[i * 3 + 2]) / 70f;

            handpoints[i].transform.localPosition = new Vector3(x, y, z);
        }
        //string data = udpReceive.data;
        ////print(data);
        //// Remove outer array brackets to get the hand landmarks array
        //data = data.Remove(0, 1);
        //data =  data.Remove(data.Length-1, 1);
        //print(data);
        //string[] points = data.Split(',');
        //print(points[0]);


        //////for (int i = 0; i < handpoints.Length && i < landmarks.Count; i++)
        //for (int i = 0; i < 21; i++)
        //{
        //    float x = float.Parse(points[i * 3])/100; // always take the x index which is in multiples of 3
        //    float y = float.Parse(points[i * 3 + 1])/100;
        //    float z = float.Parse(points[i * 3 + 2])/100;

        //    // Optionally scale or offset as needed for your scene
        //    handpoints[i].transform.localPosition = new Vector3(x, y, z);
        //}
    }
}
