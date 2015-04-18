// ************************************************************************ 
// File Name:   DialogueManager.cs 
// Purpose:    	Control dialogue displayed in the dialogue panel
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MiniJSON;


// ************************************************************************ 
// Class: DialogueManager
// ************************************************************************ 
public class DialogueManager : Singleton<DialogueManager>
{

    // ********************************************************************
    // Exposed Data Members 
    // ********************************************************************
    [SerializeField]
    private string m_dialogueScriptPath;
    [SerializeField]
    private string m_NpcSettingsFilePath;
    [SerializeField]
    private string m_audioClipPath;
    [SerializeField]
    private string m_portraitPathLarge;
    [SerializeField]
    private GameObject m_dialoguePanel;
    [SerializeField]
    private Text m_textObject;
    [SerializeField]
    private Text m_portraitText;
    [SerializeField]
    private Text m_largePortraitText;
    [SerializeField]
    private float m_defaultTextSpeed = 3.0f;
    [SerializeField]
    private int m_numCharsPerLine = 49;
    [SerializeField]
    private int m_numCharsPerLinePortrait = 30;
    [SerializeField]
    private AudioSource m_audioSource;
    [SerializeField]
    private GameObject m_waitingIcon;
    [SerializeField]
    private Canvas m_canvas;
    [SerializeField]
    private Animator[] m_largePortrait = new Animator[2]; // TODO: Large and small
    [SerializeField]
    private Image[] m_characterAnimator = new Image[2];
    [SerializeField]
    private GameObject m_choiceRoot = null;
    [SerializeField]
    private GameObject m_choiceButtonPrototype = null;
    [SerializeField]
    private float m_choicePopInDelay = 0.05f;
    

    // ********************************************************************
    // Private Data Members 
    // ********************************************************************
    private List<DialogueConversation> m_conversations = new List<DialogueConversation>();
    private Dictionary<string, DialogueConversation> m_conversationsMap = new Dictionary<string, DialogueConversation>();
    private DialogueConversation m_currentConversation;
    private DialogueFrame m_currentFrame;
    private int m_sectionIndex;
    private DialogueSection m_currentSection;
    private Dictionary<string, DialogueNPCSettings> m_NPCSettings = new Dictionary<string, DialogueNPCSettings>();
    private bool m_dialogueLoaded = false;
    private StringBuilder m_currentDisplayString;
    private int m_displayIndex = 0;
    private bool m_shouldSkip = false;
    private bool m_waitingForNextFrame = false;
    private bool m_waitingForChoiceInput = false;
    private int m_numCharSinceLastNewline;
    private AudioClip m_audioClip;
    private string m_audioClipName;
    private int m_frameCount = 0;
    private List<Animator> m_choices = new List<Animator>();


    // ********************************************************************
    // Function:	Start()
    // Purpose:		Run when new instance of the object is created.
    // ********************************************************************
    void Start()
    {
    }


    // ********************************************************************
    // Function:	Update()
    // Purpose:		Run once per frame.
    // ********************************************************************
    void Update()
    {
        if (Input.GetButtonUp("Advance") || Input.GetMouseButtonDown(0))
        {
            if (m_waitingForNextFrame)
            {
                m_waitingForNextFrame = false;
                m_waitingIcon.SetActive(false);
                if (m_currentFrame.endOnThisFrame)
                {
                    // TODO: Save conversation as seen
                    for (int i = 0; i < 2; ++i)
                    {
                        m_largePortrait[i].SetBool("Shown", false);
                    }
                }
                else
                {
                    // Choose next frame based on which frames we meet the requirements for
                    for (int i = 0; i < m_currentFrame.links.Count; ++i )
                    {
                        if (m_currentFrame.links[i].MeetsRequirements(ProfileManager.profile))
                        {
                            FollowLink(i);
                            break;
                        }
                    }
                }
            }
            else
            {
                m_shouldSkip = true;
            }
        }
        // TODO: Select next frame based on choice made.
        // TODO: Save choice made if marked to be saved.
    }


