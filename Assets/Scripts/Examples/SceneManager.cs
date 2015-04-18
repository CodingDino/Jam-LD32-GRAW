// ************************************************************************ 
// File Name:   SceneManager.cs 
// Purpose:    	
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


// ************************************************************************ 
// Attributes 
// ************************************************************************ 


// ************************************************************************ 
// Class: SceneManager
// ************************************************************************ 
public class SceneManager : MonoBehaviour {


	// ********************************************************************
	// Exposed Data Members 
	// ********************************************************************
	[SerializeField]
	private bool m_loadOnStart = true;
	[SerializeField]
	private string m_levelToLoad = "";


	// ********************************************************************
	// Private Data Members 
	// ********************************************************************
	private bool m_initialized = false;

	
	// ********************************************************************
	// Static Data Members 
	// ********************************************************************
	private static SceneManager s_instance;
	

	// ********************************************************************
	// Properties 
	// ********************************************************************
	public static SceneManager instance { 
		get {
			// Check if the instance already exists in the scene
			if (s_instance == null)
				s_instance = (SceneManager)FindObjectOfType(typeof(SceneManager));
			
			return s_instance;
		} 
	}


	// ********************************************************************
	public static void LoadLevel(string levelToLoad)
	{
		// TODO: Load appropriate zone's music based on zone data
		MusicManager.PlayMusic("Music/Levels/Default");

		// TODO: Loading screen, pass correct information to level screen
		Application.LoadLevel(levelToLoad);
	}
	// ********************************************************************

	// ********************************************************************
	public static void LoadTown(string townToLoad)
	{
		// TODO: Load appropriate zone's music based on zone data
		MusicManager.PlayMusic("Music/Towns/Default");

		// TODO: Loading screen, pass correct information to town screen
		Application.LoadLevel("Station");
	}
	// ********************************************************************
	
	// ********************************************************************
	public static void LoadMap(string zone)
	{
		// TODO: Load appropriate zone's music based on zone data
		MusicManager.PlayMusic("Music/Zones/Default");

		// TODO: Loading screen (map should center on current town)
		Application.LoadLevel("WorldMap");
	}
	// ********************************************************************
	
	// ********************************************************************
	void Update()
	{
		if (!m_initialized)
		{
			m_initialized = true;
			if (m_loadOnStart) Application.LoadLevel(m_levelToLoad);
		}
	}
	// ********************************************************************

}