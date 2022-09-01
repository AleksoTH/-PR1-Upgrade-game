using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreboardFetcher : MonoBehaviour
{
    static Dictionary<String, String> usernameToScores = new Dictionary<String, String>();
    static private List<String> usernames_by_int = new List<String>();
    static private ListView listView;
    static private List<string> items;

    void Start()
    {
        VisualElement m_Root = GetComponent<UIDocument>().rootVisualElement;
        Button button = m_Root.Q<Button>("Export");
        button.clicked += () => ExportClicked();
        StartCoroutine("DownloadScores");
       

        //bind makeItem to create a new label every time the list needs one, can be replaced with Custom item generator
        Func<VisualElement> makeItem = () => new Label();

        //assign the data to the list
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < 0 || i > usernames_by_int.Count) { return; }
            String username = usernames_by_int[i];
            String score = "";
            usernameToScores.TryGetValue(username,out score);

            (e as Label).text = "Username: "+username+" Score: "+score;
        };

        const int itemHeight = 24;
        //Prefill the labels with empty entries to enable visualization.
        items = new List<string>(usernameToScores.Count);
        for (int i = 0; i < usernameToScores.Count ; i++)
            items.Add("");
        //assign it all to the listview.
        listView = new ListView(items, itemHeight, makeItem, bindItem);

        listView.selectionType = SelectionType.Multiple;

        listView.onItemsChosen += objects => Debug.Log(objects);
        listView.onSelectionChange += objects => Debug.Log(objects);

        listView.style.flexGrow = 1.0f;

        m_Root.Add(listView);
    }

    internal static void runUpdate(ScoreEntry[] list)
    {
        usernameToScores.Clear();
        items.Clear();
        usernames_by_int.Clear();
        for (int i = 0; i < list.Length; i++)
            usernameToScores.Add(list[i].id + "", list[i].score + "");
        for (int i = 0; i <= usernameToScores.Count; i++)
            items.Add("");
        foreach (KeyValuePair<string, string> entry in usernameToScores)
        {
            usernames_by_int.Add(entry.Key);
        }
        listView.Refresh();
    }

    private void ExportClicked()
    {

        return;
    }
    private IEnumerator DownloadScores()
    {
        //fetch the scores, sort them into username / score dictionary
        yield return GameControl.api.getScores();
    }

    //this does not scale well, we can implement paging for large scale projects, but that's outside the scope.
  


}
