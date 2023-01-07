using System;
using Services.LoggerService;

namespace Services.InputService
{
	public struct InputSystemContextWrapper : IInputContext
	{
		private IInputContext inputContext;

		public string Id { get; private set; }
		
		public bool Started { get; private set; }
		
		public bool Performed { get; private set; }
		
		public bool Canceled { get; private set; }
		
		public Type ControlValueType { get; private set; }
		
		
		public InputSystemContextWrapper SetContext(IInputContext value)
		{
			inputContext = value;
			ControlValueType = inputContext.ControlValueType;
			Started = inputContext.Started;
			Performed = inputContext.Performed;
			Canceled = inputContext.Canceled;
			Id = inputContext.Id;
			return this;
		}

		public InputSystemContextWrapper SetId(string value)
		{
			Id = value;
			return this;
		}

		public InputSystemContextWrapper SetActionType(Type value)
		{
			ControlValueType = value;
			return this;
		}

		public InputSystemContextWrapper SetStarted(bool value)
		{
			Started = value;
			return this;
		}
		
		public InputSystemContextWrapper SetPerformed(bool value)
		{
			Performed = value;
			return this;
		}
		
		public InputSystemContextWrapper SetCanceled(bool value)
		{
			Canceled = value;
			return this;
		}
		
		public TValue ReadValue<TValue>() where TValue : struct
		{
			if (inputContext == null)
			{
				DefaultLogger.Error($"Input context does not setup in : {GetType().Name}.{nameof(SetContext)}({nameof(IInputContext)} context)");
				return default;
			}
			
			return inputContext.ReadValue<TValue>();
		}

		public bool TryReadValue<TValue>(out TValue result) where TValue : struct
		{
			result = default;
			if (inputContext == null)
			{
				DefaultLogger.Error($"Input context does not setup in : {GetType().Name}.{nameof(SetContext)}({nameof(IInputContext)} context)");
				return false;
			}
			return inputContext.TryReadValue(out result);
		}
	}
}