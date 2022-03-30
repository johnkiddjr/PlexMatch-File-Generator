using Microsoft.Extensions.DependencyInjection;
using PlexMatchGenerator.Services;

namespace PlexMatchGenerator
{
    public class Startup
    {
        public Startup()
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGeneratorService, GeneratorService>();
        }
    }
}
