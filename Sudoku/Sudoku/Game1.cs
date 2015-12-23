using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sudoku
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics { get;set; }
        SpriteBatch spriteBatch { get; set; }
        SpriteFont font1 { get; set; }

        KeyboardState oldKeyboard;
        MouseState oldMouse;

        private Texture2D seperator2D { get; set; }
        private Rectangle seperatorVert1 { get; set; }
        private Rectangle seperatorVert2 { get; set; }
        private Rectangle seperatorHoriz1 { get; set; }
        private Rectangle seperatorHoriz2 { get; set; }

        private String puzzle { get; set; }
        private Boolean isntGuessing { get; set; }

        public int this[int row , int column]
        {
           get
           {
              return Common.nums[FindIndex(row , column)];
           }
           set
           {
              Common.nums[FindIndex(row , column)]=value;
           }
        }

        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            this.graphics.PreferredBackBufferHeight = 440;
            this.graphics.PreferredBackBufferWidth = 440;
            graphics.ApplyChanges();

            seperatorVert1 = new Rectangle(137,0,10,440);
            seperatorVert2 = new Rectangle(287,0,10,440);
            seperatorHoriz1 = new Rectangle(0,138,440,10);
            seperatorHoriz2 = new Rectangle(0,287,440,10);

            for (int x = 0; x < 81; x++)
            {
                Common.nums[x] = 0;
            }


            String path = "...\\Debug\\Content\\SudokuPrimeArray.txt";
            StringBuilder sb = new StringBuilder();
            TextReader tr = new StreamReader(@path);
            char[] buffer = new char[81];
            tr.Read(buffer, 0, 81);
            for (int x = 0; x < 81; x++)
            {
                puzzle = String.Concat(puzzle, buffer[x]);
                if (buffer[x] != '0')
                {
                   Common.numsBackup[x]= Int32.Parse(buffer[x].ToString());
                   Common.isConfirmedBackup[x]=true;
                }

            }
            tr.Close();

            

            base.IsMouseVisible = true;
            Common.drawHeight = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            font1 = Content.Load<SpriteFont>("SpriteFont1");
            seperator2D = Content.Load<Texture2D>("35x35 box");
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        bool IsMouseInsideWindow()
        {
            MouseState ms = Mouse.GetState();
            Point pos = new Point(ms.X, ms.Y);
            return GraphicsDevice.Viewport.Bounds.Contains(pos);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();
            
            doUserControls(keyboard,mouse, gameTime);

            oldMouse = mouse;
            oldKeyboard = keyboard;
            base.Update(gameTime);
        }

        private void doUserControls(KeyboardState keyboard, MouseState mouse, GameTime gameTime)
        {
            if (keyboard.IsKeyDown(Keys.Escape))
                this.Exit();

            int x = (Mouse.GetState().X - (Mouse.GetState().X % 50)) / 50;
            int y = (Mouse.GetState().Y - (Mouse.GetState().Y % 50)) / 50 * 9;

            if (IsMouseInsideWindow())
            {
                if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton != ButtonState.Pressed)
                {
                    if (Common.nums[y + x] == 9)
                    {
                        Common.nums[y + x] = 1;
                        Common.isConfirmed[y + x] = false;
                    }
                    else
                    {
                        if (keyboard.IsKeyDown(Keys.LeftShift))
                        {
                            if (Common.nums[y + x] == 8)
                            {
                                Common.nums[y + x] = 1;
                                Common.isConfirmed[y + x] = false;
                            }
                            else
                            {
                                Common.nums[y + x] += 2;
                                Common.isConfirmed[y + x] = false;
                            }
                        }
                        else
                        {
                            Common.nums[y + x] += 1;
                            Common.isConfirmed[y + x] = false;
                        }
                    }
                }
                else if (mouse.RightButton == ButtonState.Pressed && oldMouse.RightButton != ButtonState.Pressed)
                {
                    if (Common.nums[y + x] == 1 || Common.nums[y + x] == 0)
                    {
                        Common.nums[y + x] = 9;
                        Common.isConfirmed[y + x] = false;
                    }
                    else
                    {

                        if (keyboard.IsKeyDown(Keys.LeftShift))
                        {

                            if (Common.nums[y + x] == 2)
                            {
                                Common.nums[y + x] = 9;
                                Common.isConfirmed[y + x] = false;
                            }
                            else
                            {
                                Common.nums[y + x] -= 2;
                                Common.isConfirmed[y + x] = false;
                            }

                        }
                        else
                        {
                            Common.nums[y + x] -= 1;
                            Common.isConfirmed[y + x] = false;
                        }
                    }

                }
                else if (keyboard.IsKeyDown(Keys.Delete))
                {
                    Common.nums[y + x] = 0;
                    Common.isConfirmed[y + x] = false;

                }
                else if (keyboard.IsKeyDown(Keys.D1) || keyboard.IsKeyDown(Keys.D2) || keyboard.IsKeyDown(Keys.D3) ||
                    keyboard.IsKeyDown(Keys.D4) || keyboard.IsKeyDown(Keys.D5) || keyboard.IsKeyDown(Keys.D6) ||
                    keyboard.IsKeyDown(Keys.D7) || keyboard.IsKeyDown(Keys.D8) || keyboard.IsKeyDown(Keys.D9))
                {

                    if (keyboard.GetPressedKeys().First().ToString().Length == 2)
                    {
                        Common.nums[y + x] = Convert.ToInt32(keyboard.GetPressedKeys().First().ToString().Substring(1, 1));
                        Common.isConfirmed[y + x] = false;
                    }

                }
                else if (keyboard.IsKeyDown(Keys.I) && oldKeyboard.IsKeyUp(Keys.I))
                {

                    for (int z = 0; z < 81; z++)
                    {
                        int a = 0;
                        Int32.TryParse(puzzle.Substring(z, 1), out a);
                        Common.nums[z] = a;
                        Common.isConfirmed[z] = true;
                    }
                    
                    updateBackup();
                    isntGuessing=true;
                }
                else if ( keyboard.IsKeyDown(Keys.L))
                {
                   for ( int z=0 ; z<81 ; z++ )
                   {
                      Common.nums[z]=Common.numsBackup[z];
                      Common.isConfirmed[z] = Common.isConfirmedBackup[z];
                   }
                }
                else if ( keyboard.IsKeyDown(Keys.R))
                {
                   for ( int z=0 ; z<81 ; z++ )
                   {
                      Common.nums[z]=0;
                      Common.isConfirmed[z]=false;
                   }
                }
                else if (keyboard.IsKeyDown(Keys.S) && oldKeyboard.IsKeyUp(Keys.S))
                {
                   updateBackup();
                }
                else if (keyboard.IsKeyDown(Keys.C) && oldKeyboard.IsKeyUp(Keys.C))
                {
                    Common.isConfirmed[y + x] = !Common.isConfirmed[y + x];
                }
                else if (keyboard.IsKeyDown(Keys.Enter))
                {
                   updateBackup();
                    bool isSolved = Solve(false);
                     if (! isSolved )
                     {
                        for ( int a=0 ; a<81 ; a++ )
                        {
                           Common.nums[a]=Common.numsBackup[a];
                           Common.isConfirmed[a]=Common.isConfirmedBackup[a];
                        }
                        isSolved=Solve(true);
                     }
                        
                       
                }
            
            } 
        }
     
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(seperator2D, seperatorHoriz1, Color.White);
            spriteBatch.Draw(seperator2D, seperatorHoriz2, Color.White);
            spriteBatch.Draw(seperator2D, seperatorVert1, Color.White);
            spriteBatch.Draw(seperator2D, seperatorVert2, Color.White);

            for (int x = 1; x <= 9; x++)
            {
                for (int y = 1; y <= 9; y++)
                {
                    if (Common.nums[FindIndex(x, y)] == 0)
                    {
                        spriteBatch.Draw(seperator2D, new Vector2((y * 50) - 50, (x * 50) - 50), Color.Red);
                    }
                    else
                    {
                        if (Common.isConfirmed[FindIndex(x, y)])
                        {
                            spriteBatch.Draw(seperator2D, new Vector2((y * 50) - 50, (x * 50) - 50), Color.Green);
                        }
                        else
                        {
                            spriteBatch.Draw(seperator2D, new Vector2((y * 50) - 50, (x * 50) - 50), Color.Orange);
                        }
                        spriteBatch.DrawString(font1, Common.nums[FindIndex(x, y)] + "", new Vector2((y * 50) - 40, (x * 50) - 50), Color.Black);
                    }
                }

                Common.drawHeight += 80;
            }
            
            spriteBatch.End();
                base.Draw(gameTime);
        }



        private int FindIndex(int x, int y)
        {
            return (((x- 1) * 9) + y - 1);
        }


         public bool Solve(bool lastSolveFailed)
         {

            for (int y = 1; y <= 9; y++)
            {
               for (int x = 1; x <= 9; x++)
               {
                     if (TrySolving(x, y, false))
                     {
                        // restart
                        x = 1;
                        y = 1;
                     }
               }
            }

            for (int y = 1; y <= 9; y++)
            {
               for (int x = 1; x <= 9; x++)
               {
                  if (Common.nums[FindIndex(x, y)] == 0)
                  {
                     bool failed = false;

                     if ( TrySolving(x , y , false) )
                     {
                        for (int y2 = 1; y2 <= 9; y2++)
                        {
                           for (int x2 = 1; x2 <= 9; x2++)
                           {
                              if (TrySolving(x2, y2, false))
                              {
                                 // restart
                                 x2 = 1;
                                 y2 = 1;
                              }
                              else if (Common.nums[FindIndex(x2, y2)] == 0)
                              {
                                    
                                 failed = true;
                                 break;
                              }
                           }
                           if (failed)
                           {
                              break;
                           }
                        }

                        

                        //restart;
                        x = 1;
                        y = 1;
                     }

                  }


               }
            }

            Boolean isSolved=true;
            for ( int x=1 ; x<=9 ; x++ )
            {
               for ( int y=1 ; y<=9 ; y++ )
               {
                  if ( Common.nums[FindIndex(x , y)]==0 )
                  {
                     isSolved=false;
                  }
               }
            }

            if ( isSolved )
            {
               for ( int x=1 ; x<=9 ; x++ )
               {
                  for ( int y=1 ; y<=9 ; y++ )
                  {
                     Common.isConfirmed[FindIndex(x , y)]=true;
                  }
               }
               return true;
            }
            return false;
         }
 
         private bool TrySolving(int x, int y, bool lastSolveFailed)
         {
            List<Cell> possibleValuesFound = new List<Cell>();
            if (this[x, y] == 0)
            {
               for (int possiblevalues = 1; possiblevalues <= 9; possiblevalues++)
               {
                     if (!checkRow(possiblevalues, x, y) && !checkColumn(possiblevalues, x, y) && !checkSquare(possiblevalues, x, y))
                     {
                        possibleValuesFound.Add(new Cell(x, y, possiblevalues));
                     }
               }
 
               if (possibleValuesFound.Count() == 1)
               {

                     this[possibleValuesFound[0].getX(), possibleValuesFound[0].getY()] = possibleValuesFound[0].getNum();
                     Common.isConfirmed[FindIndex(x, y)] = isntGuessing;
                     return true;
               }
               else if (lastSolveFailed && possibleValuesFound.Count()<=3 && possibleValuesFound.Count()>1)
               {

                  this[possibleValuesFound[0].getX() , possibleValuesFound[0].getY()] = possibleValuesFound[new Random().Next(possibleValuesFound.Count())].getNum();

                  isntGuessing=false;
                  Common.isConfirmed[FindIndex(x , y)]= isntGuessing;
                  return true;
               }
 
            }
 
            return false;
         }
 


         private bool checkRow(int num, int x, int y)
         {
            for (int yIndex = 1; yIndex <= 9; yIndex++)
            {
               if ((this[x, yIndex] == num) & y != yIndex)
               {
                     return true;
               }
            }
            return false;
         }
 
         private bool checkColumn(int num, int x, int y)
         {
            for (int xIndex = 1; xIndex <= 9; xIndex++)
            {
               if ((this[xIndex, y] == num) & x != xIndex)
               {
                     return true;
               }
            }
            return false;
         }
 
         private bool checkSquare(int num, int x, int y)
         {
            int xStart = ((x - 1) / 3) + 1;
            int yStart = ((y - 1) / 3) + 1;
 
            int xIndexEnd = xStart * 3;
            if (xIndexEnd == 0) xIndexEnd = 3;
            int xIndexStart = xIndexEnd - 2;
 
            int yIndexEnd = yStart * 3;
            if (yIndexEnd == 0) yIndexEnd = 3;
            int yIndexStart = yIndexEnd - 2;
 
            for (int xIndex = xIndexStart; xIndex <= xIndexEnd; xIndex++)
            {
               for (int yIndex = yIndexStart; yIndex <= yIndexEnd; yIndex++)
               {
                     if ((this[xIndex, yIndex] == num) & (yIndex != y) & (xIndex != x))
                     {
                        return true;
                     }
               }
            }
            return false;
         }

       private void updateBackup(){
          for ( int x=0 ; x<81 ; x++ )
          {
             Common.numsBackup[x]=Common.nums[x];
             Common.isConfirmedBackup[x]=Common.isConfirmed[x];
          }
       }
    }


    public class Cell
    {
        private int x;
        private int y;
        private int num;

        public Cell(int r, int c, int n)
        {
            x = r;
            y = c;
            num = n;
        }

        public int getX()
        {
            return x;
        }

        public int getY()
        {
            return y;
        }

        public int getNum()
        {
            return num;
        }

    }
}
