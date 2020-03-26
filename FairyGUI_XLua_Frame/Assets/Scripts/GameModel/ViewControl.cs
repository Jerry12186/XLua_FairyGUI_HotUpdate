using UnityEngine;

public class ViewControl : MonoBehaviour
{
    public float moveSpeed;

    public float leftBoundX;
    public float topBoundY;
    public float rightBoundX;
    public float bottomBoundY;

    public float minDis;
    public float maxDis;
    public float scaleSpeed;

    private bool isMouseDown;
    private Vector3 lastMousePosition = Vector3.zero;
    private Vector3 lastPos = Vector3.zero;
    private Vector3 offset;
    private float preDistance = 0;

    private void Update()
    {
        TwoDMove();
        ScaleView();
        #region 界限控制
        if (transform.position.x > leftBoundX
            || transform.position.x < rightBoundX
            || transform.position.y < topBoundY
            || transform.position.y > bottomBoundY)
        {
            transform.position = lastPos;
        }
        //if (transform.position.y < topBoundY || transform.position.y > bottomBoundY) return;
        else
        {
            lastPos = transform.position;
        }
        #endregion
        //点击
        ClickedEvent();
    }

    /// <summary>
    /// 缩放视角
    /// </summary>
    private void ScaleView()
    {
#if UNITY_EDITOR
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.fieldOfView <= maxDis)
                Camera.main.fieldOfView += scaleSpeed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.fieldOfView >= minDis)
                Camera.main.fieldOfView -= scaleSpeed;
        }
#else
        //移动端的缩放
        if (Input.touchCount == 2
            && Input.touches[0].phase != TouchPhase.Stationary
            && Input.touches[0].phase != TouchPhase.Stationary)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved
                && Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                float tempDistance = preDistance;

                preDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                if ((preDistance - tempDistance) > 0f)
                {
                    if (Camera.main.fieldOfView >= minDis)
                        Camera.main.fieldOfView -= scaleSpeed;
                }
                else
                {
                    if (Camera.main.fieldOfView <= maxDis)
                        Camera.main.fieldOfView += scaleSpeed;
                }
            }
        }
#endif
    }
    /// <summary>
    /// 移动视角
    /// </summary>
    private void TwoDMove()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            lastMousePosition = Vector3.zero;//这里要归零，不然会有漂移效果
        }
        if (isMouseDown)
        {
            if (lastMousePosition != Vector3.zero)
            {
                offset = Camera.main.ScreenToViewportPoint(Input.mousePosition - lastMousePosition);
                transform.position += offset * moveSpeed;
            }
            lastMousePosition = Input.mousePosition;
        }
#else
///移动端的移动
        if (Input.touchCount == 1)
        {
            if (Input.touches[0].phase == TouchPhase.Moved
                && Input.touches[0].phase != TouchPhase.Stationary)
            {
                offset = Camera.main.ScreenToViewportPoint(Input.touches[0].deltaPosition);
                transform.position += offset * moveSpeed;
            }
        }
#endif
    }

    /// <summary>
    /// 检测玩家点击的东西
    /// </summary>
    private void ClickedEvent()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                Debug.Log(hitInfo.collider.name);
            }
        }
    }
}