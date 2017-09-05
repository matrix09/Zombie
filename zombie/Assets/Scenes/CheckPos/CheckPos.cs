using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPos : MonoBehaviour {

    public GameObject Cactus;
    GameObject m_oMonster;
    SkinnedMeshRenderer m_srm;
    BoxCollider bc;
	// Use this for initialization
	void Start () {
        m_oMonster = Instantiate(Cactus) as GameObject;
        m_oMonster.transform.localScale = new Vector3(4, 4, 4);
        m_srm = m_oMonster.GetComponentInChildren<SkinnedMeshRenderer>();
        bc = m_oMonster.GetComponent<BoxCollider>();
	}

    Vector3 MonsterPos;
    Vector3 PointPos;
	// Update is called once per frame
	void Update () {


        MonsterPos = m_oMonster.transform.position;

        if (Input.GetMouseButtonDown(0))
        {
            PointPos = Camera.main.ScreenToWorldPoint(
                    new Vector3(
                            Input.mousePosition.x,
                            Input.mousePosition.y,
                            MonsterPos.z - Camera.main.transform.position.z
                        )
                );
            //Debug.Log(m_srm.bounds.size);
            //Debug.Log(m_srm.localBounds.size);
            Debug.Log(bc.size);
            Debug.Log(bc.size);
            Debug.Log("MonsterPos.x - PointPos.x = " + Mathf.Abs(MonsterPos.x - PointPos.x));
            Debug.Log("MonsterPos.y - PointPos.y = " + Mathf.Abs(MonsterPos.y - PointPos.y));

            if (
                (Mathf.Abs(MonsterPos.x - PointPos.x) >= 0 && Mathf.Abs(MonsterPos.x - PointPos.x) <= bc.size.x/2f * m_oMonster.transform.localScale.x)
                &&
                 (Mathf.Abs(MonsterPos.y - PointPos.y) >= 0 && Mathf.Abs(MonsterPos.y - PointPos.y) <= bc.size.y/2f * m_oMonster.transform.localScale.y)
                )
            {
                Debug.Log("Point out");
            }
        }


	}
}
