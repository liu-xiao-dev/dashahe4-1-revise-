using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testFloat
{
    class Program
    {
        static void Main(string[] args)
        {


//            float floatValue = 1f;
//            byte[] bytes = BitConverter.GetBytes(floatValue);//小端模式，
//            foreach (var item in bytes)
//            {
//                Console.Write(item.ToString("X2"));
//            }
////            Console.Write(bytes[0].ToString("X2"));


           byte[] floatArraytmp = { 0x45, 0x9B, 0xF7, 0x3E };
 //           byte[] floatArraytmp = { 0xCD, 0xCC, 0xC8, 0x41 };
//            Array.Reverse(floatArraytmp);
            float chlorophyll = BitConverter.ToSingle(floatArraytmp, 0);
            Console.WriteLine(chlorophyll);
            Console.ReadLine();
        }
    }
}
