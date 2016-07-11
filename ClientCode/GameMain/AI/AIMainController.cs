using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIMainController : MonoBehaviour {

	static public AIMainController instance;

	static public bool AIBegin = false;

	public class AIObj {
		public string Id;	
	}

	static public List<AIObj> AIList = new List<AIObj> ();
	static public Queue<string> AIStringList = new Queue<string> ();

	public GameObject[] positionList;

	void Awake() {
		{
			if (instance == null)
				instance = this;
		}
		AIList.Clear ();
		AIStringList.Clear ();
	}

	int getMaxHp(int roleNum) {
		if (roleNum == 0) {
			return 300;
		}
		else if (roleNum == 1) {
			return 450;
		} else {
			return 200;
		}
		return 0;
	}

	public void addAnAI () {
		// 	Debug.Log ("+");
		int playeridNum = int.Parse(Datas.MainHeroInfo.HeroId);
		int newAiNum = playeridNum + AIList.Count + 1;
		AIObj ai = new AIObj ();
		ai.Id = newAiNum.ToString ();
		AIList.Add (ai);

		int bronPositionPoint = Random.Range(0, positionList.Length);
		//Debug.Log (bronPositionPoint);
		int roleNum = Random.Range(0, 3);

		string message = newAiNum.ToString () + "-" + roleNum.ToString () + "-\"AI\"|position;"
			+ positionList [bronPositionPoint].transform.position.ToString () + "," + getMaxHp (roleNum).ToString () + ",(0, 0, 0)";
		// Debug.Log (message);
		AIStringList.Enqueue (message);
		// Debug.Log (AIStringList.Count);
	}

	// Use this for initialization
	void Start () {
	}

	public string GetAIString() {
		return AIStringList.Dequeue();
	}

    public float getDistance(Vector3 p, Vector3 q) {
        return Mathf.Sqrt((p.x - q.x) * (p.x - q.x) + (p.y - q.y) * (p.y - q.y) + (p.z - q.z) * (p.z - q.z));
    }

    Vector3 findNearestPositionToMove(Vector3 v) {
        float min = 0.0f;
        Vector3 res = new Vector3();
        min = 10000f;
        for (int i = 0; i < positionList.Length; i++)
        {
            float thisDistance = getDistance(positionList[i].transform.position, v);
            if (thisDistance < 1)
            {
                continue;
            }
            if (thisDistance < min * 1.5)
            {
                // 有几率跑向下一个点
                int r = Random.Range(0, 2);
                if (r == 1)
                {
                    res = positionList[i].transform.position;
                    min = thisDistance;
                }
            }
        }
        // Debug.Log(v.ToString() + res.ToString() + min);
        return res;
        //return positionList[Random.Range(0, positionList.Length)].transform.position;
    }

    bool needChangeMovePoint(Datas.otherPlayer o) {
        if (o == null) return true;
        if (o.targetToMove.x == 0) return true;
        if (o.transform == null) return true;

        Vector3 p = o.targetToMove;
        Vector3 q = o.transform.position;
        float distance = (p.x - q.x) * (p.x - q.x) + (p.y - q.y) * (p.y - q.y) + (p.z - q.z) * (p.z - q.z);
        // Debug.Log(distance);
        if (distance < 0.1)
        {
            // Debug.Log("change");
            return true;
        }

        return false;
    }

    void moveToPoint(Datas.otherPlayer o) {
        if (o == null || o.transform == null)
            return;

		int r = Random.Range(0, 100);
		if (r > 95) {
			moveToNextPoint (o);
			return;
		}

        Vector3 res = o.targetToMove;
        // Debug.Log(res.ToString());

        string message = o.key + "-" + o.roleNum + "-\"" + o.name + "\"|move;" + "-2,-2" + ","
             + res.x.ToString() + "," + res.y.ToString() + "," + res.z.ToString();
        AIStringList.Enqueue(message);
    }

    void moveToNextPoint(Datas.otherPlayer o) {
        if (o == null || o.transform == null)
            return;

        Vector3 nowPosition =  o.transform.position;
        Vector3 res = findNearestPositionToMove(nowPosition);
        o.targetToMove = res;
        // Debug.Log(res.ToString());

        string message = o.key + "-" + o.roleNum + "-\"" + o.name + "\"|move;" + "-2,-2" + ","
             + res.x.ToString() + "," + res.y.ToString() + ","  + res.z.ToString();
        AIStringList.Enqueue(message);
    }

	bool seeEnemy(Datas.otherPlayer o) {
        Datas.otherPlayer player = Datas.MainHeroInfo.findPlayerByKey(Datas.MainHeroInfo.HeroId);

        if (o == null || player == null || o.transform == null || player.transform == null) return false;

        Vector3 p = o.transform.position;
        Vector3 q = player.transform.position;
        float distance = Mathf.Sqrt( (p.x-q.x)* (p.x - q.x) + (p.y - q.y) * (p.y - q.y) + (p.z - q.z) * (p.z - q.z));
        // Debug.Log("vector : " + p.ToString() + q.ToString());
        if (distance > 30) return false;

        string message = o.key + "-" + o.roleNum + "-\"" + o.name + "\"|position;"
               + "(0,0,0)" + "," + Datas.HPNoChangeForAI + "," + q.ToString();
        AIStringList.Enqueue(message);
        return true;
	}

	bool HPmoreThanHalf(Datas.otherPlayer o) {
		if (o.HPC == null || o.roleNum == null) {
			return false;
		}
		return o.HPC.currentHP >= getMaxHp (o.roleNum);
	}

	void shoot(Datas.otherPlayer o) {
		if (o == null || o.key == null || o.roleNum == null || o.name == null)
			return;
		if (o.HPC.currentHP < 0.0f)
			return;
		int r = Random.Range(0, 100);
		string fireMessage = "FireNormal:0";
		if (r > 92) {
			fireMessage = "InsaneFire:0";
		}
		if (r > 98) {
			fireMessage = "UltReady:0";
		}
		// Debug.Log (0);
		string message = o.key.ToString () + "-" + o.roleNum.ToString () + "-\""+o.name+"\"|action;"
			+ fireMessage;
		// Debug.Log (message);
		AIStringList.Enqueue (message);
	}

	void escape(Datas.otherPlayer o) {
		
	}

	// Update is called once per frame
	void Update () {
		if (!Datas.isMultiPlay) {
            for (int i = 0; i < AIList.Count; i++)
            {
                AIObj j = AIList[i];
                Datas.otherPlayer o = Datas.MainHeroInfo.findPlayerByKey(j.Id);
                if (o == null)
                {
                    continue;
                }
                // 遇见敌人开枪
                if (seeEnemy(o))
                {
                    // 血量多在时候大概率开枪
                    if (HPmoreThanHalf(o))
                    {
                        int r = Random.Range(0, 100);
                        if (r > 90)
                        {
                            shoot(o);
                        }
                        else
                        {
                            escape(o);
                        }
                    }
                    // 少时小概率开枪
                    else
                    {
                        int r = Random.Range(0, 100);
                        if (r > 95)
                        {
                            shoot(o);
                        }
                        else
                        {
                            escape(o);
                        }
                    }
                }
                else
                {
                    // 是否移动到下一个巡逻点
                    if (needChangeMovePoint(o))
                    {
                        moveToNextPoint(o);
                    }
                    // 移动当前巡逻点
                    else
                    {
                        moveToPoint(o);
                    }
                }
                if (o.transform != null) { 
                    // Debug.Log(o.transform.rotation.ToString());
                }
			}
		}
	}

	void FixedUpdate() {
		if (AIStringList.Count != 0 && AIBegin) {
			UDPBack.setUDPMessageByResponseStringFormAI (GetAIString());
		}
	}
}
