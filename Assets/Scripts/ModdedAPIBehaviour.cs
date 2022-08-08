using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModdedAPIBehaviour : MonoBehaviour, IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
{
	public ModdedAPIBehaviour()
	{
		_properties = new Dictionary<string, List<Property>>();
	}

	public Property AddProperty(string name, Func<object> get, Action<object> set)
	{
		List<Property> list;
		if (!_properties.TryGetValue(name, out list))
		{
			list = new List<Property>();
			_properties.Add(name, list);
		}
		Property property = new Property(get, set);
		list.Add(property);
		return property;
	}

	public object this[string key]
	{
		get
		{
			return GetEnabledProperty(key).Get();
		}
		set
		{
			if (!_properties.ContainsKey(key))
			{
				throw new Exception("You can't add items to this Dictionary.");
			}
			Property enabledProperty = GetEnabledProperty(key);
			if (!enabledProperty.CanSet())
			{
				throw new Exception("The key \"" + key + "\" cannot be set (it is read-only).");
			}
			enabledProperty.Set(value);
		}
	}

	public int Count
	{
		get
		{
			return _properties.Count;
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return false;
		}
	}

	public ICollection<string> Keys
	{
		get
		{
			return _properties.Keys.ToList<string>();
		}
	}

	public ICollection<object> Values
	{
		get
		{
			throw new NotSupportedException("The Values property is not supported in this Dictionary.");
		}
	}

	public void Add(KeyValuePair<string, object> item)
	{
		throw new NotSupportedException("You can't add items to this Dictionary.");
	}

	public void Add(string key, object value)
	{
		throw new NotSupportedException("You can't add items to this Dictionary.");
	}

	public void Clear()
	{
		throw new NotSupportedException("You can't clear this Dictionary.");
	}

	public bool Contains(KeyValuePair<string, object> item)
	{
		throw new NotSupportedException("The Contains method is not supported in this Dictionary.");
	}

	public bool ContainsKey(string key)
	{
		return _properties.ContainsKey(key);
	}

	public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
	{
		throw new NotSupportedException("The CopyTo method is not supported in this Dictionary.");
	}

	public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
	{
		throw new NotSupportedException("The GetEnumerator method is not supported in this Dictionary.");
	}

	public bool Remove(KeyValuePair<string, object> item)
	{
		throw new NotSupportedException("The Remove method is not supported in this Dictionary.");
	}

	public bool Remove(string key)
	{
		throw new NotSupportedException("The Remove method is not supported in this Dictionary.");
	}

	public bool TryGetValue(string key, out object value)
	{
		bool result;
		try
		{
			value = GetEnabledProperty(key).Get();
			result = true;
		}
		catch
		{
			value = null;
			result = false;
		}
		return result;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotSupportedException("The GetEnumerator method is not supported in this Dictionary.");
	}

	private Property GetEnabledProperty(string name)
	{
		return _properties[name].Find((Property property) => property.Enabled);
	}

	private readonly Dictionary<string, List<Property>> _properties;

	public class Property
	{
		public Property(Func<object> get, Action<object> set)
		{
			_getDelegate = get;
			_setDelegate = set;
		}

		public object Get()
		{
			return _getDelegate();
		}

		public bool CanSet()
		{
			return _setDelegate != null;
		}

		public void Set(object value)
		{
			_setDelegate(value);
		}

		private readonly Func<object> _getDelegate;

		private readonly Action<object> _setDelegate;

		public bool Enabled = true;
	}
}