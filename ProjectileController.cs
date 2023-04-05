using PathologicalGames;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using static Utils;

public class ProjectileController : BaseController
{
	enum SpriteRenderers
	{
		ProjSprite,
	}

	enum Animators
	{
		ProjSprite,
    }

	[HideInInspector]
	public float ShotSpeed;

	[Tooltip("Ignores any speed scaling that has been set in GlobalShotManager.")]
	public bool IgnoreGlobalSpeedScale;
	protected float scale;

	[Tooltip("Sets damage amount produced when this shot collides with an object that has a ShotCollisionDamage script attached.")]
	public float DamagePerHit = 1;
	public float DMG { get { return DamagePerHit; } }

	[Range(2, 60)]
	[Tooltip("Sets the time in frames at which shots start homing on a target after being fired.")]
	public int homingEngageTime;
	public Timer homingEngage = new Timer(0);

	[HideInInspector]
	public float scaledSpeed;

	[HideInInspector]
	public Vector2 Trajectory;

	[HideInInspector]
	public string sortLayer;

	[HideInInspector]
	public int sortOrder;

	public ProjectileNeedData _projectileNeedData;

	public ProjectileState _projState;

	public ProjectileData _projData;

	public TowerData _towerData;

	public PatternData _patternData;

	public GameObject _target;

	public int _deckIndex;

	protected virtual void OnOutBounds()
	{
		if (CalcObject.IsOutBounds(transform))
			SetDead();
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindSprite(typeof(SpriteRenderers));
		Bind<Animator>(typeof(Animators));

		RefreshUI();

		return true;
	}
	public virtual void Update()
	{
		scale = (IgnoreGlobalSpeedScale) ? 1 : Managers.Play.ManagerTimeScale;
		scaledSpeed = _projData.speed * scale;
	}

    //void UpdatePos()
    //{
    //    if (_target == null || _target.activeSelf == false)
    //    {
    //        SetDead();
    //        return;
    //    }


    //    Vector3 moveDir = _target.transform.position - transform.position;
    //    transform.position += moveDir.normalized * _projData.speed * Time.deltaTime;

    //    float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
    //    GetSprite((int)SpriteRenderers.ProjSprite).transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    //    float distance = Vector2.Distance(this.transform.position, _target.transform.position);
    //    if (distance < 0.1f)
    //    {


    //    }
    //}

    public virtual void OnTriggerEnter2D(Collider2D collision)
	{
		var mc = collision.GetComponent<MonsterController>();

		if (mc != null && mc.gameObject == _target)
		{
			switch (_projData.effectType)
			{
				case ProjectileEffectType.SlowAoe:
					//Managers.Play.SkillAoe(transform.position, _damage, _projData);
					break;
				case ProjectileEffectType.Aoe:
					//Managers.Play.SkillAoe(transform.position, _damage, _projData);
					break;
				default:
					int upgradeValue = Managers.Game.GetTowerIngameUpgrade(_deckIndex);
					mc.TakeDamage(_damage, _projData);
					break;
			}

			Transform projectile = null;
			if(_projData.hitEventType != ProjectileHitEventType.None)
            {
				projectile = PoolManager.Pools["ProjectilePool"].Spawn(_projData.hitEventType.ToString());
				var pfc = projectile.GetComponent<ProjectileFieldController>();
				pfc.SetInfo(_projectileNeedData, transform.position);
			}

			SetDead();
		}
	}

	public virtual void SetDead()
    {
		PoolManager.Pools["ProjectilePool"].Despawn(this.transform);
    }
	public virtual void RefreshUI()
	{
		if (_init == false)
			return;

		if (Get<Animator>((int)Animators.ProjSprite).HasState(0, animHash))
		{
			Get<Animator>((int)Animators.ProjSprite).Play(animHash);
		}
	}

	public int animHash;
	public int _damage;
	public virtual void SetInfo(ProjectileNeedData projectileNeedData)
	{
		_projectileNeedData = projectileNeedData;

		transform.position = projectileNeedData.pos;
		_target = projectileNeedData.target;
		_projData = projectileNeedData.projectileData;
		_towerData = projectileNeedData.towerData;
		_patternData = projectileNeedData.patternData;
		_deckIndex = projectileNeedData.deckIndex;
		_damage = projectileNeedData.damage;
		
		transform.rotation = CalcObject.VectorToRotationSlerp(transform.rotation, Trajectory, 100f);

		animHash = Animator.StringToHash(projectileNeedData.projectileData.animName);

		UpdateAnimation();

		RefreshUI();
	}
}
