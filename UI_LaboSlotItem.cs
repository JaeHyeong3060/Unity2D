using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_LaboSlotItem : EnhancedScrollerCellView
{
	enum Images
	{
		LaboImage,
	}

	enum Texts
	{
		TimeText,
	}

	enum Buttons
	{
		SlotButton,
	}

	public SlotData _slotData;
	public PlayerLaboData _playerLaboData;
	public LaboData _laboData;
	public int _dataIndex;

	public SlotTime _slotTimeData;

	public bool _canUpgrade;

	Coroutine _timeCorutine;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindImage(typeof(Images));
		BindText(typeof(Texts));
		BindButton(typeof(Buttons));

		GetButton((int)Buttons.SlotButton).gameObject.BindEvent(OnClickItem);

		if (_timeCorutine != null)
			StopCoroutine(_timeCorutine);
		_timeCorutine = StartCoroutine(RefreshTime());

		RefreshUI();

		return true;
	}

	public void SetInfo(SlotData stageInfo, int dataIndex)
	{
		if (stageInfo == null || stageInfo.ID == 0)
			gameObject.SetActive(false);

		_slotData = stageInfo;
		_dataIndex = dataIndex;

		if (_slotTimeData == null)
		{
			_slotTimeData = Managers.Game.SlotTime.Find((slotData) => slotData.slotId == _slotData.ID);
			if (_slotTimeData == null)
			{
				_canUpgrade = true;
			}
			else
			{
				_laboData = Managers.Data.LaboData.Find((item) => item.ID == _slotTimeData.laboId);
				_canUpgrade = false;
			}
		}

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

		if (_laboData == null || _slotTimeData == null)
        {
			Sprite baseSprite = Managers.Resource.Load<Sprite>("Sprites/Main/Common/btn_05");
			GetImage((int)Images.LaboImage).sprite = baseSprite;
			GetText((int)Texts.TimeText).text = "";
			_canUpgrade = true;
			return;
		}
		
		_canUpgrade = false;

		Sprite sprite = Managers.Resource.Load<Sprite>(_laboData.iconPath);
		GetImage((int)Images.LaboImage).sprite = sprite;

		//if (Managers.Game.MaxStage <= _dataIndex)
		//{
		//	GetImage((int)Images.StageImage).material = Utils.SetGrayScale(true);
		//}
		//else
		//{
		//	GetImage((int)Images.StageImage).material = null;
		//}
	}

	IEnumerator RefreshTime()
    {
		while (true)
		{
			if (_slotTimeData != null)
			{
				var remainTime = _slotTimeData.endTime.Subtract(DateTime.UtcNow);

				string strTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
									  remainTime.Hours,
									  remainTime.Minutes,
									  remainTime.Seconds);

				GetText((int)Texts.TimeText).text = strTime;

				if(remainTime.TotalSeconds <= 0)
                {
					Complete();
					yield break;
				}
			}

			yield return new WaitForSeconds(1f);
		}
    }

	public void Complete()
    {
		Managers.Sound.Play(Sound.Effect, "Sound_FolderItemClick");
		Managers.Game.CompleteLaboData(_laboData);
		Managers.Game.SlotTime.Remove(_slotTimeData);
		_slotTimeData = null;
		
		RefreshUI();
	}

	public void OnClickItem()
	{
		if (_canUpgrade == false)
			return;

		Managers.Sound.Play(Sound.Effect, "Sound_FolderItemClick");
		Managers.UI.ShowPopupUI<UI_LaboSelectPopup>().SetInfo(_dataIndex, (dataIndex) =>
        {
			var laboData = Managers.Data.LaboData.Find((item) => item.ID == dataIndex + 1);
			if (laboData == null)
				return;

			_playerLaboData = PlayerLaboData.FindPlayerLaboData(laboData.ID);
			if (_playerLaboData == null)
			{
				_playerLaboData = Managers.Game.CreateAndAddLaboData(laboData);
			}
			_playerLaboData.laboState = LaboState.Proceeding;
			
			_laboData = laboData;

			var slotTimeData = Managers.Game.SlotTime.Find((slotData) => slotData.slotId == _slotData.ID);
			if(slotTimeData == null)
            {
				_slotTimeData = new SlotTime { slotId = _slotData.ID, laboId = dataIndex + 1, 
					startTime = System.DateTime.UtcNow, endTime = System.DateTime.UtcNow.AddSeconds(laboData.timeSecond) };
				Managers.Game.SlotTime.Add(_slotTimeData);
			}

			if (_timeCorutine != null)
				StopCoroutine(_timeCorutine);
			_timeCorutine = StartCoroutine(RefreshTime());

			RefreshUI();
        });
	}
}
