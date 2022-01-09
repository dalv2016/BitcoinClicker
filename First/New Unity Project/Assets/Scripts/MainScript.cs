using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class MainScript : MonoBehaviour
{
    public int money;
    private int bonusPersec = 0;
    private int bonus = 1;
    int passiveItem = 500;
    private bool endgame;
    private bool newgame;
    private int ClickMoney;
    public List<Item> shopItems = new List<Item>();
    public List<Aches> AchesList = new List<Aches>();
    public Text[] lvl;
    public Text[] CostItemText;
    public Text moneytext;

    public Button advertise;
    public Button[] Ache;
    public GameObject Pan;
    public GameObject Pan2;
    public GameObject BtnShop;
    public GameObject ContentShop;
    public GameObject BtnAches;
    public GameObject ContentAches;
    public GameObject button;
    public GameObject effect;

    private Save sv = new Save();
    private void Awake()
    {
        if (PlayerPrefs.HasKey("SV"))
        {
            sv = JsonUtility.FromJson<Save>(PlayerPrefs.GetString("SV"));
            money = sv.money;
            bonusPersec = sv.bonusPersec;
            bonus = sv.bonus;
            passiveItem = sv.passiveItem;
            ClickMoney = sv.ClickMon;
            if (sv.endgame == 0) endgame = false;
            else endgame = true;
            
            for (int i = 0; i < shopItems.Count; i++)
            {
                shopItems[i].Lvl = sv.Lvl[i];
                shopItems[i].Costs *= (int)Math.Pow(shopItems[i].MultiplicationNumber, shopItems[i].Lvl);
            }
            for (int i = 0; i < AchesList.Count; i++)
            {
                if (sv.aches[i] == 1)
                {
                    AchesList[i].isGotten = true;
                    Ache[i].interactable = false;
                }
            }
        }
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("SV"))
        {
            DateTime dt = new DateTime(sv.date[0], sv.date[1], sv.date[2], sv.date[3], sv.date[4], sv.date[5]);
            TimeSpan ts = DateTime.Now - dt;
            int offlineBonus = (int)ts.TotalSeconds * bonusPersec;
            if (offlineBonus > passiveItem) offlineBonus = passiveItem;
            money += offlineBonus;
            print("Abbsent  " + ts.Days + "Day " + ts.Hours + "Hour " + ts.Minutes + "Min " + ts.Seconds + "Sec");
            print("Вы зароботали " + offlineBonus);
        }
        UpdateCoast();
        Pan.SetActive(false);
        Pan2.SetActive(false);
        StartCoroutine(BonusPerSec());
    }
    void Update()
    {
        moneytext.text = money.ToString();
        if (money >= 1000000&& !endgame)
        {
            Time.timeScale = 0;
            Pan2.SetActive(true);
            endgame = true;
        }
        if (newgame)
        {
            PlayerPrefs.DeleteKey("SV");
            SceneManager.LoadScene(0);
        }
        UpdateCoast();
    }

    public void CloseShow(int index)
    {
        switch (index)
        {
            case 1: //ShopOpen
                {
                    Pan.SetActive(true);
                    BtnAches.SetActive(false);
                    ContentAches.SetActive(false); 
                    break;
                }
            case 2: // ShopClose
                {

                    Pan.SetActive(false);
                    BtnAches.SetActive(true);
                    ContentAches.SetActive(true);
                    break;
                }
            case 3: //AchesOpen
                {
                    Pan.SetActive(true);
                    BtnShop.SetActive(false);
                    ContentShop.SetActive(false);
                    break;
                }
            case 4: //AchesClose
                {
                    Pan.SetActive(false);
                    BtnShop.SetActive(true);
                    ContentShop.SetActive(true);
                    break;
                }

        }
    }
  
    public void ButtonСlick(int index)
    {
        if (money >= shopItems[index].Costs)
        {
            if (shopItems[index].Lvl < shopItems[index].maxLvl)
            {
                if (shopItems[index].isItemPeraSecBonus)
                {
                    bonusPersec += shopItems[index].BonusPerSec;

                }
                else if (shopItems[index].IsPessiveItem)
                {
                    passiveItem += shopItems[index].PassiveItem;

                }

                else bonus += shopItems[index].AddPointClick;
                money -= (int)shopItems[index].Costs;
                shopItems[index].Lvl++;
                shopItems[index].Costs *= shopItems[index].MultiplicationNumber;
                shopItems[index].Costs = (int)shopItems[index].Costs;
            }
            
        }
        UpdateCoast();
        

    }

    public void ButtonUp()
    {
        button.GetComponent<RectTransform>().localScale = new Vector3(4f, 4f, 0f);
    }
    private void UpdateCoast()
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            CostItemText[i].text = shopItems[i].Costs.ToString();
            lvl[i].text = shopItems[i].Lvl.ToString();
        }
    }

    public void AchGet(int index)
    {
        if (ClickMoney >= AchesList[index].condition && AchesList[index].ClickAches)
        {
            money += AchesList[index].MoneyAch;
            AchesList[index].isGotten = true;
            Ache[index].interactable = false;
        }
        else if (index>=4) if (shopItems[index-4].Lvl  >= AchesList[index].condition)
        {
            money += AchesList[index].MoneyAch;
            AchesList[index].isGotten = true;
            Ache[index].interactable = false;
        }
    }
    public void OnClick()
    {
        Instantiate(effect, button.GetComponent<RectTransform>().position.normalized, Quaternion.identity);
        button.GetComponent<RectTransform>().localScale = new Vector3(3.95f, 3.95f, 0f);
        money += bonus;
        ClickMoney++;
    }
    public void AgainORContin(int index)
    {
        if (index == 1)
        {
            Time.timeScale = 1;
            Pan2.SetActive(false);
        }
        else
        {
            newgame = true;
            Pan2.SetActive(false);
        }
    }
    //public void advertising()
    //{
    //    StartCoroutine(BonusTimer(5)); 
    //}
    IEnumerator BonusPerSec()
    {
        while (true)
        {
            money += bonusPersec;
            yield return new WaitForSeconds(1);
        }
    }
    //IEnumerator BonusTimer(float time)
    //{
    //    advertise.interactable = false;
    //    bonus *= 3;
    //    bonusPersec *= 3;
    //    yield return new WaitForSeconds(time);
    //    bonus /= 3;
    //    bonusPersec /= 3;
    //    advertise.interactable = true;
    //}
