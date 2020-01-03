# Fpl Email Bot

FPL Email Bot extracts information from the FPL API and creates a summary with weekly winners, losers, and certain fun facts. It emails the summary to a set list of emails once the GW has completed.

Example email body:

```
Beep boop! I am a robot. This is your weekly FPL update.

The winner for gameweek #21 was Kotterpool with 71 points! Rounding up the top 3 for the week was windsor wildlings (70 pts) and The B Team (68 pts).

The worst ranking teams this week were gEaR (51 pts), Hillhurst Hotspur (51 pts), JLG (44 pts), and Team Belgrave (34 pts). You should probably be embarrassed.

When it came to captaincy choice Kotterpool did the best this week with 20 point from Mohamed Salah. On the other end of the spectrum were Chupacabra, Gnarly Norwegians, Hillhurst Hotspur, Slaughterhouse 11, FC AeroZeppelin, Beeston FC, JLG, and Mount Pukki who had picked Raúl Jiménez, Kevin De Bruyne, and Harry Kane for a total of 4 points. You receive the armband of shame for this week.

This week The B Team did the best with automatic substitutions receiving 11 points from Maddison coming off the bench in liue of Vardy. JLG was not as fortunate getting a paltry 0 points from Ramsdale and Rico who covered for Adrián and Vardy. Hillhurst Hotspur will be kicking themselves after having left 14 points on the bench which was the highest in the league.

Chupacabra stay at the top of the table with 1332 points, although Gnarly Norwegians is creeping closer 92 points behind. At the other end Team Belgrave continues to languish in last place with a paltry 825 points.

As for shakers and movers Gnarly Norwegians climbed 2 spots this week. In the not so great department we have JLG dropping 2 spots.


Notable news:

- 16 teams managed to reach or beat the overall average of 48 points for the week.
- Beeston FC (-4 pts) and JLG (-4 pts) both took transfer hits.
- The Gooners (wildcard) was the only team to use a chip this week.

Your friendly FPL bot will return next gameweek with another update.

```

Example of attached standings file:

```
Standings for Oskana FC after GW#21:
------------------------------------------------------------------
Rank Chg. PW   Overall  Team                GW  Total   TT   TmVal
------------------------------------------------------------------

1    --   1    268      Chupacabra          59   1332   20   106.0
2    up   4    46.6k    Gnarly Norwegians   63   1240   15   106.6
3    dn   2    48.0k    gEaR                51   1240   24   105.1
4    dn   3    65.4k    Hillhurst Hotspur   51   1232   20   104.3
5    --   5    308k     The B Team          68   1184   17   103.9
6    --   6    320k     Slaughterhouse 11   67   1183   22   102.1
7    --   7    577k     Man City Mafia      54   1158   20   104.9
8    up   9    650k     Kotterpool          71   1152   15   102.0
9    dn   8    749k     Relegation FC       61   1145   16   100.9
10   --   10   1.07M    The Gooners         63   1125   11   100.3
11   --   11   1.76M    FC AeroZeppelin     53   1090   20   101.3
12   up   13   1.85M    Daniel's Spaniels   58   1086   30   101.1
13   up   14   2.04M    Beeston FC          56   1077   19   100.7
14   dn   12   2.11M    JLG                 48   1074   21   100.5
15   --   15   2.37M    windsor wildlings   70   1062   16    99.5
16   --   16   2.96M    Big Red Dog         63   1036   14    98.4
17   --   17   3.40M    Mount Pukki         52   1016   22   102.7
18   --   18   6.07M    Team Belgrave       34    825    2   100.5

------------------------------------------------------------------
Rank:    Current rank in league Oskana FC
Chg.:    Movement in league compared to previous week
PW:      Previous week rank in league
Overall: Rank amongst all players in FPL
GW:      Game week points
Total:   Point sum of all game weeks
TT:      Total transfers
TmVal:   Team value (including bank)

```

## Requirements

- Windows OS with .NET Framework
- An email account
- An FPL account
- Azure subscription (optional)

## Setup

Download the latest release (https://github.com/f-aa/Fpl-Email-Bot/releases) and unzip it into a folder on your computer. Open the FplBot.exe.config with a text editor (like Notepad) and fill in all the required values. Follow the instructions in the configuration file for help. If you are planning on running the file locally from your computer you can keep the useAzure and azureBlobStorage settings as they are. Make sure you save your file before attempting to run the bot.

> **PLEASE NOTE**
> 
> Starting with the 2019/2020 season the FPL API requires an authenticated user to view leagues and league standings. Due to this a username and password is required for the FPL Email Bot to function. The username and password will only be used to connect to the site to fetch league standings and does not get sent anywhere else.
> 
> The username/password does not require to be in the league you are monitoring. Create a dummy account if you don't want to use your main account.

After finishing configuring the bot you can double-click the FplBot.exe file (or run from command-line) and it will process all gameweeks up until the current one. Which gameweek to process next is controlled by the gameweek.txt file so make sure you don't delete it. Once all gameweeks have been processed you can close the window. Run it again every time a gameweek finished (or keep it running for the next 6 months, who am I to tell you what to do).

If you want to run it as an automated webjob in Azure make sure you enter your Azure connection strings as well as set useAzure to true. Also include a name you want to use for the blog storage. If you pick a name that doesn't exist one will be created for you. Next zip all your files up and upload as an Azure webjob. Make sure you set it to continuous and single-instance. If you don't understand anything in this paragraph you should only run it from your computer.

## How are weekly winners determined

Currently weekly winners are calculated using the Dan Davies rule. Rather than just being the highest scoring team for the GW, it's the highest score with the total transfer cost for that week subtracted.

Example:

- Team A has 60 points and took 2 hits for a total of -8pts.
- Team B has 58 points and took 1 hit for a total of -4 pts.
- All other teams have less than 50 points

The winner would be Team B with 58 - 4 = 54 points, and Team B would be in second place with 60 - 8 = 52 pts.

## To-do

- Add configuration to disable Dan Davies rule
