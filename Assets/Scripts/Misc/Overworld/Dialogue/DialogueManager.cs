using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;
    public Image profile;

    private Queue<string> sentences;
    private Queue<DialogueInst> dialogues;

    public Animator animator;

    public NPC notifyee;
    bool isInDialogue;

    DialogueInst currentDialogueInst;

    int counter = 0;
    WorldData worldData;
    AudioSource mySource;
    public AudioClip[] keyClips;

    public float timeBetweenKeys = .05f;


    void Awake()
    {
        sentences = new Queue<string>();
        dialogues = new Queue<DialogueInst>();
        mySource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        worldData = WorldData.instance;
    }

    private void Update()
    {
        if (worldData.colorEnum == null)
        {


        }
        
    }

    public void StartDialogue(DialogueInst dialogue)
    {
        if(worldData == null)
        {
            worldData = WorldData.instance;
        }
        if (!isInDialogue)
        {

            worldData.player.isDialogue = true;
            currentDialogueInst = dialogue;
            counter = 0;

            isInDialogue = true;
            animator.SetBool("IsOpen", true);
            sentences.Clear();

            nameText.text = dialogue.name;
            profile.sprite = dialogue.profile;

            notifyee = dialogue.NotifyReciever;

            foreach (string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }

            DisplayNextSentence();
        }
        else
        {
            dialogues.Enqueue(dialogue);
        }
    }

    public void DisplayNextSentence()
    {
        counter++;
        if (sentences.Count == 0)
        {

            EndDialogue();
            return;
        }
        if(currentDialogueInst.otherProfile != null && currentDialogueInst.otherName != "")
        {
            if(counter % 2 == 1)
            {
                profile.sprite = currentDialogueInst.profile;
                nameText.text = currentDialogueInst.name;
                //starts odd
            }
            else
            {
                profile.sprite = currentDialogueInst.otherProfile;
                nameText.text = currentDialogueInst.otherName;
                //then even
            }
        }


        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));     
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            int rand = Random.Range(0, keyClips.Length - 1);
            if(isInDialogue)
                mySource.PlayOneShot(keyClips[rand]);
            dialogueText.text += letter;

            yield return new WaitForSeconds(timeBetweenKeys);
        }
    }

    void EndDialogue()
    {


        if(notifyee != null)
        {
            notifyee.Notify();
        }
        isInDialogue = false;

        if(dialogues.Count > 0)
        {
            StartDialogue(dialogues.Dequeue());//test when get back
        }
        else
        {
            animator.SetBool("IsOpen", false);
            worldData.player.isDialogue = false;
        }

        
    }
}
