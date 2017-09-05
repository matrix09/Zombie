using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StartUp : MonoBehaviour {

	// Use this for initialization
    void Start () {
        //加载UI
        BasePlayer.UIMgr.UI<UIScene_Login>();
        BasePlayer.UIMgr.UI<UIScene_Back>();

        Destroy(gameObject);	
	}
}
