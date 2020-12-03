using System;
using System.Collections.Generic;
using System.IO;

///<summary>
///Loads an assembly code file and iterates over every line.
///The parser breaks each assembly command line into its underlying fields and symbols.
///</summary>
namespace Assembler
{
    class Parser
    {
        //A_COMMAND: @number or @label
        //C_COMMAND: dest=comp;jump
        //L_COMMAND: (label)
        public enum CommandType {A_COMMAND, C_COMMAND, L_COMMAND}

        string[] lines;
        int currentLine = 0;

        public Parser(string filePath)
        {
            try
            {
                lines = StripCommentsAndWhiteSpace(File.ReadAllLines(filePath));
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public bool HasMoreCommands()
        {
            if(lines == null || lines.Length == 0)
                return false;

            if(currentLine < lines.Length)
                return true;

            return false;
        }

        public void Advance()
        {
            currentLine++;
        }

        public void Reset()
        {
            currentLine = 0;
        }

        public CommandType GetCommandType()
        {
            string command = lines[currentLine];
            if(command.StartsWith('@'))
                return CommandType.A_COMMAND;
            else if(command.StartsWith('(') && command.EndsWith(')'))
                return CommandType.L_COMMAND;
            else
                return CommandType.C_COMMAND;
        }

        public string Symbol()
        {
            return lines[currentLine].Trim('@', '(', ')');
        }

        public string Comp()
        {
            //comp is always in the middle
            string[] trimDest = lines[currentLine].Split('=');
            string[] trimJump = trimDest[trimDest.Length - 1].Split(';');
            return trimJump[0];
        }

        public string Dest()
        {
            //dest is leftmost if present
            if(lines[currentLine].Contains('='))
                return lines[currentLine].Split('=')[0];
            else
                return string.Empty;
        }

        public string Jump()
        {
            //jump is rightmost if present
            string[] trimJump = lines[currentLine].Split(';');
            if(trimJump.Length > 1)
                return trimJump[1];
            else
                return string.Empty;
        }

        private static string[] StripCommentsAndWhiteSpace(string[] lines)
        {
            List<string> lineList = new List<string>();
            foreach (string line in lines)
            {
                if (!line.TrimStart(' ').StartsWith("//") && !string.IsNullOrWhiteSpace(line))
                {
                    string codeLine = line.Split("//")[0];
                    lineList.Add(codeLine.Trim(' '));
                }
            }
            return lineList.ToArray();
        }
    }
}