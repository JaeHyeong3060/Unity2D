using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LaboItemRow : EnhancedScrollerCellView
{
    public UI_LaboItem[] laboItemList;

    public void SetData(LaboData laboInfo,int dataIndex, int rowCount, Action<int> onClickItem)
    {
        for(int i=0;i<rowCount;i++)
        {
            int index = dataIndex * rowCount + i;

            if(index >= Managers.Data.InvenData.Count)
            {
                laboItemList[i].SetInfo(null, Define.NullIndex, null);
                continue;
            }

            laboItemList[i].SetInfo(Managers.Data.LaboData[index], index, onClickItem);
        }
    }
}
