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
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.totalPageAngle = this.totalPageAngle - eventData.delta.x * 0.005f;
        this.curPageIndex = (int)Mathf.Floor(this.totalPageAngle);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.isFliping = false;
    }

    void Update()
    {
        if(this.pages.Count == 0)
        {
            return;
        }
        this.frontTex = this.pages[this.curPageIndex % this.pages.Count];
        this.backTex = this.pages[(this.curPageIndex + 1) % this.pages.Count];
        
        this.mat.SetFloat("_CurPageAngle", this.totalPageAngle - this.curPageIndex);
        this.mat.SetTexture("_MainTex", this.frontTex);
        this.mat.SetTexture("_BackTex", this.backTex);
    }


	private IEnumerator Gravity()
    {

		while (!this.isFliping) {
			int targetPageIndex = this.totalPageAngle - this.curPageIndex > 0.5f ? this.curPageIndex + 1 : this.curPageIndex;

			this.totalPageAngle = Mathf.Lerp (this.totalPageAngle, targetPageIndex, 0.03f);

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
