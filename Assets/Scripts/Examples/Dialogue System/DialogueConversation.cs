// ************************************************************************ 
// File Name:   DialogueConversation.cs 
// Purpose:    	Information about a conversation for the dialogue system
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
// Class: DialogueConversation
// ************************************************************************ 
public class DialogueConversation {

    public string id;
    public bool autoload = true;

    // Requirements
    public List<DialogueRequirement> requirements = new List<DialogueRequirement>();

    // Defaults (Can be overridden by frames and sections)
    public bool allowSkip = true;
    public bool waitForInput = true;
    public DialoguePortraitSettings portraitSettings = new DialoguePortraitSettings();
    public DialogueTextSettings textSettings = new DialogueTextSettings();

    // Frames
    public string startingFrame;
    public Dictionary<string, DialogueFrame> frames = new Dictionary<string, DialogueFrame>();

    public static DialogueConversation Load(Dictionary<string, object> JSON, DialogueConversation defaults = null)
    {
        DialogueConversation newObject;
        if (defaults == null)
            newObject = new DialogueConversation();
        else
            newObject = defaults.MemberwiseClone() as DialogueConversation;

        if (JSON.ContainsKey("id"))
            newObject.id = JSON["id"] as string;
        if (JSON.ContainsKey("autoload"))
            newObject.autoload = bool.Parse(JSON["autoload"].ToString());

        // Defaults
        if (JSON.ContainsKey("allowSkip"))
            newObject.allowSkip = bool.Parse(JSON["allowSkip"].ToString());
        if (JSON.ContainsKey("waitForInput"))
            newObject.waitForInput = bool.Parse(JSON["waitForInput"].ToString());
        if (JSON.ContainsKey("portraitSettings"))
            newObject.portraitSettings = DialoguePortraitSettings.Load(JSON["portraitSettings"] as Dictionary<string, object>, newObject.portraitSettings);
        if (JSON.ContainsKey("textSettings"))
            newObject.textSettings = DialogueTextSettings.Load(JSON["textSettings"] as Dictionary<string, object>, newObject.textSettings);

        // Requirements
        if (JSON.ContainsKey("requirements"))
        {
            newObject.requirements = new List<DialogueRequirement>();
            List<object> rList = JSON["requirements"] as List<object>;
            foreach (object rEntry in rList)
            {
                DialogueRequirement newRequirement = DialogueRequirement.Load(rEntry as Dictionary<string, object>);
                newObject.requirements.Add(newRequirement);
            }
        }

        // Frames
        if (JSON.ContainsKey("startingFrame"))
            newObject.startingFrame = JSON["startingFrame"] as string;
        if (JSON.ContainsKey("frames"))
        {
            newObject.frames = new Dictionary<string, DialogueFrame>();

            DialogueFrame defaultFrame = new DialogueFrame();
            defaultFrame.portraitSettings = newObject.portraitSettings;
            defaultFrame.textSettings = newObject.textSettings;
            defaultFrame.allowSkip = newObject.allowSkip;
            defaultFrame.waitForInput = newObject.waitForInput;

            DialogueFrame lastFrame = null;
            bool lastFrameLinkNeeded = false;

            List<object> fList = JSON["frames"] as List<object>;
            foreach (object fEntry in fList)
            {
                DialogueFrame newFrame = DialogueFrame.Load(fEntry as Dictionary<string, object>, defaultFrame);
                newObject.frames[newFrame.id] = newFrame;

                if (lastFrameLinkNeeded)
                {
                    DialogueLink link = new DialogueLink();
                    link.linkedFrame = newFrame.id;
                    lastFrame.links = new List<DialogueLink>();
                    lastFrame.links.Add(link);
                    Debug.Log("Frame " + lastFrame.id + " linked to frame " + newFrame.id);
                }

                lastFrameLinkNeeded = (!newFrame.endOnThisFrame && (newFrame.links == null || newFrame.links.Count == 0));
                lastFrame = newFrame;

                if (newObject.startingFrame == null || newObject.startingFrame == "")
                    newObject.startingFrame = newFrame.id;
            }

            if (lastFrameLinkNeeded)
                lastFrame.endOnThisFrame = true;
            Debug.Log("Conversation " + newObject.id + " loaded " + newObject.frames.Count + " frames");
            Debug.Log("Starting frame: " + newObject.startingFrame);
        }

        if (newObject.portraitSettings == null)
            Debug.LogError("portraitSettings is null in DialogueConversation");
        if (newObject.textSettings == null)
            Debug.LogError("textSettings is null in DialogueConversation");

        return newObject;
    }

    public bool MeetsRequirements(PlayerProfile _profile)
    {
        for (int i = 0; i < requirements.Count; ++i)
        {
            if (!requirements[i].MeetsRequirements(_profile))
                return false;
        }
        return true;
    }

}
