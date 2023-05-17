using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_ResultItem : UI_Base
{
	enum Images
	{
		RewardIcon
	}

	enum Texts
	{
		RewardText,
		RewardValueText,
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindImage(typeof(Images));
		BindText(typeof(Texts));

		RefreshUI();

		return true;
	}

	RewardValuePair _reward;

	public void SetInfo(RewardValuePair reward)
	{
		_reward = reward;
		RefreshUI();
	}

	void RefreshUI()
	{
		if (_init == false)
			return;

		RewardData _rewardData;
		Managers.Data.RewardInfo.TryGetValue(_reward.type, out _rewardData);
		Sprite sprite = Managers.Resource.Load<Sprite>(_rewardData.iconPath);
		GetImage((int)Images.RewardIcon).sprite = sprite;
		GetText((int)Texts.RewardValueText).text = Utils.GetMoneyString(_reward.value);

		//GetText((int)Texts.RewardValueText).color = Utils.GetRewardColor(_reward.type, _reward.value);
	}
}
