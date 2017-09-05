using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_SelfDestruct : MonoBehaviour {
    public float m_fDestructDuration;
    float m_fStartTime;
	void Start () {
        m_fStartTime = Time.time;
	}
	
	void Update () {
        if (Time.time - m_fStartTime > m_fDestructDuration)
            Destroy(gameObject);
	}
}
