// ************************************************************************ 
// File Name:   Console.cs 
// Purpose:    	A console the player can interact with
// Project:		LD32-GRAW
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using System.Collections;


// ********************************************************************
// Enum: ConsoleType
// ********************************************************************
public enum ConsoleType
{
	NONE = -1,
	// ~~~
	POWER = 0,
	HORN,
	TAIL,
	SPOUT,
	MOUTH,
	EYE,
	FLIPPER,
	// ~~~
	NUM
}


// ************************************************************************ 
// Attributes 
// ************************************************************************ 
[RequireComponent(typeof(Collider2D))]


// ************************************************************************ 
// Class: Console
// ************************************************************************
public class Console : MonoBehaviour 
{
	
	// ********************************************************************
	// Exposed Data Members 
	// ********************************************************************
	[SerializeField]
	private ConsoleType m_consoleType;
	[SerializeField]
	private string[] m_buttonsInteract;
	[SerializeField]
	private SpriteRenderer m_activeTint;
	[SerializeField]
	private GameObject m_consoleDoc;
	[SerializeField]
	private GameObject m_mainDoc;

	
	[Header("Prompts")]
	[SerializeField]
	private Animator m_movementPrompt;
	[SerializeField]
	private Animator m_jumpPrompt;
	[SerializeField]
	private Animator m_enterPrompt;
	[SerializeField]
	private Animator m_exitPrompt;
	[SerializeField]
	private Animator m_consolePrompt;


	// ********************************************************************
	// Exposed Data Members 
	// ********************************************************************
	private bool m_playerInConsole = false;
	private bool m_playerNearConsole = false;


	// ********************************************************************
	// Events 
	// ********************************************************************
	public delegate void ConsoleEntered(bool _enter, Console _console, ConsoleType _consoleType);
	public static event ConsoleEntered OnConsoleEntered;
	
	
	// ********************************************************************
	// Function:	Start()
	// Purpose:		Called on startup.
	// ********************************************************************
	void Start () 
	{
		m_movementPrompt.SetBool("Hidden", false);
		m_jumpPrompt.SetBool("Hidden", false);
	}

	// ********************************************************************
	// Function:	Update()
	// Purpose:		Called once per frame.
	// ********************************************************************
	void Update () 
	{
		// Get input from controller/keyboard
		bool interact = false;
		for (int i = 0; i < m_buttonsInteract.Length; ++i)
		{
			interact = interact || Input.GetButtonDown(m_buttonsInteract[i]);
		}

		if (m_playerInConsole)
		{
			if (interact)
			{
				PlayerExitsConsole();
			}
			else
			{
				ProcessConsoleCommands();
			}
		}
		else
		{

			if (m_playerNearConsole)
			{
				if (interact)
					PlayerEntersConsole();
			}
		}
	}
	
	// ********************************************************************
	// Function:	ProcessConsoleCommands()
	// Purpose:		Process input to console
	// ********************************************************************
	private void ProcessConsoleCommands () 
	{
		// TODO: Check input based on console type
		// TODO: Perform actions based on console input
	}


	// ********************************************************************
	// Function:	PlayerEntersConsole()
	// Purpose:		Actions that happen when the player enters the console
	// ********************************************************************
	private void PlayerEntersConsole () 
	{
		if (OnConsoleEntered != null)
			OnConsoleEntered(true, this, m_consoleType);
		
		Debug.Log("Console entered: "+m_consoleType.ToString());
		
		m_playerInConsole = true;

		// Show player in console
		m_consoleDoc.SetActive(true);
		// Keep player from moving away
		m_mainDoc.SetActive(false);

		// Bring up console prompt
		m_consolePrompt.SetBool("Hidden", false);
		m_exitPrompt.SetBool("Hidden", false);
		m_enterPrompt.SetBool("Hidden", true);
		m_movementPrompt.SetBool("Hidden", true);
		m_jumpPrompt.SetBool("Hidden", true);
	}


	// ********************************************************************
	// Function:	PlayerExitsConsole()
	// Purpose:		Actions that happen when the player exits the console
	// ********************************************************************
	private void PlayerExitsConsole () 
	{
		if (OnConsoleEntered != null)
			OnConsoleEntered(false, this, m_consoleType);
		
		Debug.Log("Console exited: "+m_consoleType.ToString());
		
		m_playerInConsole = false;

		// Hide player in console
		m_consoleDoc.SetActive(false);
		// Release normal player sprite
		m_mainDoc.SetActive(true);
		
		m_consolePrompt.SetBool("Hidden", true);
		m_exitPrompt.SetBool("Hidden", true);
		m_enterPrompt.SetBool("Hidden", false);
		m_movementPrompt.SetBool("Hidden", false);
		m_jumpPrompt.SetBool("Hidden", false);
	}


	// ********************************************************************
	// Function:	OnTriggerEnter2D()
	// Purpose:		Called when this collider encounters another.
	// ********************************************************************
	void OnTriggerEnter2D(Collider2D otherCollider) 
	{
		Debug.Log("Neared console: "+gameObject.name);
		
		m_playerNearConsole = true;
		
		// Highlight console that player is near
		Color tintColor = m_activeTint.color;
		tintColor.a = 1.0f;
		m_activeTint.color = tintColor;
		
		m_enterPrompt.SetBool("Hidden", false);
	}


	// ********************************************************************
	// Function:	OnTriggerExit2D()
	// Purpose:		Called a collider stops colliding with this one.
	// ********************************************************************
	void OnTriggerExit2D(Collider2D otherCollider) 
	{
		Debug.Log("Moved away from console: "+gameObject.name);
		
		m_playerNearConsole = false;
		
		// De-highlight console that player is near
		Color tintColor = m_activeTint.color;
		tintColor.a = 100.0f/255.0f;
		m_activeTint.color = tintColor;
		
		m_enterPrompt.SetBool("Hidden", true);
	}


}
