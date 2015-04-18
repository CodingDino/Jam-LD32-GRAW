// ************************************************************************ 
// File Name:   PlayerProfile.cs 
// Purpose:    	Saves data to player prefs
// Project:		Framework
// Author:      Sarah Herzog  
// Copyright: 	2014 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

// ************************************************************************ 
// Class: ProfileManager
// ************************************************************************ 
public class ProfileManager : Singleton<ProfileManager> {
	
	// ********************************************************************
	// Private Data Members 
	// ********************************************************************
	[SerializeField]
	private PlayerProfile m_profile = new PlayerProfile();
	[SerializeField]
	private string m_version = "0.1";
	[SerializeField]
	private bool m_autoLoad = false;

	private bool m_loadedData = false;

	// ********************************************************************
	// Properties 
	// ********************************************************************
	public static PlayerProfile profile { 
		get {
			if (instance.m_profile == null)
				instance.m_profile = new PlayerProfile();
			return instance.m_profile;
		} 
		set {
			instance.m_profile = value;
		}
	}
	public static bool loadedData { 
		get {
			if (instance == null)
				return false;
			else
				return instance.m_loadedData;
		}
	}
	
	
	// ********************************************************************
	public static void Load(string saveID = "")
	{
		if (saveID == "")
			saveID = profile.saveID;
		
		profile = new PlayerProfile();

		string jsonString = PlayerPrefs.GetString (saveID);
		Debug.Log ("JSON String loaded: "+jsonString);

		if (jsonString != "")
		{
			Dictionary<string,object> N = Json.Deserialize(jsonString) as Dictionary<string,object>;
			string version = N["version"] as string;
			Dictionary<string,object> data = N["data"] as Dictionary<string,object>;

			// Load data
		}

		instance.m_loadedData = true;

		Save ();
	}
	// ********************************************************************


	// ********************************************************************
	public static void Save(string saveID = "")
	{
		if (saveID == "")
			saveID = profile.saveID;

		Dictionary<string,object> N = new Dictionary<string,object>();
		N["version"] = instance.m_version;
		
		Dictionary<string,object> data = new Dictionary<string,object>();

		// Save data

		N["data"] = data;


		PlayerPrefs.SetString(saveID, Json.Serialize(N));
		PlayerPrefs.Save();
		Debug.Log ("JSON String saved: "+Json.Serialize(N));
	}
	// ********************************************************************

	
	// ********************************************************************
	public static void Clear(string saveID)
	{
		PlayerPrefs.DeleteKey(saveID);
	}
	// ********************************************************************

	
	// ********************************************************************
	public static void ClearAll()
	{
		PlayerPrefs.DeleteAll();
	}
	// ********************************************************************

	
	// ********************************************************************
	void Awake()
	{
		if (m_autoLoad) Load ();
	}
	// ********************************************************************

}

// ************************************************************************ 
// Attributes 
// ************************************************************************ 
[System.Serializable]


// ************************************************************************ 
// Class: PlayerProfile
// ************************************************************************ 
public class PlayerProfile {

	public string saveID = "SaveSlot01";
	public string version = "0.1";
    
}