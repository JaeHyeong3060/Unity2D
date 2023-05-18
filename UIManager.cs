using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager
{
	public class PopupOrder
    {
		public UI_Popup popup;
		public int order;
    }

	int _order = -20;

	List<PopupOrder> _popupList = new List<PopupOrder>();

	public UI_Scene SceneUI { get; private set; }

	private UI_HudPopup _hudPopup;
	public UI_HudPopup HudPopup
	{
        get
        {
			if (_hudPopup == null)
				_hudPopup = Managers.UI.FindPopup<UI_HudPopup>();
			return _hudPopup;
        }
		set
        {

        }
	}

	public GameObject Root
	{
		get
		{
			GameObject root = GameObject.Find("@UI_Root");
			if (root == null)
				root = new GameObject { name = "@UI_Root" };

			return root;
		}
	}

	public void SetCanvas(GameObject go, bool sort = true)
	{
		Canvas canvas = Utils.GetOrAddComponent<Canvas>(go);
		canvas.renderMode = RenderMode.ScreenSpaceCamera;
		canvas.worldCamera = Camera.main;
		canvas.overrideSorting = true;

		if (sort)
		{
			canvas.sortingOrder = _order + _popupList.Count;
		}
		else
		{
			canvas.sortingOrder = 0;
			Utils.UIsortingId(go, Define.Layer7);
		}
	}

	public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/UI/SubItem/{name}");

		GameObject go = Managers.Resource.Instantiate(prefab);
		if (parent != null)
			go.transform.SetParent(parent);

		go.transform.localScale = Vector3.one;
		go.transform.localPosition = prefab.transform.position;

		return Utils.GetOrAddComponent<T>(go);
	}

	public T ShowSceneUI<T>(string name = null) where T : UI_Scene
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
		T sceneUI = Utils.GetOrAddComponent<T>(go);
		SceneUI = sceneUI;

		go.transform.SetParent(Root.transform);

		return sceneUI;
	}

	public T ShowPopupUI<T>(string name = null, Transform parent = null) where T : UI_Popup
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{name}");

		GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
		T popup = Utils.GetOrAddComponent<T>(go);
		int curOrder = _order + _popupList.Count;
		_popupList.Add(new PopupOrder { popup = popup, order = curOrder});

		if (parent != null)
			go.transform.SetParent(parent);
		else if (SceneUI != null)
			go.transform.SetParent(SceneUI.transform);
		else
			go.transform.SetParent(Root.transform);

		go.transform.localScale = Vector3.one;
		go.transform.localPosition = prefab.transform.position;

		return popup;
	}

	public T FindPopup<T>() where T : UI_Popup
	{
		var popupOrder = _popupList.Find((item) => item.popup.GetType() == typeof(T));
		if(popupOrder != null)
        {
			return popupOrder.popup as T;
		}
		return null;
		//return _popupStack.Where(x => x.GetType() == typeof(T)).FirstOrDefault() as T;
	}

	public T PeekPopupUI<T>() where T : UI_Popup
	{
		if (_popupList.Count == 0)
			return null;

		return _popupList[_popupList.Count - 1].popup as T;
	}

	public void ClosePopupUI(UI_Popup popup, bool ignorePeek = false)
	{
		if (_popupList.Count == 0)
			return;

		if (_popupList[_popupList.Count - 1].popup == popup && ignorePeek == false)
		{
			ClosePopupUILast();
			return;
		}

		ClosePopupUINotPeek(popup);
	}

	public void ClosePopupUINotPeek(UI_Popup popup)
	{
		var popupOrder = _popupList.Find((item) => item.popup.GetType() == popup.GetType());
		if (popupOrder == null)
			return;

		Managers.Resource.Destroy(popupOrder.popup.gameObject);
		_popupList.Remove(popupOrder);
	}

	public void ClosePopupUILast()
	{
		if (_popupList.Count == 0)
			return;

		UI_Popup popup = _popupList[_popupList.Count - 1].popup;
		Managers.Resource.Destroy(popup.gameObject);
		_popupList.Remove(_popupList[_popupList.Count - 1]);
		popup = null;
	}

	public void CloseAllPopupUI()
	{
		while (_popupList.Count > 0)
			ClosePopupUILast();
	}

	public void Clear()
	{
		CloseAllPopupUI();
		SceneUI = null;
	}
	
	public void RefreshHudMoeny()
    {
		if (HudPopup != null)
			HudPopup.RefreshMoney();
    }
}
