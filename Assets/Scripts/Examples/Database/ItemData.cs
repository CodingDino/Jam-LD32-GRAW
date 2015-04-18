// ************************************************************************ 
// File Name:   ItemData.cs 
// Purpose:    	Data for an item.
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


// ************************************************************************ 
// Enum: ItemType
// ************************************************************************
[Flags]
public enum ItemType {
	NONE 		= 0,
	// ~~~
	MATERIAL 	= 1 << 0,
	BLUEPRINTS 	= 1 << 1,
	CART 		= 1 << 2,
	CARGO 		= 1 << 3,
	PASSENGER 	= 1 << 4,
	WEAPON 		= 1 << 5,
	COMPONENT 	= 1 << 6,
	CONSUMABLE 	= 1 << 7,
	// ~~~
}


// ************************************************************************ 
// Class: ItemData
// ************************************************************************
public class ItemData {
	
	public string id;
	public string displayName;
	public string icon;
	public string description;
	public float value;
	public ItemType type;
	public Sprite iconSprite;
	
	public void Copy(ItemData _data)
	{
		id = _data.id;
		displayName = _data.displayName;
		icon = _data.icon;
		description = _data.description;
		value = _data.value;
		type = _data.type;
		iconSprite = _data.iconSprite;
	}

	static public ItemData Load(Dictionary<string, object> JSON, ItemData defaults = null)
	{
		ItemData newObject;
		if (defaults == null)
			newObject = new ItemData();
		else
			newObject = defaults.MemberwiseClone() as ItemData;
		
		if (JSON.ContainsKey("id"))
			newObject.id = JSON["id"] as string;
		if (JSON.ContainsKey("displayName"))
			newObject.displayName = JSON["displayName"] as string;
		if (JSON.ContainsKey("icon"))
			newObject.icon = JSON["icon"] as string;
		else 
			newObject.icon = newObject.id;
		if (JSON.ContainsKey("description"))
			newObject.description = JSON["description"] as string;
		if (JSON.ContainsKey("value"))
			newObject.value = float.Parse(JSON["value"].ToString());
		if (JSON.ContainsKey("type"))
			newObject.type = (ItemType) Enum.Parse(typeof(ItemType), JSON["type"].ToString());

		return newObject;
	}
	
	public override string ToString()
	{
		return      "id = " + id 
				+ ", displayName = " + displayName 
				+ ", icon = " + icon 
				+ ". description = " + description
				+ ". value = " + value
				+ ". type = " + type;
	}
}