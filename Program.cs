namespace VoxelTK
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game(800, 800))
            {
                game.Run();
            }
        }
    }
}