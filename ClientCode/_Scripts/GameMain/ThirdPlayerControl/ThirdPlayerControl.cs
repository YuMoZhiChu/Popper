/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HPControl))]
public class ThirdPlayerControl : MonoBehaviour
{

    static public GameObject MainHeroCamera = null;

    public delegate void XuLi(int jieduan);
    public event XuLi XuLiEvent;

    // Add public variables here:
    public GameObject sight;
    public GameObject role;
    public GameObject dieSight;
    public GameObject emotionPoint;
    public GameObject emotion;
    //5yue2hao
    //public GameObject bullet;
    //public GameObject InsaneBullet;
    public RoleControl[] roleControls;

    public GameObject HPBar;
    public Image HPBarImage;

    [System.Serializable]
    public class RoleAttributes
    {
        public int roleJumpCounts = 1;
        public float roleJumpDelay = 0.1f;
        public float roleJumpHeight = 4;
        public float roleMoveSpeed = 3;

        private int m_JumpCounts;
        public int jumpCounts
        {
            get { return m_JumpCounts; }
            set { m_JumpCounts = value; }
        }
        private float m_JumpDelay;
        public float jumpDelay
        {
            get { return m_JumpDelay; }
            set { m_JumpDelay = value; }
        }
        private float m_MoveSpeed;
        public float moveSpeed
        {
            get { return m_MoveSpeed; }
            set { m_MoveSpeed = value; }
        }

        public void Init()
        {
            weaponAttr.Init();
            m_JumpCounts = roleJumpCounts;
            m_JumpDelay = Time.time + roleJumpDelay;
            m_MoveSpeed = roleMoveSpeed;
        }

        [System.Serializable]
        public class WeaponAttributes
        {
            public float roleFireRate = 0.2f;

            //5yue6hao
            public int maxBulletsClips = 7;
            public float reloadClipTime = 3;
            public float normalFireDamage = 10;
            public float normalFireRange = 60;
            public float InsaneFireDamage = 20;
            public float UltFireDamage = 50;

            // 5yue2hao
            public float rolePrepareInsaneFireTime = 3;

            private float m_FireRate;
            public float fireRate
            {
                get { return m_FireRate; }
                set { m_FireRate = value; }
            }
            private int m_BulletsClips;
            public int bulletsClpips
            {
                get { return m_BulletsClips; }
                set { m_BulletsClips = value; }
            }

            public void Init()
            {
                m_FireRate = Time.time + roleFireRate;
                m_BulletsClips = maxBulletsClips;
            }
        }
        public WeaponAttributes weaponAttr;
    }
    public RoleAttributes roleAttr;

    [System.NonSerialized]
    public bool canUlt = false;
    [System.NonSerialized]
    public int Ults = 0;
    private float nextUlt = 0;

    private bool isRunning = false;
    private float runningTime = 0;
    private float spdMultipler = 1;


    // Add private members here:
    private Camera followedCamera;
    [System.NonSerialized]
    public HPControl hpControl;
    private Rigidbody rigidBody;
    private InputController inputController;

    private Vector3 mouseDeltaPosition;
    private Ray fireRay;
    private RaycastHit fireTargetPointRayHit = new RaycastHit();
    public Vector3 fireTargetPoint;
    private Vector3 gunPosition;
    private Animator animator;

    public bool isGrounded = true;
    private float groundedCheckDistance = 0.3f;

    public bool isDead = false;
    private bool isUlt = false;

    // 5yue2hao
    private bool sent = false;  // 避免重复发送蓄力成功event
    private bool isPrepareInsaneFire = false;
    private bool isInsaneFire = false;
    private float prepareTime = -1;

    //5yue8hao
    private int roleNum = 0;

    //5yue12hao
    private Image zidantiao;
    private bool isWarn = false;

    private bool emotionFlag = false;

    private Text zidanshu;

    //5yue13hao
    private float maxHP;

    #region 用于向服务器传输的玩家信息参数
    private float newSpeed;
    private float newDirection;

    private float nowSpeed;
    private float nowDirection;
    private float nowEulerAnglesX;
    private float nowEulerAnglesY;
    private float nowEulerAnglesZ;

    private float nowPositionX = 0.0f;
    private float nowPositionY = 0.0f;
    private float nowPositionZ = 0.0f;
    private float nowhp = 0;
    private float nowAimX = 0.0f;
    private float nowAimY = 0.0f;
    private float nowAimZ = 0.0f;

    private int positionCheckCount;

    private bool bonusUltFlag = false;
    #endregion

