using System;
using System.IO;
using System.Collections.Generic;

///<summary>
///A basic Assembler program for the Hack computer that translates the Hack symbolic language
///into binary code that can execute on the Hack hardware platform. Built as part of the nand2tetris
///course by Shimon Schocken and Noam Nisan, further information at www.nand2tetris.org.
///</summary>
namespace Assembler
{
    class Program
    {
        const string HackFileExtension = ".hack";

        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("No argument given. Path for input file must be specified.");
                return;
            }

            //read file
            Parser parser = new Parser(args[0]);

            //First pass: set memory locations for labels (LABEL)
            int currentAddress = 0;
            while(parser.HasMoreCommands())
            {
                Parser.CommandType commandType = parser.GetCommandType();
                if(commandType == Parser.CommandType.L_COMMAND)
                    SymbolTable.AddSymbol(parser.Symbol(), (short)currentAddress);
                else
                    currentAddress++;
                parser.Advance();
            }
            parser.Reset();

            //Second pass: generate machine code
            List<string> hackCode = new List<string>();
            while(parser.HasMoreCommands())
            {
                string newLine = string.Empty;
                
                Parser.CommandType commandType = parser.GetCommandType();
                if(commandType == Parser.CommandType.A_COMMAND)
                {
                    short address = SymbolTable.GetAddress(parser.Symbol());
                    newLine = Convert.ToString(address, 2).PadLeft(16, '0');
                    if(string.IsNullOrWhiteSpace(newLine))
                    {
                        newLine = UnknownCommand();
                    }
                }
                else if(commandType == Parser.CommandType.C_COMMAND)
                {
                    string comp = Code.Comp(parser.Comp());
                    string dest = Code.Dest(parser.Dest());
                    string jump = Code.Jump(parser.Jump());
                    newLine = comp + dest + jump;
                    if(string.IsNullOrWhiteSpace(comp) || string.IsNullOrWhiteSpace(dest) 
                    || string.IsNullOrWhiteSpace(jump))
                    {
                        newLine = UnknownCommand();
                    }
                    else
                        newLine = newLine.PadLeft(16, '1');
                }

                if(!string.IsNullOrWhiteSpace(newLine))
                    hackCode.Add(newLine);
                    
                parser.Advance();
            }

            if(hackCode.Count > 0)
                SaveHackCodeFile(hackCode, args[0]);
            else
                Console.WriteLine("Nothing to write.");
        }

        private static string UnknownCommand()
        {
            Console.WriteLine("Unknown command found.");
            return "##UNKNOWN-CMD###";
        }

        private static void SaveHackCodeFile(List<string> hackCode, string filePath)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                File.WriteAllLines(directoryPath + @"\" + fileName + HackFileExtension, hackCode);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
