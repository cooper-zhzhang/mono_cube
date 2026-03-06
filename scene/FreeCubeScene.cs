using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace scene
{
    public class FreeCubeScene : BaseScene
    {

        private cube_obj.Cube _cube;
        
        // 每个对象继承 obj 类，包含位置、旋转、缩放、颜色等属性。
        public FreeCubeScene(GraphicsDeviceManager graphics, Matrix _view, Matrix _projection) : base(graphics, _view, _projection)
        {
            _cube = new cube_obj.Cube(_graphics.GraphicsDevice, "UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB");
            _cube.createCubeByStage();

        }

        // TODO:  把逻辑都写在这个里面

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Gray);
            _cube.Draw(gameTime);
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            _cube.Update(gameTime);
            base.Update(gameTime);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _effect.VertexColorEnabled = true;
            _effect.LightingEnabled = false;

            _view = Matrix.CreateLookAt(new Vector3(4, 5, 8), Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                _graphics.GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f);
        }

    }
}