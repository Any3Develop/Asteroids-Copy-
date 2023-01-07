using System;
using UnityEngine;
using Zenject;

namespace Services.ZenjectExtends
{
	// For correct operation, you need to set the execution order in PlayerSettings/ScriptExecutionOrder
	// This doesn't work for ProjectContext because it forcefully disables awake initialization when the prefab is created.
	[RequireComponent(typeof(Context))]
	public class AutoLoadMonoInstallers : MonoBehaviour
	{
		private void Awake()
		{
			LoadMonoInstallers();
		}
		
		[ContextMenu(nameof(LoadMonoInstallers))]
		private void LoadMonoInstallers()
		{
			if (!TryGetComponent(out Context targetContex))
				throw new Exception($"[{nameof(AutoLoadMonoInstallers)}] Not found target context!");
			
			targetContex.Installers = GetComponentsInChildren<MonoInstaller>();
			Debug.Log($"AutoLoadMonoInstallers for : {name}");
		}
	}
}