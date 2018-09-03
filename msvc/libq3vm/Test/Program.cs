using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Q3VM;

namespace Test {
	class Program {
		const string LibName = "kernel32";
		const CallingConvention CConv = CallingConvention.Winapi;

		[DllImport(LibName, CallingConvention = CConv)]
		static extern bool SetDllDirectory(string Dir);

		static Program() {
			SetDllDirectory(IntPtr.Size == sizeof(long) ? "x64" : "x86");
		}

		static void Main(string[] Args) {
			string SC = VM.GenerateSyscallsAsm(typeof(Syscalls));

			VM V = new VM("out.qvm");
			V.RegisterSyscalls(typeof(Syscalls));

			Console.WriteLine("Result: {0}", V.Call(69).ToInt32());
			Console.ReadLine();
		}
	}

	public static class Syscalls {
		[Q3VMSyscall(-1)]
		public static void Printf(string Str) {
			Console.Write(Str);
		}

		[Q3VMSyscall(-2)]
		public static void Error(string Str) {
			ConsoleColor OldClr = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.Write(Str);
			Console.ForegroundColor = OldClr;
		}

		[Q3VMSyscall(-3)]
		public static void MemSet(IntPtr Ptr, byte Val, int Size) {
			for (int i = 0; i < Size; i++)
				Marshal.WriteByte(Ptr, i, Val);
		}

		[Q3VMSyscall(-4)]
		public static void MemCpy(IntPtr Dst, IntPtr Src, int Size) {
			for (int i = 0; i < Size; i++)
				Marshal.WriteByte(Dst, i, Marshal.ReadByte(Src, i));
		}
	}
}
