using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_QuestItem : UI_Base
{
	enum Images
	{
		RewardIcon,
	}

	enum Texts
	{
		RewardText,
	}

	enum Buttons
	{
		QuestButton,
	}
	enum QuestItemIcons
	{
		QuestItemIcon,
	}

	QuestData _questData;
	Action<int> _onClickQuestId;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		Utils.UIsortingId(gameObject, Define.Layer7);

		BindImage(typeof(Images));
		BindText(typeof(Texts));
		BindButton(typeof(Buttons));
		Bind<QuestItemIcon>(typeof(QuestItemIcons));

		GetButton((int)Buttons.QuestButton).gameObject.BindEvent(OnClickItem);

		RefreshUI();

		return true;
	}

	public void SetInfo(QuestData questInfo, Action<int> onClickQuestId)
	{
		_questData = questInfo;
		_onClickQuestId = onClickQuestId;
		RefreshUI();
	}

	public void RefreshUI()
	{
		if (_init == false)
			return;

		if (_questData == null)
			return;

		GetText((int)Texts.RewardText).text = "x" + _questData.rewardAmount;

		//Sprite sprite = Managers.Resource.Load<Sprite>(needQuestIdList.iconPath);
		//GetImage((int)Images.QuestImageA).sprite = sprite;

		Get<QuestItemIcon>((int)QuestItemIcons.QuestItemIcon).SetInfo(_questData);

		Sprite rewardSprite = Managers.Resource.Load<Sprite>(_questData.rewardIconPath);
		GetImage((int)Images.RewardIcon).sprite = rewardSprite;
	}

	public void OnClickItem()
	{
		_onClickQuestId?.Invoke(_questData.ID);
	}
}
