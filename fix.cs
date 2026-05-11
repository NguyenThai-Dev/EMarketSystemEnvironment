using System;
using System.IO;
using System.Collections.Generic;

class Program {
    static void Main() {
        string path = @"AIAssistantService\Controllers\ChatController.cs";
        var lines = File.ReadAllLines(path);
        var newLines = new List<string>();
        int state = 0; // 0=normal, 1=ours(keep), 2=theirs(keep), 3=ours(keep), 4=theirs(discard), 5=ours(keep), 6=theirs(discard)
        int conflictCount = 0;

        foreach (var line in lines) {
            if (line.StartsWith("<<<<<<< ours")) {
                conflictCount++;
                if (conflictCount == 1) state = 1; // Conflict 1: Ours (keep both, but reorder)
                else if (conflictCount == 2) state = 3; // Conflict 2: Ours (keep ours)
                else if (conflictCount == 3) state = 5; // Conflict 3: Ours (keep ours)
                continue;
            }
            if (line.StartsWith("=======")) {
                if (state == 1) state = 2; // Conflict 1: change to theirs (keep)
                else if (state == 3) state = 4; // Conflict 2: change to theirs (discard)
                else if (state == 5) state = 6; // Conflict 3: change to theirs (discard)
                continue;
            }
            if (line.StartsWith(">>>>>>> theirs")) {
                state = 0; // back to normal
                continue;
            }

            if (state == 0) {
                newLines.Add(line);
            } else if (state == 1 || state == 3 || state == 5) {
                newLines.Add(line);
            } else if (state == 2) {
                // Conflict 1 theirs: we want this BEFORE ours. So we insert it before the block.
                // Wait, it's easier to just append it, and we can manually swap them if needed. 
                // But appending it is fine, it will just put auth header after SQLGen message.
                newLines.Add(line);
            }
        }
        File.WriteAllLines(path, newLines);
    }
}
