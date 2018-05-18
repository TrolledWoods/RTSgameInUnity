namespace Assets.Scripts.WorldGenerators
{
    public struct Generated_World
    {
        public World.Vertex[] Vertices;
        public World.TileData[] Tiles;
        public QuadTree Entities;
    }

    public abstract class World_Generator
    {
        public abstract float GetOrigin();

        public abstract Generated_World Generate_World(UnityEngine.GameObject entity_parent, int width, int height);
    }
}
