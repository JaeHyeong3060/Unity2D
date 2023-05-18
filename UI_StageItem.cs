using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_StageItem : EnhancedScrollerCellView
{
	enum Images
	{
		StageImage,
	}

	enum Texts
	{
		LevelText,
	}

	enum Buttons
	{
		StageButton,
	}

	public StageData _stageData;
	public int _dataIndex;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindImage(typeof(Images));
		BindText(typeof(Texts));
		//BindButton(typeof(Buttons));

		//GetButton((int)Buttons.StageButton).gameObject.BindEvent(OnClickItem);

		RefreshUI();

		return true;
	}

	public void SetInfo(StageData stageInfo, int dataIndex)
	{
		if (stageInfo == null || stageInfo.ID == 0)
			gameObject.SetActive(false);

		_stageData = stageInfo;
		_dataIndex = dataIndex;
		RefreshUI();
	}

    public override void RefreshCellView()
    {
		RefreshUI();
	}

    public void RefreshUI()
	{
		if (_init == false)
			return;

		//GetText((int)Texts.PortraitNameText).text = Managers.GetText(_data.nameID);

		Sprite sprite = Managers.Resource.Load<Sprite>(_stageData.iconPath);
		GetImage((int)Images.StageImage).sprite = sprite;

		if (Managers.Game.MaxStage <= _dataIndex)
		{
			GetImage((int)Images.StageImage).material = Utils.SetGrayScale(true);
		}
		else
		{
			GetImage((int)Images.StageImage).material = null;
		}
	}

	public void OnClickItem()
	{
        //Managers.UI.ShowPopupUI<UI_StageSelectPopup>().SetInfo(_stageData, () =>
        //{
        //    Managers.UI.FindPopup<UI_StageSelectPopup>().SelectedStageData = _stageData;
        //});
    }
}
