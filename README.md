# Fpl Email Bot

FPL Email Bot extracts information from the FPL API and creates a summary with weekly winners, losers, and certain fun facts. It emails the summary to a set list of emails once the GW has completed.

Example email body:

```
Beep boop! I am a robot. This is your weekly FPL update.

The winner for gameweek #4 was Davies & Co. with 61 points! Rounding up the top 3 for the week was Chupacabra (56 pts) and Panama (55 pts).

The worst ranking teams this week were Big Red Dog (32 pts), HasselHoff (31 pts), FC Grunge (30 pts), and Goes to 11 (28 pts). You should probably be embarrassed. 

When it came to captaincy choices Garry's Gooners and Davies & Co. did the best this week with 26 points from Harry Kane. On the other end of the spectrum were FC Grunge who had picked Javier Hernández Balcázar for a total of 4 points. You receive the armband of shame for this week. 

Kotterpool with 230 total points is the new league leader, supplanting last weeks leader Gnarly Norwegians. At the other end Big Red Dog is the new bottom feeder with a pitiful 155 points total.

Notable news:

- 12 teams managed to reach or beat the overall average of 44 points for the week. 
- ManCityMonsters left an astonishing 23 points on the bench which was the highest in the league.
- No hits were taken this week.
- Davies & Co. (wildcard) and Goes to 11 (wildcard) decided to spend one of their chips this week.

Your friendly FPL bot will return next gameweek with another update.
```

Example of attached standings file:

```
Standings for Oskana FC after GW#4:

----------------------------------------

Rank Chg. LW   Team                 Pts.
----------------------------------------

 1   up    2   Kotterpool            230
 2   dn    1   Gnarly Norwegians     227
 3   --    3   Hillhurst Hotspur     219
 4   up    6   Kick it in the net    213
 5   dn    4   Gear                  209
 6   up    9   Chupacabra            205
 7   dn    5   Beeston               204
 8   dn    7   Windsor Wildlings     201
 9   up   11   U30                   190
10   dn    8   HasselHoff            186
11   up   13   Slaughterhouse 11     184
12   --   12   Relegation FC         178
13   dn   10   Goes to 11            177
14   up   19   Davies & Co.          168
15   --   15   ManCityMonsters       164
16   up   17   Garry's Gooners       161
17   dn   14   FC Grunge             159
18   up   20   Panama                156
19   dn   18   The B team            155
20   dn   16   Big Red Dog           155

```

## Requirements

- Windows OS
- An email account
- Azure subscription (optional)

## Setup

(TODO)

## How are weekly winners determined

Currently weekly winners are calculated using the Dan Davies rule. Rather than just being the highest scoring team for the GW, it's the highest score with the total transfer cost for that week subtracted.

Example:

- Team A has 60 points and took 2 hits for a total of -8pts.
- Team B has 58 points and took 1 hit for a total of -4 pts.
- All other teams have less than 50 points

The winner would be Team B with 58 - 4 = 54 points, and Team B would be in second place with 60 - 8 = 52 pts.

# To-do

- Documentation on how to use
- Add configuration to disable Dan Davies rule
