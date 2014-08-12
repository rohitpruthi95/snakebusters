using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace WindowsPhoneGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont myFont;
        Texture2D myTextureRed;
        Texture2D myTextureGreen;
        Texture2D myTextureBlue;
        Texture2D myTextureWhite;
        Texture2D myTexturePurple;
        SoundEffect soundCrash, soundTurn, soundPerm;
        God myGod;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(500000);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
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
            TouchPanel.EnabledGestures = GestureType.HorizontalDrag | GestureType.VerticalDrag;
            myGod = new God();
            myGod.Appear(1);
            myGod.Appear(2);
            myGod.Appear(3);
            myGod.Appear(4);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //myFont = Content.Load<SpriteFont>("myFont");
            myTextureRed = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            myTextureRed.SetData<Color>(new Color[] { Color.Red });
            myTextureGreen = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            myTextureGreen.SetData<Color>(new Color[] { Color.Green });
            myTextureBlue = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            myTextureBlue.SetData<Color>(new Color[] { Color.Blue });
            myTextureWhite = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            myTextureWhite.SetData<Color>(new Color[] { Color.White });
            myTexturePurple = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            myTexturePurple.SetData<Color>(new Color[] { Color.Purple });

            soundCrash = Content.Load<SoundEffect>("crash");
            soundTurn = Content.Load<SoundEffect>("turn");
            soundPerm = Content.Load<SoundEffect>("perm");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
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
            // TODO: Add your update logic here
            for (int ii = 1; ii < Constants.NUM_SNAKES; ii++)
            {
                myGod.snakeBool[ii] = 0;
            }
            int direction = 0;
            int x, y;
            myGod.isCrash = false; myGod.isTurn = false; 
            if (TouchPanel.IsGestureAvailable)
            {
                GestureSample myGesture = TouchPanel.ReadGesture();
                if (myGesture.GestureType == GestureType.HorizontalDrag)
                {

                    if (myGesture.Delta.X < 0)
                    {
                        direction = 4;

                    }
                    else
                    {
                        direction = 2;

                    }
                }
                if (myGesture.GestureType == GestureType.VerticalDrag)
                {
                    if (myGesture.Delta.Y < 0)
                    {
                        direction = 1;

                    }
                    else
                    {
                        direction = 3;

                    }
                }
                x = (int)myGesture.Position.X / 16;
                y = (int)myGesture.Position.Y / 16;

                int[] xy = myGod.Smoothify(x, y);
                x = xy[0]; y = xy[1];

                if (myGod.MainMatrix[x, y].ishead)
                    myGod.snakeBool[myGod.MainMatrix[x, y].snakeid] = direction;

            }
            myGod.UpdateAll();
            //TouchCollection myTouchCollection = TouchPanel.GetState();
            //foreach (TouchLocation touch in myTouchCollection)
            //{
            //    if ((touch.State == TouchLocationState.Pressed) || (touch.State == TouchLocationState.Moved)) {
            //        matrix[(int) touch.Position.X/16, (int) touch.Position.Y/16] = 1; 
            //    }
            //}

            if (myGod.isCrash)
                soundCrash.Play();
            if (myGod.isTurn)
                soundTurn.Play();
            soundPerm.Play();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    switch (myGod.MainMatrix[i, j].snakeid)
                    {
                        case 1:
                            spriteBatch.Draw(myTextureGreen, new Rectangle(16 * i, 16 * j, 16, 16), Color.Green);
                            break;
                        case 2:
                            spriteBatch.Draw(myTextureBlue, new Rectangle(16 * i, 16 * j, 16, 16), Color.Blue);
                            break;
                        case 3:
                            spriteBatch.Draw(myTextureRed, new Rectangle(16 * i, 16 * j, 16, 16), Color.Red);
                            break;
                        case 4:
                            spriteBatch.Draw(myTexturePurple, new Rectangle(16 * i, 16 * j, 16, 16), Color.Purple);
                            break;
                        default:
                            spriteBatch.Draw(myTextureWhite, new Rectangle(16 * i, 16 * j, 16, 16), Color.White);
                            break;
                    }
                }
            }
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
