namespace FluidSim
{
    public static class IX
    {
        public static int _IX(int x, int y, int N) {
            if (x < 0)
            {
                //return -1;
                x=0; 
                
            }

            if (x > N - 1)
            {
                //return -1;
                x=N-1;
            }
  
            if (y < 0) { y=0; }
            if (y > N-1) { y=N-1; }
  
            return (y * N) + x;
        }

    public static int Flatten2dTo1D(int x, int y, int rowSize) => _IX(x,y,rowSize);
    }
}