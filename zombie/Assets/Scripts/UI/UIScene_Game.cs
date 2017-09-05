using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScene_Game : MonoBehaviour
{
    
    #region 击中怪兽
    public void HitMonster(int num)
    {
        m_tsLabelScore.ResetToBeginning();
        m_tsLabelScore.enabled = true;
        m_nCurScore += num;
        m_labelScore.text = m_nCurScore.ToString();
    }
    #endregion

    #region Miss monster
    int m_nInitLifeNun = 3;
    public void MissMonster()
    {
        m_nInitLifeNun--;
        switch (m_nInitLifeNun)
        {
            case 0:
                {
                    m_uiLife[2].enabled = false;
                    LeftGame();
                    break;
                }
            case 1:
                {
                    m_uiLife[1].enabled = false;
                    break;
                }
            case 2:
                {
                    m_uiLife[0].enabled = false;
                    break;
                }
        }
    }
    #endregion

    #region 系统接口
    void Awake()
    {
        m_objGameOver.SetActive(false);
    }
    void Start () {
        m_nInitLifeNun = 3;
        UIEventListener.Get(m_objLeaveGame).onClick = PressLeaveGame;
        m_nCurTime = m_nTotalTime;
        StartCoroutine(CoutingTime());
	}

    void OnEnable()
    {
        m_nInitLifeNun = 3;
    }
	
    #endregion

    #region 生命值
    public UISprite[] m_uiLife;
    #endregion

    #region 得分动画
    public UILabel m_labelScore;
    public TweenScale m_tsLabelScore;
    int m_nCurScore = 0;

    #endregion

    #region 离开游戏
    public GameObject m_objLeaveGame;
    public GameObject m_objGameOver;
    public UILabel m_uiTotalScore;
    void LeftGame()
    {
        Time.timeScale = 0f;

        m_objGameOver.SetActive(true);
        m_uiTotalScore.text = "Total Score : " + m_nCurScore.ToString();
    }
    void PressLeaveGame(GameObject obj)
    {
        Application.Quit();
    }

    #endregion

    #region 计时器
    const int m_nTotalTime = 60;
    private int m_nCurTime;
    public UILabel m_labelTimeCounter;
    IEnumerator CoutingTime()
    {
        string leftTime;
        while (m_nCurTime > 0)
        {
            yield return new WaitForSeconds(1f);
            m_nCurTime--;
            if(m_nCurTime < 10) {
                  leftTime = "0" + m_nCurTime.ToString();
            }
            else 
                leftTime = m_nCurTime.ToString();

            m_labelTimeCounter.text = "00 : " + leftTime;
        }

        LeftGame();
      
    }
    #endregion

}
