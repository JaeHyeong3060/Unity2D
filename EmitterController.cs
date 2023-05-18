using ND_VariaBULLET;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using static Utils;

public class EmitterController : BaseController
{
    public ProjectileNeedData _projectileNeedData = new ProjectileNeedData();

    [HideInInspector]
    public BasePattern controller;

    [Tooltip("Overrides the fired shot's sprite.")]
    public Sprite SpriteOverride;

    [Tooltip("Ignores any rate scaling that has been set in GlobalShotManager.")]
    public bool IgnoreGlobalRateScale;

    [Range(200, 1)]
    [Tooltip("Sets rate of shots. [Lower number = more shots].")]
    public int ShotRate;
    private Timer shotRateCounter = new Timer(0);

    [Range(-180, 180)]
    [Tooltip("Sets local rotation for this emitter.")]
    public float LocalPitch;

    public float _attackRate;
    public TowerData _towerData;
    public PatternData _patternData;
    public ProjectileData _projData;
    public int _deckIndex;

    public GameObject _projectile;
    public GameObject _target;

    public List<GameObject> _monsterList;

    bool _canAttack;

    Coroutine _coAttack;
    Coroutine _coAttackDelay;

    private int increment;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        increment = 0;

        _monsterList = Managers.UI.FindPopup<UI_GamePlayPopup>().MonsterList;
        SetAttack(true);

        return true;
    }

    public void FindTarget()
    {
        if (_monsterList == null || _towerData == null || _projData == null)
            return;

        _target = null;

        foreach (var monster in _monsterList)
        {
            var mc = monster.GetComponent<MonsterController>();
            if (monster.activeSelf == false || mc == null || mc.CalHp == 0)
                continue;

            float distance = Utils.Distance(monster.transform.position, this.transform.position);

            if (distance < _towerData.range)
            {
                _target = monster;
                break;
            }
        }
    }

    public void InstantiateShot()
    {
        if (_projectile == null || _projData == null)
            return;

        increment++;

        int upgradeValue = Managers.Game.GetTowerIngameUpgrade(_deckIndex);
        int damage = _towerData.atk + (int)(_towerData.atk * upgradeValue * Define.TOWER_UPGRADE_ATK_RATE);

        switch (_projData.effectType)
        {
            case ProjectileEffectType.None:
                break;
            case ProjectileEffectType.SlowAoe:
                damage = 0;
                break;
        }

        var projectile = PoolManager.Pools["ProjectilePool"].Spawn(_projectile);
        var pc = projectile.GetComponent<ProjectileController>();
        pc.Trajectory = this.angleToPercentage();

        pc.Trajectory.x *= 0.2f;
        pc.Trajectory.y *= 0.2f;

        pc.homingEngageTime = _projData.engageTime;

        _projectileNeedData.pos = transform.position;
        _projectileNeedData.target = _target;
        _projectileNeedData.projectileData = _projData;
        _projectileNeedData.towerData = _towerData;
        _projectileNeedData.patternData = _patternData;
        _projectileNeedData.deckIndex = _deckIndex;
        _projectileNeedData.damage = damage;
        _projectileNeedData.shotCount = increment;

        pc.SetInfo(_projectileNeedData);

        var mc = _target.GetComponent<MonsterController>();
        mc.SetCalHp(damage);

        pc.sortOrder = increment - 9999;
    }

    public void SetInfo(List<GameObject> monsterList, TowerData towerData, PatternData patternData, int deckIndex, float coolTime)
    {
        _monsterList = monsterList;
        _towerData = towerData;
        _patternData = patternData;
        _projectile = Managers.Resource.Load<GameObject>($"Prefabs/{_patternData.projectilePath}");
        Managers.Data.Projectile.TryGetValue(_patternData.projectileType, out _projData);
        _deckIndex = deckIndex;
        _attackRate = coolTime;

        increment = 0;
    }

    public void SetAttack(bool isOn)
    {
        if (isOn == true)
        {
            if (_coAttack != null)
                StopCoroutine(_coAttack);
            if (_coAttackDelay != null)
                StopCoroutine(_coAttackDelay);
            _canAttack = true;
            _coAttackDelay = StartCoroutine(ChangeAttackDelay());
        }
        else
        {
            if (_coAttack != null)
                StopCoroutine(_coAttack);
            _canAttack = false;
        }
    }

    IEnumerator ChangeAttackDelay()
    {
        yield return new WaitForSeconds(_attackRate);
        if (_canAttack)
        {
            if (_coAttack != null)
                StopCoroutine(_coAttack);
            _coAttack = StartCoroutine(DoAttack());
        }
    }

    IEnumerator DoAttack()
    {
        var delay = new WaitForSeconds(_attackRate);
        var findTargetDelay = new WaitForSeconds(0.1f);
        while (true)
        {
            FindTarget();
            if (_target != null)
            {
                InstantiateShot();
                yield return delay;
            }
            else
            {
                yield return findTargetDelay;
            }
        }
    }

    private Vector2 angleToPercentage()
    {
        int globalDirection = (transform.lossyScale.x < 0) ? -1 : 1;
        float angle = Mathf.Abs(transform.rotation.eulerAngles.z); //absolute value fixes negative value at -360

        return CalcObject.RotationToShotVector(angle) * globalDirection;
    }
}
