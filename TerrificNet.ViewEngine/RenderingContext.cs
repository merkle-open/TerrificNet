using System;

namespace TerrificNet.ViewEngine
{
    public class RenderingContext
    {
        public RenderingContext()
        {
        }

        public RenderingContext(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; private set; }
    }
}