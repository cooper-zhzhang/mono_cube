using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace scene
{
    public class BaseScene
    {
        public GraphicsDeviceManager _graphics { get; protected set; } = null;
        public BasicEffect _effect { get; protected set; } = null;
        public Matrix _view { get; protected set; } = Matrix.Identity;
        public Matrix _projection { get; protected set; } = Matrix.Identity;
        public BaseScene(GraphicsDeviceManager graphics, Matrix view, Matrix projection)
        {
            _graphics = graphics;
            _view = view;
            _projection = projection;
        }

        public virtual void LoadContent()
        {

            _effect = new BasicEffect(_graphics.GraphicsDevice);
            _effect.VertexColorEnabled = true;

        }


        public virtual void Update(GameTime gameTime)
        {
        }
        public virtual void Draw(GameTime gameTime)
        {

        }
        public virtual void Initialize()
        {

        }
    }
}