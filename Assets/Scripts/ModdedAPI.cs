using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

internal static class ModdedAPI
{
	private static IDictionary<string, object> API
	{
		get
		{
			EnsureSharedAPI();
			return sharedAPI;
		}
	}

	public static object AddProperty(string name, Func<object> get, Action<object> set)
	{
		Type type = API.GetType();
		MethodInfo method = type.GetMethod("AddProperty", BindingFlags.Instance | BindingFlags.Public);
		return method.Invoke(sharedAPI, new object[]
		{
			name,
			get,
			set
		});
	}

	public static void SetEnabled(object property, bool enabled)
	{
		propertyType.GetField("Enabled").SetValue(property, enabled);
	}

	public static bool TryGetAs<T>(string name, out T value)
	{
		object obj;
		if (API.TryGetValue(name, out obj) && obj is T)
		{
			value = (T)((object)obj);
			return true;
		}
		value = default(T);
		return false;
	}

	public static bool TrySet(string name, object value)
	{
		if (!API.ContainsKey(name))
		{
			return false;
		}
		API[name] = value;
		return true;
	}

	private static void EnsureSharedAPI()
	{
		if (sharedAPI != null)
		{
			return;
		}
		GameObject gameObject = GameObject.Find("ModdedAPI_Info");
		if (gameObject == null)
		{
			gameObject = new GameObject("ModdedAPI_Info", new Type[]
			{
				typeof(ModdedAPIBehaviour)
			});
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		sharedAPI = gameObject.GetComponent<IDictionary<string, object>>();
		propertyType = sharedAPI.GetType().GetNestedType("Property");
	}

	private static IDictionary<string, object> sharedAPI;

	private static Type propertyType;
}