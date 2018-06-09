# Fpl Email Bot

FPL Email Bot extracts information from the FPL API and creates a summary with weekly winners, losers, and certain fun facts. It emails the summary to a set list of emails once the GW has completed.

## Requirements

- Windows OS (Sorry)
- An email account you control
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

- Better error handling (Sending emails, API calls)
- Documentation on how to use
- Expand standings.txt attachment to include previous weeks rank and movement indicator (up/down/same)
- Add configuration to disable Dan Davies rule
