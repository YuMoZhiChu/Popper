using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameFinishUIControl : MonoBehaviour {

	static public int NO1Score = 0;
	static public int NO2Score = 0;
	static public int NO3Score = 0;

	public void Init()
	{
		if (Datas.isMultiPlay == false) {
			NO1Score = Datas.MainHeroInfo.score;
			NO2Score = (int)(NO1Score / 2.0f);
			NO3Score = (int)(NO2Score / 2.0f);
		} else {
			NO1Score = Datas.MainHeroInfo.list [0].score;
			if (Datas.MainHeroInfo.list.Count >= 2) NO2Score = Datas.MainHeroInfo.list [1].score;
			if (Datas.MainHeroInfo.list.Count >= 3) NO3Score = Datas.MainHeroInfo.list [2].score;
		}

		this.transform.GetChild (0).GetChild (2).GetComponent<Text> ().text = NO1Score.ToString ();
		this.transform.GetChild (1).GetChild (2).GetComponent<Text> ().text = NO2Score.ToString ();
		this.transform.GetChild (2).GetChild (2).GetComponent<Text> ().text = NO3Score.ToString ();


		//this.transform.GetChild (1).GetChild (1).GetComponent<Image> ().fillAmount = (NO2Score + 0.0f) / NO1Score;
		//this.transform.GetChild (1).GetChild (1).GetChild(0).GetComponent<RectTransform>().anchoredPosition
		//= this.transform.GetChild (1).GetChild (1).GetComponent<RectTransform> ().localPosition.x * 
		//this.transform.GetChild (2).GetChild (1).GetComponent<Image> ().fillAmount = (NO3Score + 0.0f) / NO1Score;

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
