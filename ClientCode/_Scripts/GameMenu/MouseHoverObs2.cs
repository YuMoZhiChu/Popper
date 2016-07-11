using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MouseHoverObs2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	static public GameObject back;

	public GameObject infoBar;

	public void OnPointerEnter (PointerEventData eventData)
	{
		infoBar.SetActive (true);
		if (back != null)
			back.SetActive (false);
		back = infoBar.transform.GetChild (Datas.roleID).gameObject;
		back.SetActive (true);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		infoBar.SetActive (false);
	}
}
