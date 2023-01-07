using System.Threading.Tasks;

namespace Services.VFXService
{
    public interface IVfx
    {
        Task PlayAsync();

        Task StopAsync();
    }
}