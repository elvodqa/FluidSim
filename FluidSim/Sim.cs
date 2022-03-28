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
        
        private GameWindow Window;

        private Container container;

        private Options options;

        public const int numParticles = 100;

        public Sim()
        {
            options = new Options();
            container = new Container(0.2f, 0, 0.0000001f);
            //Window = new RenderWindow(new VideoMode((uint) (SIZE*SCALE), (uint) (SIZE*SCALE)), "Fluid Simulation", Styles.Close | Styles.Titlebar);
            Window = new GameWindow("Title",(uint) (SIZE*SCALE), (uint) (SIZE*SCALE));
            Window.Closed += (sender, e) =>
            {
                ((Window)sender)?.Close();
                Dispose();
            };
            Window.KeyPressed += (sender, e) =>
            {
                if (e.Code == Keyboard.Key.C)
                {
                    Color c = (options.GetColor() == Color.Default) ?
                        Color.Hsb : (options.GetColor() == Color.Hsb) ?
                            Color.Velocity : Color.Default;

                    options.SetColor(c);
                }
            };
            Window.MouseButtonPressed += (sender, e) =>
            {
                if (e.Button == Mouse.Button.Right)
                {
                    Color c = (options.GetColor() == Color.Default) ?
                        Color.Hsb : (options.GetColor() == Color.Hsb) ?
                            Color.Velocity : Color.Default;

                    options.SetColor(c);
                }
            };
        }

        //[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        public void Run()
        {
           Window.SetVisible(true);
           while (true)
           {
               Window.DispatchEvents();
               Window.Clear();
               Update();
               Window.Display();
           }
            
        }

        private void Update()
        {
            Vector2i previousMouse = Mouse.GetPosition(Window);
            Vector2i currentMouse = Mouse.GetPosition(Window);
            if (Mouse.IsButtonPressed(Mouse.Button.Left))			
                container.AddDensity(currentMouse.Y/SCALE, currentMouse.X/SCALE, 200);

            currentMouse = Mouse.GetPosition(Window);

            float amountX = currentMouse.X - previousMouse.X;
            float amountY = currentMouse.Y - previousMouse.Y;

            container.AddVelocity(currentMouse.Y/SCALE, currentMouse.X/SCALE, amountY / 10, amountX / 10);
	
            previousMouse = currentMouse;

            container.Step();
            container.Render(Window, options.GetColor());
            //container.Dispose();
            container.FadeDensity(SIZE*SIZE);

        }
        
        public void Dispose()
        {
            //GC.SuppressFinalize(this);
        }
    }
}