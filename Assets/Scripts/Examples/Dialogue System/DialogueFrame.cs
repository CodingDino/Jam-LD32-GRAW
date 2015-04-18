// ************************************************************************ 
// File Name:   DialogueFrame.cs 
// Purpose:    	Information about a frame of dialogue
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using System.Collections.Generic;


// ************************************************************************ 
// Class: DialogueFrame
// ************************************************************************ 
public class DialogueFrame {

    public string id;

    // Linking
    public bool endOnThisFrame = false;
    public bool displayChoices = false;
    public List<DialogueLink> links;

    // Overrides (will be set to parent values if no override present) 
    public bool allowSkip = true;
    public bool waitForInput = true;
    public DialoguePortraitSettings portraitSettings;
    public DialogueTextSettings textSettings;

    // Sections
    public List<DialogueSection> sections;

    public static DialogueFrame Load(Dictionary<string, object> JSON, DialogueFrame defaults = null)
    {
        DialogueFrame newObject;
        if (defaults == null)
            newObject = new DialogueFrame();
        else
            newObject = defaults.MemberwiseClone() as DialogueFrame;

        if (JSON.ContainsKey("id"))
            newObject.id = JSON["id"] as string;

        // Links
        if (JSON.ContainsKey("endOnThisFrame"))
            newObject.endOnThisFrame = bool.Parse(JSON["endOnThisFrame"].ToString());
        if (JSON.ContainsKey("displayChoices"))
            newObject.displayChoices = bool.Parse(JSON["displayChoices"].ToString());
        if (JSON.ContainsKey("links"))
        {
            newObject.links = new List<DialogueLink>();
            List<object> lList = JSON["links"] as List<object>;
            foreach (object lEntry in lList)
            {
                DialogueLink newLink = DialogueLink.Load(lEntry as Dictionary<string, object>);
                newObject.links.Add(newLink);
            }
        }

        // Load NPC Settings
        if (JSON.ContainsKey("npcSettings"))
        {
            DialogueNPCSettings npcSettings = DialogueManager.FetchNPCSettings(JSON["npcSettings"] as string);

            if (npcSettings != null)
            {
                Debug.Log("NPC Settings fetched for: " + JSON["npcSettings"] + " - " + npcSettings);
                if (npcSettings.portraitSettings != null)
                    newObject.portraitSettings = npcSettings.portraitSettings.ShallowCopy();
                if (npcSettings.textSettings != null)
                    newObject.textSettings = npcSettings.textSettings.ShallowCopy();
            }
            else
            {
                Debug.LogError("NPC Settings NOT FOUND for: " + JSON["npcSettings"]);
            }
        }

        // Overrides
        if (JSON.ContainsKey("allowSkip"))
            newObject.allowSkip = bool.Parse(JSON["allowSkip"] as string);
        if (JSON.ContainsKey("waitForInput"))
            newObject.waitForInput = bool.Parse(JSON["waitForInput"] as string);
        if (JSON.ContainsKey("portraitSettings"))
            newObject.portraitSettings = DialoguePortraitSettings.Load(JSON["portraitSettings"] as Dictionary<string, object>, newObject.portraitSettings);
        if (JSON.ContainsKey("textSettings"))
            newObject.textSettings = DialogueTextSettings.Load(JSON["textSettings"] as Dictionary<string, object>, newObject.textSettings);

        // Sections
        if (JSON.ContainsKey("sections"))
        {
            newObject.sections = new List<DialogueSection>();
            DialogueSection defaultSection = new DialogueSection();
            defaultSection.portraitSettings = newObject.portraitSettings;
            defaultSection.textSettings = newObject.textSettings;
            List<object> sList = JSON["sections"] as List<object>;
            foreach (object sEntry in sList)
            {
                DialogueSection newSection = DialogueSection.Load(sEntry as Dictionary<string, object>, defaultSection);
                newObject.sections.Add(newSection);
            }
            Debug.Log("Frame "+newObject.id+" loaded "+newObject.sections.Count+" sections");
        }

        if (newObject.portraitSettings == null)
            Debug.LogError("portraitSettings is null in DialogueFrame");
        if (newObject.textSettings == null)
            Debug.LogError("textSettings is null in DialogueFrame");

        return newObject;
    }
}
