namespace FluidSim
{
    public class Options
    {
        private Color color;

        public Options(Color color= Color.Default)
        {
            this.color = color;
        }

        public Color GetColor()
        {
            return this.color;
        }
        
        public void SetColor(Color c)
        {
            this.color = c;
        }
    }
}