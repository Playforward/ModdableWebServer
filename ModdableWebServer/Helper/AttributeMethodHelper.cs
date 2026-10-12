using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Serilog;

namespace ModdableWebServer.Helper;

public static class AttributeMethodHelper
{
	[RequiresUnreferencedCode("This require to get all assembly type. Use GetMethodAndAttribute<T>(type) instead.")]
	public static void Merge<T>(this Dictionary<T, MethodInfo> keyValues, Assembly? assembly) where T : Attribute
	{
		ArgumentNullException.ThrowIfNull(assembly, "assembly");
		foreach (var (val2, methodInfo2) in GetMethodAndAttribute<T>(assembly))
		{
			if (val2 != null && !(methodInfo2 == null))
			{
				if (!keyValues.TryAdd(val2, methodInfo2))
				{
					Log.Warning($"Cannot merge {val2}.");
				}
				else
				{
					Log.Verbose($"{val2} = {methodInfo2}");
				}
			}
		}
	}

	[RequiresUnreferencedCode("This require to get all assembly type. Use GetMethodAndAttribute<T>(type) instead.")]
	public static void Override<T>(this Dictionary<T, MethodInfo> keyValues, Assembly? assembly) where T : Attribute
	{
		ArgumentNullException.ThrowIfNull(assembly, "assembly");
		foreach (var (val2, methodInfo2) in GetMethodAndAttribute<T>(assembly))
		{
			if (val2 != null && !(methodInfo2 == null))
			{
				keyValues.Remove(val2);
				if (!keyValues.TryAdd(val2, methodInfo2))
				{
					throw new Exception("KeyValues adding was not possible (Even after removed old data)");
				}
				Log.Verbose($"[Override] {val2} = {methodInfo2}");
			}
		}
	}

	[RequiresUnreferencedCode("This require to get all assembly type. Use GetMethodAndAttribute<T>(type) instead.")]
	public static Dictionary<T, MethodInfo> GetMethodAndAttribute<T>(Assembly? assembly) where T : Attribute
	{
		ArgumentNullException.ThrowIfNull(assembly, "assembly");
		Dictionary<T, MethodInfo> dictionary = new Dictionary<T, MethodInfo>();
		foreach (MethodInfo item in assembly.GetTypes().SelectMany((Type select) => from method in @select.GetMethods()
			where method.IsDefined(typeof(T)) && (method.ReturnType == typeof(void) || method.ReturnType == typeof(bool))
			select method))
		{
			if (item == null)
			{
				continue;
			}
			IEnumerable<T> customAttributes = item.GetCustomAttributes<T>();
			if (customAttributes == null)
			{
				continue;
			}
			foreach (T item2 in customAttributes)
			{
				if (item2 != null)
				{
					dictionary.Add(item2, item);
				}
			}
		}
		return dictionary;
	}

	public static void Merge<T>(this Dictionary<T, MethodInfo> keyValues, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] Type? type) where T : Attribute
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		foreach (var (val2, methodInfo2) in GetMethodAndAttribute<T>(type))
		{
			if (val2 != null && !(methodInfo2 == null))
			{
				if (!keyValues.TryAdd(val2, methodInfo2))
				{
					Log.Warning($"Cannot merge {val2}.");
				}
				else
				{
					Log.Verbose($"{val2} = {methodInfo2}");
				}
			}
		}
	}

	public static void Override<T>(this Dictionary<T, MethodInfo> keyValues, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] Type? type) where T : Attribute
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		foreach (var (val2, methodInfo2) in GetMethodAndAttribute<T>(type))
		{
			if (val2 != null && !(methodInfo2 == null))
			{
				keyValues.Remove(val2);
				if (!keyValues.TryAdd(val2, methodInfo2))
				{
					throw new Exception("KeyValues adding was not possible (Even after removed old data)");
				}
				Log.Verbose($"[Override] {val2} = {methodInfo2}");
			}
		}
	}

	public static Dictionary<T, MethodInfo> GetMethodAndAttribute<T>([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] Type? type) where T : Attribute
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		Dictionary<T, MethodInfo> dictionary = new Dictionary<T, MethodInfo>();
		foreach (MethodInfo item in from method in type.GetMethods()
			where method.IsDefined(typeof(T)) && (method.ReturnType == typeof(void) || method.ReturnType == typeof(bool))
			select method)
		{
			if (item == null)
			{
				continue;
			}
			IEnumerable<T> customAttributes = item.GetCustomAttributes<T>();
			if (customAttributes == null)
			{
				continue;
			}
			foreach (T item2 in customAttributes)
			{
				if (item2 != null)
				{
					dictionary.Add(item2, item);
				}
			}
		}
		return dictionary;
	}
}
