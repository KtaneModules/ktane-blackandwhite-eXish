using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BWService : MonoBehaviour
{
	public static void ActivateNeedy(object NeedyComponent)
	{
		if (_activateNeedy != null)
		{
			_activateNeedy(NeedyComponent);
		}
		else
		{
			Debug.LogFormat("[Black and White] No expression found, using Reflection instead.", new object[0]);
			NeedyComponent.GetType().MethodCall("ResetAndStart", NeedyComponent, new object[0]);
		}
	}

	private void Start()
	{
		transform.localPosition = Vector3.zero;
		_whitePrefab = Instantiate(_fullWhitePrefab.gameObject, transform.parent).GetComponent(ReflectionHelper.FindTypeInGame("BombComponent"));
		_blackPrefab = Instantiate(_fullBlackPrefab.gameObject, transform.parent).GetComponent(ReflectionHelper.FindTypeInGame("BombComponent"));
		Type type = ReflectionHelper.FindTypeInGame("ModSource");
		type.SetField("ModName", _whitePrefab.gameObject.AddComponent(type), "blackWhiteModule");
		type.SetField("ModName", _blackPrefab.gameObject.AddComponent(type), "blackWhiteModule");
		Type type2 = ReflectionHelper.FindTypeInGame("ModNeedyComponent");
		FieldInfo field = type2.GetField("module", ReflectionHelper.Flags);
		field.SetValue(_blackPrefab.GetComponent(type2), _blackPrefab.GetComponent<KMNeedyModule>());
		field.SetValue(_whitePrefab.GetComponent(type2), _whitePrefab.GetComponent<KMNeedyModule>());
		if (_whitePrefab == null)
		{
			Debug.LogFormat("[Black and White] White not found!", new object[0]);
		}
		if (_blackPrefab == null)
		{
			Debug.LogFormat("[Black and White] Black not found!", new object[0]);
		}
		if (_whitePrefab == null || _blackPrefab == null)
		{
			throw new Exception("A module was not found.");
		}
		SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(PatchAll);
	}

	private void CreateStartNeedy()
	{
		Type type = ReflectionHelper.FindTypeInGame("NeedyComponent");
		ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "module");
		UnaryExpression instance = Expression.Convert(parameterExpression, type);
		MethodCallExpression body = Expression.Call(instance, type.Method("ResetAndStart"));
		Expression<Action<object>> expression = Expression.Lambda<Action<object>>(body, new ParameterExpression[]
		{
			parameterExpression
		});
		_activateNeedy = expression.Compile();
	}

	private void PatchAll(Scene _, LoadSceneMode __)
	{
		if (_hasPatched)
		{
			return;
		}
		Debug.Log("[Black and White] Beginning hook process.");
		CreateStartNeedy();
		Debug.Log("[Black and White] Start Needy okay");
		Harmony harmony = new Harmony("BlackAndWhiteKTANE");
		Type type = ReflectionHelper.FindTypeInGame("BombGenerator");
		MethodBase methodBase = type.Method("SelectWeightedRandomComponentType");
		HarmonyMethod harmonyMethod = new HarmonyMethod(GetType().Method("SelectRandomPostfix"));
		Harmony harmony2 = harmony;
		MethodBase methodBase2 = methodBase;
		HarmonyMethod harmonyMethod2 = harmonyMethod;
		harmony2.Patch(methodBase2, null, harmonyMethod2, null, null);
		Debug.Log("[Black and White] Select Random okay");
		methodBase = type.Method("CreateBomb");
		harmonyMethod = new HarmonyMethod(GetType().Method("CreateBombTranspiler"));
		Harmony harmony3 = harmony;
		methodBase2 = methodBase;
		harmonyMethod2 = harmonyMethod;
		harmony3.Patch(methodBase2, null, null, harmonyMethod2, null);
		Debug.Log("[Black and White] Create Bomb okay");
		methodBase = type.Method("InstantiateComponent");
		harmonyMethod = new HarmonyMethod(GetType().Method("InstantiateComponentTranspiler"));
		Harmony harmony4 = harmony;
		methodBase2 = methodBase;
		harmonyMethod2 = harmonyMethod;
		harmony4.Patch(methodBase2, null, null, harmonyMethod2, null);
		Debug.Log("[Black and White] Instantiate Component okay");
		methodBase = type.Method("GetBombPrefab");
		harmonyMethod = new HarmonyMethod(GetType().Method("CountModsPrefix"));
		HarmonyMethod harmonyMethod3 = new HarmonyMethod(GetType().Method("CountModsPostfix"));
		harmony.Patch(methodBase, harmonyMethod, harmonyMethod3, null, null);
		Debug.Log("[Black and White] Count Mods okay");
		type = ReflectionHelper.FindType("BetterCasePicker", "TweaksAssembly");
		if (type != null)
		{
			type = type.GetNestedType("<>c", ReflectionHelper.Flags);
			if (type != null)
			{
				methodBase = type.Method("<HandleGeneratorSetting>b__5_1");
				harmonyMethod = new HarmonyMethod(GetType().Method("BCPCountPostfix"));
				Harmony harmony5 = harmony;
				methodBase2 = methodBase;
				harmonyMethod2 = harmonyMethod;
				harmony5.Patch(methodBase2, null, harmonyMethod2, null, null);
				Debug.Log("[Black and White] Better Case Picker okay");
			}
			else
			{
				Debug.LogFormat("[Black and White] Better Case Picker is enabled, but modification failed!", new object[0]);
			}
		}
		else
		{
			Debug.Log("[Black and White] Better Case Picker skipped");
		}
		type = ReflectionHelper.FindTypeInGame("NeedyComponent");
		methodBase = type.Method("ResetAndStart");
		harmonyMethod = new HarmonyMethod(GetType().Method("NeedyActivateTranspiler"));
		Harmony harmony6 = harmony;
		methodBase2 = methodBase;
		harmonyMethod2 = harmonyMethod;
		harmony6.Patch(methodBase2, null, null, harmonyMethod2, null);
		Debug.Log("[Black and White] Needy Activate okay");
		type = ReflectionHelper.FindType("Repository");
		if (type != null)
		{
			type = type.GetNestedType("<LoadData>d__3", ReflectionHelper.Flags);
			if (type != null)
			{
				methodBase = type.Method("MoveNext");
				harmonyMethod = new HarmonyMethod(GetType().Method("DBMLNoFakeTranspiler"));
				Harmony harmony7 = harmony;
				methodBase2 = methodBase;
				harmonyMethod2 = harmonyMethod;
				harmony7.Patch(methodBase2, null, null, harmonyMethod2, null);
				Debug.Log("[Black and White] DBML Nofake okay");
			}
			else
			{
				Debug.Log("[Black and White] DBML is enabled, but modifications failed!");
			}
		}
		else
		{
			Debug.Log("[Black and White] DBML Nofake not okay");
		}
		type = ReflectionHelper.FindType("Repository");
		if (type != null)
		{
			StartCoroutine(TweaksRepoFix());
			Debug.Log("[Black and White] Mod Available started");
		}
		else
		{
			Debug.Log("[Black and White] Mod Available not started");
		}
		type = ReflectionHelper.FindTypeInGame("ModManager");
		methodBase = type.Method("GetSolvableBombModules");
		harmonyMethod = new HarmonyMethod(GetType().Method("GetSolvablesPrefix"));
		harmony.Patch(methodBase, harmonyMethod, null, null, null);
		Debug.Log("[Black and White] Mod Manger Fix okay");
		type = ReflectionHelper.FindType("DemandBasedLoading");
		if (type != null)
		{
			type = type.GetNestedType("<InstantiateComponents>d__25", ReflectionHelper.Flags);
			if (type == null)
			{
				Debug.Log("[Black and White] DBML Generator not okay");
				throw new Exception();
			}
			methodBase = type.Method("MoveNext");
			harmonyMethod = new HarmonyMethod(GetType().Method("DBMLGenTranspiler"));
			Harmony harmony8 = harmony;
			methodBase2 = methodBase;
			harmonyMethod2 = harmonyMethod;
			harmony8.Patch(methodBase2, null, null, harmonyMethod2, null);
			Debug.Log("[Black and White] DBML Generator okay");
		}
		Debug.Log("[Black and White] Registering API hook...");
		ModdedAPI.AddProperty("LoadOnCondition", null, new Action<object>(SetLoadOnCondition));
		Debug.Log("[Black and White] End hook process.");
		_hasPatched = true;
	}

	private void SetLoadOnCondition(object d)
	{
		if (!(d is IDictionary))
		{
			throw new Exception("SetLoadOnCondition expected an IDictionary, got " + d.GetType().Name);
		}
		IDictionary dictionary = (IDictionary)d;
		object obj = dictionary["Prefab"];
		Func<object, int> condition = (Func<object, int>)dictionary["Condition"];
		KMBombModule componentInChildren = ((GameObject)obj).GetComponentInChildren<KMBombModule>();
		Component component;
		string moduleType;
		if (componentInChildren == null)
		{
			KMNeedyModule componentInChildren2 = ((GameObject)obj).GetComponentInChildren<KMNeedyModule>();
			if (componentInChildren2 == null)
			{
				throw new Exception("SetLoadOnCondition That's not a module.");
			}
			component = Instantiate(componentInChildren2.gameObject, transform.parent).GetComponent(ReflectionHelper.FindTypeInGame("BombComponent"));
			Type type = ReflectionHelper.FindTypeInGame("ModNeedyComponent");
			FieldInfo field = type.GetField("module", ReflectionHelper.Flags);
			field.SetValue(component, component.GetComponent<KMBombModule>());
			moduleType = componentInChildren2.ModuleType;
		}
		else
		{
			component = Instantiate(componentInChildren.gameObject, transform.parent).GetComponent(ReflectionHelper.FindTypeInGame("BombComponent"));
			Type type2 = ReflectionHelper.FindTypeInGame("ModBombComponent");
			FieldInfo field2 = type2.GetField("module", ReflectionHelper.Flags);
			field2.SetValue(component, component.GetComponent<KMBombModule>());
			moduleType = componentInChildren.ModuleType;
		}
		AddOnConditionData item = new AddOnConditionData
		{
			Id = moduleType,
			Condition = condition,
			Prefab = component
		};
		_propertyAdded.Add(item);
	}

	private IEnumerator TweaksRepoFix()
	{
		Type t = ReflectionHelper.FindType("Repository");
		Type t2 = t.GetNestedType("KtaneModule", ReflectionHelper.Flags);
		Type t3 = typeof(List<>).MakeGenericType(new Type[]
		{
			t2
		});
		FieldInfo fi = t2.GetField("SteamID", ReflectionHelper.Flags);
		FieldInfo fi2 = t2.GetField("ModuleID", ReflectionHelper.Flags);
		FieldInfo mfi = t.GetField("Modules", ReflectionHelper.Flags);
		FieldInfo mfl = t.GetField("Loaded", ReflectionHelper.Flags);
		yield return new WaitUntil(() => (bool)mfl.GetValue(null));
		yield return null;
		IList i = (IList)mfi.GetValue(null);
		Debug.Log("[Black and White] Tweaks Repo loaded");
		ClearRepository();
		t = ReflectionHelper.FindTypeInGame("ModManager");
		MonoBehaviour inst = t.Field<MonoBehaviour>("Instance", null);
		if (!(inst != null))
		{
			Debug.Log("[Black and White] Failed to make modules available. Code 1");
			throw new Exception();
		}
		IDictionary dictionary = t.Field<IDictionary>("loadedBombComponents", inst);
		if (dictionary == null)
		{
			Debug.Log("[Black and White] Failed to make modules available. Code 2");
			throw new Exception();
		}
		dictionary.Remove("whiteModule");
		dictionary["blackModule"] = _blackPrefab;
		t.SetField("loadedBombComponents", inst, dictionary);
		Debug.Log("[Black and White] Mod Available okay");
		t = ReflectionHelper.FindType("DemandBasedLoading");
		if (t != null)
		{
			List<string> list = t.Field<List<string>>("fakedModules", null);
			list.RemoveAll((string s) => s.EqualsAny(new object[]
			{
				"whiteModule",
				"blackModule"
			}));
			Debug.Log("[Black and White] Fake Modules Initial Remove okay");
		}
		t = ReflectionHelper.FindType("BombGenerator");
		object instance = FindObjectOfType(t);
		if (instance != null)
		{
			Debug.Log("[Black and White] Fixing BombGenerator...");
			IList list2 = t.Field<IList>("componentPrefabs", instance);
			if (list2 != null)
			{
				Type type = ReflectionHelper.FindTypeInGame("ModNeedyComponent");
				MethodInfo methodInfo = type.Method("GetModComponentType");
				for (int j = list2.Count - 1; j >= 0; j--)
				{
					if (type.IsAssignableFrom(list2[j].GetType()) && ((string)methodInfo.Invoke(list2[j], new object[0])).EqualsAny(new object[]
					{
						"whiteModule",
						"blackModule"
					}))
					{
						list2.RemoveAt(j);
					}
				}
				Debug.Log("[Black and White] ComponentPrefabs okay");
			}
			IDictionary dictionary2 = t.Field<IDictionary>("componentPrefabDictionary", instance);
			if (dictionary2 != null)
			{
				dictionary2.Remove("whiteModule");
				dictionary2.Remove("blackModule");
				Debug.Log("[Black and White] ComponentPrefabDictionary okay");
			}
		}
		else
		{
			Debug.Log("[Black and White] No BombGenerator instances found");
		}
		t = ReflectionHelper.FindType("ModSelectorService");
		if (t != null)
		{
			Debug.Log("[Black and White] Mod selector located, updating...");
			object value = t.GetField("_instance", ReflectionHelper.Flags).GetValue(null);
			Type nestedType = t.GetNestedType("NeedyModule", ReflectionHelper.Flags);
			object value2 = Activator.CreateInstance(nestedType, new object[]
			{
				_blackPrefab.GetComponent<KMNeedyModule>(),
				_blackPrefab
			});
			IDictionary dictionary3 = t.Field<IDictionary>("_allNeedyModules", value);
			dictionary3.Add("blackModule", value2);
		}
		t = ReflectionHelper.FindType("DynamicMissionGeneratorAssembly.DynamicMissionGenerator");
		if (t != null)
		{
			object obj = t.Field<object>("Instance", null);
			if (obj != null)
			{
				object obj2 = t.Field<object>("InputPage", obj);
				if (obj2 != null)
				{
					Debug.Log("[Black and White] Dynamic mission generator located, updating...");
					obj2.GetType().MethodCall("InitModules", obj2, new object[0]);
				}
			}
		}
		yield break;
	}

	private static void CountModsPrefix(object settings)
	{
		IList list = settings.GetType().Field<IList>("ComponentPools", settings);
		IEnumerator enumerator = list.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				if (obj.GetType().Field<List<string>>("ModTypes", obj).Contains("blackModule") || obj.GetType().Field<int>("SpecialComponentType", obj) == 2)
				{
					obj.GetType().SetField("Count", obj, obj.GetType().Field<int>("Count", obj) * 2);
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	private static void CountModsPostfix(object settings)
	{
		IList list = settings.GetType().Field<IList>("ComponentPools", settings);
		IEnumerator enumerator = list.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				if (obj.GetType().Field<List<string>>("ModTypes", obj).Contains("blackModule") || obj.GetType().Field<int>("SpecialComponentType", obj) == 2)
				{
					obj.GetType().SetField("Count", obj, obj.GetType().Field<int>("Count", obj) / 2);
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	private static IEnumerable<CodeInstruction> DBMLNoFakeTranspiler(IEnumerable<CodeInstruction> instr)
	{
		List<CodeInstruction> instructions = instr.ToList();
		int i = 0;
		Type t = ReflectionHelper.FindType("Repository");
		FieldInfo f = t.GetField("Loaded", ReflectionHelper.Flags);
		while (i < instructions.Count)
		{
			if (CodeInstructionExtensions.StoresField(instructions[i], f))
			{
				yield return CodeInstruction.Call(typeof(BWService), "ClearRepository", null, null);
			}
			yield return instructions[i];
			i++;
		}
		yield break;
	}

	private static IEnumerable<CodeInstruction> DBMLGenTranspiler(IEnumerable<CodeInstruction> instr, MethodBase orig)
	{
		List<CodeInstruction> instructions = instr.ToList();
		int i = 0;
		bool flag = false;
		while (i < instructions.Count)
		{
			if (!flag && instructions[i].opcode == OpCodes.Ldnull && CodeInstructionExtensions.IsStloc(instructions[i + 1], null))
			{
				yield return instructions[i + 2];
				yield return new CodeInstruction(OpCodes.Ldloc_S, 6);
				yield return new CodeInstruction(OpCodes.Ldloc_3, 6);
				yield return new CodeInstruction(OpCodes.Ldloc_S, 5);
				yield return CodeInstruction.Call(typeof(BWService), "ChooseFaceDBML", null, null);
				flag = true;
				i++;
			}
			yield return instructions[i];
			i++;
		}
		yield break;
	}

	private static object ChooseFaceDBML(MonoBehaviour comp, object frontFace, IList bombFaceList, object bombInfo)
	{
		KMNeedyModule component = comp.GetComponent<KMNeedyModule>();
		if (!component || !(component.ModuleType == "whiteModule"))
		{
			return null;
		}
		List<object> list = bombFaceList.Cast<object>().ToList<object>();
		if (list.Count == 0)
		{
			Debug.LogFormat("[Black and White] There's no room to spawn White.", new object[0]);
			return null;
		}
		if (list.Contains(frontFace))
		{
			list.Remove(frontFace);
		}
		if (list.Count == 0)
		{
			Debug.LogFormat("[Black and White] There's no room on the back face. Using the front instead.", new object[0]);
			return frontFace;
		}
		return list[bombInfo.GetType().Field<System.Random>("Rand", bombInfo).Next(0, list.Count)];
	}

	private static void ClearRepository()
	{
		Type type = ReflectionHelper.FindType("Repository");
		IList list = type.Field<IList>("Modules", null);
		type = type.GetNestedType("KtaneModule", ReflectionHelper.Flags);
		FieldInfo field = type.GetField("ModuleID", ReflectionHelper.Flags);
		List<string> list2 = new List<string>();
		for (int i = list.Count - 1; i >= 0; i--)
		{
			string text = (string)field.GetValue(list[i]);
			if (text.EqualsAny(new object[]
			{
				"whiteModule",
				"blackModule"
			}))
			{
				list.RemoveAt(i);
				list2.Add(text);
			}
		}
		if (list2.Count > 0)
		{
			Debug.LogFormat("[Black and White] IDs removed from Tweaks: {0}", new object[]
			{
				list2.Join(", ")
			});
		}
		else
		{
			Debug.LogFormat("[Black and White] No IDs removed from Tweaks.", new object[0]);
		}
	}

	private static IEnumerable<CodeInstruction> CreateBombTranspiler(IEnumerable<CodeInstruction> instr)
	{
		List<CodeInstruction> instructions = instr.ToList();
		int i;
		for (i = 0; i < instructions.Count; i++)
		{
			if (CodeInstructionExtensions.Is(instructions[i], OpCodes.Ldstr, "Instantiating RequiresTimerVisibility components on {0}"))
			{
				CodeInstruction ld = new CodeInstruction(instructions[i])
				{
					opcode = OpCodes.Ldarg_0,
					operand = null
				};
				CodeInstruction ld2 = new CodeInstruction(instructions[i])
				{
					opcode = OpCodes.Ldarg_1,
					operand = null
				};
				CodeInstruction ld3 = new CodeInstruction(instructions[i])
				{
					opcode = OpCodes.Ldloc_S,
					operand = 8
				};
				yield return ld;
				yield return ld2;
				yield return ld3;
				yield return CodeInstruction.Call(typeof(BWService), "LoadOnCondition", new Type[]
				{
					typeof(object),
					typeof(object),
					typeof(object)
				}, new Type[0]);
				break;
			}
			yield return instructions[i];
		}
		while (i < instructions.Count)
		{
			if (CodeInstructionExtensions.Is(instructions[i], OpCodes.Ldstr, "Instantiating remaining components on any valid face."))
			{
				CodeInstruction ld4 = new CodeInstruction(instructions[i])
				{
					opcode = OpCodes.Ldarg_0,
					operand = null
				};
				CodeInstruction ld5 = new CodeInstruction(instructions[i])
				{
					opcode = OpCodes.Ldarg_1,
					operand = null
				};
				CodeInstruction ld6 = new CodeInstruction(instructions[i])
				{
					opcode = OpCodes.Ldloc_S,
					operand = 8
				};
				yield return ld4;
				yield return ld5;
				yield return ld6;
				yield return CodeInstruction.Call(typeof(BWService), "SpawnWhites", new Type[]
				{
					typeof(object),
					typeof(object),
					typeof(object)
				}, new Type[0]);
				break;
			}
			yield return instructions[i];
			i++;
		}
		while (i < instructions.Count)
		{
			yield return instructions[i];
			i++;
		}
		yield break;
	}

	private static IEnumerable<CodeInstruction> InstantiateComponentTranspiler(IEnumerable<CodeInstruction> instr)
	{
		List<CodeInstruction> instructions = instr.ToList();
		int i = 0;
		MethodInfo mi = typeof(UnityEngine.Object).GetMethods(ReflectionHelper.Flags).First(delegate(MethodInfo m)
		{
			bool result;
			if (m.IsGenericMethodDefinition)
			{
				result = (from pi in m.GetParameters().Skip(1)
				select pi.ParameterType).SequenceEqual(new Type[]
				{
					typeof(Vector3),
					typeof(Quaternion)
				});
			}
			else
			{
				result = false;
			}
			return result;
		}).MakeGenericMethod(new Type[]
		{
			typeof(GameObject)
		});
		while (i < instructions.Count)
		{
			if (CodeInstructionExtensions.Calls(instructions[i], mi))
			{
				yield return instructions[i];
				yield return instructions[i + 1];
				yield return instructions[i + 2];
				yield return CodeInstruction.Call(typeof(BWService), "ActiveTrue", new Type[]
				{
					typeof(GameObject)
				}, new Type[0]);
				i += 2;
				break;
			}
			yield return instructions[i];
			i++;
		}
		while (i < instructions.Count)
		{
			yield return instructions[i];
			i++;
		}
		yield break;
	}

	private static IEnumerable<CodeInstruction> NeedyActivateTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator generator)
	{
		List<CodeInstruction> instructions = instr.ToList();
		int i;
		for (i = 0; i < instructions.Count; i++)
		{
			if (CodeInstructionExtensions.Is(instructions[i], OpCodes.Ldstr, "needy_activated"))
			{
				yield return CodeInstructionExtensions.MoveLabelsFrom(new CodeInstruction(instructions[i + 1]), instructions[i]);
				yield return CodeInstruction.Call(typeof(BWService), "CheckNeedySound", null, null);
				Label lbl = generator.DefineLabel();
				yield return new CodeInstruction(OpCodes.Brfalse, lbl);
				while (i < instructions.Count)
				{
					yield return instructions[i];
					if (instructions[i].opcode == OpCodes.Pop)
					{
						break;
					}
					i++;
				}
				yield return CodeInstructionExtensions.WithLabels(instructions[i + 1], new Label[]
				{
					lbl
				});
				i += 2;
				break;
			}
			yield return instructions[i];
		}
		while (i < instructions.Count)
		{
			yield return instructions[i];
			i++;
		}
		yield break;
	}

	private static bool CheckNeedySound(object inst)
	{
		KMNeedyModule component = ((MonoBehaviour)inst).GetComponent<KMNeedyModule>();
		return !component || !component.ModuleDisplayName.EqualsAny(new object[]
		{
			"Black",
			"White"
		});
	}

	private static void ActiveTrue(GameObject o)
	{
		o.SetActive(true);
	}

	private static void SpawnWhites(object instance, object settings, object frontFace)
	{
		if (_blackCount == 0)
		{
			return;
		}
		Debug.LogFormat("[Black and White] Instantiating Whites on the back face.", new object[0]);
		Type type = instance.GetType();
		for (int i = 0; i < _blackCount; i++)
		{
			IList source = type.Field<IList>("validBombFaces", instance);
			List<object> list = source.Cast<object>().ToList<object>();
			if (list.Count == 0)
			{
				Debug.LogFormat("[Black and White] There's no room to spawn White.", new object[0]);
			}
			else
			{
				if (list.Contains(frontFace))
				{
					list.Remove(frontFace);
				}
				if (list.Count == 0)
				{
					Debug.LogFormat("[Black and White] There's no room on the back face. Using the front instead.", new object[0]);
					type.MethodCall("InstantiateComponent", instance, new object[]
					{
						frontFace,
						_whitePrefab,
						settings
					});
				}
				else
				{
					type.MethodCall("InstantiateComponent", instance, new object[]
					{
						list[type.Field<System.Random>("rand", instance).Next(0, list.Count)],
						_whitePrefab,
						settings
					});
				}
			}
		}
		_blackCount = 0;
	}

	private static void LoadOnCondition(object instance, object settings, object frontFace)
	{
		Type type = instance.GetType();
		Debug.LogFormat("[Black and White] Spawning per condition...", new object[0]);
		foreach (AddOnConditionData addOnConditionData in _propertyAdded)
		{
			int num = addOnConditionData.Condition(settings);
			for (int i = 0; i < num; i++)
			{
				IList source = type.Field<IList>("validBombFaces", instance);
				List<object> list = source.Cast<object>().ToList();
				if (list.Count == 0)
				{
					Debug.LogFormat("[Black and White] There's no room to spawn {0}.", new object[]
					{
						addOnConditionData.Id
					});
				}
				else
				{
					type.MethodCall("InstantiateComponent", instance, new object[]
					{
						list[type.Field<System.Random>("rand", instance).Next(0, list.Count)],
						addOnConditionData.Prefab,
						settings
					});
				}
			}
		}
	}

	private static void SelectRandomPostfix(string __result)
	{
		if (__result != "blackModule")
		{
			return;
		}
		Debug.LogFormat("[Black and White] Black selected! Adding White.", new object[0]);
		_blackCount++;
	}

	private static void BCPCountPostfix(object pool, ref int __result)
	{
		List<string> list = pool.GetType().Field<List<string>>("ModTypes", pool);
		if (list == null || list.Count == 0)
		{
			return;
		}
		if (list.Contains("blackModule"))
		{
			__result *= 2;
		}
	}

	private static void GetSolvablesPrefix(IDictionary ___loadedBombComponents)
	{
		List<string> list = new List<string>();
		IEnumerable<string> enumerable = ___loadedBombComponents.Keys.Cast<string>();
		foreach (string text in enumerable)
		{
			if (___loadedBombComponents[text] == null)
			{
				___loadedBombComponents.Remove(text);
				list.Add(text);
			}
		}
		if (list.Count > 0)
		{
			Debug.LogFormat("[Black and White] IDs removed from Mod Manager: {0}", new object[]
			{
				list.Join(", ")
			});
		}
		else
		{
			Debug.LogFormat("[Black and White] No IDs removed from Mod Manager.", new object[0]);
		}
	}

	[SerializeField]
	private GameObject _fullWhitePrefab;

	[SerializeField]
	private GameObject _fullBlackPrefab;

	private static bool _hasPatched;

	private static Component _whitePrefab;

	private static Component _blackPrefab;

	private static int _blackCount;

	private static Action<object> _activateNeedy;

	private static readonly List<AddOnConditionData> _propertyAdded = new List<AddOnConditionData>();

	private sealed class AddOnConditionData
	{
		public object Prefab;

		public Func<object, int> Condition = (object _) => 0;

		public string Id;
	}
}