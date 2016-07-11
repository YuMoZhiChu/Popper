using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MouseHoverObs : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	static public GameObject back;

	public GameObject infoBar;

	public void OnPointerEnter (PointerEventData eventData)
	{
		infoBar.SetActive (true);
		infoBar.GetComponent<RectTransform> ().position = this.GetComponent<RectTransform> ().position;
		if (back != null)
			back.SetActive (false);
		back = infoBar.transform.GetChild (Datas.roleID).GetChild (int.Parse(this.name)).gameObject;
		back.SetActive (true);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		infoBar.SetActive (false);
	}
}
