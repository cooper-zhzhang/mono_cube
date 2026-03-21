using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace cube_game_scene
{
    public abstract class BaseScene : IDisposable
    {
        public GraphicsDeviceManager Graphics { get; protected set; }
        public BasicEffect Effect { get; protected set; }
        public Matrix View { get; protected set; } = Matrix.Identity;
        public Matrix Projection { get; protected set; } = Matrix.Identity;
        
        /// <summary>
        /// 获取场景的ContentManager，用于加载场景特定资源
        /// </summary>
        protected ContentManager Content { get; }
        
        /// <summary>
        /// 获取场景是否已被释放
        /// </summary>
        public bool IsDisposed { get; private set; }
        
        /// <summary>
        /// 获取场景名称
        /// </summary>
        public abstract string Name { get; }
        
        /// <summary>
        /// 创建新的场景实例
        /// </summary>
        public BaseScene(GraphicsDeviceManager graphics, Matrix view, Matrix projection)
        {
            Graphics = graphics;
            View = view;
            Projection = projection;
            
            // 场景专用的ContentManager暂时不使用
            // 后续版本可以添加对Game实例的引用
            Content = null;
        }
        
        // 析构函数，在对象被垃圾回收器清理时调用
        ~BaseScene() => Dispose(false);
        
        /// <summary>
        /// 初始化场景
        /// </summary>
        public virtual void Initialize()
        {
            LoadContent();
        }
        
        /// <summary>
        /// 加载场景内容
        /// </summary>
        public virtual void LoadContent()
        {
            Effect = new BasicEffect(Graphics.GraphicsDevice);
            Effect.VertexColorEnabled = true;
            Effect.LightingEnabled = false;
            
            // 确保图形效果一致，避免颜色深度差异
            Effect.Alpha = 1.0f;
            Effect.DiffuseColor = Vector3.One;
            Effect.AmbientLightColor = Vector3.One;
            Effect.EmissiveColor = Vector3.Zero;
            Effect.SpecularColor = Vector3.Zero;
            Effect.SpecularPower = 0f;
        }
        
        /// <summary>
        /// 进入场景时调用
        /// </summary>
        public virtual void Entry()
        {
        }
        
        /// <summary>
        /// 离开场景时调用
        /// </summary>
        public virtual void Leave()
        {
        }
        
        /// <summary>
        /// 更新场景逻辑
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
        }
        
        /// <summary>
        /// 绘制场景
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        }
        
        /// <summary>
        /// 释放场景资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// 释放场景资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // 释放托管资源
                    Effect?.Dispose();
                    Content?.Unload();
                }
                
                // 释放非托管资源
                
                IsDisposed = true;
            }
        }
    }
}