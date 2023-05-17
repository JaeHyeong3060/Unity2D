using DG.Tweening;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Define;


public class UI_InvenPopup : UI_Popup
{
	enum Texts
	{
	}

	enum Buttons
	{
		ShopButton,
		InvenButton,
		PlayButton,
		DeckButtonA,
		DeckButtonB,
		DeckButtonC,
		DeckButtonD,
		DeckButtonE,
		PresetButtonA,
		PresetButtonB,
		PresetButtonC,
	}

	enum Images
	{
		DeckTowerImageA,
		DeckTowerImageB,
		DeckTowerImageC,
		DeckTowerImageD,
		DeckTowerImageE,

		OnImageA,
		OnImageB,
		OnImageC,
	}

    enum GameObjects
	{
	}

	enum DOTweenAnimations
    {
		Deck,
	}

	private bool _selectMode = false;
	public bool SelectMode { get { return _selectMode; } set { _selectMode = value; } }

	private InvenData _selectedInvenData;
	public InvenData SelectedInvenData { get { return _selectedInvenData; } set { _selectedInvenData = value; RefreshSelect(); } }

	public int _selectPresetIndex;

	public List<InvenData> _curDeckData = new List<InvenData>();

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		_selectedInvenData = null;
		_selectMode = false;

		_selectPresetIndex = Managers.Game.CurPresetIndex;
		_curDeckData = Managers.Game.GetCurDeckData();

		BindText(typeof(Texts));
		BindButton(typeof(Buttons));
		BindObject(typeof(GameObjects));
		BindImage(typeof(Images));

		Bind<DOTweenAnimation>(typeof(DOTweenAnimations));

        GetButton((int)Buttons.DeckButtonA).gameObject.BindEvent(()=> { OnClickDeck(0); });
        GetButton((int)Buttons.DeckButtonB).gameObject.BindEvent(()=> { OnClickDeck(1); });
        GetButton((int)Buttons.DeckButtonC).gameObject.BindEvent(()=> { OnClickDeck(2); });
        GetButton((int)Buttons.DeckButtonD).gameObject.BindEvent(()=> { OnClickDeck(3); });
        GetButton((int)Buttons.DeckButtonE).gameObject.BindEvent(()=> { OnClickDeck(4); });

        GetButton((int)Buttons.PresetButtonA).gameObject.BindEvent(()=> { OnClickPreset(0); });
        GetButton((int)Buttons.PresetButtonB).gameObject.BindEvent(()=> { OnClickPreset(1); });
        GetButton((int)Buttons.PresetButtonC).gameObject.BindEvent(()=> { OnClickPreset(2); });

		RefreshUI();

		return true;
	}

	public void RefreshUI()
	{
		if (_init == false)
			return;

		RefreshDeck();
		RefreshSelect();
		RefreshPresetImage();
	}

	public void RefreshDeck()
	{
        for (int i = 0; i < _curDeckData.Count; i++)
        {
            Sprite sprite = Managers.Resource.Load<Sprite>(_curDeckData[i].iconPath);
            GetImage(i).sprite = sprite;
        }
    }

	public void RefreshSelect()
	{
		if (_selectedInvenData != null)
			_selectMode = true;

		GetButton((int)Buttons.DeckButtonA).interactable = _selectMode;
		GetButton((int)Buttons.DeckButtonB).interactable = _selectMode;
		GetButton((int)Buttons.DeckButtonC).interactable = _selectMode;
		GetButton((int)Buttons.DeckButtonD).interactable = _selectMode;
		GetButton((int)Buttons.DeckButtonE).interactable = _selectMode;
	}

	public void RefreshPresetImage()
    {
		if (Managers.Game.CurPresetIndex == 0)
        {
			GetImage((int)Images.OnImageA).gameObject.SetActive(true);
			GetImage((int)Images.OnImageB).gameObject.SetActive(false);
			GetImage((int)Images.OnImageC).gameObject.SetActive(false);
        }
		if (Managers.Game.CurPresetIndex == 1)
		{
			GetImage((int)Images.OnImageA).gameObject.SetActive(false);
			GetImage((int)Images.OnImageB).gameObject.SetActive(true);
			GetImage((int)Images.OnImageC).gameObject.SetActive(false);
		}
		if (Managers.Game.CurPresetIndex == 2)
		{
			GetImage((int)Images.OnImageA).gameObject.SetActive(false);
			GetImage((int)Images.OnImageB).gameObject.SetActive(false);
			GetImage((int)Images.OnImageC).gameObject.SetActive(true);
		}
	}

	public void OnClickDeck(int deckIndex)
    {
        if (_selectedInvenData == null || _curDeckData.Count <= deckIndex)
            return;

        Managers.Game.ChangeDeck(_selectedInvenData, deckIndex);
		_curDeckData = Managers.Game.GetCurDeckData();

        _selectMode = false;
        _selectedInvenData = null;
        RefreshUI();
    }

	public void OnClickPreset(int presetIndex)
	{
		if (presetIndex == Managers.Game.CurPresetIndex)
			return;

		Managers.Game.ChangePreset(presetIndex);
		_curDeckData = Managers.Game.GetCurDeckData();
		Get<DOTweenAnimation>((int)DOTweenAnimations.Deck).DORestartById("ChangePreset");

		RefreshUI();
	}
}
