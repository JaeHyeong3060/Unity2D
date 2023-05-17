using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Define;

public enum ResultType
{
	Victory,
	Defeat,
}

public class UI_ResultPopup : UI_Popup
{
	enum Texts
	{
		TitleText,
	}

	enum GameObjects
	{
		RewardsLayoutGroup,
	}

	enum Images
	{
		CartoonImage
	}

	enum Buttons
    {
		AdButton,
		YesButton,
    }

	ResultType _type;
	List<RewardValuePair> _rewards;
	string _path;
	string _text;

	List<UI_ResultItem> _items = new List<UI_ResultItem>();

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindText(typeof(Texts));
		BindObject((typeof(GameObjects)));
		BindImage((typeof(Images)));
		BindButton((typeof(Buttons)));

		GetButton((int)Buttons.AdButton).gameObject.BindEvent(OnClickAdButton);
		GetButton((int)Buttons.YesButton).gameObject.BindEvent(OnClosePopup);

		switch (_type)
		{
			case ResultType.Victory:
				//gameObject.BindEvent(OnCloseCartoon);
				break;
			default:
				break;
		}
		gameObject.BindEvent(OnClosePopup);

		RefreshUI();

		return true;
	}

	public void SetInfo(ResultType type, List<RewardValuePair> rewards, string path, string text)
	{
		_type = type;
		_rewards = rewards;
		_path = path;
		_text = text;
		RefreshUI();
	}

	void RefreshUI()
	{
		if (_init == false)
			return;

		switch (_type)
		{
			case ResultType.Victory:
				GetText((int)Texts.TitleText).text = Managers.GetText(Define.PromoteSuccess);
				break;
			case ResultType.Defeat:
				GetText((int)Texts.TitleText).text = Managers.GetText(Define.PromoteFail);
				break;
		}
		
		if (_type == ResultType.Victory)
		{
			GetText((int)Texts.TitleText).gameObject.SetActive(false);
			GetImage((int)Images.CartoonImage).gameObject.SetActive(true);
			//GetImage((int)Images.CartoonImage).gameObject.GetOrAddComponent<DOTweenAnimation>().DORestartAllById("Victory");
			GetImage((int)Images.CartoonImage).sprite = Managers.Resource.Load<Sprite>(_path);
		}
		else
		{
			GetText((int)Texts.TitleText).gameObject.SetActive(true);
			GetImage((int)Images.CartoonImage).gameObject.SetActive(false);
		}

		// Rewards
		GameObject parent = GetObject((int)GameObjects.RewardsLayoutGroup);
		foreach (Transform t in parent.transform)
			Managers.Resource.Destroy(t.gameObject);

		_items.Clear();

		for (int i = 0; i < _rewards.Count; i++)
		{
			RewardValuePair reward = _rewards[i];
			UI_ResultItem item = Managers.UI.MakeSubItem<UI_ResultItem>(parent.transform);
			item.SetInfo(reward);

			_items.Add(item);
		}
	}

	void OnClickAdButton()
    {

    }

	void OnClosePopup()
	{
		Debug.Log("OnClosePopup");
		Managers.Sound.Play(Sound.Effect, "Sound_ResultStat");

		Utils.AddResultReward(_rewards);

		Managers.UI.ClosePopupUI(this);
		Managers.UI.ShowPopupUI<UI_HudPopup>();
	}
}
