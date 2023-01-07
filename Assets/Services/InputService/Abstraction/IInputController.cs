using System.Collections.Generic;

namespace Services.InputService
{
	public interface IInputController
	{
		IInputAction Get(string id);
		IEnumerable<IInputAction> GetAll();
		void Enable();
		void Disable();
	}
	
	public interface IInputController<in T> : IInputController
	{
		IInputAction Get(T action);
	}
}