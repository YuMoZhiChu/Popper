/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{

    // Add public variables here:
    static public GameControl instance = null;
    static public bool ESCGameMenu = false;
    static public float musicStaticVolume = 1;
    static public string playerName = "";

    public int playerWaitingForOtherPlayersTime = 25;
    public int playerPlayGameTime = 300;
    public int playerCountTime = 3;
    public bool startGameButton = false;

    public GameObject deCount;
    public GameObject waitRebornUI;
    public GameObject scoreArrow;
    public AudioClip finishBGMusic;
    public AudioClip killAudio;
    public AudioClip acquireUltAudio;

    [System.Serializable]
    public class GameTime
    {
        public int seconds;
        public string GetTimeText()
        {
            string text = "";
            text += (seconds / 60 / 10).ToString();
            text += (seconds / 60 % 10).ToString();
            text += ":";
            text += (seconds % 60 / 10).ToString();
            text += (seconds % 60 % 10).ToString();
            return text;
        }
    }
    public GameTime gameTime;

    [System.Serializable]
    public class GameUI
    {
        public GameObject timerText;
        public GameObject ESCMenu;
        public GameObject HelpMune;
        public GameObject gameFinishUI;
        public GameObject[] ziDanTiao;
        public GameObject[] DaZhao;
        public GameObject[] DaZhaoBG;
        public GameObject[] Head;
        public GameObject warnUI;
        public GameObject scoleUI;
        public GameObject WeaponUI;
        public GameObject zidanshuliang;
        public Text killamount;
        public GameObject FTips;
        public GameObject liansha;
    }
    public GameUI gameUI;

    public Transform PlayersLayer;
    public Transform bulletsLayer;
    public Transform ForceLayer;
    public Transform fairyLayer;

    public GameObject heroHPText;

    public GameObject Hero1;

    public GameObject Fairy1;

    //5yue12hao
    public int scorePerSecond = 100;
    public int scoreHitHero = 500;
    public int scoreKillHero = 2000;
    public Text scoreText;

    // Add private members here:
    private SphereCollider GameArea;
    private Transform[] fairyAppearLocations;
    private SomeFunctions m_someFunctions;
    public SomeFunctions someFunctions
    {
        get { return m_someFunctions; }
        set { m_someFunctions = value; }
    }

    public void PlayKillAudio()
    {
        Add2DAudio(killAudio);
    }

    public AudioSource Add2DAudio(AudioClip ac)
    {
        return AddAudio(this.gameObject, ac, 0);
    }

    static public AudioSource AddAudio(GameObject go, AudioClip ac, float is3D = 0)
    {
        AudioSource ass = go.AddComponent<AudioSource>();
        ass.loop = false;
        ass.clip = ac;
        ass.spatialBlend = is3D;
        ass.Play();
        Destroy(ass, ac.length);
        return ass;
    }

    #region some useful function
    // some useful function

    // 消除对象的所有子对象
    static public void cleanAllSubObject(GameObject parentObj)
    {
        for (int i = 0; i < parentObj.transform.childCount; i++)
        {
            GameObject go = parentObj.transform.GetChild(i).gameObject;
            Destroy(go);
        }
    }

    #endregion

    private bool lockCursor = true;

    // Add member functions here:
    void ChangeCursorState(bool visible)
    {
        if (visible == true)
        {
            Cursor.visible = true;
            lockCursor = false;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            lockCursor = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    #region 游戏界面
    public void ContinueBtnClick()  // 继续游戏
    {
        gameUI.ESCMenu.SetActive(false);
        if (ESCGameMenu == true)
        {
            ChangeCursorState(false);
            ESCGameMenu = false;
            Time.timeScale = 1;
        }
    }

    public void QuitBtnClick()  // 退出游戏
    {
        Datas.MainHeroInfo.list.Clear();
        UDPBack.udpSend(UDPBack.setLogoutString());
        ESCGameMenu = false;
        
            Time.timeScale = 1;
        
        Application.LoadLevel("GameMenu");
    }

    public void HelpBtnClick()  // 游戏帮助
    {
        if (gameUI.HelpMune.activeSelf == false)
            gameUI.HelpMune.SetActive(true);
        else
            gameUI.HelpMune.SetActive(false);
    }

    public void BackBtnClick()  // 退出游戏
    {

        Datas.MainHeroInfo.list.Clear();
        Datas.reset();
        ESCGameMenu = false;
        Time.timeScale = 1;
        Application.LoadLevel("GameMenu");
    }

    public void HPTextUpdate()
    {
        //heroHPText.GetComponent<Text>().text = Datas.MainHeroInfo.HeroTotalHP.ToString() + " / " + Datas.MainHeroInfo.HeroCurrentHP.ToString();
        float percentage = Datas.MainHeroInfo.HeroCurrentHP / Datas.MainHeroInfo.HeroTotalHP;
        //float fillAmount = heroHPText.GetComponent<Image>().fillAmount;
        //if (percentage < fillAmount)
        //{
        //    percentage = Mathf.Lerp(Time.deltaTime, fillAmount, percentage);
        //}
        //else if (percentage > fillAmount)
        //{
        //    percentage = Mathf.Lerp(Time.deltaTime, percentage, fillAmount);
        //}

        heroHPText.GetComponent<Image>().fillAmount = percentage;
        //Debug.Log(percentage);
    }
    #endregion

    #region 玩家建立函数(可延时)
    // 外部接口
    public void PlayerRebornDelayAPI(string heroId, float delayTime)
    {
        StartCoroutine(PlayerRebornDelay(heroId, delayTime));
    }
    IEnumerator PlayerRebornDelay(string heroId, float delayTime)
    {
        // Debug.Log ("bilud player!");
        #region 设置玩家的出生位置
        string bornPositionString = "";
        int heroPosionSetNum = int.Parse(heroId) % Datas.gameMaxNum;
        if (heroPosionSetNum == 0) heroPosionSetNum = Datas.gameMaxNum;
        bornPositionString = "PlayerBornPosition" + heroPosionSetNum.ToString();
        #endregion

        int roleNum = Datas.roleID;
        // 如果是当前自身的英雄
        if (heroId == Datas.MainHeroInfo.HeroName)
        {
            // 先将位置置为空，表示正在重生
            UDPBack.udpSend(UDPBack.setPositionString("(0, 0, 0)", "0", "(0, 0, 0)"));
        }
        else
        {
            // 确定另外玩家选择的角色
            roleNum = Datas.MainHeroInfo.findPlayerByKey(heroId).roleNum;
        }
        yield return new WaitForSeconds(delayTime);
        // 如果已经存在，则重置
        GameObject self = GameObject.Find(heroId);
        if (self != null)
        {
            Debug.Log("destory");
            Destroy(self);
        }

        Transform bornPositionTransfrom = GameObject.Find(bornPositionString).transform;
        Vector3 location = bornPositionTransfrom.position;
        Quaternion rotation = bornPositionTransfrom.rotation;
        GameObject hero = Instantiate(Hero1, location, rotation) as GameObject;

        hero.GetComponent<DecideRolModel>().setRool(roleNum);
        hero.transform.SetParent(PlayersLayer);
        hero.name = heroId;
        hero.tag = "PlayerHero";
        Datas.otherPlayer o = Datas.MainHeroInfo.findPlayerByKey(heroId);
        o.transform = hero.GetComponent<Transform>();
        o.TPC = hero.GetComponent<ThirdPlayerControl>();
        o.HPC = hero.GetComponent<HPControl>();
        o.hero = hero;

        if (heroId == Datas.MainHeroInfo.HeroId)
        {
            // 增加摄像机深度，保证第一视角
            o.TPC.sight.transform.FindChild("Camera").GetComponent<Camera>().depth = 100;
        }
        else
        {
            Destroy(o.TPC.sight);
            Destroy(o.TPC.dieSight);
            Destroy(hero.GetComponent<ThirdPlayerCameraControl>());
        }
    }

    #endregion

    #region 计时器

    void Decount(int num)
    {
        for (int i = 0; i < deCount.transform.childCount; i++)
        {
            deCount.transform.GetChild(i).gameObject.SetActive(false);
        }
        if (num >= 0 && num < deCount.transform.childCount)
            deCount.transform.GetChild(num).gameObject.SetActive(true);
    }

    void DecountUnactiveNum()
    {
        for (int i = 0; i < deCount.transform.childCount; i++)
        {
            deCount.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // 倒计时
    void TimeTicking()
    {
        if (gameTime.seconds > 0)
        {
            gameTime.seconds--;
        }
        else if ((Datas.FlagOperation.isGameWait() && Datas.FlagOperation.isGameStart()
            && Datas.FlagOperation.isGameCount()) || Datas.isMultiPlay == false)
        {
            if (Datas.isMultiPlay)
            {
                Datas.MainHeroInfo.getScoreTable();
            }
            GameFinish();
        }
        if (gameTime.seconds <= deCount.transform.childCount)
        {
            Decount(gameTime.seconds);
        }
        if (Datas.FlagOperation.isGameStart() && gameTime.seconds > deCount.transform.childCount)
        {
            DecountUnactiveNum();
        }
        gameUI.timerText.GetComponent<Text>().text = gameTime.GetTimeText();
    }

    // 游戏结束
    void GameFinish()
    {
        waitRebornUI.SetActive(false);
        gameUI.gameFinishUI.SetActive(true);
        ChangeCursorState(true);

        scoreArrow.GetComponent<GameFinishUIControl>().Init();
        GetComponent<AudioSource>().clip = finishBGMusic;
        GetComponent<AudioSource>().Play();

        //StartCoroutine(GameFinishDelay());
    }
    IEnumerator GameFinishDelay()
    {
        yield return new WaitForSeconds(2);
        Application.LoadLevel("GameFinish");
    }
    #endregion

    #region 火种相关
    // 随机刷新火种
    public void FairyReborn()
    {
        int rnd = Random.Range(0, fairyAppearLocations.Length);
        GameObject fairy = Instantiate(Fairy1, fairyAppearLocations[rnd].position, Quaternion.identity) as GameObject;
        fairy.transform.SetParent(fairyLayer);
    }
    #endregion

    // 游戏区域
    void OnTriggerExit(Collider other)
    {

        //5yue6hao
        //Destroy(other.gameObject);
        if (other.tag == "PlayerHero")
        {
            Destroy(other.gameObject);

            if (other.gameObject.name == Datas.MainHeroInfo.HeroId)
            {
                GameControl.instance.gameUI.DaZhao[Datas.roleID].transform.GetChild(0).gameObject.SetActive(false);
                GameControl.instance.gameUI.DaZhaoBG[Datas.roleID].SetActive(false);
                GameControl.instance.gameUI.FTips.SetActive(false);

                GameControl.instance.waitRebornUI.GetComponent<WaitRebornConreol>().SetRebornBar();

                GameControl.instance.gameUI.ziDanTiao[Datas.roleID].SetActive(false);
                GameControl.instance.gameUI.warnUI.SetActive(false);
                GameControl.instance.gameUI.zidanshuliang.SetActive(false);

                Datas.MainHeroInfo.kills = 0;
                GameControl.instance.gameUI.killamount.text = "0";
            }


            PlayerRebornDelayAPI(other.name, 5.0f);
        }
        else
        {
            HPControl HPC = other.gameObject.GetComponent<HPControl>();
            if (HPC != null)
            {
                HPC.currentHP = -1;  // kill now
            }
            else
            {
                Destroy(other.gameObject);  // destroy directly
            }
        }

        //Destroy(other.gameObject);
        //if (other.tag == "PlayerHero")
        //	PlayerRebornDelayAPI(other.name, 0.0f);

    }

    // 等待一定时间后才开始的函数接口
    IEnumerator waitForOtherPlayers()
    {
        Datas.FlagOperation.gameWait();
        gameTime.seconds = (int)playerWaitingForOtherPlayersTime;
        UDPBack.udpSend(UDPBack.setWaitString());
        yield return new WaitForSeconds(playerWaitingForOtherPlayersTime);

        if (!Datas.FlagOperation.isGameStart())
        {
            UDPBack.udpSend(UDPBack.setStartString());
            // Datas.FlagOperation.gameStart();
            StartCoroutine(delateGameStart());
        }
        // Debug.Log("start");
    }

    // 延迟开始
    IEnumerator delateGameStart()
    {
        yield return new WaitForSeconds(Datas.netDelateTime);
        Datas.FlagOperation.gameStart();
        // Debug.Log ("123");
    }

    // 游戏开始函数， 重生自身人物， 清除所有子弹， 重置火种， 重置时间
    IEnumerator gameRestart()
    {
        // 重生自身人物
        GameObject self = GameObject.Find(Datas.MainHeroInfo.HeroId);
        if (self == null)
        {
            Debug.Log("no gameObject found");
        }
        else
        {
            Destroy(self);
            PlayerRebornDelayAPI(Datas.MainHeroInfo.HeroId, 0.0f);
        }
        // 清除所有子弹
        GameObject buttlesLayer = GameObject.Find("BulletsLayer");
        GameControl.cleanAllSubObject(buttlesLayer);

        // 重置时间
        gameTime.seconds = playerCountTime;

        yield return new WaitForSeconds(playerCountTime + 0.5f);

        Datas.FlagOperation.gameCount();
        gameTime.seconds = playerPlayGameTime;
    }

    // Use this for initialization
    void Awake()
    {
        someFunctions = GetComponent<SomeFunctions>();
        GameArea = GetComponent<SphereCollider>();

        // 单例
        {
            if (instance == null)
                instance = this;
        }

        //fairyAppearLocations = fairyLayer.FindChild("FairyAppearLocations").GetComponentsInChildren<Transform>();

        if (!Datas.isMultiPlay)
        {
            playerWaitingForOtherPlayersTime = playerPlayGameTime;
        }
    }

    void Start()
    {
        HTTPBack.playerRegister(Datas.MainHeroInfo.HeroName);
        //PlayerReborn();
        //FairyReborn();

        // 计时器
        {
            gameUI.timerText.GetComponent<Text>().text = gameTime.GetTimeText();
            InvokeRepeating("TimeTicking", 1, 1);
        }

        // 背景音乐
        {
            //this.GetComponent<AudioSource>().volume = GameControl.musicStaticVolume;
        }

        //计分板
        {
            // lerpScore ();
        }

        // weaponui
        {
            gameUI.WeaponUI.transform.GetChild(Datas.roleID).gameObject.SetActive(true);
            gameUI.Head[Datas.roleID].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 如果新的玩家加入游戏, 或者作为新玩家加入
        if (Datas.isMultiPlay && (Datas.isNewPlayerComeIn || Datas.isTheNewPlayer))
        {
            // 重新刷新box
            if (!Datas.haveInitSupply)
            {
                fairyLayer.GetComponent<SupplyBoxControl>().InitSupply();
                Datas.haveInitSupply = true;
            }
            // 如果是新玩家，更新随机数组
            if (Datas.isTheNewPlayer) UDPBack.udpSend(UDPBack.setStartString());

            Datas.isNewPlayerComeIn = false;
            Datas.isTheNewPlayer = false;
            // 重置游戏控制标识符
            Datas.FlagOperation.gameReset();
            // 根据Date中的otherplayer list清空已经存在的player
            for (int i = 0; i < Datas.MainHeroInfo.list.Count; i++)
            {
                if (Datas.MainHeroInfo.list[i].hero != null)
                {
                    Destroy(Datas.MainHeroInfo.list[i].hero);
                    Datas.MainHeroInfo.list[i].value = "unbuild";
                }
            }
            Datas.FlagOperation.gameWait();
            Datas.FlagOperation.gameStart();
            // 清空游戏其他资源
            StartCoroutine(gameRestart());
        }
        // AI开始
        if (!Datas.isMultiPlay && !AIMainController.AIBegin)
        {
            AIMainController.AIBegin = true;
            AIMainController.instance.addAnAI();
        }
        // 检测序列，第一时间刷新出其他玩家
        for (int i = 0; i < Datas.MainHeroInfo.list.Count; i++)
        {
            if (Datas.MainHeroInfo.list[i].value == "unbuild")
            {
                // Debug.Log(Datas.MainHeroInfo.list[i].key + "Built!");
                PlayerRebornDelayAPI(Datas.MainHeroInfo.list[i].key, 0.0f);
                Datas.MainHeroInfo.list[i].value = "bebuilt";
            }
        }

        if (!Datas.FlagOperation.isGameWait() && !Datas.FlagOperation.isGameStart())
        {
            StartCoroutine(waitForOtherPlayers());
        }
        if (Datas.FlagOperation.isGameStart() && !startGameButton)
        {
            StartCoroutine(gameRestart());
            startGameButton = true;
        }

        // 按ESC呼出菜单并暂停
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ESCGameMenu == false)
            {
                gameUI.ESCMenu.SetActive(true);
                ChangeCursorState(true);
                ESCGameMenu = true;
                if (!Datas.isMultiPlay)
                {
                    Time.timeScale = 0;
                }
            }
            else
            {
                ContinueBtnClick();
            }
        }

        // 更新UI信息
        HPTextUpdate();

        // 锁定鼠标
        if (lockCursor == true)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    public void lerpScore()
    {
        StartCoroutine(startLerpScore());
    }

    IEnumerator startLerpScore()
    {
        while (true)
        {
            Datas.MainHeroInfo.currentScore = (int)Mathf.Lerp(Datas.MainHeroInfo.currentScore, Datas.MainHeroInfo.score, Time.deltaTime);
            GameControl.instance.scoreText.text = Datas.MainHeroInfo.currentScore.ToString();
            yield return null;
        }
    }
}
