using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using static Define;

public interface ILoader<Key, Item>
{
    Dictionary<Key, Item> MakeDic();
    bool Validate();
}

public class DataManager
{
    public StartData Start { get; private set; }
	public Dictionary<int, ShopData> Shops { get; private set; }
    public Dictionary<int, TextData> Texts { get; private set; }
	public Dictionary<int, PlayerData> Players { get; private set; }
	public Dictionary<int, TowerData> Tower { get; private set; }
	public Dictionary<int, MonsterData> Monster { get; private set; }
	public Dictionary<ProjectileType, ProjectileData> Projectile { get; private set; }
	public Dictionary<PatternType, PatternData> Pattern { get; private set; }
	public Dictionary<MoneyType, RewardData> RewardInfo { get; private set; }
    public Dictionary<SuperSkillType, SuperSkillData> SuperSkill { get; private set; }
	public List<StageData> StageInfoList { get; private set; }
	public List<InvenData> InvenData { get; private set; }
	public List<LaboData> LaboData { get; private set; }
	public List<SlotData> SlotData { get; private set; }
	public Dictionary<int, QuestData> Quest { get; private set; }
	public Dictionary<int, GoHomeData> GoHomes { get; private set; }
    public Dictionary<int, EndingData> Endings { get; private set; }

	public Dictionary<int, CollectionData> Collections { get; private set; }
	public List<CollectionData> StatCollections { get; private set; }
	public List<CollectionData> WealthCollections { get; private set; }
	public List<CollectionData> LevelCollections { get; private set; }
	public List<CollectionData> ProjectCollections { get; private set; }
	public List<CollectionData> BattleCollections { get; private set; }

	public Dictionary<int, DialogueEventData> Dialogues { get; private set; }

	public void Init()
    {
        Start = LoadSingleXml<StartData>("StartData");

		Shops = LoadXml<ShopDataLoader, int, ShopData>("ShopData").MakeDic();
		Texts = LoadXml<TextDataLoader, int, TextData>("TextData").MakeDic();
        Players = LoadXml<PlayerDataLoader, int, PlayerData>("PlayerData").MakeDic();
        Tower = LoadXml<TowerDataLoader, int, TowerData>("TowerData").MakeDic();
        Monster = LoadXml<MonsterDataLoader, int, MonsterData>("MonsterData").MakeDic();
		StageInfoList = LoadXml<StageDataLoader, int, StageData>("StageData")._stageData;
		Projectile = LoadXml<ProjectileDataLoader, ProjectileType, ProjectileData>("ProjectileData").MakeDic();
		Pattern = LoadXml<PatternDataLoader, PatternType, PatternData>("PatternData").MakeDic();
		RewardInfo = LoadXml<RewardDataLoader, MoneyType, RewardData>("RewardData").MakeDic();
		InvenData = LoadXml<InvenDataLoader, int, InvenData>("InvenData")._invenData;
		SlotData = LoadXml<SlotDataLoader, int, SlotData>("SlotData")._slotData;
		LaboData = LoadXml<LaboDataLoader, int, LaboData>("LaboData")._laboData;
		Quest = LoadXml<QuestDataLoader, int, QuestData>("QuestData").MakeDic();
		SuperSkill = LoadXml<SuperSkillDataLoader, SuperSkillType, SuperSkillData>("SuperSkillData").MakeDic();

		// Collection
		var collectionLoader = LoadXml<CollectionDataLoader, int, CollectionData>("CollectionData");
		StatCollections = collectionLoader._collectionData.Where(c => c.type == CollectionType.Stat).ToList();
		WealthCollections = collectionLoader._collectionData.Where(c => c.type == CollectionType.Wealth).ToList();
		LevelCollections = collectionLoader._collectionData.Where(c => c.type == CollectionType.Level).ToList();
		ProjectCollections = collectionLoader._collectionData.Where(c => c.type == CollectionType.Project).ToList();
		BattleCollections = collectionLoader._collectionData.Where(c => c.type == CollectionType.Battle).ToList();

		Collections = collectionLoader.MakeDic();

		// Dialogue
		var dialogueLoader = LoadXml<DialogueEventDataLoader, int, DialogueEventData>("DialogueEventData");  
        Dialogues = dialogueLoader.MakeDic();
    }

	private Item LoadSingleXml<Item>(string name)
	{
		XmlSerializer xs = new XmlSerializer(typeof(Item));
		TextAsset textAsset = Resources.Load<TextAsset>("Data/" + name);
		using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(textAsset.text)))
			return (Item)xs.Deserialize(stream);
	}

	private Loader LoadXml<Loader, Key, Item>(string name) where Loader : ILoader<Key, Item>, new()
    {
        XmlSerializer xs = new XmlSerializer(typeof(Loader));
        TextAsset textAsset = Resources.Load<TextAsset>("Data/" + name);
        using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(textAsset.text)))
            return (Loader)xs.Deserialize(stream);
    }
}
