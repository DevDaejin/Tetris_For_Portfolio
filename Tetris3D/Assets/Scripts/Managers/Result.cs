using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [SerializeField] private Transform rank;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button submit;
    [SerializeField] private Button regame;
    [SerializeField] private TMP_Text scroeTxt;
    public UnityEvent OnRegameButtonEvent { private set; get; } = new UnityEvent();

    private string nickname = string.Empty;
    private int score;
    private List<Transform> rankChildren = new List<Transform>();

    private readonly int maxSizeOfDatas = 10;
    private readonly string defaultNickName = "annanymous";
    private readonly string point = "points";
    private readonly string fileSubPath = "/RankData";
    private readonly string fileName = "/rank.json";
    private readonly string infoPath = "Info";
    private readonly string namePath = "Name";
    private readonly string scorePath = "Score";

    private class Data
    {
        public int score = 0;
        public string nickName = "-";
    }

    private List<Data> datas = new List<Data>();

    private void OnEnable()
    {
        LoadData();
        SetRank();

        if (datas.Count != 0)
        {
            if (datas[maxSizeOfDatas - 1].score < score)
            {
                submit.interactable = true;
                inputField.interactable = true;
            }
        }
    }

    private void Start()
    {
        submit.onClick.AddListener(Submit);
        regame.onClick.AddListener(OnRegameButtonEvent.Invoke);

        score = 0;
        SetScore(score);
    }

    private List<Data> LoadData()
    {
        if (Directory.Exists(DirectoryPath()))
        {
            if (File.Exists(FilePath()))
            {
                var json = File.ReadAllText(FilePath());
                return JsonConvert.DeserializeObject<List<Data>>(json);
            }
        }
        
        return new List<Data>();
    }

    private void SaveData()
    {
        if (!File.Exists(DirectoryPath()))
            Directory.CreateDirectory(DirectoryPath());

        var json = JsonConvert.SerializeObject(datas);
        File.WriteAllText(FilePath(), json);
    }

    private string FilePath()
    {
        return Path.Combine(DirectoryPath(), fileName);
    }

    private string DirectoryPath()
    {
        return Path.Combine(Application.persistentDataPath, fileSubPath);
    }

    private void Submit()
    {
        submit.interactable = false;
        inputField.interactable = false;

        nickname = inputField.text == string.Empty ? defaultNickName : inputField.text;

        datas.Add(new Data{
            nickName = this.nickname,
            score = this.score
        });

        SaveData();
        SetRank();
    }

    public void SetScore(int score)
    {
        this.score = score;
        scroeTxt.text = $"{score.ToString("N0")} {point}";
    }

    private void SetRank()
    {
        if(datas.Count == 0)
        {
            for (int i = 0; i < maxSizeOfDatas; i++)
            {
                datas.Add(new Data
                {
                    nickName = defaultNickName,
                    score = 0
                });
            }
        }

        if(datas.Count > maxSizeOfDatas)
        {
            datas.Sort();
            datas.Reverse();

            while(datas.Count > maxSizeOfDatas)
            {
                datas.RemoveAt(maxSizeOfDatas);
            }
        }

        if(rankChildren.Count == 0)
        {
            var tempChildren = rank.GetComponentsInChildren<Transform>(true).ToList();

            tempChildren.ForEach((child) =>
            {
                if (child.parent == rank)
                    rankChildren.Add(child);
            });
        }

        for (int i = 0; i < rankChildren.Count; i++)
        {
            var textList = rankChildren[i].GetComponentsInChildren<TMP_Text>(true).ToList();

            textList.ForEach((text) =>
            {
                if (text.gameObject.name == namePath)
                    text.text = datas[i].nickName;

                if (text.gameObject.name == scorePath)
                    text.text = datas[i].score.ToString("N0");
            });
        }
    }
}
