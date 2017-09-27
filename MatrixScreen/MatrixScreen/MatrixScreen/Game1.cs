using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MatrixScreen
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch _spriteBatch;

        public bool IsFullScreen;
        public bool IsMenu;
        public bool IsMessage;
        public bool IsColumns = true;
        public bool IsRows;
        public string LastMessage;
        public string Characters = "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        public string DebugMessage = "";
        public double Speed = 1;
        public int Columns;
        public int Rows;
        public int TickCount;
        public int FontSize = 14;
        public int TimeOnScreen;
        public int Alpha = 5;
        public int LastAlpha;
        public int TextAlpha = 255;
        public double[] DropsCols;
        public double[] DropsRows;
        public float Scale = 1.0f;
        public Random Rnd = new Random();
        public Color TextColor = Color.Green;
        public SpriteFont Font;
        public KeyboardState LastKeyState;
        public StringBuilder MenuItems = new StringBuilder();
        public Vector2 DropPosition;
        public Vector2 ZeroPosition = Vector2.Zero;
        public List<Vector2> Trail = new List<Vector2>();
        public Texture2D BlankTexture;
        public RenderTarget2D RenderTarget;
        public Rectangle View;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IntPtr hWnd = Window.Handle;
            var control = System.Windows.Forms.Control.FromHandle(hWnd); 
            var form = control.FindForm();
            if (form == null) return;
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Font = Content.Load<SpriteFont>("SpriteFont1");
            LoadMenuItems();
            Columns = graphics.PreferredBackBufferWidth / FontSize;
            Rows = graphics.PreferredBackBufferHeight / FontSize;
            DropsCols = new double[Columns];
            DropsRows = new double[Rows];
            MakeInitialDropsValues(DropsCols, graphics.PreferredBackBufferHeight);
            MakeInitialDropsValues(DropsRows, graphics.PreferredBackBufferWidth);
            //BlankTexture = MakeTexture(1, 1, Color.FromNonPremultiplied(0, 0, 0, Alpha));
            View = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            RenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
            // TODO: use this.Content to load your game content here
        }

        private void MakeInitialDropsValues(double[] drops, int length)
        {
            //if (Speed > 0)
            //{
                for (int x = 0, len = drops.Length; x < len; x++)
                {
                    drops[x] = 0;
                }
            //}
            //if (Speed < 0)
            //{
            //    for (int x = 0, len = drops.Length; x < len; x++)
            //    {
            //        drops[x] = length/FontSize;
            //    }
            //}
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            GC.Collect();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // TODO: Add your update logic here

            
            
            UpdateInput();
            if (Alpha != LastAlpha)
            {
                BlankTexture = null;
                BlankTexture = MakeTexture(1, 1, Color.FromNonPremultiplied(0, 0, 0, Alpha));
                LastAlpha = Alpha;
            }
            
            if (TickCount == 240)
            {
                TextColor = Color.FromNonPremultiplied(Rnd.Next(0, 255), Rnd.Next(0, 255), Rnd.Next(0, 255), TextAlpha);
                TickCount = 0;
            }
            TickCount++;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.Transparent);
            // TODO: Add your drawing code here
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.SetRenderTarget(RenderTarget);
            _spriteBatch.Begin();

            _spriteBatch.Draw(BlankTexture, View ,Color.White);
            if (IsColumns)
                DrawMatrix(DropsCols, graphics.PreferredBackBufferHeight, "COLS");
            if(IsRows)
                DrawMatrix(DropsRows, graphics.PreferredBackBufferWidth, "ROWS");
                
            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _spriteBatch.Draw(RenderTarget, View,Color.White);
            //Draw Menu
            DrawMenu();
            DrawMessage(DebugMessage);
            _spriteBatch.End();
            base.Draw(gameTime);
            GC.Collect();
        }

        private void UpdateInput()
        {
            KeyboardState newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.F1) && LastKeyState.IsKeyUp(Keys.F1))
            {
                IsMenu = !IsMenu;
            }
            if (newState.IsKeyDown(Keys.Up))
            {
                Scale=Scale + .01f;
                IsMessage = true;
                DebugMessage = String.Format("Scale:{0}", Scale);
            }
            if (newState.IsKeyDown(Keys.Down))
            {
                Scale = Scale - .01f;
                IsMessage = true;
                DebugMessage = String.Format("Scale:{0}", Scale);
            }
            if (newState.IsKeyDown(Keys.Left))
            {
                FontSize = FontSize - 1;
                IsMessage = true;
                DebugMessage = String.Format("Font Size:{0}", FontSize);
            }
            if (newState.IsKeyDown(Keys.Right))
            {
                FontSize = FontSize + 1;
                IsMessage = true;
                DebugMessage = String.Format("Font Size:{0}", FontSize);
            }
            if (newState.IsKeyDown(Keys.F12) && LastKeyState.IsKeyUp(Keys.F12))
            {
                IsFullScreen = !IsFullScreen;
                graphics.IsFullScreen = IsFullScreen;
                graphics.ApplyChanges();
            }
            if (newState.IsKeyDown(Keys.Escape) && LastKeyState.IsKeyUp(Keys.Escape))
            {
                Exit();
            }
            if (newState.IsKeyDown(Keys.Q) && newState.IsKeyDown(Keys.U) && newState.IsKeyDown(Keys.I) &&
                newState.IsKeyDown(Keys.T))
            {
                Exit();
            }
            if (newState.IsKeyDown(Keys.Subtract))
            {
                Speed = Speed - 0.01;
                IsMessage = true;
                DebugMessage = String.Format("Speed:{0}", Speed);
            }
            if (newState.IsKeyDown(Keys.Add))
            {
                Speed = Speed + 0.01;
                IsMessage = true;
                DebugMessage = String.Format("Speed:{0}", Speed);
            }
            if (newState.IsKeyDown(Keys.NumPad8))
            {
                Alpha = Alpha + 1;
                IsMessage = true;
                DebugMessage = String.Format("Alpha:{0}", Alpha);
            }
            if (newState.IsKeyDown(Keys.NumPad2))
            {
                Alpha = Alpha - 1;
                IsMessage = true;
                DebugMessage = String.Format("Alpha:{0}", Alpha);
            }
            if (newState.IsKeyDown(Keys.D1) && LastKeyState.IsKeyUp(Keys.D1))
            {
                IsRows = !IsRows;
                IsMessage = true;
                DebugMessage = String.Format("Columns {0}", IsColumns ? "ON" : "OFF");
            }
            if (newState.IsKeyDown(Keys.D2) && LastKeyState.IsKeyUp(Keys.D2))
            {
                IsColumns = !IsColumns;
                IsMessage = true;
                DebugMessage = String.Format("Rows {0}", IsRows ? "ON" : "OFF");
            }
            
            if (newState.IsKeyDown(Keys.D3) && LastKeyState.IsKeyUp(Keys.D3))
            {
                Speed = -Speed;
                IsMessage = true;
                DebugMessage = String.Format("Reverse {0}", Speed > 0 ? "ON" : "OFF");
            }
            if (newState.IsKeyDown(Keys.D4) && LastKeyState.IsKeyUp(Keys.D4))
            {
                MakeInitialDropsValues(DropsCols, graphics.PreferredBackBufferHeight / FontSize);
                //Speed = Math.Abs(Speed);
                IsMessage = true;
                DebugMessage = "Reset Columns";
            }
            if (newState.IsKeyDown(Keys.D5) && LastKeyState.IsKeyUp(Keys.D5))
            {
                MakeInitialDropsValues(DropsRows, graphics.PreferredBackBufferHeight/FontSize);
                //Speed = Math.Abs(Speed);
                IsMessage = true;
                DebugMessage = "Reset Rows";
            }
            if (newState.IsKeyDown(Keys.D6) && LastKeyState.IsKeyUp(Keys.D6))
            {
                MakeInitialDropsValues(DropsCols, graphics.PreferredBackBufferHeight/ FontSize);
                MakeInitialDropsValues(DropsRows, graphics.PreferredBackBufferWidth/ FontSize);
                //Speed = Math.Abs(Speed);
                IsMessage = true;
                DebugMessage = "Reset Both";
            }
            LastKeyState = newState;
        }

        public Texture2D MakeTexture(int width,int height, Color color)
        {
            
            Texture2D texture = new Texture2D(GraphicsDevice, width, height);
            Color[] colorArray = new Color[width / height];
            for (int i = 0; i < colorArray.Length; i++)
            {
                colorArray[i] = color;
            }
            texture.SetData(colorArray);
            return texture;
        }

        /// <summary>
        /// Draw Matrix Design
        /// </summary>
        /// <param name="drops"></param>
        /// <param name="length"></param>
        /// <param name="option">Accepted options: COLS, ROWS. Default is COLS</param>
        private void DrawMatrix(double[] drops, int length, string option)
        {
            for (int i = 0, len = drops.Length; i < len; i++)
            {
                int p = Rnd.Next(0, Characters.Length);
                char text = Characters[p];
                switch (option.ToUpper())
                {
                    case "COLS":
                        DropPosition.X = i * FontSize;
                        DropPosition.Y = (float)(drops[i] * FontSize);
                        break;
                    case "ROWS":
                        DropPosition.X = (float)(drops[i] * FontSize);
                        DropPosition.Y = i * FontSize;
                        break;
                    default:
                        DropPosition.X = i * FontSize;
                        DropPosition.Y = (float)(drops[i] * FontSize);
                        break;
                }

                _spriteBatch.DrawString(Font, text.ToString(), DropPosition, TextColor, 0, ZeroPosition, Scale,
                    SpriteEffects.None, 0.0f);

                if (drops[i] * FontSize > length && Rnd.NextDouble() > 0.975)
                {
                    drops[i] = 0;
                }

                if (drops[i] * FontSize < 0 && Rnd.NextDouble() > 0.975)
                {
                    drops[i] = length / FontSize;
                }

                drops[i] = drops[i] + Speed;
            }
        }

        public void DrawMenu()
        {
            if (IsMenu)
            {
                _spriteBatch.DrawString(Font, MenuItems, ZeroPosition, Color.White);   
            }
        }

        public double RandomIntFromInterval(int min,int max)
        {
            return Math.Floor(Rnd.NextDouble() * (max - min + 1) + min);
        }

        /// <summary>
        /// Draw Keyboard Menu IsMessage
        /// </summary>
        /// <param name="str">IsMessage</param>
        public void DrawMessage(string str)
        {
            if (IsMessage)
            {
                if (LastMessage == str)
                {
                    _spriteBatch.DrawString(Font, str, ZeroPosition, Color.White);
                    TimeOnScreen++;
                }
                else
                {
                    _spriteBatch.DrawString(Font, str, ZeroPosition, Color.White);
                    LastMessage = str;
                    TimeOnScreen = 0;
                }
            }

            if (TimeOnScreen > 60)
            {
                TimeOnScreen = 0;
                DebugMessage = "";
                IsMessage = !IsMessage;
            }
        }

        /// <summary>
        /// Load in Menu Options
        /// </summary>
        public void LoadMenuItems()
        {
            MenuItems.AppendLine("F1: Help Menu");
            MenuItems.AppendLine("F12: FullScreen");
            MenuItems.AppendLine("Esc: Close");
            MenuItems.AppendLine("+/-: Speed");
            MenuItems.AppendLine("Up/Down: Scale");
            MenuItems.AppendLine("Left/Right: Font Size");
            MenuItems.AppendLine("Num8/Num2: Alpha");
            MenuItems.AppendLine("0: All");
            MenuItems.AppendLine("1: Normal");
            MenuItems.AppendLine("2: Cross Stich");
            MenuItems.AppendLine("3: Reverse");
            MenuItems.AppendLine("4: Reset Rows");
            MenuItems.AppendLine("5: Reset Columns");
            MenuItems.AppendLine("6: Reset Both");
        }
    }
}
