using System;

namespace FluidSim
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var sim = new Sim())
            {
                sim.Run();
            }
        }
    }
}