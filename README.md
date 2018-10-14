# Fpl Email Bot

FPL Email Bot extracts information from the FPL API and creates a summary with weekly winners, losers, and certain fun facts. It emails the summary to a set list of emails once the GW has completed.

Example email body:

```
Beep boop! I am a robot. This is your weekly FPL update.

The winner for gameweek #7 was Gnarly Norwegians with 82 points! Rounding up the top 3 for the week was Kick it in the net!! (72 pts) and The B Team (63 pts).

The worst ranking teams this week were The Goon Squad (44 pts), Kotterpool (43 pts), Relegation FC (39 pts), and Big Red Dog (32 pts). You should probably be embarrassed. 

When it came to captaincy choice Kick it in the net!! did the best this week with 26 point from Harry Kane. On the other end of the spectrum were The Goon Squad who had picked Sadio Mané for a total of 2 points. You receive the armband of shame for this week. 

Gnarly Norwegians with 482 total points is the new league leader, supplanting last weeks leader Goes to 11. At the other end Big Red Dog is the new sap in last place with a miserable 295 points total.

As for shakers and movers Man City Minions climbed 4 spots this week. In the not so great department we have Relegation FC dropping 3 spots.

Notable news:

- 10 teams managed to reach or beat the overall average of 51 points for the week. 
- FC Grunge left an extraordinary 23 points on the bench which was the highest in the league.
- Gear (-4 pts) and Slaughterhouse 11 (-4 pts) both took transfer hits.
- Gnarly Norwegians (wildcard) was the only team to use a chip this week.

Your friendly FPL bot will return next gameweek with another update.

```

Example of attached standings file:

```
Standings for Oskana FC after GW#7:
------------------------------------------
Rank Chg. LW   Team                   Pts.
------------------------------------------

 1   up    3   Gnarly Norwegians       482
 2   dn    1   Goes to 11              462
 3   dn    2   Slaughterhouse 11       455
 4   up    5   Gear                    445
 5   dn    4   Chupacabra              438
 6   --    6   Beeston FC              412
 7   --    7   Hillhurst Hotspur       394
 8   --    8   JoSalah                 387
 9   up   11   Kick it in the net!!    379
10   --   10   Daniel's Spaniels       362
11   up   15   Man City Minions        355
12   dn    9   Relegation FC           353
13   dn   12   Kotterpool              348
14   --   14   FC Grunge               347
15   dn   13   The Goon Squad          344
16   --   16   windsor wildlings       330
17   up   18   The B Team              309
18   dn   17   Big Red Dog             295
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
