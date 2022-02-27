using System;
using SFML.Graphics;
using SFML.System;

namespace FluidSim
{
    public class Container : IDisposable
    {
        public static int SIZE = 70;
        public static int SCALE = 8;
        
        private Physics physics;
        private int size;
        
        private float dt;
        private float diff;
        private float visc;

        private float[] px = new float[SIZE * SIZE];
        private float[] py = new float[SIZE * SIZE];
        
        private float[] x = new float[SIZE * SIZE];
        private float[] y = new float[SIZE * SIZE];
        
        private float[] previousDensity= new float[SIZE * SIZE];
        private float[] density = new float[SIZE * SIZE];

        private void InitArr(float[] arr, int size)
        {
            for (int i = 0; i < size; i++) {
                arr[i] = 0;	
            }
        }

        private float MapToRange(float value, float minIn, float maxIn, float minOut, float maxOut)
        {
            float x = (value - minIn) / (maxIn - minIn);
            float result = minOut + (maxOut - minOut) * x;
            return (result < minOut) ? minOut : (result > maxOut) ? maxOut : result;
        }

        public Container(float dt, float diff, float visc)
        {
            physics = new Physics();
            this.size = SIZE;
            this.dt = dt;
            this.diff = diff;
            this.visc = visc;

            InitArr(this.px, SIZE*SIZE);
            InitArr(this.py, SIZE*SIZE);
            InitArr(this.x, SIZE*SIZE);
            InitArr(this.y, SIZE*SIZE);
            InitArr(this.previousDensity, SIZE*SIZE);
            InitArr(this.density, SIZE*SIZE);
        }
        
        public void AddDensity(int x, int y, float amount)
        {
            this.density[IX._IX(x,y,this.size)] += amount;
        }
        
        public void AddVelocity(float x, float y, float px, float py)
        {
            
            int index = IX._IX((int) x,(int) y,this.size);

            this.x[index] += px;
            this.y[index] += py;
        }

        public void Step()
        {
            physics.Diffuse(1, this.px, this.x, this.visc, this.dt, 16, this.size);	
            physics.Diffuse(2, this.py, this.y, this.visc, this.dt, 16, this.size);	

            physics.Project(px, this.py, this.x, this.y, 16, this.size);
	
            physics.Advect(1, this.x, this.px, this.px, this.py, dt, size);
            physics.Advect(2, this.y, this.py, px, py, dt, size);

            physics.Project(this.x, this.y, px, py, 16, size);

            physics.Diffuse(0, previousDensity, density, diff, dt, 16, size);	
            physics.Advect(0, density, previousDensity, x, y, dt, size);
        }

        public SFML.Graphics.Color Hsv(int hue, float sat, float val, float d)
        {
            hue %= 360;
            while(hue<0) hue += 360;

            if(sat<0.0f) sat = 0.0f;
            if(sat>1.0f) sat = 1.0f;

            if(val<0.0f) val = 0.0f;
            if(val>1.0f) val = 1.0f;

            int h = hue/60;
            float f = (hue)/60-h;
            float p = val*(1.0f-sat);
            float q = val*(1.0f-sat*f);
            float t = val*(1.0f-sat*(1-f));

            switch(h) {
                default:
                case 0:
                case 6: return new SFML.Graphics.Color((byte) (val*255), (byte) (t*255), (byte) (p*255), (byte) d);
                case 1: return new SFML.Graphics.Color((byte) (q*255), (byte) (val*255), (byte) (p*255), (byte) d);
                case 2: return new SFML.Graphics.Color((byte) (p*255), (byte) (val*255), (byte) (t*255), (byte) d);
                case 3: return new SFML.Graphics.Color((byte) (p*255), (byte) (q*255), (byte) (val*255), (byte) d);
                case 4: return new SFML.Graphics.Color((byte) (t*255), (byte) (p*255), (byte) (val*255), (byte) d);
                case 5: return new SFML.Graphics.Color((byte) (val*255), (byte) (p*255), (byte) (q*255), (byte) d);
            }
        }

        public void Render(ref RenderWindow win, FluidSim.Color color)
        {
            win.Clear();
            
            for (int i = 0; i < size; i++) {
                for(int j = 0; j < size; j++)
                {
                    RectangleShape rect = new RectangleShape();
                    rect.Size = new Vector2f(SCALE, SCALE);
                    rect.Position = new Vector2f(j * SCALE, i * SCALE);
			
                    switch (color) {
                        case Color.Default:
                            rect.FillColor = new SFML.Graphics.Color(255, 255, 255, (byte) ((density[IX._IX(i,j,size)] > 255) ? 255 : density[IX._IX(i,j,size)]));
                            break;
                        case Color.Hsb:
                            rect.FillColor = new SFML.Graphics.Color(Hsv((int) (density[IX._IX(i,j,size)]), 1, 1, 255));
                            break;
                        case Color.Velocity: {
                            int r = (int)MapToRange(x[IX._IX(i,j,size)], -0.05f, 0.05f, 0, 255);
                            int g = (int)MapToRange(y[IX._IX(i,j,size)], -0.05f, 0.05f, 0, 255);
                            rect.FillColor = new SFML.Graphics.Color((byte) r, (byte) g, 255);
                            break;
                        }
                        default:
                            break;
                    };	

                    win.Draw(rect);
                }
            }
        }

        public void FadeDensity(int _size)
        {
            for (int i = 0; i < _size; i++) {
                float d = this.density[i];
                density[i] = (d - 0.05f < 0) ? 0 : d - 0.05f; 
            }
        }

        public void Dispose()
        {
            //GC.SuppressFinalize(this);
        }
    }
}