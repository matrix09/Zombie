using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScene_LoginV1 : MonoBehaviour {

    //登录按钮游戏对象
    public GameObject m_oLoginBtn;

	// Use this for initialization
	void Start () {
        UIEventListener.Get(m_oLoginBtn).onClick = PressLogin;
	}

    void PressLogin(GameObject obj)
    {
        Debug.Log("Press Login");
    }
	// Update is called once per frame
	void Update () {
		
	}
}
