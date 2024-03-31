
using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace PacketGenerator
{
	class Program
	{
		static string clientRegister;
		static string hostRegister;
		static string serverRegister;

		static void Main(string[] args)
		{
			string file = "../../../Common/protoc-3.12.3-win64/bin/Protocol.proto";
			if (args.Length >= 1)
				file = args[0];

			bool startParsing = false;
			foreach (string line in File.ReadAllLines(file))
			{
				if (!startParsing && line.Contains("enum MsgId"))
				{
					startParsing = true;
					continue;
				}

				if (!startParsing)
					continue;

				if (line.Contains("}"))
					break;

				string[] names = line.Trim().Split(" =");
				if (names.Length == 0)
					continue;

				string name = names[0];
				if (name.StartsWith("HC_"))
				{
					string[] words = name.Split("_");

					string msgName = "";
					foreach (string word in words)
						msgName += FirstCharToUpper(word);

					string packetName = $"HC_{msgName.Substring(2)}";
					clientRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
				}
				else if (name.StartsWith("SC_"))
				{
					string[] words = name.Split("_");

					string msgName = "";
					foreach (string word in words)
						msgName += FirstCharToUpper(word);

					string packetName = $"SC_{msgName.Substring(2)}";
					clientRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
				}
				else if (name.StartsWith("CH_"))
				{
					string[] words = name.Split("_");

					string msgName = "";
					foreach (string word in words)
						msgName += FirstCharToUpper(word);

					string packetName = $"CH_{msgName.Substring(2)}";
					hostRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
				}
				else if (name.StartsWith("SH_"))
				{
					string[] words = name.Split("_");

					string msgName = "";
					foreach (string word in words)
						msgName += FirstCharToUpper(word);

					string packetName = $"SH_{msgName.Substring(2)}";
					hostRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
				}
				else if (name.StartsWith("HS_"))
				{
					string[] words = name.Split("_");

					string msgName = "";
					foreach (string word in words)
						msgName += FirstCharToUpper(word);

					string packetName = $"HS_{msgName.Substring(2)}";
					serverRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
				}
				else if (name.StartsWith("CS_"))
				{
					string[] words = name.Split("_");

					string msgName = "";
					foreach (string word in words)
						msgName += FirstCharToUpper(word);

					string packetName = $"CS_{msgName.Substring(2)}";
					serverRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
				}
			}

			string clientManagerText = string.Format(PacketFormat.managerFormat, clientRegister);
			File.WriteAllText("ClientPacketManager.cs", clientManagerText);
			string hostManagerText = string.Format(PacketFormat.managerFormat, hostRegister);
			File.WriteAllText("HostPacketManager.cs", hostManagerText);
			string serverManagerText = string.Format(PacketFormat.managerFormat, serverRegister);
			File.WriteAllText("ServerPacketManager.cs", serverManagerText);
		}

		public static string FirstCharToUpper(string input)
		{
			if (string.IsNullOrEmpty(input))
				return "";
			return input[0].ToString().ToUpper() + input.Substring(1).ToLower();
		}

		public static string AllCharToUpper(string input)
		{
			if (string.IsNullOrEmpty(input))
				return "";
			return input.ToUpper();
		}
	}
}
