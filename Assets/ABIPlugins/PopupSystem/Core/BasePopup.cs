using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ABIPlugins {
	[RequireComponent(typeof(Animator))]
	public class BasePopup : MonoBehaviour {
		/// <summary>
		/// Optional Animator show popup controller
		/// </summary>
		[HideInInspector]
		public Animator animator;

		/// <summary>
		/// Optional animation show
		/// </summary>
		public AnimationClip showAnimationClip;

		/// <summary>
		/// Optional animation hide
		/// </summary>
		public AnimationClip hideAnimationClip;

		protected bool isShowed;
		private int mSortOrder;
		private Transform mTransform;
		private bool overlay;
		private Stack<BasePopup> refStacks;
		private Action hideAnimationFinishCallback;
		private Action showAnimationFinishCallback;

		public virtual void Awake () {
			isShowed = false;
			animator = GetComponent<Animator>();
			mTransform = transform;
			mSortOrder = mTransform.GetSiblingIndex();
			refStacks = PopupManager.Instance.popupStacks;
			if (animator == null || showAnimationClip == null || hideAnimationClip == null) {
				BPDebug.LogMessage("Chưa gán Animator hoặc showAnimationClip, hideAnimationClip  cho popup " + GetType().ToString(), true);
			}
		}

		protected void Show (bool overlay = true, Action showAnimationFinishCallback = null) {

			if (isShowed) { //Trường hợp chỉ tạo popup duy nhất.
				Reshow();
				int topSortOrder = refStacks.Peek().SortOrder();
				if (refStacks.Count > 1 && SortOrder() != topSortOrder) { //Nếu đã bị các popup khác đè lên

					MoveElementToTopStack(ref refStacks, SortOrder()); // Đẩy popup này lên trên cùng và sắp xếp lại sortOrder cho toàn stack
					// ChangeSortOrder(topSortOrder + 1);
				}
				return;
			} else {
				this.showAnimationFinishCallback = showAnimationFinishCallback;
			}

			float waitLastPopupHide = 0;
			this.overlay = overlay;
			isShowed = true;

			if (!overlay && refStacks.Count > 0)
				ForceHideAllCurrent(ref waitLastPopupHide);

			if (!refStacks.Contains(this))
				refStacks.Push(this);

			if (refStacks.Count > 0)
				ChangeSortOrder(refStacks.Peek().SortOrder() + 1);

			if (waitLastPopupHide != 0)
				Invoke("AnimateShow", waitLastPopupHide);
			else
				AnimateShow();
		}

		public void Reshow () {
			if (animator != null && showAnimationClip != null) {
				animator.Play(showAnimationClip.name, -1, 0.0f);
				float showAnimationDuration = GetAnimationClipDuration(showAnimationClip);
				Invoke("OnShowFinish", showAnimationDuration);
			}

			PopupManager.Instance.ChangeTransparentOrder(mTransform, true);
		}

		public virtual void Hide () {
			if (!isShowed)
				return;

			isShowed = false;

			AnimateHide();
		}

		public void Hide (Action animationFinishCallback) {
			this.hideAnimationFinishCallback = animationFinishCallback;
			if (!isShowed)
				return;

			isShowed = false;

			AnimateHide();
		}

		public void OnCloseClick () {
			Hide();
		}

		private void AnimateShow () {
			if (animator != null && showAnimationClip != null) {
				float showAnimationDuration = GetAnimationClipDuration(showAnimationClip);
				Invoke("OnShowFinish", showAnimationDuration);
				animator.Play(showAnimationClip.name);
			}

			PopupManager.Instance.ChangeTransparentOrder(mTransform, true);
		}

		void OnShowFinish () {
			if (showAnimationFinishCallback != null) {
				showAnimationFinishCallback();
			}
		}

		private void AnimateHide () {
			PopupManager.Instance.ChangeTransparentOrder(mTransform, false);
			if (animator != null && hideAnimationClip != null) {
				animator.Play(hideAnimationClip.name);
				float hideAnimationDuration = GetAnimationClipDuration(hideAnimationClip);
				Invoke("Destroy", hideAnimationDuration);
			} else {
				Destroy();
			}
//			PopupManager.Instance.ChangeTransparentOrder(mTransform, false);
		}

		void Destroy () {

			if (refStacks.Contains(this))
				refStacks.Pop();

			if (gameObject.activeSelf)
				DestroyImmediate(gameObject);

			if (hideAnimationFinishCallback != null)
				hideAnimationFinishCallback();
		}

		public bool IsShowed {
			get { return isShowed; }
		}

		public int SortOrder () {
			return mSortOrder;
		}

		public void ChangeSortOrder (int newSortOrder = -1) {
			if (newSortOrder != -1) {
				mTransform.SetSiblingIndex(newSortOrder);
				mSortOrder = newSortOrder;
			}
		}

		private void ForceHideAllCurrent (ref float waitTime) {
			while (refStacks.Count > 0) {
				BasePopup bp = refStacks.Pop();
				waitTime += bp.GetAnimationClipDuration(hideAnimationClip);
				bp.Hide();
			}
		}

		private float GetAnimationClipDuration (AnimationClip clip) {
			if (animator != null && clip != null) {
				RuntimeAnimatorController rac = animator.runtimeAnimatorController;
				for (int i = 0; i < rac.animationClips.Length; i++) {
					if (rac.animationClips[i].Equals(clip))
						return rac.animationClips[i].length;
				}
			}

			return 0;
		}

		private void MoveElementToTopStack (ref Stack<BasePopup> stack, int order) {
			Stack<BasePopup> tempStack = new Stack<BasePopup>();
			BasePopup foundPopup = null;
			int minSortOrder = 0;
			while (refStacks.Count > 0) {
				BasePopup bp = refStacks.Pop();
				if (bp.SortOrder() != order) {
					tempStack.Push(bp);
					minSortOrder = bp.SortOrder();
				} else {
					foundPopup = bp;
				}
			}

			while (tempStack.Count > 0) {
				BasePopup bp = tempStack.Pop();
				bp.ChangeSortOrder(minSortOrder++);
				stack.Push(bp);
			}

			if (foundPopup != null) {
				foundPopup.ChangeSortOrder(minSortOrder);
				stack.Push(foundPopup);
			}
		}
	}
}