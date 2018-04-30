namespace Assets.Scripts.WorldGenerators
{
    public abstract class World_Generator
    {
        public abstract float GetOrigin();

        public abstract World.Vertex[] Generate_World(int width, int height);
    }
}
