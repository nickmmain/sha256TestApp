using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SHA256Sharp
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.Green;

			Console.WriteLine("Press q anytime you would like to quit"); 
			Console.WriteLine();

			Console.WriteLine("Please choose which operation you would like to complete");
   
			Console.WriteLine("a)Complete a single SHA256 loop for a 32 bit word");
			Console.WriteLine("b)Complete any number of SHA256 loops for a 32 bit word");
			Console.WriteLine("c)Complete a number of SHA256 loops for a many 32 bit words");
			Console.WriteLine("d)Hash a 32-bit word with SHA256");

			switch (Console.ReadKey(true).Key)
			{
				case ConsoleKey.A:
					{
						Console.ForegroundColor = ConsoleColor.Magenta;
						Console.WriteLine();
						var value = GetUserInput("Enter the 32 bits you would like to pass a single loop for, then press Enter");

						Console.WriteLine("Here it is:");
						Console.WriteLine($"{ShaCustomLoops(Convert.ToUInt32(value, 2), 1)}");
						break;
					}
				case ConsoleKey.B:
					{
						Console.ForegroundColor = ConsoleColor.Magenta;
						Console.WriteLine();
						var value = GetUserInput("Enter the 32 bits, then press Enter");
						var loops = Int32.Parse(GetUserInput("Enter how many loops, then press Enter"));

						Console.WriteLine("Here it is:");
						Console.WriteLine($"{ShaCustomLoops(Convert.ToUInt32(value, 2), loops)}");
						break;
					}
				case ConsoleKey.C:
					{
						Console.ForegroundColor = ConsoleColor.Magenta;
						Console.WriteLine();
						var words = Int32.Parse(GetUserInput("How many random word digests should be made ?"));
						var loops = Int32.Parse(GetUserInput("How many loops on each word?"));

						Dictionary<uint, string> wordDigests = new Dictionary<uint, string>();
						var random = new Random();

						for (int i = 0; i < words; i++) 
						{
							var word = random.Next(0, Int32.MaxValue);
							var uintWord = Convert.ToUInt32(word);
							wordDigests[uintWord] = ShaCustomLoops(uintWord, loops).ToString();
						}

						string allWordDigests = "";
						var stringsArray = wordDigests.Select(kvp => Convert.ToString(kvp.Key, 2).PadLeft(32, '0')+ " " + kvp.Value.PadLeft(256, '0'));

						foreach (var wordHash in stringsArray) 
						{
							allWordDigests += wordHash + "\n"; 
						}

						var printDir = Directory.GetCurrentDirectory();
						var sha256txtFile = Path.Combine(printDir, "SHA256Sharp.txt");

						if (Directory.Exists(sha256txtFile)) 
						{
							Directory.Delete(sha256txtFile);
						}

						File.WriteAllText(sha256txtFile, allWordDigests);

						Console.WriteLine();
						Console.WriteLine($"Random words and digests have been written at:\n {printDir}");
						break;
					}
				case ConsoleKey.D: 
					 {
						Console.ForegroundColor = ConsoleColor.Magenta;
						Console.WriteLine();
						var value = GetUserInput("Enter the 32 bit word you would like to pass a single loop for, then press Enter");

						var sysSha256 = SHA256.Create();
						Console.WriteLine($"Output from System.Security: {sysSha256.ComputeHash(BitConverter.GetBytes(Convert.ToUInt32(value, 2)))}");
						break;
					}
				case ConsoleKey.Q:
					{
						Process.GetCurrentProcess().Kill();
						break;
					}
			}

			Console.WriteLine();
			GetUserInput("Press q to quit");

		}

		public static string ShaCustomLoops(uint integer32, int loops)
		{
			var K = Convert.ToUInt32(0x428a2f98);
			var H0 = Convert.ToUInt32(0x6a09e667);
			var H1 = Convert.ToUInt32(0xbb67ae85); 
			var H2 = Convert.ToUInt32(0x3c6ef372); 
			var H3 = Convert.ToUInt32(0xa54ff53a); 
			var H4 = Convert.ToUInt32(0x510e527f);
			var H5 = Convert.ToUInt32(0x9b05688c);
			var H6 = Convert.ToUInt32(0x1f83d9ab);
			var H7 = Convert.ToUInt32(0x5be0cd19);

			var a = H0;
			var b = H1;
			var c = H2;
			var d = H3;
			var e = H4;
			var f = H5;
			var g = H6;
			var h = H7;


			for (int i = 0; i < loops; i++) 
			{
				var T1 = h + Somme1(e) + Ch(e, f, g) + K + integer32;
				var T2 = Somme0(a) + Maj(a, b, c);

				h = g;
				g = f;
				f = e;
				e = checked(d + T1);
				d = c;
				c = b;
				b = a;
				a = checked(T1 + T2);

				H0 = a + H0;
				H1 = b + H1;
				H2 = c + H2;
				H3 = d + H3;
				H4 = e + H4;
				H5 = f + H5;
				H6 = g + H6;
				H7 = h + H7;

			}

			string output = Convert.ToString(H0, 2).PadLeft(32, '0')
				+ Convert.ToString(H1, 2).PadLeft(32, '0')
				+ Convert.ToString(H2, 2).PadLeft(32, '0')
				+ Convert.ToString(H3, 2).PadLeft(32, '0')
				+ Convert.ToString(H4, 2).PadLeft(32, '0')
				+ Convert.ToString(H5, 2).PadLeft(32, '0')
				+ Convert.ToString(H6, 2).PadLeft(32, '0')
				+ Convert.ToString(H7, 2).PadLeft(32, '0');

			return output;
		}


		//left
		//(original << bits) | (original >> (32 - bits))

		//right
		//(original >> bits) | (original << (32 - bits))

		public static uint Somme0(uint a) 
		{
			var somme0 = ((a >> 2) | (a << (32 - 2))) ^ ((a >> 13) | (a << (32 - 13))) ^ ((a >> 22) | (a << (32 - 22)));
			return somme0;
		}

		public static uint Somme1(uint e)
		{
			var somme1 = ((e >> 6) | (e << (32 - 6))) ^ ((e >> 11) | (e << (32 - 11))) ^ ((e >> 25) | (e << (32 - 25)));
			return somme1;
		}

		public static uint Maj(uint a, uint b, uint c)
		{
			var maj = (a & b) ^ (a & c) ^ (b & c);
			return maj;
		}

		public static uint Ch(uint e, uint f, uint g)
		{
			var ch = (e & f) ^ ((~e) & g);
			return ch;
		}

		private static string GetUserInput(string message)
		{
			Console.WriteLine(message);
			var input = Console.ReadLine();

			if (input.Equals("q"))
			{
				Process.GetCurrentProcess().Kill();
			}

			return input;
		}


	}
}
