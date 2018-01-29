using UnityEngine;
//using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManagerScript : MonoBehaviour
{

    public List<GameObject> doors = new List<GameObject>();
    public GameObject[] prefab;
    public List<GameObject> Players = new List<GameObject>();
    private float movementSpeed = 3f;
    private pathScript pathScript;
    private pathScript targetScript;
    private dataScript dScript;
    int numRows;
    bool successfulPathing;
    bool jumping;

    System.Random rand = new System.Random();

    // Use this for initialization
    void Start()
    {
        jumping = false;
        dScript = GameObject.FindGameObjectWithTag("Game").GetComponent<dataScript>();
        successfulPathing = false;
        for (int i = 0; i < 4 - dScript.level; i++)
        {
            if (dScript.level == 0)
            {
                GameObject c = (GameObject)Instantiate(prefab[i], new Vector3(-5 + i, -3.3f, -4), Quaternion.identity);
                c.tag = "Player" + (i + 1);
                Players.Add(c);
                for (int i1 = 1; i1 <= i; i1++)
                    Physics.IgnoreCollision(Players[i].GetComponent<Collider>(), Players[i - i1].GetComponent<Collider>());
            }
            else
            {
                GameObject c = dScript.nextPlayers[i];
                Players.Add(c);
            }
        }
        //dScript.nextPlayers.RemoveRange(0, dScript.nextPlayers.Count);
        doors[doors.Count - 1].tag = "exit";
        pathScript = GameObject.FindGameObjectWithTag("exit").GetComponent<pathScript>();
        numRows = pathScript.row;

        for (int i = 0; i < doors.Count - 1; i++)
        {
            doors[i].tag = "Door" + (i + 1);
        }


        for (int i = 0; i < doors.Count - 1; i++)
        {
            makePath(i);
        }


        if (!successfulPathing)
        {
            string door1 = " ";
            string door2 = " ";
            for (int i = 0; i < doors.Count - 1; i++)
            {
                pathScript = GameObject.FindGameObjectWithTag(doors[i].tag).GetComponent<pathScript>();
                if (pathScript.target == " " && door1 == " ")
                    door1 = pathScript.tag;
                else if (pathScript.target == " " && door2 == " ")
                    door2 = pathScript.tag;
            }
            pathScript = GameObject.FindGameObjectWithTag(door1).GetComponent<pathScript>();
            pathScript.target = door2;
            pathScript = GameObject.FindGameObjectWithTag(door2).GetComponent<pathScript>();
            pathScript.target = door1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !jumping)
        {
            float[] tempY = new float[Players.Count];
            for (int i = 0; i < Players.Count; i++)
            {
                tempY[i] = Players[i].transform.position.y;
                Players[i].transform.Translate(0, 60 * Time.deltaTime, 0);
            }       
        }

        if (Input.GetKey("a") && !Input.GetKey("d"))
            Players[0].transform.Translate((movementSpeed * -1) * Time.deltaTime, 0, 0);
        else if (Input.GetKey("d") && !Input.GetKey("a"))
            Players[0].transform.Translate(movementSpeed * Time.deltaTime, 0, 0);
        if (Input.GetKey("j") && !Input.GetKey("l"))
            Players[1].transform.Translate((movementSpeed * -1) * Time.deltaTime, 0, 0);
        else if (Input.GetKey("l") && !Input.GetKey("j"))
            Players[1].transform.Translate(movementSpeed * Time.deltaTime, 0, 0);

        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            Players[2].transform.Translate((movementSpeed * -1) * Time.deltaTime, 0, 0);
        else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
            Players[2].transform.Translate(movementSpeed * Time.deltaTime, 0, 0);

        if (Input.GetKey(KeyCode.Keypad4) && !Input.GetKey(KeyCode.Keypad6))
            Players[3].transform.Translate((movementSpeed * -1) * Time.deltaTime, 0, 0);
        else if (Input.GetKey(KeyCode.Keypad6) && !Input.GetKey(KeyCode.Keypad4))
            Players[3].transform.Translate(movementSpeed * Time.deltaTime, 0, 0);

        warpLocation();
    }

    void makePath(int n)
    {
        pathScript = GameObject.FindGameObjectWithTag(doors[n].tag).GetComponent<pathScript>();
        if (pathScript.target != " ")
            return;
        string targetDoor = " ";
        long x = 0;
        int randRow = 0;
        int randCol = 0;
        while (true)
        {
            randRow = rand.Next(1, numRows + 1);
            randCol = rand.Next(1, 3 + 1);

            if (pathScript.row == 1 && randRow != 1 && randRow != numRows) // Current door is in row one, makes sure 
            {
                if (randRow == 3 && randCol == 3)
                    continue;
                else
                {
                    for (int i = 0; i < doors.Count - 1; i++)
                    {
                        targetScript = GameObject.FindGameObjectWithTag(doors[i].tag).GetComponent<pathScript>();
                        if (randRow == targetScript.row && randCol == targetScript.col && targetScript.target == " ")
                            targetDoor = targetScript.tag;
                    }
                }
            }
            else if (pathScript.row != 1 && pathScript.row != randRow)
            {
                if ((randRow == 3 && randCol == 3) || (randRow == 5 && randCol != 1))
                    continue;
                else
                {
                    for (int i = 0; i < doors.Count - 1; i++)
                    {
                        targetScript = GameObject.FindGameObjectWithTag(doors[i].tag).GetComponent<pathScript>();
                        if (randRow == targetScript.row && randCol == targetScript.col && targetScript.target == " ")
                            targetDoor = targetScript.tag;
                    }
                }

            }

            x++;
            //Debug.Log(randRow + " " + randCol);
            if (x == 5000)
            {
                successfulPathing = false;
                return;
            }
            if (targetDoor != " ")
                break;
        }

        pathScript.target = targetDoor;
        targetScript = GameObject.FindGameObjectWithTag(targetDoor).GetComponent<pathScript>();
        targetScript.target = doors[n].tag;

        //Debug.Log("Door: " + doors[n].tag + " Target: " + pathScript.target);

        successfulPathing = true;
    }

    void warpLocation()
    {
        for (int i = 0; i < doors.Count; i++)
        {
            if (Input.GetKeyDown("w"))
            {
                if (Players[0].transform.position.x < doors[i].transform.position.x + .4f && Players[0].transform.position.x > doors[i].transform.position.x - .4f && Players[0].transform.position.y < doors[i].transform.position.y && Players[0].transform.position.y > doors[i].transform.position.y - 1 && doors[i].tag != "exit")
                {
                    pathScript = GameObject.FindGameObjectWithTag(doors[i].tag).GetComponent<pathScript>();
                    targetScript = GameObject.FindGameObjectWithTag(pathScript.target).GetComponent<pathScript>();
                    Players[0].transform.position = new Vector3(targetScript.transform.position.x, targetScript.transform.position.y - .3f, Players[0].transform.position.z);
                    Debug.Log("Warped!");
                    break;
                }
                else if (Players[0].transform.position.x < doors[doors.Count - 1].transform.position.x + .4f && Players[0].transform.position.x > doors[doors.Count - 1].transform.position.x - .4f && Players[0].transform.position.y < doors[doors.Count - 1].transform.position.y && Players[0].transform.position.y > doors[doors.Count - 1].transform.position.y - 1)
                {
                    dScript.nextPlayers.Add(Players[0]);
                    Players[0].transform.Translate(0, 3.4f, 4);
                    Players[0].AddComponent<advanceScript>();
                    dScript.playersAdv++;
                    break;
                }
            }
            if (Input.GetKeyDown("i"))
            {
                if (Players[1].transform.position.x < doors[i].transform.position.x + .4f && Players[1].transform.position.x > doors[i].transform.position.x - .4f && Players[1].transform.position.y < doors[i].transform.position.y && Players[1].transform.position.y > doors[i].transform.position.y - 1 && doors[i].tag != "exit")
                {
                    pathScript = GameObject.FindGameObjectWithTag(doors[i].tag).GetComponent<pathScript>();
                    targetScript = GameObject.FindGameObjectWithTag(pathScript.target).GetComponent<pathScript>();
                    Players[1].transform.position = new Vector3(targetScript.transform.position.x, targetScript.transform.position.y - .3f, Players[1].transform.position.z);
                    break;
                }
                else if (Players[1].transform.position.x < doors[doors.Count - 1].transform.position.x && Players[1].transform.position.x > doors[doors.Count - 1].transform.position.x - 1 && Players[1].transform.position.y < doors[doors.Count - 1].transform.position.y && Players[1].transform.position.y > doors[doors.Count - 1].transform.position.y - 1)
                {
                    dScript.nextPlayers.Add(Players[1]);
                    Players[1].transform.Translate(0, 3.4f, 4);
                    Players[1].AddComponent<advanceScript>();
                    dScript.playersAdv++;
                    break;
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (Players[2].transform.position.x < doors[i].transform.position.x + .4f && Players[2].transform.position.x > doors[i].transform.position.x - .4f && Players[2].transform.position.y < doors[i].transform.position.y && Players[2].transform.position.y > doors[i].transform.position.y - 1 && doors[i].tag != "exit")
                {
                    pathScript = GameObject.FindGameObjectWithTag(doors[i].tag).GetComponent<pathScript>();
                    targetScript = GameObject.FindGameObjectWithTag(pathScript.target).GetComponent<pathScript>();
                    Players[2].transform.position = new Vector3(targetScript.transform.position.x, targetScript.transform.position.y - .3f, Players[2].transform.position.z);
                    break;
                }
                else if (Players[2].transform.position.x < doors[doors.Count - 1].transform.position.x && Players[2].transform.position.x > doors[doors.Count - 1].transform.position.x - 1 && Players[2].transform.position.y < doors[doors.Count - 1].transform.position.y && Players[2].transform.position.y > doors[doors.Count - 1].transform.position.y - 1)
                {
                    dScript.nextPlayers.Add(Players[2]);
                    Players[2].transform.Translate(0, 3.4f, 4);
                    Players[2].AddComponent<advanceScript>();
                    dScript.playersAdv++;
                    break;
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                if (Players[3].transform.position.x < doors[i].transform.position.x + .4f && Players[3].transform.position.x > doors[i].transform.position.x - .4f && Players[3].transform.position.y < doors[i].transform.position.y && Players[3].transform.position.y > doors[i].transform.position.y - 1 && doors[i].tag != "exit")
                {
                    pathScript = GameObject.FindGameObjectWithTag(doors[i].tag).GetComponent<pathScript>();
                    targetScript = GameObject.FindGameObjectWithTag(pathScript.target).GetComponent<pathScript>();
                    Players[3].transform.position = new Vector3(targetScript.transform.position.x, targetScript.transform.position.y - .3f, Players[3].transform.position.z);
                    break;
                }
                else if (Players[3].transform.position.x < doors[doors.Count - 1].transform.position.x && Players[3].transform.position.x > doors[doors.Count - 1].transform.position.x - 1 && Players[3].transform.position.y < doors[doors.Count - 1].transform.position.y && Players[3].transform.position.y > doors[doors.Count - 1].transform.position.y - 1)
                {
                    dScript.nextPlayers.Add(Players[3]);
                    Players[3].transform.Translate(0, 3.4f, 4);
                    Players[3].AddComponent<advanceScript>();
                    dScript.playersAdv++;
                    break;
                }
            }
        }
    }
}

