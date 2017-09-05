using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour {

    SphereCollider sc;
	// Use this for initialization
	void Start () {
        tag = "Knife";
        sc = gameObject.GetComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.enabled = false;
	}

    public static KnifeController CreateKnife()
    {
        UnityEngine.Object obj = Resources.Load("Prefabs/KnifeController");
        GameObject _obj = Instantiate(obj) as GameObject;
        _obj.transform.position = new Vector3(0,0,0);
        _obj.transform.rotation = Quaternion.identity;
        _obj.transform.localScale = Vector3.one;

        KnifeController kc = _obj.GetComponent<KnifeController>();
        if(null == kc)
            kc = _obj.AddComponent<KnifeController>();

        return kc;

    }


	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            sc.enabled = true;
            transform.position = Camera.main.ScreenToWorldPoint(
                    new Vector3(
                            Input.mousePosition.x,
                            Input.mousePosition.y,
                            transform.position.z - Camera.main.transform.position.z                        
                        )
                );
        }
        else if (Input.GetMouseButton(0))
        {
            transform.position = Camera.main.ScreenToWorldPoint(
                    new Vector3(
                            Input.mousePosition.x,
                            Input.mousePosition.y,
                             transform.position.z - Camera.main.transform.position.z
                        )
                );
        }
        else if (Input.GetMouseButtonUp(0))
        {
            sc.enabled = false;
        }

	}


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);

    }
#endif

}
