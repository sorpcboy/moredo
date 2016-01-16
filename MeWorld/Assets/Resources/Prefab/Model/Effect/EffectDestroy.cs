using UnityEngine;
using System.Collections;

public class EffectDestroy : MonoBehaviour 
{
    public float time;
	void Start () 
    {
        Destroy(gameObject,time);
	}
}
