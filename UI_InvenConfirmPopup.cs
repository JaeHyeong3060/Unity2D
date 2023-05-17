using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_InvenConfirmPopup : UI_Popup
{
	enum Images
	{
		TowerImage,
	}

	enum Texts
	{
		AtkText,
		AtkSpeedText,
		AtkRangeText,
		MessageText
	}

	enum Buttons
	{
		BG,
		EquipButton,
		UpgradeButton,
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindText(typeof(Texts));
		BindButton(typeof(Buttons));
		BindImage(typeof(Images));

		GetButton((int)Buttons.EquipButton).gameObject.BindEvent(OnClickEquipButton);
		GetButton((int)Buttons.UpgradeButton).gameObject.BindEvent(OnClickUpgradeButton);
		GetButton((int)Buttons.BG).gameObject.BindEvent(OnClickCancel);

		RefreshUI();
		return true;
	}

	Action onClickEquipButton;
	Action onClickUpgradeButton;
	InvenData _invenData;
	TowerData _towerData;

	public void SetInfo(InvenData invenData, Action onClickYesButton)
	{
		onClickEquipButton = onClickYesButton;
		_invenData = invenData;
		_towerData = TowerData.FindTowerByType(_invenData.type);

		RefreshUI();
	}

	void RefreshUI()
	{
		if (_init == false)
			return;

		if (_invenData == null || _towerData == null)
			return;

		Sprite sprite = Managers.Resource.Load<Sprite>(_invenData.iconPath);
		GetImage((int)Images.TowerImage).sprite = sprite;

		int enchantLevel = Managers.Game.GetTowerEnchant(_invenData.ID);

		GetText((int)Texts.AtkText).text = Utils.GetBuffString(_towerData.atk, enchantLevel);
		GetText((int)Texts.AtkRangeText).text = Utils.GetBuffString(_towerData.range, enchantLevel);
		GetText((int)Texts.AtkSpeedText).text = Utils.GetBuffString(_towerData.coolTimeA, enchantLevel);
	}

	void OnClickCancel()
	{
		Managers.Sound.Play(Sound.Effect, "Sound_CancelButton");
		OnComplete();
	}

	void OnClickEquipButton()
	{
		Managers.UI.ClosePopupUI(this);
		Managers.Sound.Play(Sound.Effect, "Sound_CheckButton");
		if (onClickEquipButton != null)
			onClickEquipButton.Invoke();
	}

	void OnClickUpgradeButton()
	{
		Managers.Sound.Play(Sound.Effect, "Sound_CancelButton");
		Managers.Game.UpgradeInvenItem(_invenData);
		RefreshUI();
	}


	void OnComplete()
	{
		Managers.UI.ClosePopupUI(this);
	}
}
