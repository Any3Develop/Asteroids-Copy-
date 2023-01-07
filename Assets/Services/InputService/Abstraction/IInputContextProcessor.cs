namespace Services.InputService
{
	public interface IInputContextProcessor
	{
		IInputContext Process(IInputContext context);
	}
}