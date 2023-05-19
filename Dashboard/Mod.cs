using System;
using Modding;
using UnityEngine;
using UnityEngine.UI;
using Besiege.UI;
using Besiege.UI.Serialization;
using UnityEngine.SceneManagement;
using Modding.Common;
using System.Collections.Generic;

namespace Dashboard
{
	public class Mod : ModEntryPoint
	{
		public const string MOD_NAME = "Dashboard";
		public static GameObject ModControllerObject;

		public static Project UIProject = null;

		public override void OnLoad()
		{
			// Called when the mod is loaded.

			// ��ʼ��ͨ�� ModControllerObject
			ModControllerObject = GameObject.Find("ModControllerObject");
			if (!ModControllerObject)
            {
				UnityEngine.Object.DontDestroyOnLoad(ModControllerObject = new GameObject("ModControllerObject"));
            }

			ModControllerObject.AddComponent<DisplayController>();

			// ����OE
			if (Mods.IsModLoaded(new Guid("3c1fa3de-ec74-44e4-807c-9eced79ddd3f")))
            {
				//ModControllerObject.AddComponent<Mapper>();
			}

			// ע��UI
			Make.RegisterSerialisationProvider(MOD_NAME, new SerializationProvider
            {
				CreateText = p => Modding.ModIO.CreateText(p, false),
				ReadAllText = p => Modding.ModIO.ReadAllText(p, false),
				GetFiles = p => Modding.ModIO.GetFiles(p, false)
            });

			// �������ע���Զ�������
			//Make.RegisterSprite(MOD_NAME, "Name", ModResource.GetTexture("texture name"));

			// OnSceneChnanged
			Events.OnActiveSceneChanged += OnSceneChanged;
			if (StatMaster.isMP)
            {
				OnSceneChanged(SceneManager.GetActiveScene(), SceneManager.GetActiveScene());
            }
		}

		static void OnSceneChanged(Scene a, Scene b)
        {
			if (SceneNotPlayable())
            {
				return;
            }

			// �����������ʱ��
			var stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();

			GameObject dashboard = Make.LoadProject(MOD_NAME, "dashboard").gameObject;
			dashboard.name = "dashboard";

			UIProject = dashboard.GetComponent<Project>();
			UIProject.RebuildTransformList();

			// text
			UIProject["SpeedText"].GetComponent<Text>().text = "Speed: 0";

			// ����ť��Ӽ�����
			UIProject["Button1"].GetComponent<Button>().onClick.AddListener(DashboardListener.OnButtonClick);

			stopwatch.Stop();
			Debug.Log($"[Dashboard] Loaded in {stopwatch.ElapsedMilliseconds}ms");
		}

		public static bool SceneNotPlayable()
		{
			return !AdvancedBlockEditor.Instance;
		}

		/// <see href="https://stackoverflow.com/a/34006336" />
		public static int CustomHash(params int[] vals)
		{
			int hash = 1009;
			foreach (int i in vals)
			{
				hash = (hash * 9176) + i;
			}
			return hash;
		}

		public static GameObject CreateObject(string name, GameObject parent, params Type[] components)
		{
			GameObject obj = new GameObject(name, components);
			obj.transform.SetParent(parent.transform, false);
			return obj;
		}

		public static Transform CreateObject(string name, Transform parent, params Type[] components)
		{
			GameObject obj = new GameObject(name, components);
			obj.transform.SetParent(parent, false);
			return obj.transform;
		}
	}
}
