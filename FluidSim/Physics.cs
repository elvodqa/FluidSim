using System;

namespace FluidSim
{
    public class Physics
    {

        public Physics(){}
    

        public void Diffuse(int b, float[] x, float[] x0, float diff, float dt, int iter, int N)
        {
            float a = dt * diff * (N - 2) * (N - 2);
            LinSolve(b, x, x0, a, 1 + 6 * a, iter, N);	
        }
        
        public void Project(float[] vx, float[] vy, float[] p, float[] div, int iter, int N)
        {
            for (int j = 1; j < N - 1; j++) {
                for (int i = 1; i < N - 1; i++) {
                    div[IX._IX(i, j, N)] = -0.5f*(
                        vx[IX._IX(i+1, j, N)]
                        -vx[IX._IX(i-1, j, N)]
                        +vy[IX._IX(i, j+1, N)]
                        -vy[IX._IX(i, j-1, N)]
                    )/N;
                    p[IX._IX(i, j, N)] = 0;
                }
            }

            SetBnd(0, div, N); 
            SetBnd(0, p, N);
            LinSolve(0, p, div, 1, 6, iter, N);
    
            for (int j = 1; j < N - 1; j++) {
                for (int i = 1; i < N - 1; i++) {
                    vx[IX._IX(i, j, N)] -= 0.5f * (p[IX._IX(i+1, j, N)] - p[IX._IX(i-1, j, N)]) * N;
                    vy[IX._IX(i, j, N)] -= 0.5f * (p[IX._IX(i, j+1, N)] -p[IX._IX(i, j-1, N)]) * N;
                }
            }
            SetBnd(1, vx, N);
            SetBnd(2, vy, N);
        }

        public void Advect(int b, float[] d, float[] d0, float[] vx, float[] vy, float dt, int N)
        {
            float i0, i1, j0, j1;
    
            float dtx = dt * (N - 2);
            float dty = dt * (N - 2);
    
            float s0, s1, t0, t1;
            float tmp1, tmp2, x, y;
    
            float Nfloat = N;
            float ifloat, jfloat;

            int i, j;
    
            for(j = 1, jfloat = 1; j < N - 1; j++, jfloat++) { 
                for(i = 1, ifloat = 1; i < N - 1; i++, ifloat++) {
                    tmp1 = dtx * vx[IX._IX(i, j, N)];
                    tmp2 = dty * vy[IX._IX(i, j, N)];
                    x = ifloat - tmp1; 
                    y = jfloat - tmp2;
                
                    if(x < 0.5f) x = 0.5f; 
                    if(x > Nfloat + 0.5f) x = Nfloat + 0.5f;
                    i0 = (float) Math.Floor(x);
                    i1 = i0 + 1.0f;
                    if(y < 0.5f) y = 0.5f; 
                    if(y > Nfloat + 0.5f) y = Nfloat + 0.5f;
                    j0 = (float) Math.Floor(y);
                    j1 = j0 + 1.0f; 

                    s1 = x - i0; 
                    s0 = 1.0f - s1; 
                    t1 = y - j0; 
                    t0 = 1.0f - t1;
                
                    int i0i = (int) i0;
                    int i1i = (int) i1;
                    int j0i = (int) j0;
                    int j1i = (int) j1;
                
                    d[IX._IX(i, j, N)] = 
                        s0 * (t0 * d0[IX._IX(i0i, j0i, N)] + t1 * d0[IX._IX(i0i, j1i, N)]) +
                        s1 * (t0 * d0[IX._IX(i1i, j0i, N)] + t1 * d0[IX._IX(i1i, j1i, N)]);
                }
            }
            SetBnd(b, d, N);
        }

        private void LinSolve(int b, float[] x, float[] x0, float a, float c, int iter, int N)
        {
            float cRecip = 1.0f / c;
            for (int k = 0; k < iter; k++) {
                for (int j = 1; j < N - 1; j++) {
                    for (int i = 1; i < N - 1; i++) {
                        x[IX._IX(i, j, N)] = (x0[IX._IX(i, j, N)] + a 
                            *(x[IX._IX(i+1, j, N)]
                              +x[IX._IX(i-1, j, N)]
                              +x[IX._IX(i, j+1, N)]
                              +x[IX._IX(i, j-1, N)]
                              +x[IX._IX(i, j, N)]
                              +x[IX._IX(i, j, N)]
                            )) * cRecip;
                    }
                }
                SetBnd(b, x, N);
            }
        }

        private void SetBnd(int b, float[] x, int N)
        {
            for(int i = 1; i < N - 1; i++) {
                x[IX._IX(i, 0, N)] = b == 2 ? -x[IX._IX(i, 1, N)] : x[IX._IX(i, 1, N)];
                x[IX._IX(i, N-1, N)] = b == 2 ? -x[IX._IX(i, N-2, N)] : x[IX._IX(i, N-2, N)];
            }

            for(int j = 1; j < N - 1; j++) {
                x[IX._IX(0, j, N)] = b == 1 ? -x[IX._IX(1, j, N)] : x[IX._IX(1, j, N)];
                x[IX._IX(N-1, j, N)] = b == 1 ? -x[IX._IX(N-2, j, N)] : x[IX._IX(N-2, j, N)];
            }
    
            x[IX._IX(0, 0, N)] = 0.33f * (x[IX._IX(1, 0, N)]
                                          + x[IX._IX(0, 1, N)]
                                          + x[IX._IX(0, 0, N)]);
            x[IX._IX(0, N-1, N)] = 0.33f * (x[IX._IX(1, N-1, N)]
                                            + x[IX._IX(0, N-2, N)]
                                            + x[IX._IX(0, N-1, N)]);
            x[IX._IX(N-1, 0, N)] = 0.33f * (x[IX._IX(N-2, 0, N)]
                                            + x[IX._IX(N-1, 1, N)]
                                            + x[IX._IX(N-1, 0, N)]);
            x[IX._IX(N-1, N-1, N)] = 0.33f * (x[IX._IX(N-2, N-1, N)]
                                              + x[IX._IX(N-1, N-2, N)]
                                              + x[IX._IX(N-1, N-1, N)]);
        }
    }
}