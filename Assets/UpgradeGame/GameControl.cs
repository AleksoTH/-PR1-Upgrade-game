using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameControl : MonoBehaviour
{
    public UIDocument sidebar;
    public CircleCollider2D cookie;
    public List<Monster> monsters = new List<Monster>();

    public List<Sprite> monsterImages = new List<Sprite>();
    public List<string> monsterNames = new List<string>();
    public List<int> monsterCosts = new List<int>();
    public List<int> monsterMultipliers= new List<int>();

    public long score = 0;

    private Label scorel;
    private ListView listView;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (cookie.OverlapPoint(mousePosition))
            {
                Debug.Log("Click!");
                score = score + 1;
                scorel.text = score+"";
            }


        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(!(monsterImages.Count == monsterNames.Count && monsterNames.Count == monsterCosts.Count)) { return; }

        generateMonsters();
        var sidebar_root = sidebar.rootVisualElement;
        scorel = sidebar_root.Q<Label>("Score");
        var MonsterEntry = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/monsterEntry.uxml");
        
        Func<VisualElement> makeItem = () => MonsterEntry.Instantiate();
        const int itemHeight = 100;
        //assign the data to the list
        Action<VisualElement, int> bindItem = (e, i) =>
        {

            var image = new StyleBackground();
            image.keyword = StyleKeyword.Auto;
            
            image.value = Background.FromSprite(monsters[i].texture);
            (e as VisualElement).Q<VisualElement>("image").style.backgroundImage = image; // set background
            (e as VisualElement).Q<Label>("Name").text = monsters[i].name; //Owned
            (e as VisualElement).Q<Label>("Cost").text = monsters[i].cost+"";
            (e as VisualElement).Q<Label>("Owned").text = monsters[i].count + "";
            (e as VisualElement).Q<Label>("Multiplier").text = monsters[i].multiplier* monsters[i].count + "";
        };
        //assign it all to the listview.
        listView = new ListView(monsters, itemHeight, makeItem, bindItem);
        listView.selectionType = SelectionType.Single;
        listView.style.flexGrow = 1.0f;
        listView.onSelectionChange += selectMonster;
        
        sidebar_root.Add(listView);
        StartCoroutine("MultiplyScore");
    }

    private void selectMonster(IEnumerable<object> obj)
    {
        Monster activeItem = (Monster) obj.First();
        if (score - activeItem.cost > 0) {
            score = score - activeItem.cost;
            activeItem.count = activeItem.count + 1;
            activeItem.cost = activeItem.cost * activeItem.count;
            scorel.text = score + "";
            listView.Refresh();
        }
    }
    private IEnumerator MultiplyScore()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < monsters.Count; i++)
            {
                Monster mons = monsters[i];
                score = score + (mons.count * mons.multiplier);
                scorel.text = score + "";
            }
        }
    }
    public void generateMonsters() {
        for (int i = 0; i < monsterImages.Count; i++)
        {
            var toAdd = new Monster();
            toAdd.name = monsterNames[i];
            toAdd.cost = monsterCosts[i];
            toAdd.multiplier = monsterMultipliers[i];
            toAdd.texture = monsterImages[i];
            monsters.Add(toAdd);
        }
    }
}

