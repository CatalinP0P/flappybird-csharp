using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Flappy_Bird_Like_Game
{
    public class GameState
    {
        public int Score { get; set; }
        public bool GameRunning { get; set; }

        public GameState()
        {
            Score = 0;
            GameRunning = true;

        }



        public void AddObstacle()
        {
            Random rand = new Random();
            
        }





    }
}
