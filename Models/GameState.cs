using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace TetrisApp.Models
{
    public class GameState
    {
        public int Score { get; set; }
        public int Level { get; set; }
        public int LinesCleared { get; set; }

        // Lưu mảng 2 chiều của bàn cờ (thường là 20x10)
        // Ta chỉ lưu ID màu sắc hoặc kiểu của Cell để nhẹ dữ liệu
        public int[,] BoardMatrix { get; set; }

        // Lưu thông tin khối gạch hiện tại và khối gạch tiếp theo
        public string CurrentTetrominoType { get; set; }
        public string NextTetrominoType { get; set; }

        public DateTime SaveTime { get; set; }
    }
}