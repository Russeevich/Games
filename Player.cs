using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace Brickz
{
    class Player
    {
        public bool isMove = false, isGround = true;
        public RectangleShape player = new RectangleShape(new Vector2f(32,32));
        public void Create(float posx, float posy, Color color)
        {
            var randPos = new Random(DateTime.Now.Second);
            player.Position = new Vector2f(randPos.Next(0,(int)posx)*32, posy);
            player.FillColor = color;
        }
    }
}
