namespace Assets.Scripts.WorldGenerators
{
    public abstract class World_Generator
    {
        public abstract World.Vertex Generate_Vertex(int x, int z);
    }
}
