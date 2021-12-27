# <img src="https://i.imgur.com/dJ3ecyL.png" width="256"/>
Volley Head is a competitive multiplayer game where there are 2 teams of 2 players. The teams will face each other with a game like volleyball. The team that reaches the target points first will win the match.

## Contents :
* [HOW TO PLAY](#how-to-play)
* [GAME RULES](#game-rules)
* [GAME DESIGN DOCUMENT](#game-design-document)
* [GAME SCREENSHOT](#game-screenshot)
* [DOWNLOAD GAME APK](#download-game-apk)
* [USED TECHNOLOGY](#used-technology)
* [VOLLEY HEAD SERVER DEPLOYMENT](#volley-head-server-deployment)

## HOW TO PLAY
1. To start the game, one player must create a room, and the other player will enter the room using the provided code.
2. If the minimum number of players is reached, the room master can click the "START" button to start the match.
3. Player can move using the left and right button.
4. Player can jump using jump button.

## GAME RULES
- Before starting the game, one of the teams will serve.
- The first team to serve is selected by random.
- Each team must head the ball, and try not to fall the ball to the ground, and player drop the ball into the opponent's area.
- Each team member may not touch the ball 2 times in a row. If they do that, the opposing team will get the points.
- Each team may not touch the ball 3 times in a row in one round of the ball when it is played the team area. If they do that, the points is for the opponent.
- When a team gets a point, that team will serve in the next round.
- First team to get 10 point is the Winner.

## GAME DESIGN DOCUMENT
You can see the Volley Head GDD in [here](/Game%20Design%20Document/GDD%20Head%20Volleyball.pdf)

## GAME SCREENSHOT
<img src="https://i.imgur.com/OyNySqK.jpg" width="720"/>
<img src="https://i.imgur.com/Uf8pZJy.jpg" width="720"/>
<img src="https://i.imgur.com/dg8rqkx.jpg" width="720"/>
<img src="https://i.imgur.com/uaYLCCP.jpg" width="720"/>
<img src="https://i.imgur.com/nKarmFs.jpg" width="720"/>
<img src="https://i.imgur.com/iA10B7c.jpg" width="720"/>

## DOWNLOAD GAME APK
[GOOGLE DRIVE](https://bit.ly/3muM81Z)

## USED TECHNOLOGY
- Unity 2020 (Game Engine)
- Mirror (Networking library for Unity)
- Azure (Server)
- Adobe Illustrator (Assets Maker)
- Soundtrap, and Audacity (Sound FX and Audio Maker)

## VOLLEY HEAD SERVER DEPLOYMENT
1. We used a **Azure Virtual Machine** with OS Linux Debian 9 to run the Volley Head Server.
<br/><img src="https://ms-azuretools.gallerycdn.vsassets.io/extensions/ms-azuretools/vscode-azurevirtualmachines/0.4.1/1629848176673/Microsoft.VisualStudio.Services.Icons.Default" width="120"/>
2. First, we create a linux virtual machine with the following specifications :
```
Region: Southeast Asia
Image / base OS : Debian 9 "Stretch"
Size : Standard - 1vcpu, 1 GiB memory
```
3. Open the virtual machine with Virtual machine IP and port 22 in CMD. Login with username and password that already listed when created virtual machine.
<br/><img src="https://i.ibb.co/DGLWRN4/image.png" width="640"/>
4. Setup the virtual machine. Enter the command that listed in below.
  <br/>Update OS Package :
  <br/>``` sudo apt-get update ```
  <br/>Install screen and unzip :
  <br/>``` sudo apt-get install -y screen unzip ```
5. Configure virtual machine firewall. On Networking menu add inbound port with a same port in Unity Project. We used a default value for Mirror Networking: 7777.
<br/><img src="https://i.ibb.co/TPxhVGT/image.png" width="240"/>&emsp;&emsp;<img src="https://i.ibb.co/YNCThP2/image.png" width="360"/>
6. Next, setup the network ip in unity project and fill that with a virtual machine private ip.
<br/><img src="https://i.ibb.co/h7P9hKK/image.png" width="360"/>&emsp;&emsp;<img src="https://i.ibb.co/PNmDGr8/image.png" width="240"/>
7. Build the unity project with target build for Linux and enable server build setting. And then ZIP all the build file.
<br/><img src="https://i.ibb.co/VVxk0Z2/image.png" width="360"/>
8. Upload the zipped build file to virtual machine. To upload run a command in CMD.
<br/>``` scp file_location username@vm_ip:target_location ```
9. Back to virtual machine command prompt. And unzip the server build file that already uploaded with command:
<br/>``` unzip location_file ```
10. Mark server game file as executable. Command:
<br/>``` chmod +x executable_file_location```
11. Make a screen in Linux to run the server even if virtual machine is not opened. Command to create screen in Linux:
<br/>``` screen -d -m -S [screenName] [file_location] -logfile [screen_log_location]```
12. Mark screen file as executable.
13. Run the screen file.
14. Now, game client can be connected to server with Virtual Machine public IP.


