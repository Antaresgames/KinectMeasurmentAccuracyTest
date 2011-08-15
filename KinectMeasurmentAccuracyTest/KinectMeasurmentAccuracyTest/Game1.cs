using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;


namespace KinectMeasurmentAccuracyTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Kinect kinect;
        Vector3[,] redLinePoints;

        Texture2D yellowDot;
        Camera2 camera;

        KeyboardState prevState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1300;
            Content.RootDirectory = "Content";
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            camera = new Camera2(this, Vector3.Zero, 1.0f);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            kinect = new Kinect(GraphicsDevice);
            yellowDot = Content.Load<Texture2D>("yellowDot");
            // TODO: use this.Content to load your game content here
        }

            

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            kinect.Uninitalize();
            // TODO: Unload any non ContentManager content here
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
                this.Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                kinect.LineUp();
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                kinect.LineDown();
            if (Keyboard.GetState().IsKeyDown(Keys.R))
                kinect.CameraReset();
            if (Keyboard.GetState().IsKeyDown(Keys.C) && !Keyboard.GetState().Equals(prevState))
            {
                Components.Clear();
                redLinePoints = kinect.Capture();
                foreach (Vector3 point in redLinePoints)
                {
                    if (point.Y < 1700 && point.Y != 0)
                    {
                        camera.LookAt(point);
                        Components.Add(new GameComponent1(this, point, camera));
                    }
                }
            }
            camera.Update(gameTime);
            // TODO: Add your update logic here
            prevState = Keyboard.GetState();
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            spriteBatch.Begin();
            if (kinect.GetImage() != null && kinect.GetLastDepth() == null)
            {
                spriteBatch.Draw(kinect.GetImage(), new Microsoft.Xna.Framework.Rectangle(0, 0, 640, 480), Microsoft.Xna.Framework.Color.White);
                spriteBatch.Draw(kinect.GetDepth(), new Microsoft.Xna.Framework.Rectangle(640, 0, 320, 240), Microsoft.Xna.Framework.Color.White);
                
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
