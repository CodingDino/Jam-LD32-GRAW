// ************************************************************************ 
// File Name:   DraggableObject.cs 
// Purpose:    	Object can be click+dragged or touch+dragged around the screen
// Project:		
// Author:      Sarah Herzog  
// Copyright: 	2014 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using System.Collections;


// ************************************************************************ 
// Attributes 
// ************************************************************************ 


// ************************************************************************ 
// Class: DraggableObject
// ************************************************************************ 
public class DraggableObject : MonoBehaviour {


	// ********************************************************************
	// Serialized Data Members 
	// ********************************************************************
	[SerializeField]
	private bool m_allowDragging = true;
	[SerializeField]
	private bool m_clampToScreenByRenderer = false;
	[SerializeField]
	private bool m_clampToScreenByLimits = false;
	[SerializeField]
	private Renderer m_renderer = null;
	[SerializeField]
	private Transform m_transform = null;
	[SerializeField]
	private bool m_restrictX = false;
	[SerializeField]
	private bool m_restrictY = false;
	[SerializeField]
	private Transform m_minLimit = null;
	[SerializeField]
	private Transform m_maxLimit = null;
	[SerializeField]
	private float m_deceleration = 10.0f;
	[SerializeField]
	private float m_maxSpeed = 10.0f;


	// ********************************************************************
	// Private Data Members 
	// ********************************************************************
	private bool m_isDragging = false;
	private bool m_draggedThisFrame = false;
	private Vector3 m_colliderScreenStartingPoint = Vector3.zero;
	private Vector3 m_worldSpaceMouseColliderOffset = Vector3.zero;
	private Vector3 m_velocity = Vector3.zero;
	
	
	// ********************************************************************
	// Properties 
	// ********************************************************************
	public bool isDragging { get { return m_isDragging; } }


	// ********************************************************************
	// Function:	LateUpdate()
	// Purpose:		Called once per frame, after other functions.
	// ********************************************************************
	void LateUpdate () 
	{
		if (!m_draggedThisFrame)
			m_isDragging = false;
		m_draggedThisFrame = false;

		// Apply speed and deceleration
		if (!m_isDragging)
		{
			if(m_deceleration != 0) m_velocity = m_velocity / m_deceleration;
		}

		float clampedSpeed = Mathf.Min (m_velocity.magnitude, m_maxSpeed);
		m_velocity = m_velocity.normalized * clampedSpeed;
		Vector3 currentWorldPoint = m_transform.position + m_velocity * Time.deltaTime;
		m_transform.position = CheckBounds(currentWorldPoint);
	}


	// ********************************************************************
	// Function:	OnMouseDown()
	// Purpose:		Called when the user presses the mouse button over this 
	//				collider. 
	// ********************************************************************
	void OnMouseDown()
	{
		m_colliderScreenStartingPoint = 
			Camera.main.WorldToScreenPoint(m_transform.position);

		m_worldSpaceMouseColliderOffset = m_transform.position - 
			Camera.main.ScreenToWorldPoint(
				new Vector3(Input.mousePosition.x, 
			            	Input.mousePosition.y, 
			            	m_colliderScreenStartingPoint.z));
	}

	
	// ********************************************************************
	// Function:	OnMouseDrag()
	// Purpose:		Called when the user clicks on the collider and holds 
	//				down the mouse button.
	// ********************************************************************
	void OnMouseDrag()
	{

		if (!m_allowDragging)
			return;

		m_draggedThisFrame = true;
		m_isDragging = true;

		Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, 
		                                         Input.mousePosition.y, 
		                                         m_colliderScreenStartingPoint.z);

		Vector3 currentWorldPoint = 
			  Camera.main.ScreenToWorldPoint(currentScreenPoint)
			+ m_worldSpaceMouseColliderOffset;
		
		Vector3 oldWorldPoint = m_transform.position;

		//m_transform.position = CheckBounds(currentWorldPoint);

		m_velocity = (currentWorldPoint-oldWorldPoint)/Time.deltaTime;
	}

	
	// ********************************************************************
	// Function:	CheckBounds()
	// Purpose:		Returns nearest point within world bounds
	// ********************************************************************
	Vector3 CheckBounds(Vector3 toCheck)
	{
		Vector3 oldWorldPoint = m_transform.position;
		Vector3 currentWorldPoint = toCheck;

		if (m_restrictX)
			currentWorldPoint.x = oldWorldPoint.x;
		if (m_restrictY)
			currentWorldPoint.y = oldWorldPoint.y;
		
		if (m_clampToScreenByRenderer && m_renderer != null)
		{
			Vector3 minScreenPoint = new Vector3(0,0,m_colliderScreenStartingPoint.z);
			Vector3 maxScreenPoint = new Vector3(Screen.width,Screen.height,m_colliderScreenStartingPoint.z);
			Vector3 minWorldPoint = Camera.main.ScreenToWorldPoint(minScreenPoint);
			Vector3 maxWorldPoint = Camera.main.ScreenToWorldPoint(maxScreenPoint);
			
			m_transform.position = currentWorldPoint;
			
			if (   m_renderer.bounds.min.x < minWorldPoint.x 
			    || m_renderer.bounds.max.x > maxWorldPoint.x
			    || m_renderer.bounds.min.y < minWorldPoint.y
			    || m_renderer.bounds.max.y > maxWorldPoint.y)
				currentWorldPoint = oldWorldPoint;

			m_transform.position = oldWorldPoint;
		}
		else if (m_clampToScreenByLimits && m_minLimit != null && m_maxLimit != null)
		{
			Vector3 minScreenPoint = new Vector3(0,0,m_colliderScreenStartingPoint.z);
			Vector3 maxScreenPoint = new Vector3(Screen.width,Screen.height,m_colliderScreenStartingPoint.z);
			Vector3 minWorldPoint = Camera.main.ScreenToWorldPoint(minScreenPoint);
			Vector3 maxWorldPoint = Camera.main.ScreenToWorldPoint(maxScreenPoint);
			
			m_transform.position = currentWorldPoint;

			Vector3 minLimitPoint = m_minLimit.position;
			Vector3 maxLimitPoint = m_maxLimit.position;

			if (   minLimitPoint.x <= minWorldPoint.x 
			    || maxLimitPoint.x >= maxWorldPoint.x
			    || minLimitPoint.y <= minWorldPoint.y
			    || maxLimitPoint.y >= maxWorldPoint.y )
				currentWorldPoint = oldWorldPoint;
			
			m_transform.position = oldWorldPoint;
		}

		return currentWorldPoint;
	}
}
