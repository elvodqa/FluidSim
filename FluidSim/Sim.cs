using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace FluidSim
{
    public class Sim : IDisposable
    {
        public static int rowPxSIZE = 70;
        public static int columnPxSIZE = 70;
        public static int screenPerPX = 10;

        
        //private RenderWindow win;
        private GameWindow Window;
        private Container container;

        private Options options;

        public const int numParticles = 100;

        public Sim()
        {
            options = new Options();
            container = new Container(0.2f, 0, 0.0000001f);
            //win = new RenderWindow(new VideoMode((uint) (rowPxSIZE*screenPerPX), (uint) (columnPxSIZE*screenPerPX)), "Fluid Simulation", Styles.Close | Styles.Titlebar);
            Window = new GameWindow("Title",(uint) (rowPxSIZE*screenPerPX), (uint) (columnPxSIZE*screenPerPX));
            
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

        public void Run()
        {
            var stopwatch = new Stopwatch();
            
            Window.SetVisible(true);
            while (true)
            {
                Window.DispatchEvents();
                Window.Clear();
                Update(stopwatch);
                Window.Display();
            }
            
        }

        private void Update(Stopwatch stopwatch)
        {
            Vector2i previousMouse = Mouse.GetPosition(Window);
            Vector2i currentMouse = Mouse.GetPosition(Window);
            
            stopwatch.Reset();
            stopwatch.Start();
                
            if (Mouse.IsButtonPressed(Mouse.Button.Left))			
                container.AddDensity(currentMouse.Y/screenPerPX, currentMouse.X/screenPerPX, 200);

            currentMouse = Mouse.GetPosition(Window);

            float amountX = currentMouse.X - previousMouse.X;
            float amountY = currentMouse.Y - previousMouse.Y;

            container.AddVelocity(currentMouse.Y/screenPerPX, currentMouse.X/screenPerPX, amountY / 10, amountX / 10);
		
            previousMouse = currentMouse;

            container.Step();
            container.Render(Window, options.GetColor());
            //container.Dispose();
            // container.FadeDensity(screenPerPX*screenPerPX);
		        
            Window.Display();
            Thread.Yield();
            if (stopwatch.ElapsedMilliseconds<33)
            {
                    
                Thread.Sleep(TimeSpan.FromMilliseconds(33)-stopwatch.Elapsed);
            }
        }
        
        public void Dispose()
        {
            //GC.SuppressFinalize(this);
        }
    }
}