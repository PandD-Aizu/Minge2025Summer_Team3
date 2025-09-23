using UnityEngine;
using UnityEngine.AI;

public class RouteJudge : MonoBehaviour
{
    [SerializeField] private GameObject SearchMarker; // 探索するマーカー
    [SerializeField] private GameObject[] Marker; //探索されるマーカー群
    private bool judge = false; //良いマップかどうかの判定
    private int markerNum = 0;

    private NavMeshPath path;
    private float timer = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        path = new NavMeshPath();

        //全てのマーカーに到達可能か調べる
        for (markerNum = 0; markerNum < Marker.Length; markerNum++)
        {
            if (NavMesh.CalculatePath(SearchMarker.transform.position, Marker[markerNum].transform.position, NavMesh.AllAreas, path))
            {
                Debug.Log("Can go to Marker" + markerNum);
                judge = true;
            }
            else
            {
                Debug.Log("Cannot go to Marker" + markerNum);
                judge = false;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //到達可能な場合の処理
        if (markerNum == Marker.Length && judge == true)
        {
            Debug.Log("goodMap");
        }

        //到達不可能な場合の処理
        timer += Time.deltaTime;
        if (timer > 0.3f && judge == false)
        {
            Debug.Log("badMap");
        }
    }
}