#if UNITY_ANDROID && !UNITY_EDITOR
    private void AOnApplicationPause(bool pause)
    {
    if(pause)
    {
      sv.bonus = bonus;
        sv.bonusPersec = bonusPersec;
        sv.money = money;
        sv.passiveItem = passiveItem;
        sv.ClickMon = ClickMoney;
        sv.Lvl = new int[shopItems.Count];
        sv.aches = new int[AchesList.Count];
        for (int i = 0; i < shopItems.Count; i++)
        {
            sv.Lvl[i] = shopItems[i].Lvl;
        }
        for (int i = 0; i < AchesList.Count-1; i++)
        {
           if (AchesList[i].isGotten)
           {
                sv.aches[i] = 1;
           }
        }
        sv.date[0] = DateTime.Now.Year;
        sv.date[1] = DateTime.Now.Month;
        sv.date[2] = DateTime.Now.Day;
        sv.date[3] = DateTime.Now.Hour;
        sv.date[4] = DateTime.Now.Minute;
        sv.date[5] = DateTime.Now.Hour;
        if (endgame) sv.endgame = 1;
        else sv.endgame = 0;
        PlayerPrefs.SetString("SV", JsonUtility.ToJson(sv));
        }
    }
#else
    private void OnApplicationQuit()
    {
        sv.bonus = bonus;
        sv.bonusPersec = bonusPersec;
        sv.money = money;
        sv.passiveItem = passiveItem;
        sv.ClickMon = ClickMoney;
        sv.Lvl = new int[shopItems.Count];
        sv.aches = new int[AchesList.Count];
        for (int i = 0; i < shopItems.Count; i++)
        {
            sv.Lvl[i] = shopItems[i].Lvl;
        }
        for (int i = 0; i < AchesList.Count-1; i++)
        {
           if (AchesList[i].isGotten)
           {
                sv.aches[i] = 1;
           }
        }
        sv.date[0] = DateTime.Now.Year;
        sv.date[1] = DateTime.Now.Month;
        sv.date[2] = DateTime.Now.Day;
        sv.date[3] = DateTime.Now.Hour;
        sv.date[4] = DateTime.Now.Minute;
        sv.date[5] = DateTime.Now.Hour;
        if (endgame) sv.endgame = 1;
        else sv.endgame = 0;
        PlayerPrefs.SetString("SV", JsonUtility.ToJson(sv));
    }
#endif
    [Serializable]
    public class Item
    {
        public int AddPointClick;
        [Space]
        public float Costs;
        public int Lvl;
        public int maxLvl;
        public float MultiplicationNumber;
        [Space]
        public bool isItemPeraSecBonus;
        public int BonusPerSec;
        [Space]
        public bool IsPessiveItem;
        public int PassiveItem;
        [Space]
        public bool ISmultiplibtn;
    }
    [Serializable]
    public class Aches
    {
        public int MoneyAch;
        public bool isGotten;
        public int condition;
        public bool ClickAches;

    }
    [Serializable]
    public class Save
    {
        public int money;
        public int bonusPersec;
        public int bonus;
        public int[] Lvl;
        public int[] date = new int[6];
        public int[] aches;
        public int ClickMon;
        public int passiveItem;
        public int endgame;
        public int newgame;
    }


}
