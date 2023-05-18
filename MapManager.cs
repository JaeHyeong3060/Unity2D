using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    public Color indicatorDefaultColor;
    public Color indicatorActiveColor;

    public List<UI_GridItem> _gridItemList;
    private List<GameObject> _monsterList;

    public Dictionary<int, int> curTowerDic = new Dictionary<int, int>();

    private UI_SellButton _sellButton;

    public void Init()
    {
        //CreateGridPosition();
        //CreateIndicators();
        //HideIndicators();

        //m_Plane = new Plane(Vector3.up, Vector3.zero);

        //_gridTowerArray = new GameObject[hexMapSizeX, hexMapSizeY / 2];

        ////tell other scripts that map is ready
        //this.SendMessage("OnMapReady", SendMessageOptions.DontRequireReceiver);
    }

    //public void ShowIndicators()
    //{
    //    indicatorContainer.SetActive(true);
    //}

    //public void HideIndicators()
    //{
    //    indicatorContainer.SetActive(false);
    //}

    public Dictionary<int, int> UpdateCurTowerDic()
    {
        curTowerDic.Clear();

        foreach (var item in _gridItemList)
        {
            if (item.Tower == null)
                continue;

            var tc = item.Tower.GetComponent<TowerController>();
            if (tc == null)
                continue;

            if (curTowerDic.ContainsKey(tc.TowerData.ID) == false)
                curTowerDic.Add(tc.TowerData.ID, 1);
            else
                curTowerDic[tc.TowerData.ID] += 1;
        }

        Managers.Play.CheckQuestCount();
        return curTowerDic;
    }
    private int GetTowerCountOnHexGrid()
    {
        int count = 0;

        return count;
    }

    private void RemoveTowerFromArray(int type, int gridX, int gridZ)
    {

    }

    public int FindEmptyIndex()
    {
        if (_gridItemList == null)
            return Define.NullIndex;

        foreach(var gridItem in _gridItemList)
        {
            if (gridItem.Tower == null)
                return gridItem.Index;
        }

        return Define.NullIndex;
    }

    public bool CheckMergeTower(GameObject srcTower, GameObject destTower)
    {
        if (srcTower == null || destTower == null)
            return false;

        TowerData srcData = srcTower.GetOrAddComponent<TowerController>().TowerData;
        TowerData destData = destTower.GetOrAddComponent<TowerController>().TowerData;

        if (srcData.level == destData.level && srcData.level < Define.MaxLevel && srcData.towerType == destData.towerType)
        {
            return true;
        }
        return false;
    }

    public TowerData MergeTower(GameObject mergeTower)
    {
        TowerData srcData = mergeTower.GetOrAddComponent<TowerController>().TowerData;
        TowerData resultData = srcData;

        var curDeckData = Managers.Game.GetCurDeckData();
        int rand = UnityEngine.Random.Range(0, 5);

        if (rand >= curDeckData.Count || curDeckData[rand] == null)
            return resultData;

        var type = curDeckData[rand].type;

        TowerData towerData = TowerData.FindTowerByType(type);

        if (towerData == null)
            return resultData;

        int destLevel = srcData.level + 1;

        foreach (var item in Managers.Data.Tower.Values)
        {
            if (item.towerType == towerData.towerType && item.level == destLevel)
            {
                resultData = item;
            }
        }

        return resultData;
    }

    public GameObject SellTower(GameObject srcTower)
    {        
        var tc = srcTower.GetComponent<TowerController>();
        if (tc == null)
            return null;
        int sellMoney = (int)(tc.TowerData.level * Define.TOWER_SELL_RATE * Define.SPAWN_TOWER_MONEY);
        PoolManager.Pools["TowerPool"].Despawn(srcTower.transform);
        Managers.Game.AddScoreMoney(sellMoney);

        return null;
    }

    public GameObject DraggedObject 
    { 
        get { return _draggedObject; } 
        set 
        {
            if(_draggedObject != null)
            {
                var tc = _draggedObject.GetComponent<TowerController>();
                if (tc == null)
                    return;

                tc.ShowRange(false);
                ShowGrid(true);
            }

            _draggedObject = value;

            if(_draggedObject != null)
            {
                _draggedObject.GetComponent<TowerController>().ShowRange(true);
                ShowGrid(false);
            }
        } 
    }

    public int CurIndex { get { return _curIndex; } set { _curIndex = value; } }

    private GameObject _draggedObject;

    private int _curIndex;

    public void SetObjectIndex(int destIndex, int srcIndex, Action<int> mergeCallback)
    {
        //srcIndex = 시작지 dest == 도착지 
        //srcTower = 드래그하고 있는 타워  destTower = 도착지의 타워
        if (_gridItemList == null)
            return;

        if (_gridItemList.Count < destIndex)
            return;

        if(CheckMergeTower(_gridItemList[srcIndex].Tower, _gridItemList[destIndex].Tower))
        {
            PoolManager.Pools["TowerPool"].Despawn(_gridItemList[srcIndex].Tower.transform);
            TowerController tc = _gridItemList[destIndex].Tower.GetComponent<TowerController>();
            tc.SetInfo(MergeTower(_gridItemList[destIndex].Tower), tc.DeckIndex, _monsterList, destIndex);

            _gridItemList[srcIndex].Tower = null;
            mergeCallback?.Invoke(destIndex);
        }
        else
        {
            GameObject temp = _gridItemList[destIndex].Tower;
            _gridItemList[destIndex].Tower = _gridItemList[srcIndex].Tower;
            _gridItemList[srcIndex].Tower = temp;
        }
    }

    public void SetInfo(List<UI_GridItem> gridItems, GameObject sellButtonObject, List<GameObject> monsterList)
    {
        _gridItemList = gridItems;
        _sellButton = sellButtonObject.GetComponent<UI_SellButton>();
        _monsterList = monsterList;
    }

    public void ShowGrid(bool isOn)
    {
        if (_gridItemList == null)
            return;

        if(isOn == true)
        {
            foreach(var item in _gridItemList)
            {
                item.SetImage(false);
            }
            _sellButton?.SetImage(false);
        }
        else
        {
            foreach (var item in _gridItemList)
            {
                item.SetImage(true);
            }
            _sellButton?.SetImage(true);
        }
    }
}
