using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {

    
    public static List<Point> points = new List<Point>();  //设置静态变量用于存放所有的点
    public  List<Point> nearPoints = new List<Point>();    //用于存放相连的点
    // Use this for initialization
    void Awake () {
        points.Add(this);                                  //将自身加入points所有点的集合
    }
    //判断两个点是否相连
    public bool IsConnected(Point point) {
        if (nearPoints.Contains(point) && point.nearPoints.Contains(this)) return true;
        return false;
    }
    //连线完成后互相去除，防止再连线
    public void RemoveConnected(Point point) {
        point.nearPoints.Remove(this);
        nearPoints.Remove(point);
    }


}
