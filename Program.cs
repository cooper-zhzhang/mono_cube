using System;
using Kociemba;

string[] facelets = new string[] {
"UUUUUULLLURRURRURRFFFFFFFFFRRRDDDDDDLLDLLDLLDBBBBBBBBB",
"BUUBUUBLLURRURRURRUFFUFFLFFFRRFDDFDDLLLLLLDDDBBDBBDBBR",

};

string info;
//首次调用
// string solution = SearchRunTime.solution(facelets, out info, buildTables: true);
// Console.WriteLine(solution);

// 后续调用

for (int i = 0; i < facelets.Length; i++)
{
    Console.WriteLine($"第{i}个状态:");
    DateTime startTime = DateTime.Now;
    Console.WriteLine($"开始时间: {startTime:yyyy-MM-dd HH:mm:ss.fff}");
    string solution = Search.solution(facelets[i], out info);
    Console.WriteLine(solution);
    DateTime endTime = DateTime.Now;
    TimeSpan totalTime = endTime - startTime;
    Console.WriteLine($"结束时间: {endTime:yyyy-MM-dd HH:mm:ss.fff}");
    Console.WriteLine($"总用时: {totalTime.TotalMilliseconds:F3} 毫秒");
}


// using var game = new cube_game.Game1("FreeCubeScene");
// game.Run();



/*
UUUUUULLL
URRURRURR
FFFFFFFFF
RRRDDDDDD
LLDLLDLLD
BBBBBBBBB

*/