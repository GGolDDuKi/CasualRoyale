protoc.exe -I=./ --csharp_out=./ ./Protocol.proto 
IF ERRORLEVEL 1 PAUSE

START ../../../PixelSquadServer/PacketGenerator/bin/PacketGenerator.exe ./Protocol.proto
XCOPY /Y Protocol.cs "../../../PixelSquadClient/Assets/Scripts/ServerCore"
XCOPY /Y Protocol.cs "../../../PixelSquadServer/Server/Packet"
XCOPY /Y ClientPacketManager.cs "../../../PixelSquadClient/Assets/Scripts/Client/Packet"
XCOPY /Y HostPacketManager.cs "../../../PixelSquadClient/Assets/Scripts/HostServer/Packet"
XCOPY /Y ServerPacketManager.cs "../../../PixelSquadServer/Server/Packet"