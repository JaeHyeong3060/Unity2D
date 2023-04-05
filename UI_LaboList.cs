using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LaboList : MonoBehaviour, IEnhancedScrollerDelegate
{
    private List<LaboData> _laboData;

    public EnhancedScroller scroller;

    public EnhancedScrollerCellView cellViewPrefab;

    public Action<int> _onClickItem;

    public const int ROW_COUNT = 2;

    void Start()
    {
        scroller.Delegate = this;

        _laboData = Managers.Data.LaboData;

        scroller.ReloadData();
    }

    #region EnhancedScroller Handlers

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _laboData.Count / ROW_COUNT + 1;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 400f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        UI_LaboItemRow cellView = scroller.GetCellView(cellViewPrefab) as UI_LaboItemRow;

        cellView.name = "Cell " + dataIndex.ToString();

        cellView.SetData(_laboData[dataIndex], dataIndex, ROW_COUNT, _onClickItem);

        return cellView;
    }

    public void SetInfo(Action<int> onClickItem)
    {
        _onClickItem = onClickItem;
    }

    #endregion
}
