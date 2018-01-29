using UnityEngine;
using System.Collections;

public class pathScript : MonoBehaviour {

    public int row;
    public int col;
    public string target;
	// Use this for initialization
	void Start () {
        if (this.gameObject.tag == "exit")
            target = "exit";
        else
            target = " ";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
