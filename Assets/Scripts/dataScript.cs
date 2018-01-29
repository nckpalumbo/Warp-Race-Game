using UnityEngine;
//using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class dataScript : MonoBehaviour {

    public int level;
    public int playersAdv = 0;
    public List<GameObject> nextPlayers = new List<GameObject>();
    private GameManagerScript gameManager;
	// Use this for initialization
	void Start () {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerScript>();
        DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
	    switch(level)
        {
            case 0: 
                if(playersAdv == 4)
                {
                    gameManager.Players.RemoveRange(0, gameManager.Players.Count);
                    //SceneManager.LoadScene(1);
                    Debug.Log("Next player size (before adding): " + nextPlayers.Count);
                    for (int i = 0; i < nextPlayers.Count; i++)
                    {
                        nextPlayers[i].transform.position = new Vector3(-5 + i, -3.3f, -4);
                        Debug.Log("Adding " + nextPlayers[i].tag);
                        //Destroy(nextPlayers[i].GetComponent<advanceScript>());
                    }
                    Debug.Log("Next Player [2] tag: " + nextPlayers[2].tag);
                    playersAdv = 0;
                    level = 1;
                }
                break;
            case 1:             
                if(playersAdv == 2)
                {
                    //SceneManager.LoadScene(2);
                    playersAdv = 0;
                }
                break;
            case 2:
                if(playersAdv == 1)
                {
                    //SceneManager.LoadScene(3);
                    playersAdv = 0;
                }
                break;
        }
	}
}
