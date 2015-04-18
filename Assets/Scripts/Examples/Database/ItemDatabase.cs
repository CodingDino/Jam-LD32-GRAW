// ************************************************************************ 
// File Name:   ItemDatabase.cs 
// Purpose:    	Loads item data to be made available to the game
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;


// ************************************************************************ 
// Class: ItemDatabase
// ************************************************************************ 
public class ItemDatabase : Singleton<ItemDatabase> {


	// ********************************************************************
	// Exposed Data Members 
	// ********************************************************************
	[SerializeField]
	private string[] m_itemDataFiles;
	[SerializeField]
	private string m_itemSpriteSheet;


	// ********************************************************************
	// Private Data Members 
	// ********************************************************************
	private Dictionary<string, Sprite> m_itemSprites = new Dictionary<string, Sprite>();
	private Dictionary<string, ItemData> m_itemData = new Dictionary<string, ItemData>();
	private bool m_initialised = false;

	// ********************************************************************
	// Function:	Start()
	// Purpose:		Run when new instance of the object is created.
	// ********************************************************************
	void Start()
	{
		Initialise();
	}
	
	
	// ********************************************************************
	// Function:	GetItemData()
	// Purpose:		Returns item data for an item
	// ********************************************************************
	public static ItemData GetItemData(string _id)
	{
		instance.Initialise();
		if (instance.m_itemData.ContainsKey(_id))
			return instance.m_itemData[_id];
		else 
		{
			Debug.LogError("ItemDatabase: Attempt to load data for unknown item "+_id);
			return null;
		}
	}
	
	
	// ********************************************************************
	// Function:	Initialise()
	// Purpose:		Loads the item data
	// ********************************************************************
	private void Initialise()
	{
		if (!m_initialised)
		{
			// Load sprites
			Sprite[] sprites = Resources.LoadAll<Sprite>(m_itemSpriteSheet);
			for (int i = 0; i < sprites.Length; ++i)
			{
				m_itemSprites[sprites[i].name] = sprites[i];
			}

			// Load item data
			for (int i = 0; i < m_itemDataFiles.Length; ++i)
			{
				TextAsset file = Resources.Load(instance.m_itemDataFiles[i]) as TextAsset;
				string jsonString = file.text;
				Debug.Log("ItemDatabase: JSON String loaded: " + jsonString);
				
				if (jsonString != "")
				{
					Dictionary<string, object> N = Json.Deserialize(jsonString) as Dictionary<string, object>;
					List<object> itemList = N["items"] as List<object>;
					foreach (object itemEntry in itemList)
					{
						ItemData newItem = ItemData.Load(itemEntry as Dictionary<string, object>);
						if (m_itemSprites.ContainsKey(newItem.icon))
							newItem.iconSprite = m_itemSprites[newItem.icon];
						instance.m_itemData[newItem.id] = newItem;
					}
				}
			}
			
			Debug.Log("ItemDatabase: Loaded " + instance.m_itemData.Count + " items");
			m_initialised = true;
		}
	}
}
