using ND_VariaBULLET;
using PathologicalGames;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class TowerController : BaseController
{
	public enum EmoticonType
	{
		None,
		Exclamation,
		Question
	}
    enum Images
    {
        TowerImage,
        RangeImage,
    }

    enum Buttons
    {
        Tower,
    }

    public TowerData TowerData => _towerData;
    private TowerData _towerData;

    private GameObject _target;

    private Coroutine _coAttack;

    // µ¦¿¡¼­ À§Ä¡
    public int DeckIndex => _deckIndex;
    private int _deckIndex;

    bool _spawnPattern = false;
    bool _spawnSuperSkill = false;

    TowerState _state;

    private int _gridIndex;

    List<GameObject> _monsterList;

    public List<GameObject> _basePatternsList = new List<GameObject>();

    public TowerState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;
        }
    }

    public override bool Init()
	{
		if (base.Init() == false)
			return false;

        BindImage(typeof(Images));

        GetImage((int)Images.TowerImage).gameObject.SetActive(true);
        GetImage((int)Images.RangeImage).gameObject.SetActive(true);

        //GetImage((int)Images.TowerImage).gameObject.BindEvent(OnPointerDown, UIEvent.PointerDown);
        //GetImage((int)Images.TowerImage).gameObject.BindEvent(OnDrag, UIEvent.Pressed);
        //GetImage((int)Images.TowerImage).gameObject.BindEvent(OnPointerUp, UIEvent.PointerUp);

        ShowRange(false);

        RefreshUI();

        return true;
	}

    void SpawnPattern(int level)
    {
        if (_spawnPattern == true)
            return;

        _spawnPattern = true;

        if (level > 0)
            SpawnPatternDesc(_towerData.patternFirst,_towerData.coolTimeA);
        if (level >= 3)
            SpawnPatternDesc(_towerData.patternSecond, _towerData.coolTimeB);
        if (level >= 5)
            SpawnPatternDesc(_towerData.patternThird, _towerData.coolTimeC);

        if(_spawnSuperSkill == false && _towerData.superSkillType != SuperSkillType.None)
        {
            Managers.Play.SpawnSuperSkillItem(_towerData);
        }
    }

    void SpawnPatternDesc(PatternType patternType, float coolTime)
    {
        PatternData patternData;
        Managers.Data.Pattern.TryGetValue(patternType, out patternData);
        if (patternData == null)
            return;

        GameObject go = Managers.Resource.Instantiate(patternData.patternPath);
        go.transform.SetParent(GetImage((int)Images.TowerImage).transform);
        go.transform.position = GetImage((int)Images.TowerImage).transform.position;
        var basePattern = go.GetComponent<BasePattern>();
        basePattern.SetInfo(_towerData, patternData, _deckIndex, _monsterList, coolTime);

        _basePatternsList.Add(go);
    }

    public void SetInfo(TowerData data, int deckIndex, List<GameObject> listMonster,int gridIndex)
    {
        _towerData = data;
        _deckIndex = deckIndex;
        _monsterList = listMonster;
        _spawnPattern = false;
        _spawnSuperSkill = false;

        _gridIndex = gridIndex;

        foreach (var item in _basePatternsList)
        {
            var basePattern = item.GetComponent<BasePattern>();
            Destroy(item.gameObject);
        }
        _basePatternsList.Clear();

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_init == false)
            return;

        GetImage((int)Images.TowerImage).sprite = Managers.Resource.Load<Sprite>(_towerData.iconPath);
        GetImage((int)Images.TowerImage).SetNativeSize();

        SpawnPattern(_towerData.level);

        ShowRange(false);
    }

    public void RefreshOrder(int index)
    {
        Canvas canvas = Utils.GetOrAddComponent<Canvas>(gameObject);
        canvas.sortingOrder = CANVAS_TOWER_SORT - index;
    }

	public override void LookLeft(bool flag)
	{
		base.LookLeft(flag);

		Vector3 scale = transform.localScale;
	}

    private float _extraDelay = 0f;
    public void Update()
    {
        switch (_state)
        {
            case TowerState.Idle:
                if (_extraDelay > 0)
                {
                    _extraDelay -= Time.deltaTime;
                    return;
                }
                foreach (var item in _basePatternsList)
                {
                    var bp = item.GetComponent<BasePattern>();
                    if (bp != null)
                        bp.SetEmittorAttack(true);
                }
                _state = TowerState.Attack;
                break;
            case TowerState.Moving:
                foreach (var item in _basePatternsList)
                {
                    var bp = item.GetComponent<BasePattern>();
                    if (bp != null)
                        bp.SetEmittorAttack(false);
                }
                break;
            case TowerState.Skill:
                break;
            case TowerState.Dead:
                break;
            case TowerState.Stop:
                break;
            case TowerState.Attack:
                break;
            case TowerState.Hold:
                break;
            default:
                break;
        }
    }

    //public void FindTarget()
    //{
    //    if (_monsterList == null)
    //        return;

    //    foreach (var monster in _monsterList)
    //    {
    //        var mc = monster.GetComponent<MonsterController>();
    //        if (monster.activeSelf == false || mc == null || mc.CalHp == 0)
    //            continue;

    //        float distance = Utils.Distance(monster.transform.position, this.transform.position);

    //        if (distance < _towerData.range)
    //        {
    //            _target = monster;
    //            break;
    //        }
    //    }
    //}

    //IEnumerator DoAttack()
    //{
    //    var delay = new WaitForSeconds(_towerData.coolTimeA);
    //    while (true)
    //    {
    //        if (_target == null)
    //        {
    //            yield break;
    //        }

    //        float distance = Utils.Distance(this.transform.position, _target.transform.position);
    //        if(distance > _towerData.range)
    //        {
    //            _target = null;
    //            yield break;
    //        }

    //        var mc = _target.GetComponent<MonsterController>();
    //        if(mc != null)
    //        {
    //            if(mc.CalHp == 0 || mc.Hp == 0)
    //            {
    //                _target = null;
    //                _state = TowerState.Idle;
    //                yield break;
    //            }
    //            mc.SetCalHp(_towerData.atk);
    //            //Debug.Log("player" + transform.localPosition + "target" + _target.transform.localPosition + "dist" + distance);
    //            //var projectile = PoolManager.Pools["ProjectilePool"].Spawn("Projectile");
    //            //var pc = projectile.GetComponent<ProjectileController>();
    //            //pc.SetInfo(transform.localPosition, _target, _towerData.projectileA, _towerData, _deckIndex);

    //            var projectile = PoolManager.Pools["ProjectilePool"].Spawn("HomingProjectile");
    //            var pc = projectile.GetComponent<HomingController>();
    //            pc.SetInfo(transform.localPosition, _target, _towerData.projectileA, _towerData, _deckIndex);
    //        }

    //        yield return delay;
    //    }
    //}

    public void ShowRange(bool show)
    {
        if(show == true)
        {
            GetImage((int)Images.RangeImage).gameObject.SetActive(true);
            GetImage((int)Images.RangeImage).rectTransform.sizeDelta = new Vector3(_towerData.range * 2, _towerData.range * 2, 0);
        }
        else
        {
            GetImage((int)Images.RangeImage).gameObject.SetActive(false);
        }
    }

    //public void DoAttack()
    //{
    //    if (_target == null)
    //    {
    //        _state = TowerState.Idle;
    //    }

    //    if (_target != null)
    //    {
    //        if (_target.GetComponent<MonsterController>()._isDead == true) //target champion is alive
    //        {
    //            _target = FindTarget();
    //        }
    //        else
    //        {
    //            if (_isAttacking == false)
    //            {
    //                float distance = Vector2.Distance(this.transform.position, _target.transform.position);

    //                //if we are close enough to attack 
    //                if (distance < _dataTower.attackRange)
    //                {
    //                    DoAttack();
    //                }
    //                else
    //                {
    //                    _target = FindTarget();
    //                }
    //            }
    //        }

    //        //if (GameController.instance.ListMonster.Count == 0)
    //        //    _coAttack = null;
    //        //else
    //        //    _coAttack = StartCoroutine("CoStartAttack");
    //    }
    //}

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    _startPos = transform.position;
    //    GetImage((int)Images.TowerImage).raycastTarget = false;
    //    Managers.Map.DragObjectTest(this.gameObject);
    //    //_startParent = transform.parent;
    //    //transform.SetParent(GameObject.FindGameObjectWithTag("UI Canvas").transform);
    //}







    //public void OnDrag(PointerEventData eventData)
    //{
    //    transform.position = eventData.position;
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    GetImage((int)Images.TowerImage).raycastTarget = true;
    //    if(Managers.Map.DraggedObject != null && Managers.Map.DraggedObject == gameObject)
    //    {
    //        transform.position = _startPos;
    //    }
    //}
}
