using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreboardFetcher : MonoBehaviour
{
    Dictionary<String, String> usernameToScores = new Dictionary<String, String>();
    void Start()
    {
        VisualElement m_Root = GetComponent<UIDocument>().rootVisualElement;
        Button button = m_Root.Q<Button>("Export");
        button.clicked += () => ExportClicked();
        //fetch the scores, sort them into username / score dictionary
        usernameToScores = fetchAllScores(usernameToScores);


        var usernames_by_int = new List<String>(usernameToScores.Count);
        foreach (KeyValuePair<string, string> entry in usernameToScores)
        {
            usernames_by_int.Add(entry.Key);
        }

        //bind makeItem to create a new label every time the list needs one, can be replaced with Custom item generator
        Func<VisualElement> makeItem = () => new Label();

        //assign the data to the list
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            String username = usernames_by_int[i];
            String score = "";
            usernameToScores.TryGetValue(username,out score);

            (e as Label).text = "Username: "+username+" Score: "+score;
        };

        const int itemHeight = 24;
        //Prefill the labels with empty entries to enable visualization.
        var items = new List<string>(usernameToScores.Count);
        for (int i = 1; i <= usernameToScores.Count ; i++)
            items.Add("");
        //assign it all to the listview.
        var listView = new ListView(items, itemHeight, makeItem, bindItem);

        listView.selectionType = SelectionType.Multiple;

        listView.onItemsChosen += objects => Debug.Log(objects);
        listView.onSelectionChange += objects => Debug.Log(objects);

        listView.style.flexGrow = 1.0f;

        m_Root.Add(listView);
    }

    private void ExportClicked()
    {

        return;
    }


    //this does not scale well, we can implement paging for large scale projects, but that's outside the scope.
    private Dictionary<string, string> fetchAllScores(Dictionary<string, string> usernameToScores)
    {
        const int itemCount = 500;
        var items = new List<String>(itemCount);
        for (int i = 1; i <= itemCount; i++)
            usernameToScores.Add("testy"+i,"test");
        return usernameToScores;
    }


}
