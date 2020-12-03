using System.Collections.Generic;

///<summary>
///Keeps a correspondence between symbolic labels and numeric addresses.
///</summary>
namespace Assembler
{
    static class SymbolTable
    {
        static readonly Dictionary<string, short> Symbols = new Dictionary<string, short>
        {
            {"SP", 0}, {"LCL", 1}, {"ARG", 2}, {"THIS", 3}, {"THAT", 4}, {"SCREEN", 16384}, {"KBD", 24756},
            {"R0", 0}, {"R1", 1}, {"R2", 2}, {"R3", 3}, {"R4", 4}, {"R5", 5}, {"R6", 6}, {"R7", 7}, {"R8", 8}, 
            {"R9", 9}, {"R10", 10}, {"R11", 11}, {"R12", 12}, {"R13", 13}, {"R14", 14}, {"R15", 15}
        };
        static short symbolAddress = 16; //first 16 numbers are already taken

        public static void AddSymbol(string symbol, short address)
        {
            Symbols.TryAdd(symbol, address);
        }

        public static void AddSymbol(string symbol)
        {
            if(Symbols.TryAdd(symbol, symbolAddress))
                symbolAddress++;
        }

        public static short GetAddress(string symbol)
        {
            if(short.TryParse(symbol, out short address)) //number address
                return address;
            else if(Symbols.ContainsKey(symbol)) //symbol occured before
                return Symbols[symbol];
            else //first time symbol occured
            {
                AddSymbol(symbol);
                return Symbols[symbol];
            }
        }
    }
}