    // ********************************************************************
    // Function:	LoadDialogueFilesInFolder()
    // Purpose:		Loads a set of DialogueConversations from a folder
    // ********************************************************************
    public void LoadDialogueFilesInFolder(string _folderName)
    {
        Debug.Log("Loading dialogue files in " + m_dialogueScriptPath + _folderName + "...");

        Object[] files = Resources.LoadAll(m_dialogueScriptPath + _folderName);
        if (files == null)
        {
            Debug.LogError("No dialogue files found: " + m_dialogueScriptPath + _folderName);
            return;
        }

        Debug.Log("DialogueManager found " + files.Length + " dialogue files in " + m_dialogueScriptPath + _folderName);
        for (int i = 0; i < files.Length; ++i)
        {
            LoadDialogue(files[i] as TextAsset);
        }
    }


    // ********************************************************************
    // Function:	LoadDialogueFromFile()
    // Purpose:		Loads a set of DialogueConversations from file
    // ********************************************************************
    public void LoadDialogueFromFile(string _fileName)
    {
        Debug.Log("Loading dialogue file " + m_dialogueScriptPath + _fileName + "...");
        TextAsset dialogueFile = Resources.Load(m_dialogueScriptPath + _fileName ) as TextAsset;
        if (dialogueFile == null)
        {
            Debug.LogError("No dialogue file found: " + m_dialogueScriptPath + _fileName);
            return;
        }

        LoadDialogue(dialogueFile);
    }


    // ********************************************************************
    // Function:	LoadDialogue()
    // Purpose:		Loads a set of DialogueConversations
    // ********************************************************************
    public void LoadDialogue(TextAsset _dialogueFile)
    {
        string jsonString = _dialogueFile.text;
        Debug.Log("JSON String loaded: " + jsonString);

        if (jsonString != "")
        {
            Dictionary<string, object> N = Json.Deserialize(jsonString) as Dictionary<string, object>;

            if (N.ContainsKey("conversations"))
            {
                List<object> cList = N["conversations"] as List<object>;

                foreach (object cEntry in cList)
                {
                    DialogueConversation conversation = DialogueConversation.Load(cEntry as Dictionary<string, object>);

                    m_conversationsMap[conversation.id] = conversation;
                    m_conversations.Add(conversation);
                }

            }
        }
    }


    // ********************************************************************
    // Function:	StartConversation()
    // Purpose:		Determines a conversation to use and starts it
    // ********************************************************************
    public void StartConversation()
    {
        // Determine correct conversation
        if (m_conversations.Count == 0)
        {
            Debug.LogError("No conversations loaded!");
        }
        for (int i = 0; i < m_conversations.Count; ++i)
        {
            if (m_conversations[i].autoload && m_conversations[i].MeetsRequirements(ProfileManager.profile))
            {
                m_currentConversation = m_conversations[i];
                Debug.Log("Conversation loaded: "+m_currentConversation.id);
                break;
            }
        }
        if (m_currentConversation == null) // Can't load any conversation!
        {
            Debug.LogError("Don't meet requirements for any conversations!");
            return; 
        }

        // Initialize stuff for new conversation
        m_currentFrame = m_currentConversation.frames[m_currentConversation.startingFrame];

        DisplayFrame();
    }


    // ********************************************************************
    // Function:	DisplayFrame()
    // Purpose:		Removes current text on frame and shows new frame
    // ********************************************************************
    private void DisplayFrame()
    {
        // Initialize stuff for new frame
        m_sectionIndex = 0;
        m_numCharSinceLastNewline = 0;
        m_textObject.text = "";
        m_currentDisplayString = new StringBuilder();
        ++m_frameCount;

        // Load correct portrait
        int side = -1;
        DialoguePortraitSettings portraitSettings = m_currentFrame.portraitSettings;
        if (portraitSettings != null && portraitSettings.active)
        {
            // TODO: Small portraits
            side = (int) portraitSettings.position;
            Debug.Log("Setting portrait to: " + m_portraitPathLarge + portraitSettings.image);
            m_largePortrait[side].SetBool("Shown", true);
            m_characterAnimator[side].sprite = Resources.Load<Sprite>(m_portraitPathLarge + portraitSettings.image);
        }
        for (int i = 0; i < 2; ++i)
        {
            if (i != side) m_largePortrait[i].SetBool("Shown", false);
        }
        StartCoroutine(DisplaySection());
    }


