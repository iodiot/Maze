using System;

namespace Maze
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MazeGame game = new MazeGame())
            {
                game.Run();
            }
        }
    }
#endif
}