    // Add member functions here:
    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundedCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundedCheckDistance))
        {
            isGrounded = true;
            roleAttr.jumpCounts = roleAttr.roleJumpCounts;  // 刷新跳跃次数
        }
        else
        {
            isGrounded = false;
        }
    }

    #region 角色控制
    void MoveJump(string value)
    {
        //rigidBody.velocity += new Vector3(0, 7, 0);

        roleAttr.jumpDelay = Time.time + roleAttr.roleJumpDelay;

        transform.DOMoveY(transform.position.y + roleAttr.roleJumpHeight, 0.3f);

        --roleAttr.jumpCounts;
        isGrounded = false;

    }

    List<Vector3> getTwoVectorFromMessage(string message)
    {
        List<Vector3> l = new List<Vector3>();
        string[] temp = message.Split(',');
        if (temp.Length == 6)
        {
            float pX = float.Parse(temp[0].Split('(')[1]);
            float pY = float.Parse(temp[1]);
            float pZ = float.Parse(temp[2].Split(')')[0]);

            float aX = float.Parse(temp[3].Split('(')[1]);
            float aY = float.Parse(temp[4]);
            float aZ = float.Parse(temp[5].Split(')')[0]);

            l.Add(new Vector3(pX, pY, pZ));
            l.Add(new Vector3(aX, aY, aZ));

            return l;
        }
        else
        {
            Debug.Log("error fire message : " + message);
            return l;
        }
    }

    void FireNormal(string value)
    {
        animator.SetBool("Fire", true);
        roleAttr.weaponAttr.fireRate = Time.time + roleAttr.weaponAttr.roleFireRate;
        /*List<Vector3> l = getTwoVectorFromMessage(message);
        if (l.Count == 2)
        {
            ResetGunPositionAndAimPoint(l[0], l[1]);
        }*/
        //5yue7hao
        if (roleNum == 0 || roleNum == 1)  //如果是Don || hunk则代码控制开枪
            TestGun();
    }

    void InsaneFire(string message)
    {
        animator.SetBool("InsaneFire", true);
        roleAttr.weaponAttr.fireRate = Time.time + roleAttr.weaponAttr.roleFireRate;
        //List<Vector3> l = getTwoVectorFromMessage(message);
        //if (l.Count == 2)
        //{
        //	ResetGunPositionAndAimPoint(l[0], l[1]);
        //}

        // 5yue2hao
        StartInsaneFire();
        roleAttr.weaponAttr.fireRate = Time.time + roleAttr.weaponAttr.roleFireRate;
        //InsaneGun();

    }

    // 5yue2hao
    void StartInsaneFire()
    {
        if (isDead == false && isUlt == false && isInsaneFire == false)
        {
            animator.SetBool("InsaneFire", true);
        }
    }

    void UltReady(string value)
    {
        animator.SetBool("Ult", true);
        animator.Play("Ult");
        isUlt = true;
        roleControls[roleNum].isUlt = true;
        roleAttr.weaponAttr.fireRate = Time.time + roleAttr.weaponAttr.roleFireRate;
        //InsaneGun();
    }

    void ResetUltState()
    {
        isUlt = false;
        roleControls[roleNum].isUlt = false;
        nextUlt = Time.time + 1.0f;
    }




    void ResetTwoPoints(string message)
    {
        //List<Vector3> l = getTwoVectorFromMessage(message);
        //if (l.Count == 2)
        //{
        //	ResetGunPositionAndAimPoint(l[0], l[1]);
        //}
        // Debug.Log ("1");
    }


    #endregion

    #region 武器控制
    //5yue2hao
    void ResetGunPositionAndAimPoint(Vector3 v1, Vector3 v2)
    {
        gunPosition = v1;
        fireTargetPoint = v2;
    }

    void TestGun()
    {
        //5yue2hao
        roleControls[roleNum].NormalFire(gunPosition, fireTargetPoint);
        // Debug.Log ("2");

        //5yue6hao
        --roleAttr.weaponAttr.bulletsClpips;
        if (gameObject.name == Datas.MainHeroInfo.HeroId && roleAttr.weaponAttr.bulletsClpips <= 0)
        {
            ReloadClip();
        }
    }

    void InsaneGun()
    {
        //5yue2hao
        isInsaneFire = true;
        roleControls[roleNum].InsaneFire(gunPosition, fireTargetPoint);
        isInsaneFire = false;
    }
    void UltFire()
    {

        //5yue2hao
        roleControls[roleNum].UltFire(gunPosition, fireTargetPoint);


    }
    #endregion

    #region 角色跟随鼠标左右旋转
    void FollowMouseRotation()
    {
        mouseDeltaPosition = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
        if (gameObject.name != Datas.MainHeroInfo.HeroId)
            return;
        // 更新角色旋转
        {
            //Quaternion rotation = Quaternion.AngleAxis(-mouseDeltaPosition.y, 
            transform.Rotate(0, mouseDeltaPosition.x, 0);
            transform.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0));
        }

    }
    #endregion

    public void HeroDie(string id)
    {
        animator.SetBool("Die", true);
        ResetUltState();

        if (name == Datas.MainHeroInfo.HeroId)
        {
            GameControl.instance.gameUI.DaZhao[Datas.roleID].transform.GetChild(0).gameObject.SetActive(false);
            GameControl.instance.gameUI.DaZhaoBG[Datas.roleID].SetActive(false);
            GameControl.instance.gameUI.FTips.SetActive(false);

            GameControl.instance.gameUI.ziDanTiao[Datas.roleID].SetActive(false);
            GameControl.instance.gameUI.warnUI.SetActive(false);
            GameControl.instance.gameUI.zidanshuliang.SetActive(false);

            Datas.MainHeroInfo.kills = 0;
            GameControl.instance.gameUI.killamount.text = "0";
        }

        //if (this.tag == "PlayerHero")  // 死的是英雄
        {
            //if (isDead)
            //{
            //isDead = false;
            //hpControl.currentHP = 1.0f;
            GameControl.instance.PlayerRebornDelayAPI(id, GetComponent<DelayDie>().delayDieTime);
            //}
        }

        // 移动摄像机
        {
            if (gameObject.name == Datas.MainHeroInfo.HeroId)
            {
                followedCamera.depth = 0;
                dieSight.transform.DOLocalRotate(new Vector3(0, 360, 0), 5f, RotateMode.FastBeyond360);
                dieSight.transform.GetChild(0).GetChild(0).DOLocalMoveZ(-15, 5);

                GameControl.instance.waitRebornUI.GetComponent<WaitRebornConreol>().SetRebornBar();
            }

            if (sight != null)
                sight.SetActive(false);
        }
    }
    /*
    IEnumerator WaitReborn()
    {
        yield return new WaitForSeconds(GetComponent<DelayDie>().delayDieTime - 0.1f);
        GameControl.instance.PlayerReborn();
    }*/

    //5yue6hao
    void ReloadClip()
    {
        StartCoroutine(Reloading());
    }
    IEnumerator Reloading()
    {
        zidantiao.fillAmount = 0;
        zidanshu.text = "0";
        DG.Tweening.DOTween.To(() => zidantiao.fillAmount, x => zidantiao.fillAmount = x, 1, roleAttr.weaponAttr.reloadClipTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(roleAttr.weaponAttr.reloadClipTime);
        roleAttr.weaponAttr.bulletsClpips = roleAttr.weaponAttr.maxBulletsClips;


        GameControl.AddAudio(sight, roleControls[roleNum].reloadClip, 1);

    }

    void InitRoleAndWeapon()
    {
        roleAttr.roleJumpCounts = roleControls[roleNum].roleAttr.roleJumpCounts;
        roleAttr.roleJumpDelay = roleControls[roleNum].roleAttr.roleJumpDelay;
        roleAttr.roleJumpHeight = roleControls[roleNum].roleAttr.roleJumpHeight;
        roleAttr.roleMoveSpeed = roleControls[roleNum].roleAttr.roleMoveSpeed;

        roleAttr.weaponAttr.roleFireRate = roleControls[roleNum].roleAttr.weaponAttr.roleFireRate;
        roleAttr.weaponAttr.maxBulletsClips = roleControls[roleNum].roleAttr.weaponAttr.maxBulletsClips;
        roleAttr.weaponAttr.reloadClipTime = roleControls[roleNum].roleAttr.weaponAttr.reloadClipTime;
        roleAttr.weaponAttr.normalFireDamage = roleControls[roleNum].roleAttr.weaponAttr.normalFireDamage;
        roleAttr.weaponAttr.normalFireRange = roleControls[roleNum].roleAttr.weaponAttr.normalFireRange;
        roleAttr.weaponAttr.InsaneFireDamage = roleControls[roleNum].roleAttr.weaponAttr.InsaneFireDamage;
        roleAttr.weaponAttr.UltFireDamage = roleControls[roleNum].roleAttr.weaponAttr.UltFireDamage;
        roleAttr.weaponAttr.rolePrepareInsaneFireTime = roleControls[roleNum].roleAttr.weaponAttr.rolePrepareInsaneFireTime;

        // 初始化角色信息
        roleAttr.Init();
    }

    //5yue8hao
    void Emotion(string msg)
    {
        int i = int.Parse(msg);
        GameObject emj = Instantiate(emotion, emotionPoint.transform.position, Quaternion.identity) as GameObject;
        emj.transform.GetChild(i).gameObject.SetActive(true);
        emj.transform.SetParent(emotionPoint.transform);
        emj.transform.DORotate(new Vector3(0, 360, 0), 2, RotateMode.FastBeyond360).SetEase(Ease.Linear);
    }

    // Use this for initialization
    void Awake()
    {

        rigidBody = GetComponent<Rigidbody>();
        hpControl = GetComponent<HPControl>();
        followedCamera = sight.transform.FindChild("Camera").GetComponent<Camera>();

        inputController = GetComponent<InputController>();

    }

    void Start()
    {
        {
            int hp = 0;
            if (roleNum == 0)
                hp = 400;
            else if (roleNum == 1)
                hp = 800;
            else
                hp = 300;
            hpControl.HP = hp;
            hpControl.currentHP = hp;
        }

        animator = role.GetComponent<Animator>();
        // 隐藏鼠标
        Cursor.visible = false;
        nowDirection = nowSpeed = nowEulerAnglesX = nowEulerAnglesY = nowEulerAnglesZ = 0.0f;
        positionCheckCount = 0;

        //5yue8hao
        string id = gameObject.name;
        Datas.otherPlayer o = Datas.MainHeroInfo.findPlayerByKey(id);
        roleNum = o.roleNum;

        //5yue6hao
        InitRoleAndWeapon();

        //5yue12hao
        zidantiao = GameControl.instance.gameUI.ziDanTiao[Datas.roleID].transform.GetChild(0).GetComponent<Image>();
        zidanshu = GameControl.instance.gameUI.zidanshuliang.transform.GetChild(0).GetComponent<Text>();
        if (gameObject.name == Datas.MainHeroInfo.HeroId)
        {
            GameControl.instance.gameUI.ziDanTiao[Datas.roleID].SetActive(true);
            
            GameControl.instance.gameUI.zidanshuliang.SetActive(true);
            GameControl.instance.gameUI.DaZhao[Datas.roleID].SetActive(true);
        }

        if (gameObject.name == Datas.MainHeroInfo.HeroId)
            // InvokeRepeating ("AddScoreByTime", 1, 1);

            //5yue13hao
            if (gameObject.name == Datas.MainHeroInfo.HeroId)
            {
                HPBar.SetActive(false);
                MainHeroCamera = followedCamera.gameObject;
            }
            else
            {

            }
        maxHP = hpControl.HP;
    }

    void AddScoreByTime()
    {
        Datas.MainHeroInfo.changeMyScore(GameControl.instance.scorePerSecond);
    }

    // Update is called once per frame
    void Update()
    {
        // 增加AI
        // if (Input.GetKeyDown(KeyCode.Equals)) {
        // 	AIMainController.instance.addAnAI();
        // }

        if (GameControl.instance.gameTime.seconds % 60 == 0)  // 每分钟+1大招
        {
            if (bonusUltFlag == true)
            {
                gameObject.GetComponent<ThirdPlayerControl>().Ults += 1;

                //audio
                GameControl.AddAudio(this.gameObject, GameControl.instance.acquireUltAudio);

                // UI
                if (gameObject.name == Datas.MainHeroInfo.HeroId)
                {
                    GameControl.instance.gameUI.DaZhao[Datas.roleID].transform.GetChild(0).gameObject.SetActive(true);
                    GameControl.instance.gameUI.DaZhao[Datas.roleID].transform.GetChild(1).gameObject.SetActive(true);
                    GameControl.instance.gameUI.DaZhao[Datas.roleID].transform.GetChild(1).GetComponent<Animator>().Play("ultanim");
                    GameControl.instance.gameUI.DaZhaoBG[Datas.roleID].SetActive(true);
                    GameControl.instance.gameUI.FTips.SetActive(true);
                }
                bonusUltFlag = false;
            }
        }
        else
        {
            bonusUltFlag = true;
        }

        //5yue13hao
        if (MainHeroCamera != null)
            HPBar.transform.LookAt(MainHeroCamera.transform.position);
        if (HPBar.activeSelf == true)
            HPBarImage.fillAmount = hpControl.currentHP / maxHP;

        if (gameObject.name == Datas.MainHeroInfo.HeroId)
        {
            // Debug.Log (fireTargetPoint);
            //获取瞄准点
            fireRay = followedCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(fireRay, out fireTargetPointRayHit, roleAttr.weaponAttr.normalFireRange, 1 << 8))  // 场景物件在第8层
            {
                fireTargetPoint = fireTargetPointRayHit.point;
            }
            else
            {
                fireTargetPoint = followedCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 100));
            }
        }

        // 状态初始化
        {
            animator.SetBool("Ult", false);
            animator.SetBool("Jump", false);
            animator.SetBool("DoubleJump", false);
            animator.SetBool("Fire", false);

            // 5yue2hao
            animator.SetBool("InsaneFire", false);

        }

        //自单挑同步
        {
            if (gameObject.name == Datas.MainHeroInfo.HeroId && roleAttr.weaponAttr.bulletsClpips > 0)  // main hero
            {
                zidantiao.fillAmount = (roleAttr.weaponAttr.bulletsClpips + 0.0f) / roleAttr.weaponAttr.maxBulletsClips;
                zidanshu.text = roleAttr.weaponAttr.bulletsClpips.ToString();
            }
        }

        //危险提示
        {
            if (gameObject.name == Datas.MainHeroInfo.HeroId)
            {
                if (Datas.MainHeroInfo.HeroCurrentHP / Datas.MainHeroInfo.HeroTotalHP <= 0.2f && isWarn == false)
                {
                    GameControl.instance.gameUI.warnUI.SetActive(true);
                    isWarn = true;
                } if (Datas.MainHeroInfo.HeroCurrentHP / Datas.MainHeroInfo.HeroTotalHP > 0.2f && isWarn == true)
                {
                    GameControl.instance.gameUI.warnUI.SetActive(false);
                    isWarn = false;
                }
            }
        }

        if (gameObject.name == Datas.MainHeroInfo.HeroId && MulTiEscGameMenuNotDistrub() && (isDead == false) && (isUlt == false || roleNum == 2 || roleNum == 1))
            FollowMouseRotation();

        // 更新数据
        #region 血量显示
        if (gameObject.name == Datas.MainHeroInfo.HeroId)
        {
            Datas.MainHeroInfo.HeroTotalHP = hpControl.HP;
            Datas.MainHeroInfo.HeroCurrentHP = hpControl.currentHP;
        }
        if (hpControl.currentHP <= 0 && isDead == false)
        {
            isDead = true;
            HeroDie(gameObject.name);
        }
        #endregion

        #region 英雄操作
        if (Datas.FlagOperation.canMove() && gameObject.name == Datas.MainHeroInfo.HeroId && MulTiEscGameMenuNotDistrub()
            && (isDead == false) && (isUlt == false || roleNum == 2))  // 只能操作同名英雄
        {
            // 跳跃
            if (Input.GetButtonDown("Jump"))
            {
                CheckGroundStatus();
                if (isGrounded == true)
                {
                    animator.SetBool("Jump", true);
                    inputController.PushAnAction("MoveJump:0");
                    UDPBack.udpSend("action;MoveJump:0");
                }
                else if (roleAttr.jumpCounts > 0)
                {
                    animator.SetBool("DoubleJump", true);
                    animator.Play("JumpTwice");
                    inputController.PushAnAction("MoveJump:0");
                    UDPBack.udpSend("action;MoveJump:0");
                }
            }

            // 开火
            if (Input.GetButtonDown("Fire1") && CanNormalFire())
            {
                string fireMessageString = "FireNormal:" + "0";
                UDPBack.udpSend("action;" + fireMessageString);
                inputController.PushAnAction(fireMessageString);
            }

            // 疯狂开火
            //5yue2hao
            if (Input.GetButtonDown("Fire2") && CanPrepareInsaneFire())
            {
                if (XuLiEvent != null)
                    XuLiEvent(0);
                isPrepareInsaneFire = true;
                prepareTime = 0;
            }
            if (Input.GetButton("Fire2") && CanBePreparingInsaneFire())
            {
                if (CanBePreparingInsaneFire())
                    prepareTime += Time.deltaTime;
                else
                    prepareTime = 0;
                if (prepareTime >= roleAttr.weaponAttr.rolePrepareInsaneFireTime && sent == false)
                {
                    if (XuLiEvent != null)
                        XuLiEvent(1);
                    sent = true;
                }
            }
            if (Input.GetButtonUp("Fire2") && CanLaunchInsaneFire())
            {
                if (prepareTime >= roleAttr.weaponAttr.rolePrepareInsaneFireTime)
                {
                    if (XuLiEvent != null)
                        XuLiEvent(2);
                    string fireMessageString = "InsaneFire:0";
                    UDPBack.udpSend("action;" + fireMessageString);
                    inputController.PushAnAction(fireMessageString);
                    prepareTime = 0;
                }
                else
                {
                    if (XuLiEvent != null)
                        XuLiEvent(3);
                }
                isPrepareInsaneFire = false;
                sent = false;
            }

            // 大招
            if (Input.GetKeyDown(KeyCode.F) && canUltfire())
            {
                Ults -= 1;

                if (Ults <= 0 && name == Datas.MainHeroInfo.HeroId)
                {
                    GameControl.instance.gameUI.DaZhao[Datas.roleID].transform.GetChild(0).gameObject.SetActive(false);
                    GameControl.instance.gameUI.DaZhaoBG[Datas.roleID].SetActive(false);
                    GameControl.instance.gameUI.FTips.SetActive(false);
                }

                isUlt = true;
                inputController.PushAnAction("UltReady:0");
                UDPBack.udpSend("action;" + "UltReady:0");
            }


            //加速跑
            if (Input.GetKeyDown(KeyCode.LeftShift) && CanRun())
            {
                isRunning = true;
                runningTime = Time.time + 2;
            }
            if (Input.GetKey(KeyCode.LeftShift) && isRunning == true)
            {
                if (runningTime >= Time.time)
                {
                    spdMultipler = 2;
                }
                else
                {
                    spdMultipler = 1;
                }
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) && isRunning == true)
            {
                spdMultipler = 1;
                isRunning = false;
                runningTime = Time.time + 5;
            }

        }
        #endregion

        #region 表情
        if (canEmotion())
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && canEmotion())
            {
                inputController.PushAnAction("Emotion:0");
                UDPBack.udpSend("action;" + "Emotion:0");
                emotionFlag = true;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && canEmotion())
            {
                inputController.PushAnAction("Emotion:1");
                UDPBack.udpSend("action;" + "Emotion:1");
                emotionFlag = true;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && canEmotion())
            {
                inputController.PushAnAction("Emotion:2");
                UDPBack.udpSend("action;" + "Emotion:2");
                emotionFlag = true;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && canEmotion())
            {
                inputController.PushAnAction("Emotion:3");
                UDPBack.udpSend("action;" + "Emotion:3");
                emotionFlag = true;
            }

        }
        #endregion

        ActionAnother();

        if (gameObject.name == Datas.MainHeroInfo.HeroId && Datas.isMultiPlay)
        {

            #region hero1向服务器发送移动动作
            // 同步位置消息
            // 瞄准点
            Vector3 newPosition = gameObject.transform.position;
            Vector3 newFireTargetPoint = fireTargetPoint;
            Datas.otherPlayer o = Datas.MainHeroInfo.findPlayerByKey(gameObject.name);
            float hp = nowhp;
            if (o != null && o.HPC != null) hp = o.HPC.currentHP;

            // 过滤消息
            if (System.Math.Abs(newPosition.x - nowPositionX) > 0.1
                || System.Math.Abs(newPosition.y - nowPositionY) > 0.1
                || System.Math.Abs(newPosition.z - nowPositionZ) > 0.1
                || System.Math.Abs(hp - nowhp) > 0.1
                || System.Math.Abs(newFireTargetPoint.x - nowAimX) > 0.1
                || System.Math.Abs(newFireTargetPoint.y - nowAimY) > 0.1
                || System.Math.Abs(newFireTargetPoint.z - nowAimZ) > 0.1
            )
            {
                UDPBack.udpSend(UDPBack.setPositionString(newPosition.ToString(), hp.ToString(), newFireTargetPoint.ToString()));
                nowPositionX = newPosition.x;
                nowPositionY = newPosition.y;
                nowPositionZ = newPosition.z;
                nowhp = hp;
                nowAimX = newFireTargetPoint.x;
                nowAimY = newFireTargetPoint.y;
                nowAimZ = newFireTargetPoint.z;
            }
            // 移动
            float speed = newSpeed;
            float direction = newDirection;

            // 相位
            float eulerAnglesX_ = gameObject.transform.rotation.eulerAngles.x;
            float eulerAnglesY_ = gameObject.transform.rotation.eulerAngles.y;
            float eulerAnglesZ_ = gameObject.transform.rotation.eulerAngles.z;

            //// 过滤消息
            //// 只有当出现位移变换，视角变换时，才对服务器发送请求，同时记录帧数，以保证位移的确定
            if (!(System.Math.Abs(nowDirection - direction) < 0.1
                && System.Math.Abs(nowSpeed - speed) < 0.1
                && System.Math.Abs(nowEulerAnglesX - eulerAnglesX_) < 1
                && System.Math.Abs(nowEulerAnglesY - eulerAnglesY_) < 1
                && System.Math.Abs(nowEulerAnglesZ - eulerAnglesZ_) < 1))
            {
                //    // 向服务器发送移动信息
                //    // 更新当前物体的 速度，方向，3个欧拉角度
                nowSpeed = speed;
                nowDirection = direction;
                nowEulerAnglesX = eulerAnglesX_;
                nowEulerAnglesY = eulerAnglesY_;
                nowEulerAnglesZ = eulerAnglesZ_;
                // Debug.Log(moveString);
                // Debug.Log(thisGameObject.transform.position);
                // Debug.Log(nowSpeed);
                // Debug.Log(nowDirection);
                UDPBack.udpSend(UDPBack.setMoveString(nowSpeed, nowDirection, nowEulerAnglesX, nowEulerAnglesY, nowEulerAnglesZ));

            }
            #endregion
        }
    }

    void FixedUpdate()
    {

        if (gameObject.name == Datas.MainHeroInfo.HeroId && isDead == false && (isUlt == false || roleNum == 2)
            && (isDead == false) && Datas.FlagOperation.canMove())  // 只能操作同名英雄
        {
            // 移动
            float speed = Input.GetAxis("Vertical");
            float direction = Input.GetAxis("Horizontal");

            newSpeed = speed * spdMultipler * roleAttr.moveSpeed;
            newDirection = direction * spdMultipler * roleAttr.moveSpeed;

            animator.SetFloat("Speed", speed);
            animator.SetFloat("Direction", direction);
            Vector3 horizon = newDirection * transform.right;
            Vector3 vertical = newSpeed * transform.forward;
            // Vector3 force = new Vector3(0, rigidBody.velocity.y, 0) + horizon * roleAttr.moveSpeed + vertical * roleAttr.moveSpeed;
            // this.rigidBody.AddForce(new Vector3(force.x*0.1f, force.y*0.1f, force.z*0.1f), ForceMode.VelocityChange);
            rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0) + horizon + vertical;

            // send

            // receive

        }

        if (rigidBody.velocity.sqrMagnitude <= 0.01)
        {

        }

    }

    // 另外一个单位的行动同步
    void ActionAnother()
    {
        if (UDPBack.UDPMessageList.Count == 0) return;
        UDPBack.UDPMessage udpMessage;
        udpMessage = UDPBack.UDPMessageList.Dequeue();
        if (udpMessage == null)
            return;
        GameObject go = GameObject.Find(udpMessage.idString);
        if (go == null)
        {
            // Debug.Log (udpMessage.idString);
            Debug.Log("no gameObject found");
            return;
        }
        Datas.otherPlayer o = Datas.MainHeroInfo.findPlayerByKey(go.name);
        if (o == null)
        {
            Debug.Log("no this player found!");
            return;
        }
        if (o.TPC.isDead)
        {
            // Debug.Log ("the player is dead!");
            return;
        }
        if (udpMessage.actionString != "")
        {
            #region 英雄2action操作
            string newMessageForPlayer2 = udpMessage.actionString;
            if (newMessageForPlayer2 != null)
            {
                // 跳跃
                if (newMessageForPlayer2.Split(':')[0] == "MoveJump" && MulTiEscGameMenuNotDistrub())
                {
                    CheckGroundStatus();
                    if (isGrounded == true)
                    {
                        o.TPC.inputController.PushAnAction(newMessageForPlayer2);
                    }
                    else if (roleAttr.jumpCounts > 0)
                    {
                        o.TPC.inputController.PushAnAction(newMessageForPlayer2);
                    }
                }
                // 表情
                if (newMessageForPlayer2.Split(':')[0] == "Emotion" && MulTiEscGameMenuNotDistrub())
                {
                    o.TPC.inputController.PushAnAction(newMessageForPlayer2);
                }
                // 开火
                if (newMessageForPlayer2.Split(':')[0] == "FireNormal" && MulTiEscGameMenuNotDistrub() && Time.time > roleAttr.weaponAttr.fireRate)
                {
                    o.TPC.inputController.PushAnAction(newMessageForPlayer2);
                }
                // InsaneFire
                if (newMessageForPlayer2.Split(':')[0] == "InsaneFire" && MulTiEscGameMenuNotDistrub() && Time.time > roleAttr.weaponAttr.fireRate)
                {
                    o.TPC.inputController.PushAnAction(newMessageForPlayer2);
                }
                // ult
                if (newMessageForPlayer2.Split(':')[0] == "UltReady" && MulTiEscGameMenuNotDistrub() && Time.time > roleAttr.weaponAttr.fireRate)
                {
                    o.TPC.inputController.PushAnAction(newMessageForPlayer2);
                }
                // logout
                if (newMessageForPlayer2.Split(':')[0] == "UltReady" && MulTiEscGameMenuNotDistrub())
                {

                }
            }
            #endregion
        }
        //if (udpMessage.cameraString != "")
        //{
        //    #region 英雄2摄像机操作
        //    string[] temp = udpMessage.cameraString.Split(',');
        //    if (temp.Length == 7)
        //    {
        //        float pX = float.Parse(temp[1].Split('(')[1]);
        //        float pY = float.Parse(temp[2]);
        //        float pZ = float.Parse(temp[3].Split(')')[0]);

        //        float eX = float.Parse(temp[4].Split('(')[1]);
        //        float eY = float.Parse(temp[5]);
        //        float eZ = float.Parse(temp[6].Split(')')[0]);

        //        Camera camera = go.GetComponent<ThirdPlayerControl>().sight.transform.FindChild("Camera").GetComponent<Camera>();

        //        camera.transform.position = new Vector3(pX, pY, pZ);
        //        camera.transform.eulerAngles = new Vector3(eX, eY, eZ);

        //        go.GetComponent<ThirdPlayerControl>().followedCamera = camera;
        //    }
        //    #endregion
        //}
        if (udpMessage.moveString != "")
        {
            #region 英雄2移动操作
            string newMessageForPlayer2 = udpMessage.moveString;
            if (newMessageForPlayer2 != null && newMessageForPlayer2 != "")
            {

                string[] temp = newMessageForPlayer2.Split(',');
                if (temp.Length == 5)
                {
                    float moveDataForPlayer2Speed = float.Parse(temp[0]);
                    float moveDataForPlayer2Direction = float.Parse(temp[1]);
                    float moveDataForPlayer2X = float.Parse(temp[2]);
                    float moveDataForPlayer2Y = float.Parse(temp[3]);
                    float moveDataForPlayer2Z = float.Parse(temp[4]);

                    if (moveDataForPlayer2Speed - Datas.RotateNoChangeForAI <= 0.01 && !Datas.isMultiPlay)
                    {
                        // AI移动
                        o.TPC.animator.SetFloat("Speed", 1);
                        o.TPC.animator.SetFloat("Direction", 1);
                        Vector3 lookat = new Vector3(moveDataForPlayer2X, moveDataForPlayer2Y, moveDataForPlayer2Z);
                        // Debug.Log("lookat: " + lookat.ToString());
                        // o.transform.DORotate(lookat, 0.5f);
                        o.transform.LookAt(new Vector3(moveDataForPlayer2X, o.transform.position.y, moveDataForPlayer2Z));
                        Vector3 oP = o.transform.position;
                        float value = Mathf.Sqrt((lookat.x - oP.x) * (lookat.x - oP.x) + (lookat.y - oP.y) * (lookat.y - oP.y) + (lookat.z - oP.z) * (lookat.z - oP.z));
                        Vector3 movement = (lookat - o.transform.position);
                        // movement.y = 0;
                        // Debug.Log(value);
                        // 9 是速度参数，之后可以进行修改
                        o.TPC.rigidBody.velocity = (lookat - o.transform.position) * 9 / value;
                    }
                    else
                    {
                        // 设置选择角度
                        go.transform.rotation = Quaternion.Euler(new Vector3(moveDataForPlayer2X, moveDataForPlayer2Y, moveDataForPlayer2Z));
                        // 移动
                        o.TPC.animator.SetFloat("Speed", moveDataForPlayer2Speed);
                        o.TPC.animator.SetFloat("Direction", moveDataForPlayer2Direction);
                        Vector3 horizon = moveDataForPlayer2Direction * transform.right;
                        Vector3 vertical = moveDataForPlayer2Speed * transform.forward;
                        o.TPC.rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0) + horizon * roleAttr.moveSpeed + vertical * roleAttr.moveSpeed;
                    }
                }

            }
            #endregion
        }
        if (udpMessage.positionString != "")
        {
            #region 英雄2位置与血量,瞄准点同步操作
            string[] temp = udpMessage.positionString.Split(',');
            if (temp.Length == 7)
            {
                float pX = float.Parse(temp[0].Split('(')[1]);
                float pY = float.Parse(temp[1]);
                float pZ = float.Parse(temp[2].Split(')')[0]);
                float hp = float.Parse(temp[3]);
                float fX = float.Parse(temp[4].Split('(')[1]);
                float fY = float.Parse(temp[5]);
                float fZ = float.Parse(temp[6].Split(')')[0]);

                // if is AI
                if (hp - Datas.HPNoChangeForAI < 0.1 && !Datas.isMultiPlay)
                {
                    Vector3 lookat = new Vector3(fX, fY, fZ);
                    // Debug.Log("lookat: " + lookat.ToString());
                    // o.transform.DORotate(lookat, 0.5f);
                    o.transform.LookAt(new Vector3(fX, o.transform.position.y, fZ));
                    o.TPC.fireTargetPoint = new Vector3(fX, fY, fZ);
                }
                else
                {
                    // multi player work
                    float resetTime = 0.0f;
                    if (o.TPC.isGrounded)
                    {
                        resetTime = Time.deltaTime * 5;
                    }
                    else
                    {
                        resetTime = Time.deltaTime * 2;
                    }

                    go.transform.DOLocalMove(new Vector3(pX, pY, pZ), resetTime).SetEase(Ease.Linear);
                    // go.transform.position = new Vector3(pX, pY, pZ);

                    o.HPC.currentHP = hp;
                    o.HPC.CheckHP();


                    o.TPC.fireTargetPoint = new Vector3(fX, fY, fZ);
                }
            }
            #endregion
        }

    }


    #region 5.2
    bool CanNormalFire()
    {
        return (Time.time > roleAttr.weaponAttr.fireRate && roleAttr.weaponAttr.bulletsClpips > 0 && isDead == false && isUlt == false && isPrepareInsaneFire == false && isInsaneFire == false);
    }
    bool CanPrepareInsaneFire()
    {
        return (isDead == false && isUlt == false && isPrepareInsaneFire == false && isInsaneFire == false);
    }
    bool CanBePreparingInsaneFire()
    {
        return (isDead == false && isUlt == false && isPrepareInsaneFire == true && isInsaneFire == false);
    }
    bool CanLaunchInsaneFire()
    {
        return (isDead == false && isUlt == false && isPrepareInsaneFire == true && isInsaneFire == false);
    }
    bool canInsanefire()
    {
        return (isDead == false && isUlt == false && isPrepareInsaneFire == true && isInsaneFire == false);
    }
    bool canUltfire()
    {
        return (isDead == false && isUlt == false && isPrepareInsaneFire == false && isInsaneFire == false && Ults > 0 && Time.time >= nextUlt);
    }
    bool CanRun()
    {
        return (Time.time > runningTime && isDead == false && isUlt == false && isPrepareInsaneFire == false && isInsaneFire == false);
    }
    bool canEmotion()
    {
        return (isDead == true && MulTiEscGameMenuNotDistrub() && emotionFlag == false);
    }
    bool MulTiEscGameMenuNotDistrub()
    {
        if (Datas.isMultiPlay)
            return true;
        return GameControl.ESCGameMenu == false;
    }
    #endregion



}




