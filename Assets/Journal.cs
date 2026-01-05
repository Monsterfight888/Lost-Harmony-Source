using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Journal : MonoBehaviour
{
    public GameObject journalGFX;

    public Text[] journalEntries;

    public string[] newJournalEntryText;

    public Image[] disks;
    public Material defaultUI;

    private void Start()
    {
        for (int i = 0; i < journalEntries.Length; i++)
        {
            if(i <= SceneMessenger.instance.CompletedGames)
            {
                journalEntries[i].text = newJournalEntryText[i];

                Transform gfx = journalEntries[i].transform.Find("GFX");
                if (gfx != null)
                {
                    gfx.gameObject.SetActive(true);
                }
            }
            if(i < SceneMessenger.instance.CompletedGames)
            {
                disks[i].material = defaultUI;
                disks[i].color = Color.white;
            }
        }
    }
    public void ToggleActive()
    {
        journalGFX.SetActive(!journalGFX.activeSelf);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            journalGFX.SetActive(!journalGFX.activeSelf);
        }
    }
}
