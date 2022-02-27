using System;
using System.Diagnostics.CodeAnalysis;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace FluidSim
{
    public class Sim : IDisposable
    {
        public static int SIZE = 70;
        public static int SCALE = 8;
        
        private RenderWindow win;

        private Container container;

        private Options options;

        public const int numParticles = 100;

        public Sim()
        {
            options = new Options();
            container = new Container(0.2f, 0, 0.0000001f);
            win = new RenderWindow(new VideoMode((uint) (SIZE*SCALE), (uint) (SIZE*SCALE)), "Fluid Simulation", Styles.Close | Styles.Titlebar);
        }

        //[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        public void Run()
        {
            Vector2i previousMouse = Mouse.GetPosition(win);
            Vector2i currentMouse = Mouse.GetPosition(win);

            while (win.IsOpen)
            {
                win.Closed += (sender, e) =>
                {
                    ((Window)sender)?.Close();
                    Dispose();
                };
                win.KeyPressed += (sender, e) =>
                {
                    if (e.Code == Keyboard.Key.C)
                    {
                        Color c = (options.GetColor() == Color.Default) ?
                            Color.Hsb : (options.GetColor() == Color.Hsb) ?
                                Color.Velocity : Color.Default;

                        options.SetColor(c);
                    }
                };
                
                if (Mouse.IsButtonPressed(Mouse.Button.Left))			
                    container.AddDensity(currentMouse.Y/SCALE, currentMouse.X/SCALE, 200);

                currentMouse = Mouse.GetPosition(win);

                float amountX = currentMouse.X - previousMouse.X;
                float amountY = currentMouse.Y - previousMouse.Y;

                container.AddVelocity(currentMouse.Y/SCALE, currentMouse.X/SCALE, amountY / 10, amountX / 10);
		
                previousMouse = currentMouse;

                container.Step();
                container.Render(win, options.GetColor());
                container.FadeDensity(SIZE*SIZE);
		        
                win.Display();
            }
            
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}