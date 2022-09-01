using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HttpAPI
{
    string url = "http://localhost:8080/";
    user LoggedInUser = null;
    List<string> cookies = new List<string>();

    public void performLogin(string username, string password) {
        var Base64 = toBase64(username, password);
        Login(Base64);
    }

    string toBase64(string username, string password)
    {
        string auth = username + ":" + password;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        auth = "Basic " + auth;
        return auth;
    }

    public IEnumerator getScores() {
        UnityWebRequest www = UnityWebRequest.Get(url + "scoreboard");
        foreach (var s in cookies)
        {
            www.SetRequestHeader("Cookie", s);
        }
        www.timeout = 1;
        UnityWebRequestAsyncOperation req = www.SendWebRequest();
        req.completed += ao =>
        {
            if (!(www.result == UnityWebRequest.Result.Success))
            {
                Debug.LogError("Failed request: " + www.result);
            }
            else {
                Debug.Log(www.downloadHandler.text);
                var list =  JsonHelper.FromJson<ScoreEntry>(www.downloadHandler.text);
                ScoreboardFetcher.runUpdate(list);
            }
        };
        yield return req;
    }
    public bool updateScore(long score) {
        
        WWWForm form = new WWWForm();
        form.AddField("score", score+"");
        UnityWebRequest www = UnityWebRequest.Post(url + LoggedInUser.id + "/new_score", form);
        foreach (var s in cookies)
        {
            www.SetRequestHeader("Cookie", s);
        }
        www.timeout = 1;
        UnityWebRequestAsyncOperation req = www.SendWebRequest();
        req.completed += ao =>
        {
            if (!(www.result == UnityWebRequest.Result.Success))
            {
                Debug.LogError("Failed request: " + www.result);
            }
        };
        return false;
    }

    void Login(string base64) {
        UnityWebRequest www = UnityWebRequest.Get(url+"login");
        www.SetRequestHeader("AUTHORIZATION", base64);
        www.timeout = 1;
        UnityWebRequestAsyncOperation req = www.SendWebRequest();
        req.completed += ao =>
        {
            if (!(www.result == UnityWebRequest.Result.Success))
            {
                Debug.LogError("Failed login: " + www.result);
            }
            else
            {
                LoggedInUser = CreateFromJSON(www.downloadHandler.text);
                Debug.Log(LoggedInUser);
                Debug.Log(www.downloadHandler.text);
                cookies.Clear();
                foreach (var s in www.GetResponseHeaders())
                {
                    if (s.Key.Equals("Set-Cookie")) {
                        Debug.Log("s=" + s.Value);
                        cookies.Add(s.Value);
                    }
                }
                LoginComplete();
            }
        };
        
    }

    private void LoginComplete()
    {
        Debug.Log("We are logged in!");
    }

    user CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<user>(jsonString);
    }
}

[System.Serializable]
class user
{
    public long id;
    public List<ScoreEntry> scores;
    public string username;
}

[System.Serializable]
class ScoreEntry
{
    public long id;
    public long score;
}