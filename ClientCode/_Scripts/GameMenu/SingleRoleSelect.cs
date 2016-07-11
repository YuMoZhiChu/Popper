/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SingleRoleSelect : MonoBehaviour {

	// Add public variables here:
    public GameObject circle1;
    public GameObject circle2;
    public GameObject circle3;
    public GameObject selected;

	public GameObject[] texiao;
	public GameObject[] zhuangbei;
	public GameObject[] zuoshangjiao;

	public Text name;
	
	// Add private members here:
	private int num = 0;
	
	// Add member functions here:
	public void SelectRole(int id)
	{
		foreach (GameObject go in MainMenuControl.AllRolesArray) {
			go.SetActive(false);
		}
		MainMenuControl.AllRolesArray [id].SetActive (true);

		foreach (GameObject go in zhuangbei) {
			go.SetActive(false);
		}
		zhuangbei[id].SetActive (true);

		foreach (GameObject go in zuoshangjiao) {
			go.SetActive(false);
		}
		zuoshangjiao[id].SetActive (true);

		Datas.roleID = id;

		if (num != id) {
			num = id;
			Instantiate (texiao [num]);
		}
	}

    public void OnClick1()
    {
        selected.GetComponent<RectTransform>().position = circle1.GetComponent<RectTransform>().position;
		SelectRole (0);
		name.text = "Don";
    }
    public void OnClick2()
    {
        selected.GetComponent<RectTransform>().position = circle2.GetComponent<RectTransform>().position;
		SelectRole (1);
		name.text = "Hunk";
    }
    public void OnClick3()
    {
        selected.GetComponent<RectTransform>().position = circle3.GetComponent<RectTransform>().position;
		SelectRole (2);
		name.text = "Betty";
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
