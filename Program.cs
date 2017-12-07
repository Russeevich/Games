using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Threading;
using System.IO;

namespace Brickz
{
    class Program
    {
        
        static int tempX = 0, tempY = 0, ScoreFill = 0, ScoreFills=0,Score=0,level=1;
        static bool stop = false;
        static float distance = 0, downy = 192,timey=0;
        static Vector2f Mpos;
        static Player game = new Player();
        static Color[] clr = new Color[5];
        static int[] randoms = new int[5];
        static RenderWindow app;
        static uint h=400, w=160,lengthy=h/32+h/100+2,lengthx=w/32;
        static RectangleShape[,] shape = new RectangleShape[lengthx, lengthy];


        static void Main(string[] args)
        {
            app = new RenderWindow(new VideoMode(w, h), "Bricks");
            app.Closed += new EventHandler(OnClose);
            app.MouseMoved += App_MouseMoved;

            CreateColor();
                for (int j = 7; j < lengthy; j++)
                {
                int r = 0;
                for (int i = 0; i < lengthx; i++)
                    {

                    var rand = new Random(DateTime.Now.Millisecond);
                    shape[i, j] = new RectangleShape(new Vector2f(32, 32));
                    shape[i, j].Position = new Vector2f(i * 32, j * 32);

                    if(r==0)
                    { 
                        STEP:
                        for (int sear = 0; sear < 5; sear++)
                        {
                            randoms[sear] = rand.Next(0, 5);
                        }
                        for (int sear = 0; sear < 5; sear++)
                        {
                            for (int searx = 0; searx < 5; searx++)
                            {
                                if (sear != searx)
                                    if (randoms[sear] == randoms[searx])
                                        goto STEP;
                            }
                        }
                    }
                    Thread.Sleep(1);
                    shape[i, j].FillColor = clr[randoms[r]];
                    if (ScoreFill == 0)
                        ScoreFill = rand.Next(5, 11);
                    if (r < 4)
                        r++;
                    else r = 0;
                }
            }

            Color windowColor = new Color(0, 0, 0);
            View views = new View();
            views.Size = new Vector2f(w, h);

            Clock clock = new Clock();

            var rands = new Random(DateTime.Now.Millisecond);

            game.Create(lengthx, 192, clr[rands.Next(0,5)]);
            Font f = new Font("arial.ttf");
            Text text = new Text("0", f, 16);

            while (app.IsOpen)
            {
                app.DispatchEvents();
                if (!stop)
                {
                    views.Center = new Vector2f(w / 2, downy);
                    app.SetView(views);
                    float time = clock.ElapsedTime.AsMicroseconds();

                    clock.Restart();
                    time = time / 800;

                    Vector2i pixelPos = Mouse.GetPosition(app);
                    Mpos = app.MapPixelToCoords(pixelPos);

                    app.Clear(windowColor);

                    app.Draw(game.player);
                        for (int j = 7; j < lengthy; j++)
                            for (int i = 0; i < lengthx; i++)
                            app.Draw(shape[i, j]);

                    if (shape[(int)(game.player.Position.X / 32), 7].FillColor == game.player.FillColor && game.player.Position.X >= shape[(int)(game.player.Position.X / 32), 7].Position.X && game.player.Position.X <= shape[(int)(game.player.Position.X / 32), 7].Position.X + 16 && game.player.Position.Y >= 192)
                    {
                        ScoreFills++;
                        Score++;
                        if(downy > 90)
                            downy -= 7 * ScoreFills;
                        game.player.Position = new Vector2f(game.player.Position.X, game.player.Position.Y - 14);
                        game.isGround = false;
                        for (int j = 7; j < lengthy - 1; j++)
                        {
                            int r =0;
                                for (int i = 0; i < lengthx; i++)
                                {
                                    shape[i, j].FillColor = shape[i, j + 1].FillColor;
                                    var rand = new Random(DateTime.Now.Millisecond);
                                    if (r == 0)
                                    {
                                        STEP:
                                        for (int sear = 0; sear < 5; sear++)
                                        {
                                            randoms[sear] = rand.Next(0, 5);
                                        }
                                        for (int sear = 0; sear < 5; sear++)
                                        {
                                            for (int searx = 0; searx < 5; searx++)
                                            {
                                                if (sear != searx)
                                                    if (randoms[sear] == randoms[searx])
                                                        goto STEP;
                                            }
                                        }
                                    }
                                shape[i, lengthy - 1].FillColor = clr[randoms[r]];
                                    if (r < 4)
                                        r++;
                                    else r = 0;
                            }
                        }
                        if (ScoreFill == ScoreFills)
                        {
                            level++;
                            STEP:
                                var rand = new Random(DateTime.Now.Millisecond);
                                ScoreFill = rand.Next(5, 11);
                                ScoreFills = 0;
                            var x = rand.Next(0, 5);
                            if (game.player.FillColor == clr[x])
                                goto STEP;
                            game.player.FillColor = clr[x];
                        }
                    }

                    if (game.isMove)
                    {
                        distance = (float)Math.Sqrt((tempX - game.player.Position.X) * (tempX - game.player.Position.X) + (tempY - game.player.Position.Y) * (tempY - game.player.Position.Y));

                        if (distance > 2)
                        {
                            float x = game.player.Position.X, y = game.player.Position.Y;
                            x += (float)0.32 * time * (tempX - x) / distance;
                            game.player.Position = new Vector2f(x, y);
                        }
                        else { game.isMove = false; }
                    }
                    text.Position = new Vector2f(game.player.Position.X + 9, game.player.Position.Y);
                    text.FillColor = new Color(0, 0, 0);
                    text.DisplayedString = Score.ToString();
                    app.Draw(text);

                    if(!game.isGround)
                        game.player.Position = new Vector2f(game.player.Position.X, game.player.Position.Y+0.08f);

                    if (game.player.Position.Y >= 192)
                        game.isGround = true;

                    if(timey < 0.6f)
                        timey += time / 150;

                    downy += 0.07f * 1.5f * timey;

                    Console.WriteLine(downy);
                    if (downy-h > -150)
                    {
                        if (!File.Exists("records.txt"))
                            File.Create("records.txt");
                        FileStream file = new FileStream("records.txt", FileMode.Open,FileAccess.ReadWrite);
                        StreamReader reader = new StreamReader(file);
                        int Best = Convert.ToInt16(reader.ReadLine());
                        reader.Close();
                        file.Close();

                        if (Best < Score)
                        {
                            File.WriteAllText("records.txt", Score.ToString());
                            Console.WriteLine("GameOver!");
                            Console.WriteLine("NewScore:" + Score);
                            Console.WriteLine("OldScore:" + Best);

                        }
                        else
                        {
                            Console.WriteLine("GameOver!");
                            Console.WriteLine("BestScore:" + Best);
                            Console.WriteLine("Score:" + Score);
                        }

                        stop = true;
                    }
                }
                app.Display();
            }
        }

        private static void CreateColor()
        {
                clr[0] = new Color(255, 255, 0);//yellow
                clr[1] = new Color(128, 0, 128);//purple
                clr[2] = new Color(0, 128, 0);//green
                clr[3] = new Color(0, 0, 255);//blue
                clr[4] = new Color(128, 0, 0);//maroon
        }

        private static void App_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            game.isMove = true;
            tempX = (int)Mpos.X-16;
            tempY = (int)Mpos.Y;
        }

        private static void OnClose(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }
    }
}
