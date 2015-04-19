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
using System.Collections.Generic;


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
	[SerializeField]
	private List<Animator> m_specialPromptAnimators;
	[SerializeField]
	private List<Animation> m_specialPromptAnimations;
	[SerializeField]
	private TextMesh m_promptText;


	// ********************************************************************
	// Exposed Data Members 
	// ********************************************************************
	private bool m_acceptConsoleInput = false;
	private bool m_playerInConsole = false;
	private bool m_playerNearConsole = false;
	private string m_currentButton = "A";
	private int m_consoleStage = 0;


	// ********************************************************************
	// Events 
	// ********************************************************************
	public delegate void ConsoleEntered(bool _enter, Console _console, ConsoleType _consoleType);
	public static event ConsoleEntered OnConsoleEntered;
	
	
	// ********************************************************************
	// Function:	OnEnable()
	// Purpose:		Called when the script is enabled.
	// ********************************************************************
	void OnEnable() 
	{ 
		AnimationListener.OnAnimationEvent += OnAnimationEvent; 
	}
	
	
	// ********************************************************************
	// Function:	OnDisable()
	// Purpose:		Called when the script is disabled.
	// ********************************************************************
	void OnDisable() 
	{ 
		AnimationListener.OnAnimationEvent -= OnAnimationEvent; 
	}


	// ********************************************************************
	// Function:	Start()
	// Purpose:		Called on startup.
	// ********************************************************************
	void Start () 
	{
		m_movementPrompt.SetBool("Hidden", false);
		m_jumpPrompt.SetBool("Hidden", false);
		SetConsoleDefaults();

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

		bool consoleInput;

		switch (m_consoleType)
		{
		case ConsoleType.POWER :
			consoleInput = Input.GetButtonDown("Jump");
			if (consoleInput)
			{
				// Show pulse
				m_specialPromptAnimations[0].Play();
				
				// TODO: Play good sound

				ExecuteConsoleCommand(m_consoleType);
			}
			break;
		case ConsoleType.TAIL :
			consoleInput = Input.GetButtonDown(m_currentButton);
			if (consoleInput)
			{
				if (m_acceptConsoleInput)
				{
					// Show pulse
					m_specialPromptAnimations[0].Play();

					// TODO: Play good sound

					if (m_currentButton == "A")
					{
						m_currentButton = "D";
						m_specialPromptAnimators[0].SetBool("Disabled",true);
						m_specialPromptAnimators[1].SetBool("Disabled",false);
					}
					else
					{
						m_currentButton = "A";
						m_specialPromptAnimators[0].SetBool("Disabled",false);
						m_specialPromptAnimators[1].SetBool("Disabled",true);
					}
					m_acceptConsoleInput = false;
					ExecuteConsoleCommand(m_consoleType);
				}
				else
				{
					// TODO: Show red tint
					// TODO: Play bad sound
				}
			}
			break;
		case ConsoleType.HORN :
			consoleInput = Input.GetButtonDown("Jump");
			if (consoleInput)
			{
				ExecuteConsoleCommand(m_consoleType);
				if (m_consoleStage == 0)
				{
					// Show pulse
					m_specialPromptAnimations[0].Play();
					
					// TODO: Play good sound
					
					m_promptText.text = "Fire";

					m_consoleStage = 1;
				}
				else
				{
					// Show pulse
					m_specialPromptAnimations[0].Play();
					
					// TODO: Play good sound


					PlayerExitsConsole();
					m_promptText.text = "Aim";
				}
			}
			break;
		case ConsoleType.MOUTH :
			consoleInput = Input.GetButtonDown(m_currentButton);
			if (consoleInput)
			{
				// Show pulse
				m_specialPromptAnimations[m_consoleStage].Play();
				
				// TODO: Play good sound

				if (m_currentButton == "A")
				{
					m_consoleStage = 1;
					m_currentButton = "W";
					m_specialPromptAnimators[0].SetBool("Disabled",true);
					m_specialPromptAnimators[1].SetBool("Disabled",false);
					m_specialPromptAnimators[2].SetBool("Disabled",true);
				}
				else if (m_currentButton == "W")
				{
					m_consoleStage = 2;
					m_currentButton = "D";
					m_specialPromptAnimators[0].SetBool("Disabled",true);
					m_specialPromptAnimators[1].SetBool("Disabled",true);
					m_specialPromptAnimators[2].SetBool("Disabled",false);
				}
				else
				{
					m_consoleStage = 0;
					m_currentButton = "A";
					m_specialPromptAnimators[0].SetBool("Disabled",false);
					m_specialPromptAnimators[1].SetBool("Disabled",true);
					m_specialPromptAnimators[2].SetBool("Disabled",true);
				}
				ExecuteConsoleCommand(m_consoleType);
			}
			break;
		case ConsoleType.SPOUT :
			if (m_acceptConsoleInput)
			{
				consoleInput = Input.GetButtonDown("Jump");
				if (consoleInput)
				{
					ExecuteConsoleCommand(m_consoleType);

					// TODO: Play good sound

					// Show pulse
					m_specialPromptAnimations[m_consoleStage].Play();

					SetConsoleDefaults();
				}
			}
			else
			{
				consoleInput = Input.GetButtonDown(m_currentButton);
				if (consoleInput)
				{
					ExecuteConsoleCommand(m_consoleType);

					// Show pulse
					m_specialPromptAnimations[m_consoleStage].Play();
					
					// TODO: Play good sound
					
					if (m_currentButton == "A")
					{
						m_consoleStage = 1;
						m_currentButton = "D";
						m_specialPromptAnimators[0].SetBool("Disabled",true);
						m_specialPromptAnimators[1].SetBool("Disabled",false);
					}
					else
					{
						m_consoleStage = 0;
						m_currentButton = "A";
						m_specialPromptAnimators[0].SetBool("Disabled",false);
						m_specialPromptAnimators[1].SetBool("Disabled",true);
					}
				}

				// TODO: If they press other button, play bad sound and red tint
			}
			break;
		case ConsoleType.EYE :
			consoleInput = Input.GetButtonDown("Jump");
			m_consoleStage = consoleInput ? 1 : 0;
			ExecuteConsoleCommand(m_consoleType);
			break;
		default :
			m_acceptConsoleInput = true;
			break;
		}

	}


	// ********************************************************************
	// Function:	ExecuteConsoleCommand()
	// Purpose:		Execute command for console type
	// ********************************************************************
	private void ExecuteConsoleCommand (ConsoleType _type) 
	{
		Debug.Log("Executing console command: "+_type.ToString());
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

		// Console defaults
		SetConsoleDefaults();
	}


	// ********************************************************************
	// Function:	SetConsoleDefaults()
	// Purpose:		Sets up the defaults for each console
	// ********************************************************************
	private void SetConsoleDefaults () 
	{
		switch (m_consoleType)
		{
		case ConsoleType.TAIL :
			m_acceptConsoleInput = false;
			m_specialPromptAnimators[0].SetBool("Disabled",false);
			m_specialPromptAnimators[1].SetBool("Disabled",true);
			m_currentButton = "A";
			break;
		case ConsoleType.HORN :
			m_consoleStage = 0;
			m_promptText.text = "Aim";
			break;
		case ConsoleType.MOUTH :
			m_consoleStage = 0;
			m_currentButton = "A";
			m_specialPromptAnimators[0].SetBool("Disabled",false);
			m_specialPromptAnimators[1].SetBool("Disabled",true);
			m_specialPromptAnimators[2].SetBool("Disabled",true);
			break;
		case ConsoleType.SPOUT :
			m_specialPromptAnimators[0].SetBool("Disabled",false);
			m_specialPromptAnimators[1].SetBool("Disabled",true);
			m_specialPromptAnimators[0].gameObject.SetActive(true);
			m_specialPromptAnimators[1].gameObject.SetActive(true);
			m_specialPromptAnimators[2].gameObject.SetActive(false);
			m_promptText.text = "Loosen Valve";
			m_consoleStage = 0;
			m_currentButton = "A";
			break;
		default :
			m_acceptConsoleInput = true;
			break;
		}
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
	
	
	// ********************************************************************
	// Function:	OnAnimationEvent()
	// Purpose:		Called when an animation event is triggered.
	// ********************************************************************
	void OnAnimationEvent(GameObject animationObject, string eventID) 
	{
		if (m_consoleType == ConsoleType.TAIL)
		{
			if (eventID == "metronome-enter")
			{
				m_acceptConsoleInput = true;

				if (m_currentButton == "A")
					m_specialPromptAnimators[0].SetBool("Highlighted",true);
				else
					m_specialPromptAnimators[1].SetBool("Highlighted",true);
			}
			else if (eventID == "metronome-exit")
			{
				m_acceptConsoleInput = false;

				if (m_currentButton == "A")
					m_specialPromptAnimators[0].SetBool("Highlighted",false);
				else
					m_specialPromptAnimators[1].SetBool("Highlighted",false);
			}
		}
	}


}