    // ********************************************************************
    // Function:	DisplaySection()
    // Purpose:		Performs actions described in a section
    // ********************************************************************
    private IEnumerator DisplaySection()
    {
        // Initialize stuff for new section
        m_currentSection = m_currentFrame.sections[m_sectionIndex];
        m_displayIndex = 0;

        // Set text settings
        DialogueTextSettings textSettings = m_currentSection.textSettings;
        if (textSettings.textAudio != m_audioClipName)
        {
            m_audioClipName = textSettings.textAudio;
            m_audioClip = Resources.Load(m_audioClipPath + m_audioClipName) as AudioClip;
            m_audioSource.clip = m_audioClip;
        }

        // TODO: Set portrait settings

        PrintText();

        // Print text until we're done
        if (m_currentSection.text != null)
        {
            float textSpeed = m_defaultTextSpeed * textSettings.textSpeed;
            float secondsToWait = 1.0f / textSpeed;
            while (m_displayIndex < m_currentSection.text.Length)
            {
                if (!m_shouldSkip)
                    yield return new WaitForSeconds(secondsToWait);
                //yield return new WaitForSeconds(1.0f / (m_defaultTextSpeed*m_currentSection.textSettings.textSpeed));
                PrintText();
            }
        }

        // TODO: Trigger special animations and effects
        // TODO: Wait for animation to finish if we triggered a special animation.

        // TODO: Some kind of manual "wait" system? (for cutscenes)

        // Load next section
        ++m_sectionIndex;
        if (m_sectionIndex < m_currentFrame.sections.Count)
        {
            StartCoroutine(DisplaySection());
        }
        else
        {
            // TODO: Bring up choices if applicable
            m_shouldSkip = false;

            if (m_currentFrame.displayChoices)
            {
                m_waitingForChoiceInput = true;

                List<int> validLinks = new List<int>();
                for (int i = 0; i < m_currentFrame.links.Count; ++i)
                {
                    if (m_currentFrame.links[i].MeetsRequirements(ProfileManager.profile))
                    {
                        validLinks.Add(i);
                    }
                }
                Debug.Log("Choices found for frame " + m_currentFrame.id + ": " + validLinks.Count);
                for (int i = 0; i < validLinks.Count; ++i)
                {
                    int index = validLinks[i];
                    DialogueLink link = m_currentFrame.links[index];
                    Debug.Log("Creating button for "+index+" link conv: " + link.linkedConversation + " frame: " + link.linkedFrame);
                    GameObject choiceButton = GameObject.Instantiate(m_choiceButtonPrototype) as GameObject;
                    choiceButton.transform.SetParent(m_choiceRoot.transform);
                    choiceButton.GetComponentInChildren<Text>().text = link.text;
                    AddListenerForChoice(choiceButton.GetComponent<Button>(), index);
                    m_choices.Add(choiceButton.GetComponent<Animator>());
                }

                StartCoroutine(HideChoices(false));
            }
            else
            {
                m_waitingForNextFrame = true;
                m_waitingIcon.SetActive(true);
            }
        }

        yield return null;
    }


    // ********************************************************************
    // Function:	AddListenerForChoice()
    // Purpose:		Adds a listener cause lambdas are stupid
    // ********************************************************************
    private void AddListenerForChoice(Button _button, int _linkIndex)
    {
        _button.onClick.AddListener(() => FollowLink(_linkIndex));
    }


    // ********************************************************************
    // Function:	FollowLink()
    // Purpose:		Applies the given link
    // ********************************************************************
    private void FollowLink(int _index)
    {
        if (_index >= m_currentFrame.links.Count)
        {
            Debug.LogError("Index " + _index + " is out of range for frame links from frame " + m_currentFrame.id);
        }

        DialogueLink link = m_currentFrame.links[_index];
        Debug.Log("Following " + _index + " link");
        FollowLink(link.linkedConversation, link.linkedFrame);
    }   
    private void FollowLink(string _linkedConv, string _linkedFrame)
    {
        Debug.Log("Following link to conv: " + _linkedConv + " frame: " + _linkedFrame);
        if (_linkedConv != null && _linkedConv != "")
        {
            m_currentConversation = m_conversationsMap[_linkedConv];
            m_currentFrame = m_currentConversation.frames[m_currentConversation.startingFrame];
        }

        if (_linkedFrame != null && _linkedFrame != "")
        {
            m_currentFrame = m_currentConversation.frames[_linkedFrame];
        }

        DisplayFrame();

        if (m_waitingForChoiceInput)
        {
            m_waitingForChoiceInput = false;
            StartCoroutine(HideChoices(true));
        }
    }


