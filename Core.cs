using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace cube_game
{

public class Core : Game
{
    internal static Core s_instance;

    private static  Dictionary<string, object> s_blackboard = new Dictionary<string, object>();

    /// <summary>
    /// Gets a reference to the Core instance.
    /// </summary>
    public static Core Instance => s_instance;

    public static Matrix View;
    public static Matrix Projection;


    public static  cube_game_input.InputManager Input;


    // The scene that is currently active.
    private static cube_game_scene.BaseScene s_activeScene;

    // The next scene to switch to, if there is one.
    private static cube_game_scene.BaseScene s_nextScene;

    /// <summary>
    /// Gets the graphics device manager to control the presentation of graphics.
    /// </summary>
    public static GraphicsDeviceManager Graphics { get; private set; }

    /// <summary>
    /// Gets the graphics device used to create graphical resources and perform primitive rendering.
    /// </summary>
    public static new GraphicsDevice GraphicsDevice { get; private set; }

    /// <summary>
    /// Gets the sprite batch used for all 2D rendering.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; private set; }

    /// <summary>
    /// Gets the content manager used to load global assets.
    /// </summary>
    public static new ContentManager Content { get; private set; }

    /// <summary>
    /// Gets or Sets a value that indicates if the game should exit when the esc key on the keyboard is pressed.
    /// </summary>
    public static bool ExitOnEscape { get; set; }

    /// <summary>
    /// Creates a new Core instance.
    /// </summary>
    /// <param name="title">The title to display in the title bar of the game window.</param>
    /// <param name="width">The initial width, in pixels, of the game window.</param>
    /// <param name="height">The initial height, in pixels, of the game window.</param>
    /// <param name="fullScreen">Indicates if the game should start in fullscreen mode.</param>
    public Core(string title, int width, int height, bool fullScreen)
    {
        // Ensure that multiple cores are not created.
        if (s_instance != null)
        {
            throw new InvalidOperationException($"Only a single Core instance can be created");
        }

        // Store reference to engine for global member access.
        s_instance = this;

        // Create a new graphics device manager.
        Graphics = new GraphicsDeviceManager(this);

        // Set the graphics defaults
        Graphics.PreferredBackBufferWidth = width;
        Graphics.PreferredBackBufferHeight = height;
        Graphics.IsFullScreen = fullScreen;

        // Set the window title
        Window.Title = title;

        // Set the core's content manager to a reference of the base Game's
        // content manager.
        Content = base.Content;

        // Set the root directory for content.
        Content.RootDirectory = "Content";

        // Mouse is visible by default.
        IsMouseVisible = true;

        // Exit on escape is true by default
        ExitOnEscape = true;    
    }


    public void SetBlackboard(string key, object value)
    {
        s_blackboard[key] = value;
    }

    public static T GetBlackboard<T>(string key) where T : class
    {
        if (s_blackboard.TryGetValue(key, out object value) && value is T typedValue)
        {
            return typedValue;
        }
        
        // 对于引用类型，返回更友好的默认值
        if (typeof(T) == typeof(string))
        {
            return (T)(object)string.Empty;
        }
        
        // 对于其他类类型，尝试创建新实例
        if (typeof(T).IsClass && typeof(T) != typeof(string))
        {
            try
            {
                return Activator.CreateInstance<T>();
            }
            catch
            {
                // 如果无法创建实例，返回 null
                return null;
            }
        }
        
        return default(T);
    }

    protected override void Initialize()
    {
        base.Initialize();

        // graphics device.
        Input = new cube_game_input.InputManager();
        GraphicsDevice = base.GraphicsDevice;

        // 设置图形设备状态
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        // 设置剔除模式：剔除顺时针面，保留逆时针面作为正面（右手坐标系习惯）
        GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

        // 设置相机参数
        View = Matrix.CreateLookAt(new Vector3(4, 5, 8), Vector3.Zero, Vector3.Up);
        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            GraphicsDevice.Viewport.AspectRatio,
            0.1f,
            100f);

        // Create the sprite batch instance.
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        // 切换到初始场景
        ChangeScene(new cube_game_scene.TitleScene());
    }

    protected override void UnloadContent()
    {
        base.UnloadContent();
    }


    protected override void LoadContent()
    {
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        // Update the input manager.
        Input.Update(gameTime);

        if (ExitOnEscape && Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            Exit();
        }

        if (s_nextScene != null)
        {
            TransitionScene();
        }

        // If there is an active scene, update it.
        if (s_activeScene != null)
        {
            s_activeScene.Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // If there is an active scene, draw it.
        if (s_activeScene != null)
        {
            s_activeScene.Draw(gameTime);
        }

        base.Draw(gameTime);
    }

    public static void ChangeScene(cube_game_scene.BaseScene next)
    {
        // Only set the next scene value if it is not the same
        // instance as the currently active scene.
        if (s_activeScene != next)
        {
            s_nextScene = next;
        }
    }

    private static void TransitionScene()
    {
        // If there is an active scene, dispose of it.
        if (s_activeScene != null)
        {
            s_activeScene.Dispose();
        }

        // Force the garbage collector to collect to ensure memory is cleared.
        GC.Collect();

        // 重置图形设备状态
        ResetGraphicsDeviceState();

        // Change the currently active scene to the new scene.
        s_activeScene = s_nextScene;

        // Null out the next scene value so it does not trigger a change over and over.
        s_nextScene = null;

        // If the active scene now is not null, initialize it.
        // Remember, just like with Game, the Initialize call also calls the
        // Scene.LoadContent
        if (s_activeScene != null)
        {
            s_activeScene.Initialize();
        }
    }

    private static void ResetGraphicsDeviceState()
    {
        if (GraphicsDevice != null)
        {
            // 重置深度缓冲区
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            
            // 重置剔除模式：剔除顺时针面，保留逆时针面作为正面（右手坐标系习惯）
            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            
            // 重置混合状态
            GraphicsDevice.BlendState = BlendState.Opaque;
            
            // 重置采样状态
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
}
