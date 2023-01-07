using System;
using System.Collections.Generic;
using System.Linq;
using Services.LoggerService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Services.InputService
{
	public enum InputSystemType
	{
		LegacyInputManager,
		NewInputSystem,
	}

	public class InputHelper
	{
		
	#region Usage
		/// <summary>
		/// Raycast at the pointer position, looks for the first object that has raycast enabled and compares them to the target object.
		/// </summary>
		/// <param name="target">Reference to the object to check under the pointer</param>
		/// <param name="systemType">What input system to use to detect the pointer device?</param>
		/// <returns></returns>
		public static bool IsPointerOver(GameObject target, InputSystemType systemType)
		{
			var focused = FirstObjectUnderPointer(systemType);
			if (!focused)
				return false;

			return focused == target || focused.transform.IsChildOf(target.transform);
		}
		
		/// <summary>
		/// Raycast at the pointer position, looks for the first object that has raycast enabled and compares them to the target object.
		/// Automatic detect InputSystemType.
		/// </summary>
		/// <param name="target">Reference to the object to check under the pointer</param>
		/// <returns></returns>
		public static bool IsPointerOver(GameObject target)
		{
			return IsPointerOver(target, GetInputSystemType());
		}

		/// <summary>
		/// Raycast at pointer position, looking for the first object that has raycast enabled.
		/// </summary>
		/// <returns>First found object under pointer</returns>
		/// <param name="systemType">What input system to use to detect the pointer device?</param>
		public static GameObject FirstObjectUnderPointer(InputSystemType systemType)
		{
			try
			{
				return RaycastUnderPointer(systemType).gameObject;
			}
			catch (Exception e)
			{
				DefaultLogger.Error(e);
				return default;
			}
		}
		
		/// <summary>
		/// Raycast at pointer position, looking for the first object that has raycast enabled.
		/// Automatic detect InputSystemType.
		/// </summary>
		/// <returns>First found object under pointer</returns>
		public static GameObject FirstObjectUnderPointer()
		{
			return FirstObjectUnderPointer(GetInputSystemType());
		}
	#endregion
		

	#region InputSystem / LegacyInputManager
		
		/// <summary>
		/// Get device id currently used as pointer
		/// </summary>
		/// <param name="systemType">target input system</param>
		/// <returns>Device ID currently used as pointer</returns>
		public static int GetPointerId(InputSystemType systemType)
		{
			switch (systemType)
			{
				
#if ENABLE_INPUT_SYSTEM
				case InputSystemType.NewInputSystem:  return Pointer.current.device.deviceId;
#endif
				case InputSystemType.LegacyInputManager : return PointerId.maxPointers;
				default: throw new NotImplementedException($"{systemType} not included in project.");
			}
		}

		/// <summary>
		/// Get current InputSystemType.
		/// </summary>
		/// <returns>Current input system used</returns>
		public static InputSystemType GetInputSystemType()
		{
			var systemType = InputSystemType.LegacyInputManager;
#if ENABLE_INPUT_SYSTEM
			systemType = InputSystemType.NewInputSystem;
#endif
			return systemType;
		}
	
		/// <summary>
		/// The position of the screen below the switch device from some input system.
		/// </summary>
		/// <returns>The position of the pointer in screen space.</returns>
		/// <param name="systemType">What input system to use to detect the pointer device?</param>
		public static Vector2 GetPositionUnderPointer(InputSystemType systemType)
		{
			switch (systemType)
			{
				
#if ENABLE_INPUT_SYSTEM
				case InputSystemType.NewInputSystem:  return Pointer.current.position.ReadValue();
#endif
				case InputSystemType.LegacyInputManager : return Input.mousePosition;
				default: throw new NotImplementedException($"{systemType} not enabled in project.");
			}
		}
		
		/// <summary>
		/// The position of the screen below the switch device from some input system.
		/// Automatic detect InputSystemType. 
		/// </summary>
		/// <returns>The position of the pointer in screen space.</returns>
		public static Vector2 GetPositionUnderPointer()
		{
			return GetPositionUnderPointer(GetInputSystemType());
		}
		
	#endregion
		

	#region Raycast
		
		private static readonly List<RaycastResult> CACHED_RAYCAST_RESULTS = new();
		private static PointerEventData cachedPointerEventData;
		private static int snapshotRaycastFrame;
		
		/// <summary>
		/// Raycast under the pointer, the input system is automatically detected.
		/// </summary>
		/// <returns>Current raycast result</returns>
		public static RaycastResult RaycastUnderPointer()
		{
			return RaycastUnderPointer(GetInputSystemType());
		}
		
		/// <summary>
		/// Raycast under the pointer, using the desired type of input system.
		/// </summary>
		/// <param name="systemType">target input system</param>
		/// <returns>Current raycast result</returns>
		public static RaycastResult RaycastUnderPointer(InputSystemType systemType)
		{
			if (EventSystem.current == null)
			{
				DefaultLogger.Error("EventSystem not initialized");
				return default;
			}
			
			// raycast once per frame
			if (snapshotRaycastFrame == Time.frameCount)
				return cachedPointerEventData.pointerCurrentRaycast;
			
			if (cachedPointerEventData == null)
				cachedPointerEventData = new PointerEventData(EventSystem.current);
			
			snapshotRaycastFrame = Time.frameCount;
			cachedPointerEventData.position = GetPositionUnderPointer(systemType);
			EventSystem.current.RaycastAll(cachedPointerEventData, CACHED_RAYCAST_RESULTS);
			cachedPointerEventData.pointerCurrentRaycast = CACHED_RAYCAST_RESULTS.FirstOrDefault(raycast => raycast.gameObject != null);
			CACHED_RAYCAST_RESULTS.Clear();

			return cachedPointerEventData.pointerCurrentRaycast;
		}
		
	#endregion
	}
}