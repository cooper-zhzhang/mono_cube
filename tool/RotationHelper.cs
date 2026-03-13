using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace tool
{

    public class RotationTable
    {
        public int[] face;
        public int[] edge;
    };
    public static class RotationHelper
    {
        // 面标识符常量
        public const char FACE_UP = 'U';
        public const char FACE_DOWN = 'D';
        public const char FACE_LEFT = 'L';
        public const char FACE_RIGHT = 'R';
        public const char FACE_FRONT = 'F';
        public const char FACE_BACK = 'B';

        // 面索引常量（用于数组索引）
        public const int INDEX_FRONT = 0;  // 前(Z正)
        public const int INDEX_BACK = 1;   // 后(Z负)
        public const int INDEX_UP = 2;     // 上(Y正)
        public const int INDEX_DOWN = 3;   // 下(Y负)
        public const int INDEX_RIGHT = 4;  // 右(X正)
        public const int INDEX_LEFT = 5;   // 左(X负)
        public const int INDEX_MAX = 6;   // 最大索引值（用于循环遍历）

        // 旋转指令字符串常量
        public const string CMD_UP = "U";
        public const string CMD_UP_PRIME = "U'";
        public const string CMD_DOWN = "D";
        public const string CMD_DOWN_PRIME = "D'";
        public const string CMD_LEFT = "L";
        public const string CMD_LEFT_PRIME = "L'";
        public const string CMD_RIGHT = "R";
        public const string CMD_RIGHT_PRIME = "R'";
        public const string CMD_FRONT = "F";
        public const string CMD_FRONT_PRIME = "F'";
        public const string CMD_BACK = "B";
        public const string CMD_BACK_PRIME = "B'";
        public const string CMD_UP_2 = "U2";
        public const string CMD_DOWN_2 = "D2";
        public const string CMD_LEFT_2 = "L2";
        public const string CMD_RIGHT_2 = "R2";
        public const string CMD_FRONT_2 = "F2";
        public const string CMD_BACK_2 = "B2";

        // 正值表示顺时针，负值表示逆时针
        public static float GetRotationAngle(string cmd)
        {
            switch (cmd)
            {
                case CMD_UP: // 最上层面绕Y轴旋转
                    return -MathHelper.PiOver2;
                case CMD_UP_PRIME: // 最上层面绕Y轴旋转
                    return MathHelper.PiOver2;
                case CMD_DOWN: // 最下层面绕Y轴旋转
                    return MathHelper.PiOver2;
                case CMD_DOWN_PRIME: // 最下层面绕Y轴旋转
                    return -MathHelper.PiOver2;
                case CMD_LEFT: // 最左层面绕X轴旋转
                    return MathHelper.PiOver2;
                case CMD_LEFT_PRIME: // 最左层面绕X轴旋转
                    return -MathHelper.PiOver2;
                case CMD_RIGHT: // 最右层面绕X轴旋转
                    return -MathHelper.PiOver2;
                case CMD_RIGHT_PRIME: // 最右层面绕X轴旋转
                    return MathHelper.PiOver2;
                case CMD_FRONT: // 最前层面绕Z轴旋转
                    return -MathHelper.PiOver2;
                case CMD_FRONT_PRIME: // 最前层面绕Z轴旋转
                    return MathHelper.PiOver2;
                case CMD_BACK: // 最后层面绕Z轴旋转
                    return MathHelper.PiOver2;
                case CMD_BACK_PRIME: // 最后层面绕Z轴旋转
                    return -MathHelper.PiOver2;
                case CMD_UP_2: // 最上层面绕Y轴旋转
                    return -MathHelper.Pi;
                case CMD_DOWN_2: // 最下层面绕Y轴旋转
                    return MathHelper.Pi;
                case CMD_LEFT_2: // 最左层面绕X轴旋转
                    return MathHelper.Pi;
                case CMD_RIGHT_2: // 最右层面绕X轴旋转
                    return -MathHelper.Pi;
                case CMD_FRONT_2: // 最前层面绕Z轴旋转
                    return -MathHelper.Pi;
                case CMD_BACK_2: // 最后层面绕Z轴旋转
                    return MathHelper.Pi;
                default:
                    return 0;
            }
        }

        public static Matrix CreateRotationMatrix(string face, float angle)
        {
            switch (face)
            {
                case CMD_UP: // 最上层面绕Y轴旋转
                case CMD_UP_2:
                    return Matrix.CreateRotationY(-angle);
                case CMD_UP_PRIME: // 最上层面绕Y轴旋转
                    return Matrix.CreateRotationY(angle);
                case CMD_DOWN: // 最下层面绕Y轴旋转
                case CMD_DOWN_2:
                    return Matrix.CreateRotationY(angle);
                case CMD_DOWN_PRIME: // 最下层面绕Y轴旋转
                    return Matrix.CreateRotationY(-angle);
                case CMD_LEFT: // 最左层面绕X轴旋转
                case CMD_LEFT_2:
                    return Matrix.CreateRotationX(angle);
                case CMD_LEFT_PRIME: // 最左层面绕X轴旋转
                    return Matrix.CreateRotationX(-angle);
                case CMD_RIGHT: // 最右层面绕X轴旋转
                case CMD_RIGHT_2:
                    return Matrix.CreateRotationX(-angle);
                case CMD_RIGHT_PRIME: // 最右层面绕X轴旋转
                    return Matrix.CreateRotationX(angle);
                case CMD_FRONT: // 最前层面绕Z轴旋转
                case CMD_FRONT_2:
                    return Matrix.CreateRotationZ(-angle);
                case CMD_FRONT_PRIME: // 最前层面绕Z轴旋转
                    return Matrix.CreateRotationZ(angle);
                case CMD_BACK: // 最后层面绕Z轴旋转
                case CMD_BACK_2:
                    return Matrix.CreateRotationZ(angle);
                case CMD_BACK_PRIME: // 最后层面绕Z轴旋转
                    return Matrix.CreateRotationZ(-angle);
                default:
                    return Matrix.Identity;
            }
        }

        public static readonly IReadOnlyDictionary<string, RotationTable> RotationTableMap =
        new Dictionary<string, RotationTable>
        {
            {"F", new RotationTable{
                face = new int[] {18, 19, 20, 23, 26, 25, 24, 21},
                edge = new int[] {6, 7, 8, 9, 12, 15, 29, 28, 27, 44, 41, 38}
            }},

          {"R", new RotationTable{
            face = new int[] {9, 10, 11,  14, 17, 16, 15, 12},
            edge = new int[] {8, 5, 2, 45, 48, 51, 35, 32, 29, 26 ,23, 20},// error

			//9 10 11 14 17 16 15 12
			// 8 5 1 45 48 51 35 32 29 26 23 20
          }},

          {"U", new RotationTable{
              face = new int[] {0, 1, 2, 5, 8, 7, 6, 3},
              edge = new int[] {47, 46, 45, 11, 10, 9, 20, 19, 18, 38, 37, 36}
            }
          },


          {"D", new RotationTable{
              face = new int[] {27,28,29,32,35,34,33,30},
              edge = new int[] {24,25,26,15,16,17,51,52,53,42,43,44}
            }
          },

          {"L", new RotationTable{
              face = new int[] {36, 37, 38, 41, 44, 43, 42, 39 },
              edge = new int[] {0 ,3 ,6 ,18, 21 ,24 ,27, 30 ,33 ,53 ,50 ,47} // error
            }
          },

          {"B", new RotationTable{
              face = new int[] {45,46,47,50,53,52,51,48},
              edge = new int[] {2,1,0,36,39,42,33,34,35,17,14,11}
            }
          },
        };

        /*
          "qian": {
            "face": {18, 19, 20, 23, 26, 25, 24, 21},
            "edge":{6, 7, 8, 9, 12, 15, 29, 28, 27, 44, 41, 38}
          },		

          "you":{
            "face":{9, 10, 11,  14, 17, 16, 15, 12},
            "edge":{8, 7, 6, 45, 48, 51, 35, 32, 29, 26, 23, 20},
          }

                "shang":{
              "face":{0, 1, 2, 5, 8, 7, 6, 3},
              "edge":{47, 46, 45, 11, 10, 9, 20, 19, 18, 38, 37, 36}
            }				


            "xia":{
              "face":{27,28,29,32,35,34,33,30},
              "edge":{24,25,26,15,16,17,51,52,53,42,43,44}
            }

            "zuo":{
              "face":{9,10,11,14,17,16,15,12},
              "edge":{8,7,6,45,48,51,35,32,29,26,23,20}
            }


          "hou":{
            "face":{45,46,47,50,53,52,51,48},
            "edge":{2,1,0,36,39,42,33,34,35,17,14,11}
          }

        */

        public static string RotationStage(string cube, char cmove, int times)
        {
            var rotationTable = RotationTableMap[cmove.ToString()];
            if (rotationTable == null)
            {
                return cube;
            }

            for (int j = 0; j < times; j++)
            {
                char[] oldCubeArray = (char[])cube.ToCharArray();
                char[] cubeArray = (char[])cube.ToCharArray().Clone();
                // 深拷贝 cube
                string copyCube = cube;
                for (int i = 0; i < rotationTable.face.Length; i++)
                {
                    cubeArray[rotationTable.face[(i + 2) % rotationTable.face.Length]] = oldCubeArray[rotationTable.face[i]];
                }

                for (int i = 0; i < rotationTable.edge.Length; i++)
                {
                    cubeArray[rotationTable.edge[(i + 3) % rotationTable.edge.Length]] = oldCubeArray[rotationTable.edge[i]];
                }

                cube = new string(cubeArray);

            }

            return cube;
        }

        /*
                public static string RotationStage(string cubeState, char face, int times = 1)
                {
                    Dictionary<int, int> rotationMap = clockwiseMaps[face];
                    for (int i = 0; i < times; i++)
                    {
                        char[] new_state = cubeState.ToCharArray();
                        foreach (var pair in rotationMap)
                        {
                            new_state[pair.Key] = cubeState[pair.Value];
                        }
                        cubeState = new string(new_state);
                    }

                    return cubeState;
                }*/
    }
}