using System;
using UnityEngine.InputSystem;

namespace Services.InputService
{
	public class InputSystemInputAction : IInputAction, IDisposable
	{
		private readonly InputAction bindedAction;

		public bool Enabled { get; private set; }
		public string Id => bindedAction.name;
		public event Action<IInputContext> OnStarted;
		public event Action<IInputContext> OnPerformed;
		public event Action<IInputContext> OnCanceled;
		public event Action<IInputContext> OnAnyStateChanged;
		public event Action OnEnableChanged;

		public InputSystemInputAction(InputAction bindAction)
		{
			bindedAction = bindAction;
			if (bindedAction.enabled) // initialize if enabled
				Enable();
		}

		public TValue ReadValue<TValue>() where TValue : struct
		{
			return bindedAction.ReadValue<TValue>();
		}

		public void Enable()
		{
			if (Enabled)
				return;

			Enabled = true;
			bindedAction.Enable();
			bindedAction.started += OnBindedActionStarted;
			bindedAction.performed += OnBindedActionPerformed;
			bindedAction.canceled += OnBindedActionCanceled;
			OnEnableChanged?.Invoke();
		}
		
		public void Disable()
		{
			if (!Enabled)
				return;
			
			Enabled = false;
			bindedAction.Disable();
			bindedAction.started -= OnBindedActionStarted;
			bindedAction.performed -= OnBindedActionPerformed;
			bindedAction.canceled -= OnBindedActionCanceled;
			OnEnableChanged?.Invoke();
		}
		
		private void OnBindedActionCanceled(InputAction.CallbackContext context)
		{
			OnInputTriggered(context, OnCanceled);
		}

		private void OnBindedActionPerformed(InputAction.CallbackContext context)
		{
			OnInputTriggered(context, OnPerformed);
		}

		private void OnBindedActionStarted(InputAction.CallbackContext context)
		{
			OnInputTriggered(context, OnStarted);
		}

		private void OnInputTriggered(InputAction.CallbackContext context, Action<IInputContext> trigger)
		{
			var wrappContext = new InputSystemInputContext(context, Id);
			trigger?.Invoke(wrappContext);
			OnAnyStateChanged?.Invoke(wrappContext);
		}

		public void Dispose()
		{
			Disable();
			OnEnableChanged = null;
			OnAnyStateChanged = null;
			OnStarted = null;
			OnPerformed = null;
			OnCanceled = null;
		}
	}
}