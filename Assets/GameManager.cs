using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject wave1, wave2, wave3, wave4, wave5;
    [SerializeField] TMP_Text timer, round, score, ammo;
    private GameObject[] targets;
    private int waveNum = 1;
    private bool waveCompleted = true;
    private int Score = 0;
    private float Timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (waveCompleted)
        {
            switch (waveNum)
            {
                case 1:
                    Instantiate(wave1);
                    waveCompleted = false;
                    Timer = 15;
                    break;
                case 2:
                    Instantiate(wave2);
                    waveCompleted = false;
                    Timer = 15;
                    break;
                case 3:
                    Instantiate(wave3);
                    waveCompleted = false;
                    Timer = 15;
                    break;
                case 4:
                    Instantiate(wave4);
                    waveCompleted = false;
                    Timer = 15;
                    break;
                case 5:
                    Instantiate(wave5);
                    waveCompleted = false;
                    Timer = 15;
                    break;
                case 6:
                    //win game
                    break;
                case 7:
                    //game over lose
                    break;
            }
        }
        Timer -= Time.deltaTime;
        targets = GameObject.FindGameObjectsWithTag("Target");
        if (targets.Length == 0)
        {
            waveNum++;
            waveCompleted = true;
            Score += (int)Mathf.Floor(Timer * 100);
        }
        if (Timer < 0)
        {
            waveNum = 7;
            waveCompleted = true;
        }

        //update UI
        round.text = "Round : " + waveNum;
        score.text = "Score : " + Score;
        int wholeNumber = (int)Mathf.Floor(Timer);
        timer.text = wholeNumber + ":" + (wholeNumber -Timer).ToString()[3] + (wholeNumber - Timer).ToString()[4];
    }

    public void AddScore(int num)
    {
        Score += num;
    }

    public void AmmoCount(int mag, int magsize)
    {
        ammo.text = mag + "/" + magsize;
    }
}
