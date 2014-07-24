using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.IO;
using hyperSpaceDrive;


namespace hyperSpaceDrive
{
    public class Object
    {

        public Vector3 Position;
        public Vector2 velocity;
        public int type;
        public Object()
        {
            Position = new Vector3(0, 0, 0);
        }
        public Object(int x, int y, int z)
        {
            Position = new Vector3(x, y, z);
        }
    }
    public partial class GamePage : PhoneApplicationPage
    {
        Random rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        GameTimer timer;
        ContentManager content;
        SpriteBatch spriteBatch;
        UIElementRenderer uiRenderer;

        Texture2D hole, bullet, back, up, down, mist;
        Texture2D[] thingiesText;
        BasicEffect basicEffect;
        SoundEffect right, wrong;
        VertexPositionColor[] vertices;
        int score = 0, incr = 50, maxScore, mistY = 0, maxup, maxdown;

        bool display;
        float scale = 0.6f;
        int fheight = 200, maxNoOfObjects = 4, spaceToAccomodate = 10,lower=50,upper=SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height-200;
        LinkedList<Object> list = new LinkedList<Object>();


        public GamePage()
        {
            InitializeComponent();
          // Get the application's ContentManager
            content = (Application.Current as App).Content;

            // Create a timer for this page
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;

            LayoutUpdated += new EventHandler(GamePage_LayoutUpdated);
        }
        protected void loadfn()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            if (store.FileExists("test.txt")) // Check if file exists
            {
                IsolatedStorageFileStream save = new IsolatedStorageFileStream("test.txt", FileMode.Open, store);
                BinaryReader reader = new BinaryReader(save);
                string mystring = reader.ReadString();
                maxScore = (int)reader.ReadSingle();
                reader.Close();
            }
        }

