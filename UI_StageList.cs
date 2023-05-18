using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_StageList : MonoBehaviour, IEnhancedScrollerDelegate
{
    public List<StageData> _stageInfoList;

    public EnhancedScroller scroller;

    public EnhancedScrollerCellView cellViewPrefab;

    void Start()
    {
        scroller.Delegate = this;
        scroller.scrollerSnapped = ScrollerSnapped;
        scroller.scrollerScrollingChanged = ScrollerScrollingChangedDelegate;
        
        _stageInfoList = Managers.Data.StageInfoList;

        // ID는 1부터 시작이므로 Index는 -1

        scroller.ReloadData();
        scroller.JumpToDataIndex(Managers.Game.CurStage-1);
        scroller.Snap();
    }

    #region EnhancedScroller Handlers

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _stageInfoList.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 400f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        UI_StageItem cellView = scroller.GetCellView(cellViewPrefab) as UI_StageItem;

        cellView.name = "Cell " + dataIndex.ToString();

        cellView.SetInfo(_stageInfoList[dataIndex], dataIndex);

        scroller.RefreshActiveCellViews();
        return cellView;
    }

    private UI_StageSelectPopup _stageSelectPopup = null;
    private void ScrollerSnapped(EnhancedScroller scroller, int cellIndex, int dataIndex, EnhancedScrollerCellView cellView)
    {
        // ID는 1부터 시작하므로 + 1
        Debug.Log("dataIndex : " + dataIndex.ToString());
        if (_stageSelectPopup == null)
            _stageSelectPopup = Managers.UI.FindPopup<UI_StageSelectPopup>();
        if (_stageSelectPopup != null)
            _stageSelectPopup.SelectedStageID = dataIndex + 1;

        if (Managers.Game.MaxStage >= _stageSelectPopup.SelectedStageID)
            _stageSelectPopup.SetSelecetButton(true);

    }
    public void ScrollerScrollingChangedDelegate(EnhancedScroller scroller, bool scrolling)
    {
        if(_stageSelectPopup == null)
            _stageSelectPopup = Managers.UI.FindPopup<UI_StageSelectPopup>();

        if (_stageSelectPopup == null)
            return;

        if (scrolling == false)
        {
            if(scroller.NormalizedScrollPosition == 0)
            {
                // ID는 1부터 시작하므로 + 1
                _stageSelectPopup.SelectedStageID = 1;
                scroller.JumpToDataIndex(0);
                _stageSelectPopup.SetSelecetButton(true);
            }
            if(scroller.NormalizedScrollPosition == 1)
            {
                var max = Managers.Data.StageInfoList.Count - 1;
                // ID는 1부터 시작하므로 + 1
                _stageSelectPopup.SelectedStageID = max + 1;
                scroller.JumpToDataIndex(max);
                if (Managers.Game.MaxStage >= _stageSelectPopup.SelectedStageID)
                    _stageSelectPopup.SetSelecetButton(true);
            }
        }
        else
        {
            _stageSelectPopup.SetSelecetButton(false);
        }
    }

    #endregion
}
