using System;
using Kociemba;

// string facelets = "UUUUUULLLURRURRURRFFFFFFFFFRRRDDDDDDLLDLLDLLDBBBBBBBBB"; // 打乱状态

// DateTime startTime = DateTime.Now;
// Console.WriteLine($"开始时间: {startTime:yyyy-MM-dd HH:mm:ss.fff}");

// string info;
// string solution = SearchRunTime.solution(facelets, out info, maxDepth: 22, timeOut: 6000, useSeparator: false);
// Console.WriteLine(solution);

// DateTime endTime = DateTime.Now;
// TimeSpan totalTime = endTime - startTime;
// Console.WriteLine($"结束时间: {endTime:yyyy-MM-dd HH:mm:ss.fff}");
// Console.WriteLine($"总用时: {totalTime.TotalMilliseconds:F3} 毫秒");

/*
string facelets = "UUUUUULLLURRURRURRFFFFFFFFFRRRDDDDDDLLDLLDLLDBBBBBBBBB"; // 打乱状态
string info;
// string solution = SearchRunTime.solution(facelets, out info, buildTables: true);
// Console.WriteLine(solution);

string solution = Search.solution(facelets, out info);
Console.WriteLine(solution);
*/


using var game = new cube_game.Game1();
game.Run();



/*
UUUUUULLL
URRURRURR
FFFFFFFFF
RRRDDDDDD
LLDLLDLLD
BBBBBBBBB

*/