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
        Vector2[] redLinePoints;

        Texture2D yellowDot;

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
            // TODO: Add your initialization logic here
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
                redLinePoints = kinect.Capture();

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
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            spriteBatch.Begin();
            if (kinect.GetImage() != null)
            {
                spriteBatch.Draw(kinect.GetImage(), new Microsoft.Xna.Framework.Rectangle(0, 0, 640, 480), Microsoft.Xna.Framework.Color.White);
                spriteBatch.Draw(kinect.GetDepth(), new Microsoft.Xna.Framework.Rectangle(640, 0, 320, 240), Microsoft.Xna.Framework.Color.White);
                if (kinect.GetLastDepth() != null)
                {
                    spriteBatch.Draw(kinect.GetLastDepth(), new Microsoft.Xna.Framework.Rectangle(640, 240, 320, 240), Microsoft.Xna.Framework.Color.White);
                    spriteBatch.Draw(yellowDot, new Microsoft.Xna.Framework.Rectangle(0,480, 900, 480), Microsoft.Xna.Framework.Color.DarkSlateBlue);
                    foreach (Vector2 point in redLinePoints)
                    {
                        spriteBatch.Draw(yellowDot, new Microsoft.Xna.Framework.Rectangle((int)(point.Y),(int)(480+point.X), 2, 2), Microsoft.Xna.Framework.Color.White);
                    }
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
