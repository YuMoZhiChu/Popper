/**
* ======== All the code changes must be recorded below. ========
* 
* ==============================================
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class MainMenuControl : MonoBehaviour {

	static public bool OnStart = false;
	static public GameObject[] AllRolesArray;

	// Add public variables here:
    public GameObject mainMenu;
    public GameObject paiHangMenu;
    public GameObject optionMenu;
    public GameObject helpMenu;
    public GameObject selectRoleSingleMenu;

    public GameObject Roles;
	public GameObject wutai;
	public GameObject yanhua;

    public GameObject nameInputField;
	public GameObject onstartMask;

	public GameObject[] AllRoles;

	public GameObject ps;

	public Material MainMenuskybox;
	public Material Selectskybox;

	public AudioSource mainAudioSource;
	public AudioClip switchScene;
	public AudioClip buttonAudio;

    public enum MenuType : int
    {
        Main = 0,
        PaiHang = 1,
        Option = 2,
        Help = 3,
        SingleSelect = 4
    }
	
	// Add private members here:
    private GameObject[] Menus = new GameObject[5];
    private Vector2 canvasSize;

	void playAudio(AudioClip ac)
	{
		mainAudioSource.clip = ac;
		mainAudioSource.Play ();
	}

    // Add member functions here:
    #region 单人选人菜单
    public void StartBtn()
    {
		playAudio (buttonAudio);
		if (InputNameClick () == true) {

			onstartMask.SetActive(true);
			OnStart = true;
			foreach (GameObject go in AllRoles)
			{
				go.GetComponent<Animator>().SetBool("inBattle", OnStart);
			}

			StartCoroutine(StartCount());
		} else {
			GameObject.Find("NameInputTips").GetComponent<NameTipsFade>().ShowTips();
		}
    }

	IEnumerator StartCount()
	{
		yield return new WaitForSeconds (5);
		Application.LoadLevel ("JinDuTiao");

	}
    #endregion

    #region 帮助菜单

    #endregion

    #region 设置菜单
    public void MusicVolumeChange(float fpara)
    {
        this.GetComponent<AudioSource>().volume = fpara;
        GameControl.musicStaticVolume = fpara;
    }
    #endregion

    #region 排行榜
    public void BackToMain()
    {
		playAudio (buttonAudio);
        SelectMenu(MenuType.Main);
    }
    #endregion

    #region 主菜单
    public void PaiHangBtnClick()
    {
        SelectMenu(MenuType.PaiHang);
    }

    public void OptionBtnClick()
    {
        SelectMenu(MenuType.Option);
    }

    public void HelpBtnClick()
    {
        SelectMenu(MenuType.Help);
    }

    public void SingleModeBtnClick()
    {
		playAudio (buttonAudio);
        //if (PopNameInput() == false)
        {
			Datas.reset ();
			Datas.MainHeroInfo.list.Clear ();
			Datas.isMultiPlay = false;
            SelectMenu(MenuType.SingleSelect);
        }
    }

	public void MultiModeBtnClick()
	{
		playAudio (buttonAudio);
		//if (PopNameInput() == false)
		{
			Datas.reset ();
			Datas.MainHeroInfo.list.Clear ();
			Datas.isMultiPlay = true;
			SelectMenu(MenuType.SingleSelect);
		}
	}

    public void QuitGame()
    {
		playAudio (buttonAudio);
        Application.Quit();
    }
    #endregion

    #region 名字输入框（只在第一次启动）
    bool PopNameInput()
    {
        if (Datas.MainHeroInfo.HeroName == "TestPlayerOnly")
        {
			nameInputField.SetActive(true);
            return true;
        }
        else
        {
			nameInputField.SetActive(false);
            return false;
        }
    }
    public bool InputNameClick()
    {
		string name = nameInputField.GetComponent<InputField>().text;
        if (name.Length == 0)
        {
            name = "giligili";
        }
        Datas.MainHeroInfo.HeroName = name;
        GameControl.playerName = name;
            //PopNameInput();
            //SelectMenu(MenuType.SingleSelect);
		return true;
    }
    #endregion

    public void SelectMenu(MenuType type, int direction = 1)  // 1表示从右到左滑动
    {
		mainAudioSource.clip = switchScene;
		mainAudioSource.Play ();
		if (type == MenuType.Main)
			//foreach (ParticleSystem p in ps.GetComponentsInChildren<ParticleSystem>()) {
 {			//	p.enableEmission = true;//}
			ps.SetActive (true);
			GetComponent<Skybox> ().material = MainMenuskybox;
		} else {
			//foreach (ParticleSystem p in ps.GetComponentsInChildren<ParticleSystem>()) {
			//	p.enableEmission = false;
			//}
			ps.SetActive (false);
			GetComponent<Skybox> ().material = Selectskybox;
		}
		
        for (int i = 0; i < Menus.Length; i++)
        {
            if (i == (int)type)
            {
				/*
                Menus[i].GetComponent<RectTransform>().position = new Vector3(direction * canvasSize.x * 1.5f, canvasSize.y / 2, 0f);
                Tweener tw = Menus[i].GetComponent<RectTransform>().DOMoveX(canvasSize.x / 2, 0.5f);
                Menus[i].SetActive(true);

                // 出现角色可以拖动
                if (type == MenuType.SingleSelect)
                {
                    tw.OnComplete(() =>
                    {
                        SetRoles(true);
                    });
                }
                else
                {
                    SetRoles(false);
                }
                */
				SetRoles(false);
				Menus[i].SetActive(true);
				foreach (Graphic gc in Menus[i].GetComponentsInChildren<Graphic>()) {
					gc.GetComponent<Graphic> ().CrossFadeAlpha (0, 0.0f, false);
				}
				//Menus [i].GetComponent<Graphic> ().CrossFadeAlpha (0, 0.0f, false);
				//Menus [i].GetComponent<Graphic> ().CrossFadeAlpha (1, 0.5f, false);
				StartCoroutine(FadeIn(0.5f, type));
            }
            else
            {
				/*
                Menus[i].GetComponent<RectTransform>().DOMoveX(-direction * canvasSize.x / 2, 0.5f).OnComplete(() =>
                    {
                        //Menus[i].SetActive(false);
                    });
                    */
				foreach (Graphic gc in Menus[i].GetComponentsInChildren<Graphic>()) {
					gc.GetComponent<Graphic> ().CrossFadeAlpha (0, 0.5f, false);
				}
				StartCoroutine(FadeOut(0.5f, i));
            }
        }
    }

	IEnumerator FadeIn(float time, MenuType menuid)
	{
		yield return new WaitForSeconds (time);
		foreach (Graphic gc in Menus[(int)menuid].GetComponentsInChildren<Graphic>()) {
			gc.GetComponent<Graphic> ().CrossFadeAlpha (1, 0.5f, false);
		}

		if (menuid == MenuType.SingleSelect)
		{
			SetRoles(true);
		}
		else
		{
			SetRoles(false);
		}
	}

	IEnumerator FadeOut(float time, int menuid)
	{
		yield return new WaitForSeconds (time);
		Menus[menuid].SetActive(false);

	}

    public void RegistMenus()
    {
        Menus[0] = mainMenu;
        Menus[1] = paiHangMenu;
        Menus[2] = optionMenu;
        Menus[3] = helpMenu;
        Menus[4] = selectRoleSingleMenu;
    }

    public void SetRoles(bool active)
    {
        Roles.transform.rotation = Quaternion.identity;
        Roles.SetActive(active);
		wutai.SetActive (active);
		yanhua.SetActive (active);
    }

	// Use this for initialization
	void Start () {
        RegistMenus();
        canvasSize = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta;

        //this.GetComponent<AudioSource>().volume = GameControl.musicStaticVolume;
        //optionMenu.transform.FindChild("MusicVolumeSlider").GetComponent<Slider>().value = GameControl.musicStaticVolume;

		onstartMask.SetActive (false);
		AllRolesArray = AllRoles;

		GetComponent<SingleRoleSelect>().SelectRole (0);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
