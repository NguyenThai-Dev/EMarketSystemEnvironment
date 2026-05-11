using System;
using System.IO;
using System.Collections.Generic;

class Program {
    static void Main() {
        string path = @"AIAssistantService\Program.cs";
        var lines = File.ReadAllLines(path);
        var newLines = new List<string>();
        int state = 0; // 0=normal, 1=ours(discard), 2=theirs(keep)

        foreach (var line in lines) {
            if (line.StartsWith("<<<<<<< Updated upstream")) {
                state = 1; 
                continue;
            }
            if (line.StartsWith("=======")) {
                state = 2;
                continue;
            }
            if (line.StartsWith(">>>>>>> Stashed changes")) {
                state = 0;
                continue;
            }

            if (state == 0 || state == 2) {
                newLines.Add(line);
            }
        }
        File.WriteAllLines(path, newLines);
    }
}
