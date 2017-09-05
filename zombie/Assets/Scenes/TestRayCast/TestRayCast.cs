using UnityEngine;
public class TestRayCast : MonoBehaviour {

    #region 系统接口
    void Update () {
		
        if(Input.GetMouseButtonDown (0) || Input.GetMouseButton(0))
        {
            RayCast(Input.mousePosition);
        }

	}
    #endregion

    #region 自定义接口
    void RayCast (Vector3 inPos){
        //获得从相机到inPos的射线
        Ray ray = Camera.main.ScreenPointToRay(inPos);
        //射线的距离
        float dist = Camera.main.farClipPlane - Camera.main.nearClipPlane;
        //射线碰撞的目标单位
        int mask = 1 << LayerMask.NameToLayer("Monster");
        //返回的碰撞信息
        RaycastHit hitInfo;
        //检测碰撞
        if (Physics.Raycast(ray, out hitInfo, dist, mask)){
            //绘制射线
            Debug.DrawLine(Camera.main.transform.position, hitInfo.point, Color.blue);
        }
    }
    #endregion

}
