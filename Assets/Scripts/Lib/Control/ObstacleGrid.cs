using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGrid : MonoBehaviour
{
    public static ObstacleGrid instance { get; private set; }
    public bool ShowOBS = true;
    public GameObject[] Roots;
    public float GridSize = 1;
    public byte[] ObstacleData = new byte[256 * 256];

    public void Start()
    {
        instance = this;
    }
    public void OnDrawGizmos()
    {
        int Width = 256;

        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Width; j++)
            {
                Gizmos.color = Color.green;
                Vector3 startpos = new Vector3(i * GridSize, 0, j * GridSize);
                Gizmos.DrawLine(startpos, startpos + Vector3.right * GridSize);
                Gizmos.DrawLine(startpos, startpos + Vector3.forward * GridSize);
                byte obs = ObstacleData[i + j * Width];
                if (obs > 0 && ShowOBS)
                {
                    Gizmos.color = Color.red;
                    startpos = new Vector3(i * GridSize + 0.5f, 1, j * GridSize + 0.5f);
                    Gizmos.DrawCube(startpos, new Vector3(1, 2, 1));
                }
            }
    }
    [ContextMenu("Find")]
    public void Find()
    {
        for (int i = 0; i < Roots.Length; i++)
        {
            Transform[] tfs = Roots[i].GetComponentsInChildren<Transform>(true);
            for (int c = 0; c < tfs.Length; c++)
            {
                CheckObstacle(tfs[c]);
            }
        }
    }

    private void CheckObstacle(Transform tf)
    {
        if(tf.name.Contains("LP_TreeCedar")||tf.name.Contains("LP_Tree_1")||tf.name.Contains("LP_Tree_CS")||tf.name.Contains("LI_Sunshade")||tf.name.Contains("LB_FenceIkegaki")||tf.name.Contains("LI_Fence")||tf.name.Contains("LB_FenceVerticalWood")||tf.name.Contains("LB_FenceWallRenga_")||tf.name.Contains("LB_FenceLattice_new_")||tf.name.Contains("LI_StreetLamp02")||tf.name.Contains("LI_Trash02")||tf.name.Contains("LB_FenceIronAndStone")||tf.name.Contains("LI_Telephonepole_")||tf.name.Contains("LI_Waterview_")||tf.name.Contains("LI_Surfboard")||tf.name.Contains("LI_Barbecuegrill"))
        {
            SetObstacle(tf.position - new Vector3(1f, 0, 1f), 2, 2);
        }
        else if(tf.name.Contains("LI_ChildRunner"))
        {
            SetObstacle(tf.position - new Vector3(3f, 0, 3f), 6, 6);
        }
        else if(tf.name.Contains("LI_FountainWhite"))
        {
            SetObstacle(tf.position - new Vector3(2f, 0, 2f), 4, 4);
        }
        else if(tf.name.Contains("LI_Poolsidebed_")||tf.name.Contains("LI_BarbecueRack")|| tf.name.Contains("LI_Bench"))
        {
            SetObstacle(tf.position - new Vector3(2f, 0, 1f), 4, 2);
        }
        else if(tf.name.Contains("ObsCube"))
        {
            SetObstacle(tf.position - tf.localScale * 0.5f, Mathf.RoundToInt(tf.localScale.x), Mathf.RoundToInt(tf.localScale.z));
        }
    }

    private void SetObstacle(Vector3 pos, int width, int height)
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                SetOneObstacle(Mathf.RoundToInt(pos.x + x), Mathf.RoundToInt(pos.z + y));
            }
    }
    private void SetOneObstacle(int x,int y)
    {
        if (x < 0 || y < 0 || x > 255 || y > 255)
            return;
        ObstacleData[x + 256 * y] = 1;
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        ObstacleData = new byte[256 * 256];
    }

    public static Vector3 ObstacleClamp(Vector3 pos, Vector3 dir)
    {
        if (instance == null)
            return pos;
        return instance.obstacleClamp(pos, dir);
    }
    private Vector3 obstacleClamp(Vector3 pos,Vector3 dir)
    {
        int mx = (int)pos.x;
        int my = (int)pos.z;
        if (dir.x != 0)
        {
            pos = OneMinObstacleCheck(mx, my, (dir.x > 0 ? 1 : -1), 0, pos);
            pos = OneMinObstacleCheck(mx, my, (dir.x > 0 ? 1 : -1), 1, pos);
            pos = OneMinObstacleCheck(mx, my, (dir.x > 0 ? 1 : -1), -1, pos);
        }
        if (dir.z != 0)
        {
            pos = OneMinObstacleCheck(mx, my, 0, (dir.z > 0 ? 1 : -1), pos);
            pos = OneMinObstacleCheck(mx, my, 1, (dir.z > 0 ? 1 : -1), pos);
            pos = OneMinObstacleCheck(mx, my, -1, (dir.z > 0 ? 1 : -1), pos);
        }
        return pos;
    }

    private Vector3 OneMinObstacleCheck(int x, int y, int i, int j, Vector3 pos)
    {
        int index = x + i + (y + j) * 256;
        int value = 0;
        if (index < 0 || (x + i) < 0 || (y + j) < 0 || index >= ObstacleData.Length || x + i >= 256 || y + j >= 256)
            value = 1;
        else
            value = ObstacleData[index];
        float distance = 0;
        Vector3 dir = Vector3.zero;
        if (value > 0)
        {
            dir.x = x + i + 0.5f;
            dir.z = y + j + 0.5f;
            dir = pos - dir;
        }
        else
            return pos;
        dir.y = 0;
        distance = 0.5f + 1 - dir.magnitude;
        if (distance > 0)
        {
            pos += dir.normalized * distance;
        }
        return pos;
    }
}
