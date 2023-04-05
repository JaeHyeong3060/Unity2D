using DG.Tweening;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Define;


public class UI_LaboratoryPopup : UI_Popup
{
	enum Texts
	{
	}

	enum Buttons
	{
	}

	enum Images
	{
	}

	enum GameObjects
	{
	}

	GameManagerEx _game;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindText(typeof(Texts));
		BindButton(typeof(Buttons));
		BindObject(typeof(GameObjects));
		BindImage(typeof(Images));

		RefreshUI();

		return true;
	}

	public void RefreshUI()
	{
		if (_init == false)
			return;
	}
}
