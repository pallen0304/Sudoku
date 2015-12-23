using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    public static class Common
    {
        public static bool[] isConfirmed = new bool[81];
        public static bool[] isConfirmedBackup=new bool[81];
        public static int[] nums = new int[81];
        public static int[] numsBackup = new int[81];
        public static int drawHeight { get; set; }
    }
}
