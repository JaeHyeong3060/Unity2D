using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LaboItem : EnhancedScrollerCellView
{
	enum Images
	{
		LaboIcon,
		ProceedBG,
		ProceedIcon,
	}

	enum Texts
	{
		EffectText,
		CostText,
		TimeText,
	}

	enum Buttons
	{
		LaboButton,
	}

	public LaboData _laboData;
	public int _dataIndex;

	public Action<int> _onClickItem;

	public bool _canUpgrade;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindImage(typeof(Images));
		BindText(typeof(Texts));
		BindButton(typeof(Buttons));

		GetButton((int)Buttons.LaboButton).gameObject.BindEvent(OnClickItem);

		GetImage((int)Images.ProceedBG).gameObject.SetActive(false);
		GetImage((int)Images.ProceedIcon).gameObject.SetActive(false);

		_canUpgrade = false;

		RefreshUI();

		return true;
	}

	public void SetInfo(LaboData laboInfo, int dataIndex, Action<int> onClickItem)
	{
		if (laboInfo == null)
			gameObject.SetActive(false);

		_laboData = laboInfo;
		_dataIndex = dataIndex;
		_onClickItem = onClickItem;
		RefreshUI();
	}

	public void RefreshUI()
	{
		if (_init == false)
			return;

		if (_laboData == null)
			return;

		Sprite sprite = Managers.Resource.Load<Sprite>(_laboData.iconPath);
		GetImage((int)Images.LaboIcon).sprite = sprite;
		GetText((int)Texts.CostText).text = _laboData.cost.ToString();
		GetText((int)Texts.TimeText).text = _laboData.timeSecond.ToString();

		//cost check
		if (Managers.Game.Diamond < _laboData.cost)
			_canUpgrade = false;
		else
			_canUpgrade = true;

		//complete or Proceeding Check
		var playerLaboData = Managers.Game.playerLaboData.Find((s) => s.laboId == _laboData.ID);
		if (playerLaboData != null)
        {
			if (playerLaboData.level >= _laboData.maxLevel)
				playerLaboData.laboState = LaboState.Done;

			if (playerLaboData.laboState == LaboState.Done || playerLaboData.laboState == LaboState.Proceeding)
            {
				_canUpgrade = false;
			}
			else
            {
				_canUpgrade = true;
			}
		}

		if (_canUpgrade == false)
        {
			GetImage((int)Images.ProceedBG).gameObject.SetActive(true);
			GetImage((int)Images.ProceedIcon).gameObject.SetActive(true);
		}
		else
        {
			GetImage((int)Images.ProceedBG).gameObject.SetActive(false);
			GetImage((int)Images.ProceedIcon).gameObject.SetActive(false);
		}

    }

	public void OnClickItem()
    {
		if (_canUpgrade == false)
			return;

		Managers.Game.SpendDiamond(_laboData.cost);
		Managers.UI.FindPopup<UI_LaboSelectPopup>().ClosePopupUI();
		_onClickItem?.Invoke(_dataIndex);
	}
}
