using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShader : MonoBehaviour {

    public GameObject player;
    GameObject m_objPlayer;
    Shader m_sOrig;
    SkinnedMeshRenderer m_smr;
    Shader m_sDeadShader = null;
	// Use this for initialization
	void Start () {

        m_objPlayer = Instantiate(player) as GameObject;
        m_smr = m_objPlayer.GetComponentInChildren<SkinnedMeshRenderer>();
        m_sOrig = m_smr.materials[0].shader;
        m_objPlayer.transform.Rotate(Vector3.up, 180f);
	}
	
    float alpha = 0.6f;
    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200f, 200f), "Change Shader"))
        {
            if (null == m_sDeadShader)
            {
                m_sDeadShader = Shader.Find("Custom/SimpleAlpha");
            }
            m_smr.materials[0].shader = m_sDeadShader;
            StartCoroutine(WaitingForAlpha());
        }
    }

    IEnumerator WaitingForAlpha()
    {
        while ( alpha > 0.0f) {
            m_smr.materials[0].SetFloat("_TransVal", alpha);
            yield return null;
            alpha -= Time.deltaTime * 0.5f;
        }
    }




}
