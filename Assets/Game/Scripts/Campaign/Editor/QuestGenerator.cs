using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// This script creates a csv file which is used to import quests from.
/// </summary>
namespace QuestGen {
	public class QuestGenerator : EditorWindow {

		public TextAsset questFile;
		public int numOfQuest;
		Vector2 scrollVector;

		[SerializeField]
		List<RandomValueQuest> questList;

		void Awake () {
			if (EditorPrefs.HasKey("editor_questdata")) {
				QuestGenData questData = JsonUtility.FromJson<QuestGenData>(EditorPrefs.GetString("editor_questdata"));
				questList = questData.questList;
				numOfQuest = questData.numOfQuest;
			} else
				questList = new List<RandomValueQuest>();
		}

		void OnDestroy () {
			EditorPrefs.SetString("editor_questdata", JsonUtility.ToJson(new QuestGenData(numOfQuest, questList)));
		}

		[MenuItem("Window/Quest Generator")]
		public static void ShowWinDow () {
			EditorWindow.GetWindow(typeof(QuestGenerator));
		}

		void OnGUI () {
			questFile = (TextAsset)EditorGUILayout.ObjectField("Quest file", questFile, typeof(TextAsset), false);
			numOfQuest = EditorGUILayout.IntField("Number of quests", numOfQuest);
			GUILayout.Label("Select quest types to generate", EditorStyles.boldLabel);
			// show 2 buttons: add 1 quest type and add all quest types
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add a new quest type")) {
				questList.Add(new RandomValueQuest());
			}
			if (GUILayout.Button("Add all quest types")) {
				questList.Clear();
				for (int i = 0; i < (int)QUEST_TYPE.NONE; i++) {
					RandomValueQuest c = new RandomValueQuest();
					c.type = (QUEST_TYPE)i;
					questList.Add(c);
				}
			}
			if (GUILayout.Button("Clear all")) {
				questList.Clear();
			}
			GUILayout.EndHorizontal();
			// show all quest type in list
			scrollVector = GUILayout.BeginScrollView(scrollVector);
			ScriptableObject target = this;
			SerializedObject so = new SerializedObject(target);
			SerializedProperty stringsProperty = so.FindProperty("questList");
			EditorGUILayout.PropertyField(stringsProperty, true);
			so.ApplyModifiedProperties();
			GUILayout.EndScrollView();
			// "Generate Quest" button
			if (GUILayout.Button("Generate Quest")) {
				File.WriteAllText(Application.dataPath + "/Game/Resources/" + Const.QUEST_DATA + ".csv", GenerateQuest());
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				questFile = Resources.Load(Const.QUEST_DATA) as TextAsset;
			}
		}

		string GenerateQuest () {
			string text = "";
			text += "quest;value;value2;reward\n";
			for (int i = 0; i < questList.Count; i++) {
				questList[i].loop = 0;
			}
			for (int k = 0; k < numOfQuest;) {
				for (int i = 0; i < questList.Count; i++, k++) {
					RandomValueQuest rvq = questList[i];
					text += rvq.type.ToString() + ";" + rvq.value1[Mathf.Clamp(rvq.loop, 0, rvq.value1.Length - 1)] + ";" + rvq.value2 + ";" + rvq.reward + "\n";
					rvq.loop++;
				}
			}
			return text;
		}

		[System.Serializable]
		public class RandomValueQuest {
			public QUEST_TYPE type;
			public int[] value1;
			public int value2;
			public string reward = "100_gold";
			[HideInInspector]public int loop;
		}

		[System.Serializable]
		public class QuestGenData {
			public int numOfQuest;
			public List<RandomValueQuest> questList;

			public QuestGenData (int num, List<RandomValueQuest> l) {
				numOfQuest = num;
				questList = l;
			}
		}
	}
}