using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    
    public List<Objective> objectives;
    public int curObjIdx;  // Index of the current objective

    public float time;
    public Text timerTxt;

    // Dictionary<string, Dictionary<string, int>> items = new Dictionary<string, Dictionary<string, int>>();

    public bool Finished()
    {
        return curObjIdx == objectives.Count;
    }

    public bool IsOnLastObjective()
    {
        return curObjIdx == objectives.Count - 1;
    }
    
    private void Awake() 
    {
        instance = this;

        // Dictionary<string, int> boltAdic = new Dictionary<string, int>();
        
        // boltAdic.Add("total", 3);
        // boltAdic.Add("putInBox", 0);
        // items.Add("BoltA", boltAdic);

        // TODO: Replace dir
        IDictionary<string, string[]> info = new Dictionary<string, string[]>();
        string[] lines = System.IO.File.ReadAllLines(@"C:/Users/diana/Desktop/FirstOne.json");
        foreach (string line in lines)
        {
            string[] words = line.Split('"');
            string[] newInfo = new string[] { Char.ToString(words[6][1]), words[9] };
            info[words[3]] = newInfo;
        }


        objectives = new List<Objective>();
        //objectives.Add(new Objective("BoltA", 3, "x"));
        //objectives.Add(new Objective("NutA", 2, "y"));

        foreach (KeyValuePair<string, string[]> kvp in info)
        {
            Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, String.Join(" - ", kvp.Value));
            Debug.Log($"------------->: {kvp.Key}");
            Debug.Log($"------------->: {kvp.Value[0]}");
            Debug.Log($"------------->: {kvp.Value[1]}");
            objectives.Add(new Objective(kvp.Key, Int32.Parse(kvp.Value[0]), kvp.Value[1]));
        }

        curObjIdx = 0;

        Debug.Log($"objectives length: {objectives.Count}");
        Debug.Log($"Current objective: {objectives[curObjIdx]}");

        time = 0;
    }

    public void HandleObjInBox(GameObject obj) 
    {

        // if (obj.tag == "BoltA")
        // {
        //     items["BoltA"]["putInBox"]++;
        //     Debug.Log("BoltA: " + items["BoltA"]["putInBox"] + "/" + items["BoltA"]["total"]);
        // }
        // Destroy(obj);
        Objective curObj = objectives[curObjIdx];
        if (obj.tag == curObj.item)
        {
            curObj.AddOne();
            Debug.Log(curObj);

            if (curObj.Done())
            {
                Debug.Log($"Objective #{curObjIdx} done.");
                curObjIdx++;

                if (Finished())
                {
                    Debug.Log("Finished.");
                }
                else 
                {
                    Debug.Log($"New objective: {objectives[curObjIdx]}");
                }
                
            }
        }
    }

    public string CurrentObjectiveText()
    {
        return $"Current Objective\n{objectives[curObjIdx].ToString()}";
    }

    public string NextObjectivesText() 
    {
        StringBuilder sb = new StringBuilder("");
        if (!IsOnLastObjective())
        {
            sb.Append("Next Objectives");
            for (int i = curObjIdx + 1; i < Math.Min(objectives.Count, curObjIdx + 4); i++)
                sb.Append("\n").Append(objectives[i].ToString());
        }
        return sb.ToString();
    }

    public string ObjectiveText()
    {
        return Finished() ? "Finished!" : $"{CurrentObjectiveText()}\n{NextObjectivesText()}";
    }

    void Update() 
    {
        if (!Finished())
            UpdateTime();
    }

    private void UpdateTime()
    {
        time += Time.deltaTime;
        int mins = (int) time / 60;
        int secs = (int) (time % 60);
        timerTxt.text = $"{mins.ToString("D2")}:{secs.ToString("D2")}";
    }
}
