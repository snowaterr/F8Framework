﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
	public static class ModuleCenter
	{
		private class ModuleWrapper
		{
			public int Priority { private set; get; }
			public IModule Module { private set; get; }

			public bool ShouldBeRemoved = false;
			public ModuleWrapper(IModule module, int priority)
			{
				Module = module;
				Priority = priority;
			}
		}

		private static MonoBehaviour _behaviour;
		private static List<ModuleWrapper> _coms = new List<ModuleWrapper>(100);
		private static List<ModuleWrapper> _comsUpdate = new List<ModuleWrapper>(100);
		private static List<ModuleWrapper> _comsLateUpdate = new List<ModuleWrapper>(100);
		private static List<ModuleWrapper> _comsFixedUpdate = new List<ModuleWrapper>(100);
		// 标记要添加的模块
		private static List<ModuleWrapper> _comsUpdateToAdd = new List<ModuleWrapper>(100);
		private static List<ModuleWrapper> _comsLateUpdateToAdd = new List<ModuleWrapper>(100);
		private static List<ModuleWrapper> _comsFixedUpdateToAdd = new List<ModuleWrapper>(100);
		// 标记要删除的模块
		private static List<ModuleWrapper> _comsUpdateToRemove = new List<ModuleWrapper>(100);
		private static List<ModuleWrapper> _comsLateUpdateToRemove = new List<ModuleWrapper>(100);
		private static List<ModuleWrapper> _comsFixedUpdateToRemove = new List<ModuleWrapper>(100);
		
		private static bool _isDirty = false;
		private static bool _isDirtyLate = false;
		private static bool _isDirtyFixed = false;
		private static long _frame = 0;

		/// <summary>
		/// 初始化框架
		/// </summary>
		public static void Initialize(MonoBehaviour behaviour)
		{
			if (behaviour == null)
				LogF8.LogError("MonoBehaviour 为空。");
			if (_behaviour != null)
				LogF8.LogError($"{nameof(ModuleCenter)} 已初始化。");

			UnityEngine.Object.DontDestroyOnLoad(behaviour.gameObject);
			_behaviour = behaviour;

			behaviour.StartCoroutine(CheckFrame());
		}

		/// <summary>
		/// 检测ModuleCenter更新方法
		/// </summary>
		private static IEnumerator CheckFrame()
		{
			var wait = new WaitForSeconds(1f);
			yield return wait;

			// 说明：初始化之后，如果忘记更新ModuleCenter，这里会抛出异常
			if (_frame == 0)
				LogF8.LogError($"请调用更新方法：ModuleCenter.Update");
		}

		/// <summary>
		/// 更新框架
		/// </summary>
		public static void Update()
		{
			_frame++;

			// 添加标记的模块
			foreach (var moduleToAdd in _comsUpdateToAdd)
			{
				_comsUpdate.Add(moduleToAdd);
			}
			if (_comsUpdateToAdd.Count > 0)
			{
				_comsUpdateToAdd.Clear();
			}
			
			// 如果有新模块需要重新排序
			if (_isDirty)
			{
				_isDirty = false;

				_comsUpdate.Sort((left, right) =>
				{
					if (left.Priority < right.Priority)
						return -1;
					else if (left.Priority == right.Priority)
						return 0;
					else
						return 1;
				});
			}
			
			// 遍历所有模块
			foreach (var moduleWrapper in _comsUpdate)
			{
				if (moduleWrapper == null || moduleWrapper.Module == null || moduleWrapper.ShouldBeRemoved)
				{
					_comsUpdateToRemove.Add(moduleWrapper);
					// 如果需要删除该模块，将其标记为删除
					continue;
				}

				// 执行模块更新
				moduleWrapper.Module.OnUpdate();
			}

			// 删除标记的模块
			foreach (var moduleToRemove in _comsUpdateToRemove)
			{
				_comsUpdate.Remove(moduleToRemove);
			}
			if (_comsUpdateToRemove.Count > 0)
			{
				_comsUpdateToRemove.Clear();
			}
		}
		
		/// <summary>
		/// 更新框架
		/// </summary>
		public static void LateUpdate()
		{
			// 添加标记的模块
			foreach (var moduleToAdd in _comsLateUpdateToAdd)
			{
				_comsLateUpdate.Add(moduleToAdd);
			}
			if (_comsLateUpdateToAdd.Count > 0)
			{
				_comsLateUpdateToAdd.Clear();
			}
			
			// 如果有新模块需要重新排序
			if (_isDirtyLate)
			{
				_isDirtyLate = false;
				
				_comsLateUpdate.Sort((left, right) =>
				{
					if (left.Priority < right.Priority)
						return -1;
					else if (left.Priority == right.Priority)
						return 0;
					else
						return 1;
				});
			}
			
			// 遍历所有模块
			foreach (var moduleWrapper in _comsLateUpdate)
			{
				if (moduleWrapper == null || moduleWrapper.Module == null || moduleWrapper.ShouldBeRemoved)
				{
					_comsLateUpdateToRemove.Add(moduleWrapper);
					// 如果需要删除该模块，将其标记为删除
					continue;
				}

				// 执行模块更新
				moduleWrapper.Module.OnLateUpdate();
			}

			// 删除标记的模块
			foreach (var moduleToRemove in _comsLateUpdateToRemove)
			{
				_comsLateUpdate.Remove(moduleToRemove);
			}
			if (_comsLateUpdateToRemove.Count > 0)
			{
				_comsLateUpdateToRemove.Clear();
			}
		}
		
		/// <summary>
		/// 更新框架
		/// </summary>
		public static void FixedUpdate()
		{
			// 添加标记的模块
			foreach (var moduleToAdd in _comsFixedUpdateToAdd)
			{
				_comsFixedUpdate.Add(moduleToAdd);
			}
			if (_comsFixedUpdateToAdd.Count > 0)
			{
				_comsFixedUpdateToAdd.Clear();
			}
			
			// 如果有新模块需要重新排序
			if (_isDirtyFixed)
			{
				_isDirtyFixed = false;
				
				_comsFixedUpdate.Sort((left, right) =>
				{
					if (left.Priority < right.Priority)
						return -1;
					else if (left.Priority == right.Priority)
						return 0;
					else
						return 1;
				});
			}
			
			// 遍历所有模块
			foreach (var moduleWrapper in _comsFixedUpdate)
			{
				if (moduleWrapper == null || moduleWrapper.Module == null || moduleWrapper.ShouldBeRemoved)
				{
					_comsFixedUpdateToRemove.Add(moduleWrapper);
					// 如果需要删除该模块，将其标记为删除
					continue;
				}

				// 执行模块更新
				moduleWrapper.Module.OnFixedUpdate();
			}

			// 删除标记的模块
			foreach (var moduleToRemove in _comsFixedUpdateToRemove)
			{
				_comsFixedUpdate.Remove(moduleToRemove);
			}
			if (_comsFixedUpdateToRemove.Count > 0)
			{
				_comsFixedUpdateToRemove.Clear();
			}
		}
		
		/// <summary>
		/// 查询游戏模块是否存在
		/// </summary>
		public static bool Contains<T>() where T : class, IModule
		{
			System.Type type = typeof(T);
			return Contains(type);
		}

		/// <summary>
		/// 查询游戏模块是否存在
		/// </summary>
		public static bool Contains(System.Type moduleType)
		{
			for (int i = 0; i < _coms.Count; i++)
			{
				if (_coms[i].Module.GetType() == moduleType)
					return true;
			}
			return false;
		}

		/// <summary>
		/// 创建游戏模块
		/// </summary>
		/// <typeparam name="T">模块类</typeparam>
		/// <param name="priority">运行时的优先级，优先级越小越早执行。如果没有设置优先级，那么会按照添加顺序执行</param>
		public static T CreateModule<T>(int priority = 0) where T : class, IModule
		{
			return CreateModule<T>(null, priority);
		}

		/// <summary>
		/// 创建游戏模块
		/// </summary>
		/// <typeparam name="T">模块类</typeparam>
		/// <param name="createParam">创建参数</param>
		/// <param name="priority">运行时的优先级，优先级越小越早执行。如果没有设置优先级，那么会按照添加顺序执行</param>
		public static T CreateModule<T>(System.Object createParam, int priority = 0) where T : class, IModule
		{
			if (priority < 0)
			{
				LogF8.LogError("优先级不能为负");
				priority = 0;
			}
			
			if (Contains(typeof(T)))
			{
				LogF8.LogError($"游戏模块 {typeof(T)} 已存在");
				return null;
			}
			
			// 如果没有设置优先级
			if (priority == 0)
			{
				int minPriority = GetMaxPriority();
				priority = ++minPriority;
			}

			LogF8.LogModule($"创建游戏模块: {typeof(T)}");

			T module = null;

			// 检查类型是否是 MonoBehaviour 的子类
			if (typeof(MonoBehaviour).IsAssignableFrom(typeof(T)))
			{
				GameObject obj = new GameObject(typeof(T).Name, typeof(T));
				module = obj.GetComponent<T>();
			}
			else
			{
				module = Activator.CreateInstance<T>();
			}

			ModuleWrapper wrapper = new ModuleWrapper(module, priority);
			wrapper.Module.OnInit(createParam);
			_coms.Add(wrapper);
			_coms.Sort((left, right) =>
			{
				if (left.Priority < right.Priority)
					return -1;
				else if (left.Priority == right.Priority)
					return 0;
				else
					return 1;
			});
			if (typeof(T).GetCustomAttributes(typeof(UpdateRefreshAttribute), false).Length > 0)
			{
				_comsUpdateToAdd.Add(wrapper);
				_isDirty = true;
			}
			if (typeof(T).GetCustomAttributes(typeof(LateUpdateRefreshAttribute), false).Length > 0)
			{
				_comsLateUpdateToAdd.Add(wrapper);
				_isDirtyLate = true;
			}
			if (typeof(T).GetCustomAttributes(typeof(FixedUpdateRefreshAttribute), false).Length > 0)
			{
				_comsFixedUpdateToAdd.Add(wrapper);
				_isDirtyFixed = true;
			}
			return module;
		}

		/// <summary>
		/// 销毁模块
		/// </summary>
		/// <typeparam name="T">模块类</typeparam>
		public static bool DestroyModule<T>()
		{
			var moduleType = typeof(T);
			for (int i = 0; i < _comsUpdate.Count; i++)
			{
				if (_comsUpdate[i].Module.GetType() == moduleType)
				{
					_comsUpdate[i].ShouldBeRemoved = true;
				}
			}
			
			for (int i = 0; i < _comsLateUpdate.Count; i++)
			{
				if (_comsLateUpdate[i].Module.GetType() == moduleType)
				{
					_comsLateUpdate[i].ShouldBeRemoved = true;
				}
			}
			
			for (int i = 0; i < _comsFixedUpdate.Count; i++)
			{
				if (_comsFixedUpdate[i].Module.GetType() == moduleType)
				{
					_comsFixedUpdate[i].ShouldBeRemoved = true;
				}
			}
			
			for (int i = 0; i < _coms.Count; i++)
			{
				if (_coms[i].Module.GetType() == moduleType)
				{
					_coms[i].Module.OnTermination();
					_coms.RemoveAt(i);
					return true;
				}
			}
			
			return false;
		}

		/// <summary>
		/// 获取游戏模块
		/// </summary>
		/// <typeparam name="T">模块类</typeparam>
		public static T GetModule<T>() where T : class, IModule
		{
			System.Type type = typeof(T);
			for (int i = 0; i < _coms.Count; i++)
			{
				if (_coms[i].Module.GetType() == type)
					return _coms[i].Module as T;
			}

			LogF8.LogError($"未找到游戏模块 {type}");
			return null;
		}

		/// <summary>
		/// 获取当前模块里最大的优先级数值
		/// </summary>
		private static int GetMaxPriority()
		{
			int maxPriority = int.MinValue; // 初始化为 int 类型的最小值
			for (int i = 0; i < _coms.Count; i++)
			{
				if (_coms[i].Priority > maxPriority)
					maxPriority = _coms[i].Priority;
			}
			return maxPriority; // 大于等于零
		}

		#region 协程相关
		/// <summary>
		/// 开启一个协程
		/// </summary>
		public static Coroutine StartCoroutine(IEnumerator coroutine)
		{
			if (_behaviour == null)
				LogF8.LogError($"{nameof(ModuleCenter)} 未初始化。使用 ModuleCenter.Initialize");
			return _behaviour.StartCoroutine(coroutine);
		}

		/// <summary>
		/// 停止一个协程
		/// </summary>
		public static void StopCoroutine(Coroutine coroutine)
		{
			if (_behaviour == null)
				LogF8.LogError($"{nameof(ModuleCenter)} 未初始化。使用 ModuleCenter.Initialize");
			_behaviour.StopCoroutine(coroutine);
		}


		/// <summary>
		/// 开启一个协程
		/// </summary>
		public static void StartCoroutine(string methodName)
		{
			if (_behaviour == null)
				LogF8.LogError($"{nameof(ModuleCenter)} 未初始化。使用 ModuleCenter.Initialize");
			_behaviour.StartCoroutine(methodName);
		}

		/// <summary>
		/// 停止一个协程
		/// </summary>
		public static void StopCoroutine(string methodName)
		{
			if (_behaviour == null)
				LogF8.LogError($"{nameof(ModuleCenter)} 未初始化。使用 ModuleCenter.Initialize");
			_behaviour.StopCoroutine(methodName);
		}


		/// <summary>
		/// 停止所有协程
		/// </summary>
		public static void StopAllCoroutines()
		{
			if (_behaviour == null)
				LogF8.LogError($"{nameof(ModuleCenter)} 未初始化。使用 ModuleCenter.Initialize");
			_behaviour.StopAllCoroutines();
		}
		#endregion
	}
}