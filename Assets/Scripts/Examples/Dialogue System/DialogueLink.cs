// ************************************************************************ 
// File Name:   DialogueLink.cs 
// Purpose:    	Information about how two frames link together
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using System.Collections.Generic;


// ************************************************************************ 
// Class: DialogueLink
// ************************************************************************ 
public class DialogueLink {

    public string linkedConversation;
    public string linkedFrame;
    public List<DialogueRequirement> requirements;
    public string text;
    public bool saveChoice;

    static public DialogueLink Load(Dictionary<string, object> JSON, DialogueLink defaults = null)
    {
        DialogueLink newObject;
        if (defaults == null)
            newObject = new DialogueLink();
        else
            newObject = defaults.MemberwiseClone() as DialogueLink;

        if (JSON.ContainsKey("linkedFrame"))
            newObject.linkedFrame = JSON["linkedFrame"] as string;
        if (JSON.ContainsKey("linkedConversation"))
            newObject.linkedConversation = JSON["linkedConversation"] as string;
        if (JSON.ContainsKey("text"))
            newObject.text = JSON["text"] as string;
        if (JSON.ContainsKey("saveChoice"))
            newObject.saveChoice = bool.Parse(JSON["saveChoice"].ToString());
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

        return newObject;
    }

    public bool MeetsRequirements(PlayerProfile _profile)
    {
        if (requirements == null) return true;
        for (int i = 0; i < requirements.Count; ++i)
        {
            if (!requirements[i].MeetsRequirements(_profile))
                return false;
        }
        return true;
    }
}
