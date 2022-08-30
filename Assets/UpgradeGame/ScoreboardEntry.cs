using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreboardEntry : Label
{
    public string username;
    public string score;

    public ScoreboardEntry()
    {
    }

    public ScoreboardEntry(string usr,string scor) {
        this.style.width = 256;
        this.style.height = 128;
        this.username = usr;
        this.score = scor;
        this.text = username +" : "+ score;
    }
}
