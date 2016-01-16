using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
    float i = 0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void LateUpdate()
    {
        if (i >= 2)
        {
            i = 0;
            transform.Rotate(new Vector3(0, 1, 0), 2);
        }
        i++;
    }
}
