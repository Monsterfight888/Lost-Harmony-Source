using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject heartOne;
    public GameObject heartTwo;
    public GameObject heartThree;

    public Image enemyDomainSliderIcon;
    public Image enemyDomainNextSliderIcon;
    public Slider EnemyDomainSlider;

    public Text overflow;

    public void SetSlider(int maxVal)
    {
        EnemyDomainSlider.value = 0;
        EnemyDomainSlider.maxValue = maxVal;
    }
    public void SetSliderValue(float value)
    {
        EnemyDomainSlider.value = value;
    }
    public void UpdateHealth(int hp)
    {
        if(hp <= 0)
        {
            heartOne.SetActive(false);
            heartTwo.SetActive(false);
            heartThree.SetActive(false);

            overflow.text = "";
        }
        else if (hp == 1)
        {
            heartOne.SetActive(true);
            heartTwo.SetActive(false);
            heartThree.SetActive(false);

            overflow.text = "";
        }
        else if (hp == 2)
        {
            heartOne.SetActive(true);
            heartTwo.SetActive(true);
            heartThree.SetActive(false);

            overflow.text = "";
        }
        else if (hp == 3)
        {
            heartOne.SetActive(true);
            heartTwo.SetActive(true);
            heartThree.SetActive(true);

            overflow.text = "";
        }
        else if(hp > 3)
        {
            /*heartOne.SetActive(true);
            heartTwo.SetActive(true);
            heartThree.SetActive(true);

            overflow.text = "+ " + (hp - 3);*/
        }
    }
}
