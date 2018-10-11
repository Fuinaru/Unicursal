using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{


    private Transform currentStartPoint;    //当前连线的起始点
    public GameObject line;
    private GameObject lineObject;         //用来保存实例化的line
    public GameObject unline;
    public GameObject star;
    // Use this for initialization
    void Start()
    {
        DrawUnLined();
    }

    // Update is called once per frame
    void Update()
    {
       if (currentStartPoint==null) MouseDownSet();
       if (currentStartPoint!= null) {
            Draw();
            MouseUpReset();  
            //鼠标滑过新点时
            if (GetPointOnMouse()!=null&& !GetPointOnMouse().Equals(currentStartPoint)) SetNext();
        }
    }


    //通过遍历点集合Points中的所有点，连接相连
    private void DrawUnLined()
    {
        foreach (Point point in Point.points) {
            foreach (Point p in point.nearPoints) {
                if (Point.points.IndexOf(point) > Point.points.IndexOf(p)) { //防止画两遍
                    Vector3 pos1 = point.transform.position;
                    Vector3 pos2 = p.transform.position;
                    float dis = (pos1 - pos2).magnitude;
                    Vector3 dir= (pos2 - pos1).normalized;
                    lineObject = Instantiate(unline);
                    lineObject.transform.position = pos1;
                    lineObject.transform.localScale = new Vector3(lineObject.transform.localScale.x, dis, lineObject.transform.localScale.z);
                    lineObject.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.x, -dir.y) * 180 / Mathf.PI);
                    GameManager.lineNum += 1;
                }
            }
        }
    }



    //鼠标点击时设置连线起始点，并将星星激活，实例化line
    private void MouseDownSet()
    {
      
            if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (GetPointOnMouse() != null) {             
                star.SetActive(true);
                SetLine();
            }
        }

    
    }

    //获取两点的距离与方向，设置当前线段，并SetLine指向新创建的线
    private void SetNext()
    {
        //当相连时才执行
        if (currentStartPoint.GetComponent<Point>().IsConnected(GetPointOnMouse().GetComponent<Point>()))
        {
            float dis = ((Vector2)GetPointOnMouse().position - (Vector2)currentStartPoint.position).magnitude;
            Vector2 dir = ((Vector2)GetPointOnMouse().position - (Vector2)currentStartPoint.position).normalized;
            lineObject.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.x, -dir.y) * 180 / Mathf.PI);
            lineObject.transform.localScale = new Vector3(lineObject.transform.localScale.x, dis, lineObject.transform.localScale.z);

            //已连接，互相移除，防止再连
            currentStartPoint.GetComponent<Point>().RemoveConnected(GetPointOnMouse().GetComponent<Point>());
            //完成一条线，计数减少
            GameManager.GM.LineComplete();
           
           SetLine();

        }
    }


    //设置连线起始点，实例化line
    private void SetLine()
    {
        currentStartPoint = GetPointOnMouse();
        //于当前点位置播放特效
        EffectManager.effectManager.PlaySoundEffectAtPos(currentStartPoint.transform.position);
        //当线画完时，不执行,并将lineObject变为null，使线固定
        if (GameManager.lineNum > 0)
        {
            lineObject = Instantiate(line);
            lineObject.transform.SetParent(currentStartPoint);
            lineObject.transform.localPosition = Vector3.zero;
        }
        else { lineObject = null; }
    }

        //鼠标左键松开时重置
        private void MouseUpReset()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            star.SetActive(false);
            //currentStartPoint = null;
            //Destroy(lineObject); 
            GameManager. RestartCurrentStage();
        }
    }
    //通过射线获取当前鼠标所在点

    private Transform GetPointOnMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hitInfo = Physics2D.Raycast(new Vector2(ray.origin.x, ray.origin.y), Vector2.zero);
        if (hitInfo)
        {
            if (hitInfo.collider.transform.tag == "Point") return hitInfo.collider.transform;
           
        }
        return null;

    }
    private void Draw()
    {

        //星星跟随
        star.transform.position = GetMousePos();

        //画线  旋转与长度
        if (lineObject != null)
        {
            lineObject.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(GetLineDir().x, -GetLineDir().y) * 180 / Mathf.PI);
            lineObject.transform.localScale = new Vector3(lineObject.transform.localScale.x, GetLineLength(), lineObject.transform.localScale.z);
        }
        }

    //计算鼠标在屏幕的坐标
    private Vector2 GetMousePos()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
        return pos;
 

    }
    //计算currentStartPoint到鼠标的距离
    private float GetLineLength() {
        if (currentStartPoint != null) return (GetMousePos() - (Vector2)currentStartPoint.position).magnitude;
        else return 0;
    }
    //计算currentStartPoint到鼠标的方向向量
    private Vector2 GetLineDir()
    {
        if (currentStartPoint != null) return (GetMousePos() - (Vector2)currentStartPoint.position).normalized;
        else return Vector2.zero;
    }


}