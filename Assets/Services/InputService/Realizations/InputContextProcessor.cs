using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Services.InputService
{
	public class InputContextProcessor : IInputContextProcessor
	{
		public IInputContext Process(IInputContext context)
		{
			if (context.TryReadValue(out TouchState touchState))
				return ProcessTouchPhase(context, touchState.phase);

			if (context.TryReadValue(out TouchPhase touchPhase))
				return ProcessTouchPhase(context, touchPhase);

			return context;
		}

		private IInputContext ProcessTouchPhase(IInputContext context, TouchPhase value)
		{
			return new InputSystemContextWrapper()
				.SetContext(context)
				.SetStarted(value is TouchPhase.Began)
				.SetPerformed(value is TouchPhase.Moved or TouchPhase.Began)
				.SetCanceled(value is TouchPhase.Canceled or TouchPhase.Ended or TouchPhase.None);
		}
	}
}