protoc.exe -I=./ --csharp_out=./ ./Protocol.proto 
IF ERRORLEVEL 1 PAUSE

START ../../../CasualRoyaleServer/PacketGenerator/bin/PacketGenerator.exe ./Protocol.proto
XCOPY /Y Protocol.cs "../../../CasualRoyaleClient/Assets/Scripts/ServerCore"
XCOPY /Y Protocol.cs "../../../CasualRoyaleServer/Server/Packet"
XCOPY /Y ClientPacketManager.cs "../../../CasualRoyaleClient/Assets/Scripts/Client/Packet"
XCOPY /Y HostPacketManager.cs "../../../CasualRoyaleClient/Assets/Scripts/HostServer/Packet"
XCOPY /Y ServerPacketManager.cs "../../../CasualRoyaleServer/Server/Packet"