using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.InputService
{
	public class InputController<T> : IInputController<T> where T : struct
	{
		private readonly Dictionary<T, IInputAction> inputActions;
		
		public InputController(IEnumerable<IInputAction> inputActions)
		{
			if (inputActions == null)
				throw new ArgumentException($"Cannot be null argument = {nameof(inputActions)}");
			
			this.inputActions = inputActions.ToDictionary(x => Enum.Parse<T>(x.Id));
		}
		
		public IInputAction Get(T action)
		{
			if (!inputActions.ContainsKey(action))
				throw new ArgumentException($"Action with ID : {action} does not represent in collection");

			return inputActions[action];
		}

		public IInputAction Get(string id)
		{
			return Get(Enum.Parse<T>(id));
		}

		public IEnumerable<IInputAction> GetAll()
		{
			return inputActions.Values;
		}

		public void Enable()
		{
			foreach (var value in inputActions.Values.Where(value => !value.Enabled))
			{
				value.Enable();
			}
		}

		public void Disable()
		{
			foreach (var value in inputActions.Values.Where(value => value.Enabled))
			{
				value.Disable();
			}
		}
	}
}