using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttTypeDefine;
public class UIScene_Login : MonoBehaviour {

    public GameObject m_oLogin;
    StartUp m_startUpScript;

	// Use this for initialization
	void Start () {
        UIEventListener.Get(m_oLogin).onClick = PressLogin;
	}

    void PressLogin(GameObject _obj)
    {

        //销毁UI
        Destroy(gameObject);

        //加载Game UI
        UIScene_Game ui_game = BasePlayer.UIMgr.UI<UIScene_Game>();

        #region 做了三个发射器
        float height = Camera.main.orthographicSize / 2f;
        //启动发射器
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = new Vector3(
                    Camera.main.transform.position.x,
                    (Camera.main.transform.position.y + Camera.main.orthographicSize - (i + 1) * (height)),
                    0f
                );

            GameObject obj = Instantiate(Resources.Load("Controller/MonsterLauncher")) as GameObject;
            obj.transform.position = pos;
            MonsterLauncher ml = obj.GetComponent<MonsterLauncher>();
            ml.OnStart(ui_game);
        }
        #endregion

        //播放声音
        AudioManager.PlayAudio(null, eAudioType.Audio_BackGround, "BackGround", 1, true);

        //加载刀片
        KnifeController.CreateKnife();

    }

}
