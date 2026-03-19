using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using tool;

namespace cube_game_scene
{
    public class TitleScene : BaseScene
    {
        // 用于2D绘制的精灵批处理
        private SpriteBatch _spriteBatch;
        private Texture2D _pixelTexture;
        
        // 动态窗口大小变化相关字段
        private float _timer = 0f;
        private const float INTERVAL = 5f; // 5秒间隔
        private int _widthIncrement = 0;
        private int _heightIncrement = 0;

        public override string Name => "TitleScene";

        public TitleScene() : base(cube_game.Core.Graphics, Matrix.CreateLookAt(new Vector3(4, 5, 8), Vector3.Zero, Vector3.Up),  Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            cube_game.Core.GraphicsDevice.Viewport.AspectRatio,
            0.1f,
            100f))
        {
            Debug.WriteLine("[TitleScene] 构造函数被调用");
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            
            // 创建精灵批处理和像素纹理
            _spriteBatch = new SpriteBatch(Graphics.GraphicsDevice);
            _pixelTexture = new Texture2D(Graphics.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            
        }

        public override void Update(GameTime gameTime)
        {
            // 动态窗口大小变化逻辑
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (_timer >= INTERVAL)
            {
                _timer = 0f; // 重置计时器
                
                // 每5秒增加窗口大小（长宽各增加10像素）
                _widthIncrement += 10;
                _heightIncrement += 10;
                
                // 更新窗口大小
                if (cube_game.Core.Graphics != null)
                {
                    cube_game.Core.Graphics.PreferredBackBufferWidth = 1024 + _widthIncrement;
                    cube_game.Core.Graphics.PreferredBackBufferHeight = 768 + _heightIncrement;
                    cube_game.Core.Graphics.ApplyChanges();
                    
                    Debug.WriteLine($"[TitleScene] 窗口大小已更新: {1024 + _widthIncrement}x{768 + _heightIncrement}");
                }
            }
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Debug.WriteLine("[BuildScene] Draw 开始");
            
            // 绘制背景
            Graphics.GraphicsDevice.Clear(Color.Gray);
            Debug.WriteLine("[BuildScene] 背景已清除为灰色");

            // 检查资源是否已加载
            if (_spriteBatch == null)
            {
                Debug.WriteLine("[BuildScene] 错误: _spriteBatch 为 null");
                return;
            }
            if (_pixelTexture == null)
            {
                Debug.WriteLine("[BuildScene] 错误: _pixelTexture 为 null");
                return;
            }

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _spriteBatch?.Dispose();
                _pixelTexture?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}