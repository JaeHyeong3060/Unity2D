using DG.Tweening;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Define;


public class UI_HudPopup : UI_Popup
{
	enum Texts
	{
		PlayerLevelText,
		CoinText,
		DiamondText,
	}

	enum Buttons
	{
		ProfileButton,
		SettingButton,
	}

	enum Images
	{
		ExpBarImage,
		ShopIcon,
		InvenIcon,
		BattleIcon,
		LaboratoryIcon,
		ArcadeIcon,
	}

	enum GameObjects
	{

	}

	enum BottomTab
	{
		None,
		Shop,
		Inven,
		Battle,
		Laboratory,
		Arcade,
	}

	enum Toggles
	{
		ShopToggle,
		InvenToggle,
		BattleToggle,
		LaboratoryToggle,
		ArcadeToggle,
	}

	GameManagerEx _game;

	BottomTab _curTab;

	bool _sceneChange;
	public override bool Init()
	{
		if (base.Init(order:false) == false)
			return false;

		_game = Managers.Game;

		_curTab = BottomTab.None;
		_sceneChange = false;

		BindText(typeof(Texts));
		BindButton(typeof(Buttons));
		BindObject(typeof(GameObjects));
		BindImage(typeof(Images));
		BindToggle(typeof(Toggles));

		GetToggle((int)Toggles.ShopToggle).onValueChanged.AddListener(OnClickShopButton);
		GetToggle((int)Toggles.InvenToggle).onValueChanged.AddListener(OnClickInvenButton);
		GetToggle((int)Toggles.BattleToggle).onValueChanged.AddListener(OnClickBattleButton);
		GetToggle((int)Toggles.LaboratoryToggle).onValueChanged.AddListener(OnClickLaboratoryButton);
		GetToggle((int)Toggles.ArcadeToggle).onValueChanged.AddListener(OnClickArcadeButton);

		GetToggle((int)Toggles.ShopToggle).SetIsOnWithoutNotify(true);
		GetToggle((int)Toggles.BattleToggle).isOn = true;

		StartCoroutine(CoSaveGame(3.0f));

		Managers.Sound.Clear();
		Managers.Sound.Play(Sound.Bgm, "Sound_MainPlayBGM", volume: 0.2f);

		_init = true;
		RefreshUI();
		return true;
	}

	public void RefreshUI()
	{
        if (_init == false)
            return;

        RefreshBottomTab();
		RefreshMoney();
	}

	public void RefreshBottomTab()
	{
	}


	public void RefreshMoney(bool playSoundAndEffect = false)
	{
        if (_init == false)
            return;

        if (GetText((int)Texts.CoinText).text != Utils.GetMoneyString(_game.Coin))
		{
			if (playSoundAndEffect)
			{
				Managers.Sound.Play(Sound.Effect, "Sound_Coin");
			}
			GetText((int)Texts.CoinText).text = Utils.GetMoneyString(_game.Coin);
		}

		if (GetText((int)Texts.DiamondText).text != Utils.GetMoneyString(_game.Diamond))
		{
			if (playSoundAndEffect)
			{
				Managers.Sound.Play(Sound.Effect, "Sound_Coin");
			}
			GetText((int)Texts.DiamondText).text = Utils.GetMoneyString(_game.Diamond);
		}
	}

	public void ClosePrevTab()
    {
        switch (_curTab)
        {
            case BottomTab.Shop:
				Managers.UI.FindPopup<UI_ShopPopup>()?.ClosePopupUI();
				break;
            case BottomTab.Inven:
				Managers.UI.FindPopup<UI_InvenPopup>()?.ClosePopupUI();
				break;
            case BottomTab.Battle:
				Managers.UI.FindPopup<UI_LobbyPopup>()?.ClosePopupUI();
				break;
            case BottomTab.Laboratory:
				Managers.UI.FindPopup<UI_LaboratoryPopup>()?.ClosePopupUI();
				break;
            case BottomTab.Arcade:
				Managers.UI.FindPopup<UI_ArcadePopup>()?.ClosePopupUI();
				break;
        }
    }

	void OnClickShopButton(bool isOn)
	{
		if (_sceneChange)
			return;

		if (isOn && _curTab != BottomTab.Shop)
        {
			ClosePrevTab();
			_curTab = BottomTab.Shop;
			Debug.Log("OnClickShopButton");
			Managers.Sound.Play(Sound.Effect, "Sound_FolderItemClick");
			Managers.UI.ShowPopupUI<UI_ShopPopup>();
		}
	}

	void OnClickInvenButton(bool isOn)
	{
		if (_sceneChange)
			return;

		if (isOn && _curTab != BottomTab.Inven)
        {
			ClosePrevTab();
			_curTab = BottomTab.Inven;
			Debug.Log("OnClickInvenButton");
			Managers.Sound.Play(Sound.Effect, "Sound_FolderItemClick");
			Managers.UI.ShowPopupUI<UI_InvenPopup>();
		}
	}

	void OnClickBattleButton(bool isOn)
	{
		if (_sceneChange)
			return;

		if (isOn && _curTab != BottomTab.Battle)
		{
			ClosePrevTab();
			_curTab = BottomTab.Battle;
			Debug.Log("OnClickUI_LobbyPopup");
			Managers.Sound.Play(Sound.Effect, "Sound_FolderItemClick");
			Managers.UI.ShowPopupUI<UI_LobbyPopup>().SetInfo(SceneChange);
		}
	}
	void OnClickLaboratoryButton(bool isOn)
	{
		if (_sceneChange)
			return;

		if (isOn && _curTab != BottomTab.Laboratory)
		{
			ClosePrevTab();
			_curTab = BottomTab.Laboratory;
			Debug.Log("OnClickInvenButton");
			Managers.Sound.Play(Sound.Effect, "Sound_FolderItemClick");
			Managers.UI.ShowPopupUI<UI_LaboratoryPopup>();
		}
	}
	void OnClickArcadeButton(bool isOn)
	{
		if (_sceneChange)
			return;

		if (isOn && _curTab != BottomTab.Arcade)
		{
			ClosePrevTab();
			_curTab = BottomTab.Arcade;
			Debug.Log("OnClickInvenButton");
			Managers.Sound.Play(Sound.Effect, "Sound_FolderItemClick");
			Managers.UI.ShowPopupUI<UI_ArcadePopup>();
		}
	}

	public void SceneChange()
    {
		_sceneChange = true;
		GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/Effect/FadeOut");
		GameObject go = Managers.Resource.Instantiate(prefab, transform);
		go.GetComponent<DOTweenAnimation>().onComplete.AddListener(SceneAnimationEnd);
	}

	void SceneAnimationEnd()
	{
		_sceneChange = false;
		Managers.UI.CloseAllPopupUI();
		var GamePopup = Managers.UI.ShowPopupUI<UI_GamePlayPopup>();
		GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/Effect/FadeIn");
		GameObject go = Managers.Resource.Instantiate(prefab, GamePopup.transform);
	}
	IEnumerator CoSaveGame(float interval)
	{
		while (true)
		{
			yield return new WaitForSeconds(interval);
			Managers.Game.SaveGame();
		}
	}
}
