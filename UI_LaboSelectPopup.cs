using DG.Tweening;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Define;


public class UI_LaboSelectPopup : UI_Popup
{
	enum Texts
	{
	}

	enum Buttons
	{
		Tint,
	}

	enum Images
	{
	}
	enum UI_LaboLists
	{
		LaboList,
	}

	public int _slotIndex;
	public Action<int> _onClickLaboItem;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindText(typeof(Texts));
		BindButton(typeof(Buttons));
		BindImage(typeof(Images));

		Bind<UI_LaboList>(typeof(UI_LaboLists));
		Get<UI_LaboList>((int)UI_LaboLists.LaboList).SetInfo(_onClickLaboItem);
		GetButton((int)Buttons.Tint).gameObject.BindEvent(OnClickCancel);

		RefreshUI();

		return true;
	}

	public void SetInfo(int slotIndex, Action<int> onClickLaboItem)
    {
		_slotIndex = slotIndex;
		_onClickLaboItem = onClickLaboItem;
	}

	public void OnClickCancel()
    {
		Managers.UI.ClosePopupUI(this);
	}

	public void RefreshUI()
	{
		if (_init == false)
			return;
	}
}
