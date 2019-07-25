using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABIPlugins;

namespace ABIPlugins {
	public class PopupSystemDemo : MonoBehaviour {

		void Awake () {
			Invoke("ShowFistInfoPopup", 1);
			Invoke("ShowConfirmPopup", 3);
			Invoke("ShowInfoPopupOnlyOneInstance", 5);
			Invoke("ShowQuestionPopup", 7);
			Invoke("ShowInfoPopupNewInstance", 9);
			Invoke("ShowExamplePopup", 10);
		}

		void ShowExamplePopup () {
			PopupManager.CreateNewInstance<ExamplePopup>().Show("This is example popup");
		}

		public void ShowFistInfoPopup () {
			PopupManager.CreateNewInstance<InfoPopup>().Show("Info Dialog", "Đây là Info Popup xuất hiện lần đầu tiên", true);
		}

		public void ShowInfoPopupNewInstance () {
			PopupManager.CreateNewInstance<InfoPopup>().Show("Info Dialog", "Đây là Info Popup nhưng tạo thêm một thể hiện mới", true);
		}

		public void ShowInfoPopupOnlyOneInstance () {
			InfoPopup.Instance.Show("Info Dialog", "Đây là Info Popup sử dụng lại", true);
		}

		void ShowConfirmPopup () {
			PopupManager.CreateNewInstance<ConfirmPopup>().Show("Confirm Dialog", "Đây là Confirm dialog", true);
		}

		void ShowQuestionPopup () {
			PopupManager.CreateNewInstance<QuestionPopup>().Show("Question Dialog", "Đây là Question dialog", true);
		}
	}
}