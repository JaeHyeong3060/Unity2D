using DG.Tweening;
using PathologicalGames;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class MonsterController : BaseController
{
    enum Images
    {
        MonsterImage,
        Gauge,
    }

    enum GameObjects
    {
        HpBar,
    }
    enum DOTweenAnimations
    {
        MonsterImage,
    }

    public Action<Transform> _deadAction;
    public Action _bossAction;

    [SerializeField] public Vector3[] _destPos;

    const float EPSILLON = 20.0f;
    protected int _moveCnt;

    public MonsterData MonsterData => _monsterData;
    private MonsterData _monsterData;

    public int CalHp { get { return _calHp; } set { _calHp = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }

    // projectile이 발사 되는 순간 계산되는 hp
    [SerializeField]
    protected int _calHp = 0;

    // 실제 탄환이 도착하면 적용 되는 hp
    [SerializeField]
    protected int _hp = 0;
    protected int _maxHp = 0;

    protected GameObject _target;

    protected Coroutine _coAttack;

    [SerializeField]
    protected int _speed;
    public int MoveSpeed { get { return _speed; } set { _speed = value; } }

    protected float _slowTime;
    protected float _woundTime;
    protected float _woundValue;

    protected bool _isSlow = false;

    protected MonsterState _state;

    Material _oldMaterial;
    bool isDamageEffect = false;

    public MonsterState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;
            _state = value;
        }
    }
    public void ClearAll()
    {
        _moveCnt = 0;
        _speed = 0;
        _slowTime = 0;
        _woundTime = 0;
        _woundValue = 0;
        _isSlow = false;
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindObject(typeof(GameObjects));
        Bind<DOTweenAnimation>(typeof(DOTweenAnimations));

        //GetImage((int)Images.Image).gameObject.SetActive(true);

        GetObject((int)GameObjects.HpBar).gameObject.SetActive(false);

        RefreshUI();

        return true;
    }

    public virtual void SetInfo(Action<Transform> deadAction, Vector3[] destPos, MonsterData monsterData, int scaledMaxhp)
    {
        ClearAll();

        _deadAction = deadAction;
        _monsterData = monsterData;
        _maxHp = scaledMaxhp;
        _hp = scaledMaxhp;
        _calHp = scaledMaxhp;
        _destPos = destPos;
        transform.localPosition = destPos[0];
        _speed = _monsterData.speed;

        isDamageEffect = false;

        RefreshUI();
    }


    public virtual void RefreshUI()
    {
        if (_init == false)
            return;

        GetImage((int)Images.MonsterImage).material = Managers.Resource.Load<Material>("Sprites/Material/DoodleDraw");
        GetImage((int)Images.MonsterImage).sprite = Managers.Resource.Load<Sprite>(_monsterData.iconPath);
        GetImage((int)Images.MonsterImage).SetNativeSize();


        

        RefreshHp();
    }

    public void RefreshHp()
    {
        float ratio = Mathf.Clamp(((float)_hp / _maxHp), 0, 1);

        GetImage((int)Images.Gauge).transform.localScale = new Vector3(ratio, 1, 1);

        if (ratio >= 1)
        {
            GetObject((int)GameObjects.HpBar).gameObject.SetActive(false);
        }

        if (_monsterData.type == MonsterType.Boss)
        {
            _bossAction?.Invoke();
        }
    }

    public void LookLeftImage(bool flag)
    {
        Vector3 scale = transform.localScale;
        if (flag)
            GetImage((int)Images.MonsterImage).transform.localScale = new Vector3(-Math.Abs(scale.x), scale.y, scale.z);
        else
            GetImage((int)Images.MonsterImage).transform.localScale = new Vector3(Math.Abs(scale.x), scale.y, scale.z);
    }

    public virtual void FixedUpdate()
    {
        if(_isSlow && _slowTime < 0)
        {
            _isSlow = false;
            _speed = _monsterData.speed;
        }
        else
        {
            _slowTime -= Time.deltaTime;
        }

        UpdatePos();
    }

    protected virtual void UpdatePos()
    {
        Vector3 dir = _destPos[_moveCnt] - transform.localPosition;

        // 목표 지점에 도착
        if (dir.magnitude < EPSILLON)
        {
            _moveCnt = ++_moveCnt % 4;
            Vector3 nextDir = _destPos[_moveCnt] - transform.localPosition;
            nextDir = nextDir.normalized;
            Vector3 roundedVector = new Vector3(Mathf.RoundToInt(nextDir.x), Mathf.RoundToInt(nextDir.y), Mathf.RoundToInt(nextDir.z));
            if (Vector3.left == roundedVector)
            {
                LookLeftImage(true);
            }
            else if (Vector3.right == roundedVector)
            {
                LookLeftImage(false);
            }

            return;
        }

        // 이동
        dir = dir.normalized;

        transform.localPosition += _speed * dir * Time.deltaTime;
        return;
    }

    public virtual void TakeDamage(int damage, ProjectileData projData)
    {
        if (gameObject == null || gameObject.activeSelf == false)
            return;

        Get<DOTweenAnimation>((int)DOTweenAnimations.MonsterImage).DORestart();

        if(isDamageEffect == false)
            StartCoroutine(MaterialEffect(0.2f));

        _hp = Mathf.Max(0, _hp - damage);
        Utils.SpawnDamage(damage, transform.position);

        switch (projData.effectType)
        {
            case ProjectileEffectType.None:
                break;
            case ProjectileEffectType.Slow:
                _slowTime = projData.effectTime;
                _isSlow = true;
                _speed = Math.Min(_speed, (int)(_monsterData.speed * projData.effectValue));
                break;
            case ProjectileEffectType.SlowAoe:
                _slowTime = projData.effectTime;
                _isSlow = true;
                _speed = Math.Min(_speed, (int)(_monsterData.speed * projData.bombTime));
                break;
            case ProjectileEffectType.Stop:
                _slowTime = projData.effectTime;
                _isSlow = true;
                _speed = 0;
                break;
            case ProjectileEffectType.Wound:
                _woundTime = projData.effectTime;
                _woundValue = Math.Max(_woundValue, projData.effectValue);
                break;
            default:
                break;
        }

        int _woundDamage = (int)(damage * _woundValue);
        if (_woundDamage != 0)
        {
            _hp = Mathf.Max(0, _hp - _woundDamage);
            Utils.SpawnDamage(_woundDamage, transform.position + Define.WoundTextPos, Color.red);
        }

        if(_hp != 0)
        {
            GetObject((int)GameObjects.HpBar)?.gameObject.SetActive(true);
            RefreshHp();
        }

        if (_hp <= 0)
        {
            SetDead();
            return;
        }

    }

    public int SetCalHp(int damage)
    {
        _calHp = Mathf.Max(0, _calHp - damage);
        int _woundDamage = (int)(damage * _woundValue);
        if (_woundDamage != 0)
        {
            _calHp = Mathf.Max(0, _calHp - _woundDamage);
        }
        return _calHp;
    }

    public void SetDead()
    {
        ClearAll();
        if(gameObject.activeSelf)
            _deadAction?.Invoke(this.transform);
    }
    IEnumerator MaterialEffect(float delay)
    {
        isDamageEffect = true;
        _oldMaterial = GetImage((int)Images.MonsterImage).material;
        GetImage((int)Images.MonsterImage).material = Managers.Resource.Load<Material>("Sprites/Material/ColorWhite");
        yield return new WaitForSeconds(delay);
        GetImage((int)Images.MonsterImage).material = _oldMaterial;
        isDamageEffect = false;
    }

}
