using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Opening_Script : MonoBehaviour
{
    public Sprite[] frames;
    public SpriteRenderer imageSpawn;
    public GameObject blank;
    public Animator anim;
    public Animator textFadeAnim;

    public GameObject text;
    public GameObject play;
    public GameObject title;

    public Color maroon;

    private void Start()
    {
        imageSpawn.color = maroon;
        play.SetActive(false);
        title.SetActive(false);
        text.SetActive(true);

        StartCoroutine("IntroSeq");
    }

    IEnumerator IntroSeq()
    {
        yield return new WaitForSecondsRealtime(1f);
        imageSpawn.color = new Color(255, 255, 255);

        for(int i = 0; i < 10; i++)
        {
            imageSpawn.sprite = frames[i];

            anim.SetTrigger("FadeIn");

            yield return new WaitForSecondsRealtime(3f);

            anim.ResetTrigger("FadeIn");
            anim.SetTrigger("FadeOut");

            if(i == 1)
            {
                textFadeAnim.SetTrigger("TextFade");
            }

            yield return new WaitForSecondsRealtime(1f);
        }

        imageSpawn.sprite = frames[10];
        textFadeAnim.gameObject.SetActive(false);
        text.SetActive(false);

        anim.SetTrigger("FadeIn");

        yield return new WaitForSecondsRealtime(1.5f);


        

        title.SetActive(true);
        play.SetActive(true);
        

    }


    public void StartGame()
    {
        SceneManager.LoadScene("OverWorld");
    }
}
