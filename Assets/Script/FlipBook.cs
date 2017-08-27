//
// created by jiadong chen
// http://www.chenjd.me
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlipBook : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{

    #region 字段

    public List<Texture> pages = new List<Texture>();

    private int curPageIndex;
    private float curPageAngle;
    private float totalPageAngle;

    private Texture frontTex;
    private Texture backTex;
    private bool isFliping;
    private Material mat;


    #endregion


    #region 方法

    public void Start()
    {
        StartCoroutine(Gravity());
        this.mat = this.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.isFliping = true;
        Debug.Log("true");
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.totalPageAngle = this.totalPageAngle - eventData.delta.x * 0.005f;
        this.curPageIndex = (int)Mathf.Floor(this.totalPageAngle);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.isFliping = false;
        Debug.Log("false");
    }


	private IEnumerator Gravity()
    {
		while (this.isFliping) {
			yield return null;
		}

		while (!this.isFliping) {
			int targetPageIndex = this.totalPageAngle - this.curPageIndex > 0.5f ? this.curPageIndex + 1 : this.curPageIndex;
			this.totalPageAngle = Mathf.Lerp (this.totalPageAngle, targetPageIndex, 0.05f);

			if (Mathf.Abs (this.totalPageAngle - targetPageIndex) < 0.001f)
            {
				this.totalPageAngle = targetPageIndex;
                this.curPageIndex = targetPageIndex;
				break;
			}
			yield return null;
		}
		this.isFliping = true;

		yield return null;

		StartCoroutine (Gravity());
	}


    #endregion

}
