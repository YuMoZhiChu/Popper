using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MouseHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public float scaleFactor = 0.8f;

	public void OnPointerEnter (PointerEventData eventData)
	{
		this.transform.localScale = new Vector3 (scaleFactor, scaleFactor, 1);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		this.transform.localScale = new Vector3 (1, 1, 1);
	}

}
