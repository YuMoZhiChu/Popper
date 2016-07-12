/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

static public class Datas
{

    static public bool isMultiPlay = false;

    static public Queue<int> RandomNumQueue = new Queue<int>();

    static public int[] RandomArray;

    static public float netDelateTime = 0.0f;

    static public float HPNoChangeForAI = 999f;

    static public float RotateNoChangeForAI = -2f;

    static public bool isNewPlayerComeIn = false;

    static public bool isTheNewPlayer = true;

    static public bool haveInitSupply = false;

    static public Vector3 SupplyPoisition;

    static public void reset()
    {
        HTTPBack.reset();
        UDPBack.reset();
        AIMainController.AIBegin = false;
    }

    public class otherPlayer
    {
        public string key;
        public string value;
        public int roleNum;
        public string name;
        public int score;
        public ThirdPlayerControl TPC;
        public HPControl HPC;
        public Transform transform;
        public GameObject hero;
        // value for ai
        public Vector3 targetToMove;
    }
    #region 基础信息
    static public class MainHeroInfo
    {
        static public string HeroName = "LJK";
        static public string HeroId = "";
        static public int score = 0;
        static public int currentScore = 0;
        static public float HeroTotalHP;
        static public float HeroCurrentHP;

        static public string Hero2Name = "";
        static public string Hero2Id = "";

        static public int kills = 0;

        static public void changeMyScore(int detalScore)
        {
            score += detalScore;
            UDPBack.udpSend(UDPBack.setChangeScoreString(detalScore));

            //5yue12hao
            GameControl.instance.scoreText.text = score.ToString();
        }

        static public void getScoreTable()
        {
            UDPBack.udpSend(UDPBack.getScoreTable());
        }

        static public List<otherPlayer> list = new List<otherPlayer>();

        static public void setUpPlayerDictionary(string id, int roleNum, string name)
        {
            bool findId = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].key == id) findId = true;
            }

            if (!findId)
            {
                otherPlayer o = new otherPlayer();
                o.key = id;
                o.value = "unbuild";
                o.roleNum = roleNum;
                o.name = name;
                o.score = 0;
                o.TPC = null;
                o.HPC = null;
                list.Add(o);

                // 如果游戏已经开始，则新玩家加入游戏
                if (FlagOperation.isGameStart() && isMultiPlay)
                {
                    haveInitSupply = false;
                    isNewPlayerComeIn = true;
                }
            }

            // Debug.Log(list.ToString());
        }

        static public otherPlayer findPlayerByKey(string key)
        {
            otherPlayer o = new otherPlayer();
            for (int i = 0; i < list.Count; i++)
            {
                if (key == list[i].key)
                {
                    o = list[i];
                    break;
                }
            }

            return o;
        }

        static public void setPlayerLogout(string key) {
            for (int i = 0; i < list.Count; i++)
            {
                if (key == list[i].key)
                {
                    // Debug.Log("the local score : " + score);
                    list.RemoveAt(i);
                    break;
                    // Debug.Log("the online score : " + onlineScore);
                }
            }
        }

        static public void setPlayerValueByKey(string key, string value) {
            for (int i = 0; i < list.Count; i++)
            {
                if (key == list[i].key)
                {
                    // Debug.Log("the local score : " + score);
                    list[i].value = value;
                    break;
                    // Debug.Log("the online score : " + onlineScore);
                }
            }
        }

        static public void setPlayerScoreByKey(string key, int onlineScore)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (key == list[i].key)
                {
                    // Debug.Log("the local score : " + score);
                    list[i].score = onlineScore;
                    break;
                    // Debug.Log("the online score : " + onlineScore);
                }
            }
        }

        static public void setPlayerTPCByKey(string key, ref ThirdPlayerControl tpc)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (key == list[i].key)
                {
                    // Debug.Log("the local score : " + score);
                    list[i].TPC = tpc;
                    break;
                    // Debug.Log("the online score : " + onlineScore);
                }
            }
        }

        static public void setPlayerHPCByKey(string key, ref HPControl hpc)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (key == list[i].key)
                {
                    // Debug.Log("the local score : " + score);
                    list[i].HPC = hpc;
                    break;
                    // Debug.Log("the online score : " + onlineScore);
                }
            }
        }
    }
    #endregion

    public enum BulletMoveType
    {
        Line,
        Path
    }

    static public int gameMaxNum = 6;
    static public int roleID = 1;


    #region 子弹信息
    static public class buttleMessage
    {
        static public string buttleTag = "bulletTag";
    }
    #endregion

    #region 标识符操作
    static public class FlagOperation
    {

        /*
        标志符操作

            */

        static private int gameFlag = 0;
        static private int wait_flag = 1 << 0;
        static private int start_flag = 1 << 1;
        static private int count_flag = 1 << 2;

        static public int getGameFlag()
        {
            return gameFlag;
        }

        static public bool isGameWait()
        {
            if (0 == (gameFlag & wait_flag))
                return false;
            return true;
        }

        static public void gameWait()
        {
            gameFlag |= wait_flag;
        }

        static public bool isGameStart()
        {
            if (0 == (gameFlag & start_flag))
                return false;
            return true;
        }

        static public void gameStart()
        {
            gameFlag |= start_flag;
        }

        static public bool isGameCount()
        {
            if (0 == (gameFlag & count_flag))
                return false;
            return true;
        }

        static public void gameCount()
        {
            gameFlag |= count_flag;
        }

        static public bool canMove()
        {
            if (isGameStart() && !isGameCount())
            {
                // Debug.Log(gameFlag);
                return false;
            }
            return true;
        }

        static public void gameReset()
        {
            gameFlag = 0;
        }
    }
    #endregion
}
