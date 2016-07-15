/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using System.Collections;

public class ThirdPlayerCameraControl : MonoBehaviour {

	// Add public variables here:
    public float minAngle = 45;
    public float maxAngle = 45;
    public float minFollowDistance = 1;
    public float maxFollowDistance = 9;
	
	// Add private members here:
    private Vector3 mouseDeltaPosition;
    private Transform sightPoint;
    private Ray cameraRay = new Ray();
    private RaycastHit cameraRayHit = new RaycastHit();
    private float cameraCurrentDistance = 9;

    // Add member functions here:
    #region 相机跟随鼠标上下旋转
    void FollowMouseRotation()
    {

        mouseDeltaPosition = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

        // 更新相机位置
        {
            //Quaternion rotation = Quaternion.AngleAxis(-mouseDeltaPosition.y, 
            sightPoint.Rotate(-mouseDeltaPosition.y, 0, 0);
            //sightPoint.transform.rotation = Quaternion.Euler(new Vector3(sightPoint.transform.rotation.eulerAngles.x, sightPoint.transform.rotation.eulerAngles.y, 0));

            // 限制最大最小角度
            float limitAngle = sightPoint.transform.rotation.eulerAngles.x;
            if (limitAngle > 0 + minAngle && limitAngle < 180)
                limitAngle = 0 + minAngle;
            else if (limitAngle > 180 && limitAngle < 360 - maxAngle)
                limitAngle = 360 - maxAngle;
            sightPoint.transform.rotation = Quaternion.Euler(new Vector3(limitAngle, sightPoint.transform.rotation.eulerAngles.y, 0));
        }

    }
    #endregion

    // Use this for initialization
	void Start () {
        mouseDeltaPosition = new Vector3(0, 0, 0);
        sightPoint = transform.parent;
        cameraCurrentDistance = maxFollowDistance;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (GameControl.ESCGameMenu == false)
            FollowMouseRotation();


        // 摄像机距离
        if (Input.GetAxis("Mouse ScrollWheel") != 0f && GameControl.ESCGameMenu == false)
        {
            Vector3 pos = transform.localPosition + new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * 2.1f);
            float limitZ = Mathf.Clamp(pos.z, -maxFollowDistance, -minFollowDistance);
            pos.z = limitZ;
            transform.localPosition = pos;
            cameraCurrentDistance = -limitZ;
        }

        // 调整室内摄像机
        cameraRay.origin = sightPoint.position;
        cameraRay.direction = sightPoint.TransformVector(new Vector3(0, 0, -cameraCurrentDistance));
        //Debug.Log(cameraRay.origin);
        //Debug.Log(cameraRay.direction);
        Debug.DrawLine(sightPoint.position, sightPoint.TransformPoint(new Vector3(0, 0, -cameraCurrentDistance)));
        if (Physics.Raycast(cameraRay, out cameraRayHit, 100, 1 << 8))
        {
            if ((cameraRayHit.point - sightPoint.position).magnitude <= cameraCurrentDistance)
                transform.position = cameraRayHit.point;
            else
                transform.localPosition = new Vector3(0, 0, -cameraCurrentDistance);
        }
        else
        {
            transform.localPosition = new Vector3(0, 0, -cameraCurrentDistance);
        }

	}

    void FixedUpdate()
    {
        
    }
}
