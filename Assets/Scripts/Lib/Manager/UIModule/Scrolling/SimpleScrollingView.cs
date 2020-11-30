using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleScrollingView : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Transform content;

    public RectTransform heightHolder;

    public GridLayoutGroup group;

    private List<SimpleScrollingCell> cells;
    private GameObject cellObj;

    private float cellSize;

    private int cellCountByGroup;

    private float originShowItem;

    void Awake()
    {
        cells = new List<SimpleScrollingCell>();
    }

    public void Init(string cellPath)
    {
        cellObj = ResourceManager.LoadSync<GameObject>(cellPath).Asset;
        if (scrollRect.vertical)
        {
            cellSize = group.cellSize.y + group.spacing.y;
        }
        else
        {
            cellSize = group.cellSize.x + group.spacing.x;
        }
        cellCountByGroup = group.constraintCount;

        if (scrollRect.vertical)
        {   
            originShowItem = heightHolder.sizeDelta.y;
        }
        else
        {
            originShowItem = heightHolder.sizeDelta.x;
        }
        
    }

    public void Clear()
    {
        foreach (var item in cells)
        {
            GameObject.Destroy(item.gameObject);
        }
        cells.Clear();
    }

    public List<SimpleScrollingCell> Fill(int num)
    {
        Clear();
        for (int i = 0; i < num; i++)
        {
            var go = GameObjectUtil.InstantiatePrefab(cellObj, content.gameObject);
            cells.Add(go.GetComponent<SimpleScrollingCell>());
        }

        RestScrollingArea();

        return cells;
    }

    void RestScrollingArea()
    {
        Debug.LogFormat("cell count {0} group count {1}", cells.Count, cellCountByGroup);
        int groupRate = Mathf.CeilToInt( (float)cells.Count / (float)cellCountByGroup );
        float realSize = cellSize * groupRate;
        Debug.LogFormat("real size is {0}  rate :{1}", realSize, groupRate);
        realSize = realSize > originShowItem ? realSize : originShowItem;
        if (scrollRect.vertical)
        {   
            GameObjectUtil.SetRectTransformSizeDelta(heightHolder.gameObject, heightHolder.sizeDelta.x, realSize);
        }
        else
        {
            GameObjectUtil.SetRectTransformSizeDelta(heightHolder.gameObject, realSize, heightHolder.sizeDelta.y);
        }
    }
}