        protected void strfn()
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication(); // grab the storage
            FileStream stream = store.OpenFile("test.txt", FileMode.Create); // Open a file in Create mode
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write("SCORE : ");
            writer.Write(maxScore);
            writer.Close();
        }

        void GamePage_LayoutUpdated(object sender, EventArgs e)
        {
            int width = (int)ActualWidth;
            int height = (int)ActualHeight;

            if (width <= 0 || height <= 0)
                return;

            if (uiRenderer != null &&
                uiRenderer.Texture != null &&
                uiRenderer.Texture.Width == width &&
                uiRenderer.Texture.Height == height)
            {
                return;
            }

            if (uiRenderer != null)
                uiRenderer.Dispose();

            uiRenderer = new UIElementRenderer(this, width, height);
        }



        private float distsqr(Vector2 a, Vector3 b)
        {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            // TODO: use this.content to load your game content here

            
            content.RootDirectory = "content"; 
            
            thingiesText = new Texture2D[5];
            basicEffect = new BasicEffect(SharedGraphicsDeviceManager.Current.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
               (0, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width,     // left, right
                SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1);                                         // near, far plane

            vertices = new VertexPositionColor[2];
            vertices[0].Position = new Vector3(100,SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height- fheight, 0);
            vertices[0].Color = Color.Beige;
            vertices[1].Position = new Vector3(SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width - 100,SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height - fheight, 0);
            vertices[1].Color = Color.Beige;
            loadfn();


            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);
            // Create a new SpriteBatch, which can be used to draw textures.
            hole = content.Load<Texture2D>(rnd.Next(0,2)==1?"bluehole":"yellowhole");
            bullet = content.Load<Texture2D>("bullet");
            back = content.Load<Texture2D>("back");
            thingiesText[0] = content.Load<Texture2D>("asteroid1");
            thingiesText[1] = content.Load<Texture2D>("asteroid2");
            thingiesText[2] = content.Load<Texture2D>("asteroid3");
            thingiesText[3] = content.Load<Texture2D>("ship");
            thingiesText[4] = content.Load<Texture2D>("startrek");
            right = content.Load<SoundEffect>("right");
            wrong = content.Load<SoundEffect>("wrong");
            mist = content.Load<Texture2D>("mist");
            up = content.Load<Texture2D>("up");
            down = content.Load<Texture2D>("down");
            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            // TODO: Add your update logic here

            // Move the sprite around.


            // TODO: Add your update logic here
            mistY = ((mistY + 1) % ((SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width) * 4));
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            { }//Microsoft.Xna.Framework.
                //Microsoft.Xna.Framework.Exit();

            // TODO: Add your update logic here
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation touch in touchCollection)
            {
                if ((touch.State == TouchLocationState.Pressed) || (touch.State == TouchLocationState.Moved))
                {
                    if(touch.Position.X<down.Width  & touch.Position.Y>-down.Height+SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height){
                        fheight=fheight-10<lower?lower:fheight-10;
                    }
                    else if ( touch.Position.X > SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width - down.Width & touch.Position.Y > -down.Height + SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height)
                    {
                        fheight = fheight + 10 > upper ? upper : fheight + 10;
                    }
                    else
                    {
                        if (distsqr(touch.Position, vertices[0].Position) < distsqr(touch.Position, vertices[1].Position))
                        {
                            //                        vertices[0].Position.X = touch.Position.X;
                            vertices[0].Position.X = touch.Position.X;
                        }
                        else
                        {
                            //                        vertices[1].Position.X = touch.Position.X;
                            vertices[1].Position.X = touch.Position.X;
                        }
                    }
                }
            }
            if (score > maxScore)
            {
                maxScore = score;
                strfn();
            }
            if (list.Count < maxNoOfObjects)
            {
                Object tmp = new Object(rnd.Next(0, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width - 91),-300 , 0);
                tmp.velocity.Y = rnd.Next(2, 6);
                tmp.type = rnd.Next(0, thingiesText.Length);
                list.AddLast(tmp);
            }
            var node = list.First;
            while (node != null)
            {
                node.Value.Position.Y += node.Value.velocity.Y;
                int i = node.Value.type;
                float ttx = node.Value.Position.X + thingiesText[i].Width/ 2;
                if (node.Value.Position.Y > SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height)
                {
                    var node2 = node;
                    node = node.Next;
                    list.Remove(node2);
                    score -= i < 3 ? -incr * 2 / 3 : incr;
                }
                else if (ttx < vertices[1].Position.X & ttx > vertices[0].Position.X & node.Value.Position.Y > SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height - fheight - thingiesText[i].Height/ 2 & node.Value.Position.Y < 20 + SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height - fheight - thingiesText[i].Height / 2)
                {
                    var node2 = node;
                    node = node.Next;
                    list.Remove(node2);
                    score += i < 3 ? -incr : incr * 2 / 3;
                    (i < 3 ? wrong : right).Play();
                    spaceToAccomodate -= i < 3 ? 1 : 0;
                    if (spaceToAccomodate <= 0) {// Microsoft.Xna.Framework.Exit(); 
                    } //return;
                }
                else node = node.Next;

            }


        }


        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            // Draw the Silverlight UI into the texture
            uiRenderer.Render();

            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.SlateGray);

            // TODO: Add your drawing code here

            // Draw the sprite
            spriteBatch.Begin();
            //            graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
            spriteBatch.Draw(back, new Microsoft.Xna.Framework.Rectangle(0, 0, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width,
                SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.Draw(mist, new Microsoft.Xna.Framework.Rectangle(mistY - 2 * SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width,0,
                                SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width,
                                SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.Draw(hole, new Microsoft.Xna.Framework.Rectangle((int)(vertices[0].Position.X), (int)(vertices[0].Position.Y), 
                                (int)(vertices[1].Position.X - vertices[0].Position.X),30), Color.White);
           
            foreach (Object tmp in list)
            {
                spriteBatch.Draw(thingiesText[tmp.type], new Vector2(tmp.Position.X, tmp.Position.Y), null, Color.White, 0f, Vector2.Zero, scale,
                    SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(bullet, new Microsoft.Xna.Framework.Rectangle((int)vertices[0].Position.X - 5, (int)vertices[0].Position.Y +5, 20, 20), Color.White);
            spriteBatch.Draw(bullet, new Microsoft.Xna.Framework.Rectangle((int)vertices[1].Position.X - 5, (int)vertices[1].Position.Y +5, 20, 20), Color.White);
            spriteBatch.Draw(up, new Vector2(SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width - up.Width, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height - up.Height), Color.White);
            spriteBatch.Draw(down, new Vector2(0,
                                SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height- down.Height), Color.White);

            spriteBatch.DrawString(content.Load<SpriteFont>("SpriteFont1"), score.ToString(), new Vector2(10, 10), Color.White);
            spriteBatch.End();
        }
    }
}
