using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LaboSlotList : MonoBehaviour, IEnhancedScrollerDelegate
{
    public List<SlotData> _slotList;

    public EnhancedScroller scroller;

    public EnhancedScrollerCellView cellViewPrefab;

    void Start()
    {
        scroller.Delegate = this;

        _slotList = Managers.Data.SlotData;

        scroller.ReloadData();
    }

    #region EnhancedScroller Handlers

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _slotList.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 370f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        UI_LaboSlotItem cellView = scroller.GetCellView(cellViewPrefab) as UI_LaboSlotItem;

        cellView.name = "Cell " + dataIndex.ToString();

        cellView.SetInfo(_slotList[dataIndex], dataIndex);

        scroller.RefreshActiveCellViews();
        return cellView;
    }

    #endregion
}
