using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

public static class ReflectionHelper
{
	public static Type FindTypeInGame(string fullName)
	{
		return GameAssembly.GetSafeTypes().FirstOrDefault((Type t) => t.FullName.Equals(fullName));
	}

	public static Type FindGameType(string fullName)
	{
		return FindType(fullName, "Assembly-CSharp");
	}

	public static Type FindType(string fullName, string assemblyName = null)
	{
		return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetSafeTypes()).FirstOrDefault(t => t.FullName != null && t.FullName.Equals(fullName) && (assemblyName == null || t.Assembly.GetName().Name.Equals(assemblyName)));
	}

	public static Type FindType(string fullName)
	{
		IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
		if (f__mgcache0 == null)
		{
			f__mgcache0 = new Func<Assembly, IEnumerable<Type>>(GetSafeTypes);
		}
		return assemblies.SelectMany(f__mgcache0).FirstOrDefault((Type t) => t.FullName.Equals(fullName));
	}

	public static MethodInfo Method(this Type t, string name)
	{
		return t.GetMethod(name, Flags);
	}

	public static void SetField<T>(this Type t, string name, object o, T value)
	{
		t.GetField(name, Flags).SetValue(o, value);
	}

	public static T Field<T>(this Type t, string name, object o)
	{
		return (T)(t.GetField(name, Flags).GetValue(o));
	}

	public static T MethodCall<T>(this Type t, string name, object o, object[] args)
	{
		return (T)(t.Method(name).Invoke(o, args));
	}

	public static void MethodCall(this Type t, string name, object o, object[] args)
	{
		t.Method(name).Invoke(o, args);
	}

	private static IEnumerable<Type> GetSafeTypes(this Assembly assembly)
	{
		IEnumerable<Type> result;
		try
		{
			result = assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			result = from x in ex.Types
			where x != null
			select x;
		}
		catch (Exception)
		{
			result = null;
		}
		return result;
	}

	private static Assembly GameAssembly
	{
		get
		{
			if (_gameAssembly == null)
			{
				_gameAssembly = FindType("KTInputManager").Assembly;
			}
			return _gameAssembly;
		}
	}

	public static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

	private static Assembly _gameAssembly;

	[CompilerGenerated]
	private static Func<Assembly, IEnumerable<Type>> f__mgcache0;
}
