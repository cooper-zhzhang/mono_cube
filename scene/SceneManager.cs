using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace scene
{
    public class SceneManager
    {
        // 场景名称常量
        public const string SCENE_FREE_CUBE = "FreeCubeScene";
        public const string SCENE_SOLVE_CUBE = "SloveCubeScene";

        // 单例实例
        private static SceneManager _instance;
        private static readonly object _lock = new object();

        // 存储所有注册的场景
        private Dictionary<string, BaseScene> _scenes = new Dictionary<string, BaseScene>();

        // 当前激活的场景
        private BaseScene _currentScene;
        private GraphicsDeviceManager _graphics;
        private Matrix _view;
        private Matrix _projection;

        // 私有构造函数，防止外部实例化
        private SceneManager()
        {
        }

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static SceneManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SceneManager();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 初始化场景管理器
        /// </summary>
        public void Initialize(GraphicsDeviceManager graphics, Matrix view, Matrix projection)
        {
            _graphics = graphics;
            _view = view;
            _projection = projection;

            // 注册场景
            RegisterScene(SCENE_FREE_CUBE, new FreeCubeScene(graphics, view, projection));
            RegisterScene(SCENE_SOLVE_CUBE, new SloveCubeScene(graphics, view, projection));

            foreach (var scene in _scenes.Values)
            {
                scene.Initialize();
                scene.LoadContent();
            }

            // 切换到指定场景
            ChangeScene(SCENE_FREE_CUBE);
        }

        /// <summary>
        /// 注册场景
        /// </summary>
        public void RegisterScene(string sceneName, BaseScene scene)
        {
            if (!_scenes.ContainsKey(sceneName))
            {
                _scenes[sceneName] = scene;
            }
        }

        /// <summary>
        /// 获取指定名称的场景
        /// </summary>
        public BaseScene GetScene(string sceneName)
        {
            if (_scenes.TryGetValue(sceneName, out BaseScene scene))
            {
                return scene;
            }
            return null;
        }

        /// <summary>
        /// 获取当前激活的场景
        /// </summary>
        public BaseScene GetCurrentScene()
        {
            return _currentScene;
        }

        /// <summary>
        /// 切换到指定场景
        /// </summary>
        public void ChangeScene(string sceneName)
        {
            // TODO: 切换场景的时候，支持在不同scene之间传数据。
            BaseScene newScene = GetScene(sceneName);
            if (newScene != null)
            {
                // TODO: 判断是否需要init和loadcontent
                _currentScene = newScene;
            }
        }

        /// <summary>
        /// 初始化当前场景
        /// </summary>
        public void InitializeCurrentScene()
        {
            _currentScene?.Initialize();
        }

        /// <summary>
        /// 加载当前场景内容
        /// </summary>
        public void LoadContent()
        {
            _currentScene?.LoadContent();
        }

        /// <summary>
        /// 更新当前场景
        /// </summary>
        public void Update(GameTime gameTime)
        {
            _currentScene?.Update(gameTime);
        }

        /// <summary>
        /// 绘制当前场景
        /// </summary>
        public void Draw(GameTime gameTime)
        {
            _currentScene?.Draw(gameTime);
        }
    }
}
