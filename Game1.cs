using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace cube_game
{
    public class Game1 : Game
    {
        scene.BaseScene _freeCubeScene;
        private GraphicsDeviceManager _graphics;
        private BasicEffect _effect;
        private Matrix _view;
        private Matrix _projection;

        private string _sceneName = "FreeCubeScene";
   
        public Game1(string sceneName)
        {
            _sceneName  = sceneName == "" ? _sceneName : sceneName;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            _effect = new BasicEffect(GraphicsDevice);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            // 设置剔除模式：剔除顺时针面，保留逆时针面作为正面（右手坐标系习惯）
            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _effect.VertexColorEnabled = true;
            _effect.LightingEnabled = false;


            // 相机位置：(4, 5, 4)，目标点：(0, 0, 0)，上方向：(0, 1, 0)
            _view = Matrix.CreateLookAt(new Vector3(4, 5, 8), Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f);


            if (_sceneName == "SloveCubeScene")
            {
                _freeCubeScene = new scene.SloveCubeScene(_graphics, _view, _projection);
            }
            else
            {
                _freeCubeScene = new scene.FreeCubeScene(_graphics, _view, _projection);
            }   
            _freeCubeScene .Initialize();
            _freeCubeScene .LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            // 退出条件（按ESC或手柄返回键）
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _freeCubeScene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _freeCubeScene.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}