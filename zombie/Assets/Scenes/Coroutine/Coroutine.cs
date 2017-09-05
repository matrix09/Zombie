//测试协程
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coroutine : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(WaitFor());
	}

    IEnumerator WaitFor()
    {
        Debug.Log("Before");
        yield return null;
        Debug.Log("After");
    }

	void Update () {
        Debug.Log("Update");	
	}
}