    // ********************************************************************
    // Function:	HideChoices()
    // Purpose:		Hides or shows choices
    // ********************************************************************
    private IEnumerator HideChoices(bool _hide)
    {
        for (int i = 0; i < m_choices.Count; ++i)
        {
            m_choices[i].SetBool("Hidden", _hide);
            yield return new WaitForSeconds(m_choicePopInDelay);
        }

        if (_hide)
        {
            yield return new WaitForSeconds(1.0f);
            for (int i = 0; i < m_choices.Count; ++i)
            {
                GameObject.Destroy(m_choices[i].gameObject);
            }
            m_choices.Clear();
        }
        
        yield return null;
    }


    // ********************************************************************
    // Function:	PrintText()
    // Purpose:		Prints the next character of text
    // ********************************************************************
    private void PrintText()
    {
        char currentChar = m_currentSection.text[m_displayIndex];
        if (currentChar == ' ' )
        {
            // Insert lines for wrapping between words
            // When at a space, check number of characters til next space, 
            //     if that pushes us over the limit, insert a newline instead 
            //     of a space here.
            int indexToCheck = m_displayIndex+1;
            while (indexToCheck + 1 < m_currentSection.text.Length && m_currentSection.text[indexToCheck] != ' ')
            {
                ++indexToCheck;
            }
            if (m_numCharSinceLastNewline + (indexToCheck - m_displayIndex) > m_numCharsPerLine) // TODO - num char per line different if portrait present
            {
                currentChar = '\n'; // We will wrap when this word is fully printed, so put in a newline
            }
        }

        if (currentChar == '\n')
            m_numCharSinceLastNewline = 0;
        else
            ++m_numCharSinceLastNewline;

        m_currentDisplayString.Append(currentChar);
        m_textObject.text = m_currentDisplayString.ToString();

        ++m_displayIndex;

        // Play a sound
        if (currentChar != '\n' && currentChar != ' ' && !m_shouldSkip)
        {
            float randomPitchVariation = Mathf.PerlinNoise(((float)m_displayIndex) * 0.1f, (float)m_frameCount);
            m_audioSource.pitch = m_currentSection.textSettings.textPitch + randomPitchVariation * m_currentSection.textSettings.textPitchVariation;
            m_audioSource.Play();
        }
    }


    // ********************************************************************
    // Function:	FetchNPCSettings()
    // Purpose:		Returns settings for an NPC, loads from file if needed
    // ********************************************************************
    public static DialogueNPCSettings FetchNPCSettings(string _NPCName)
    {
        LoadNPCSettings();
        if (instance.m_NPCSettings.ContainsKey(_NPCName))
        {            
            return instance.m_NPCSettings[_NPCName];
        }
        return null;
    }


    // ********************************************************************
    // Function:	LoadNPCSettings()
    // Purpose:		Loads NPC Settings from file
    // ********************************************************************
    public static void LoadNPCSettings()
    {
        if (!instance.m_dialogueLoaded)
        {
            instance.m_dialogueLoaded = true;
            TextAsset NPCFile = Resources.Load(instance.m_NpcSettingsFilePath) as TextAsset;
            string jsonString = NPCFile.text;
            Debug.Log("JSON String loaded: " + jsonString);

            if (jsonString != "")
            {
                Dictionary<string, object> N = Json.Deserialize(jsonString) as Dictionary<string, object>;
                List<object> NPCList = N["NPCs"] as List<object>;
                foreach (object NPCEntry in NPCList)
                {
                    DialogueNPCSettings newNPC = DialogueNPCSettings.Load(NPCEntry as Dictionary<string, object>);
                    instance.m_NPCSettings[newNPC.id] = newNPC;
                }

                Debug.Log("Loaded dialogue settings for " + instance.m_NPCSettings.Count + " NPCs");
            }
        }
    }

    public void Hide()
    {
        m_waitingIcon.SetActive(false);
        m_textObject.text = "";
        StartCoroutine(HideChoices(true));
        for (int i = 0; i < 2; ++i)
        {
            m_largePortrait[i].SetBool("Shown", false);
        }
    }
}
